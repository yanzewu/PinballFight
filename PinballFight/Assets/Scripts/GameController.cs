using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Entry of Event Dispath
public class GameController : MonoBehaviour {

    public class PlayerItem {
        public GameObject board;
        public GameObject launcher;
        public GameObject ball_indicator;
        public GameObject[] HPs;
    };

    GameParam game_param;
    LevelParam level_param;
    GameState game_state = new GameState();

#if UNITY_EDITOR
    public GameState game_state_display;
    public LevelParam level_param_display;
#endif

    UIManager ui_manager = new UIManager();
    OnGameUIManager ongame_ui_manager = new OnGameUIManager();
    TerrainManager game_terrian = new TerrainManager();
    BallManager ball_manager = new BallManager();
    BrickManager brick_manager = new BrickManager();
    List<GameObject> ongame_items = new List<GameObject>();    // collections of all different items

    Dictionary<string, GameObject> item_prefabs;
    PlayerItem[] player_item = new PlayerItem[2]{new PlayerItem(), new PlayerItem()};

    public bool is_paused {get; private set;}
    public bool is_finished {get; private set;}
    float time_begin;
    int checkpoint_state;
    
    void Awake()
    {
        // static resources (does not change by changing level)

        var item_prefab_list = new List<string>{
            "Ball", "Board", "Launcher", "BallIndicator", "HP"
        };

        item_prefabs = new Dictionary<string, GameObject>();
        foreach (var item_name in item_prefab_list){
            item_prefabs.Add(item_name, ResManager.load_prefab(item_name));
        }

        game_param = ParamManager.load_param(0);
        //game_param = new GameParam();
        //ParamManager.save_param(0, game_param);
        game_state.initialize(game_param);
#if UNITY_EDITOR
        game_state_display = game_state;
        level_param_display = game_param.level_params[0];
#endif
        ongame_ui_manager.initialize();

        for (int i = 0; i < 2; i++){
            player_item[i].board = Instantiate(item_prefabs["Board"]);
            player_item[i].launcher = Instantiate(item_prefabs["Launcher"]);
            player_item[i].ball_indicator = Instantiate(item_prefabs["BallIndicator"]);
            player_item[i].HPs = new GameObject[game_param.level_params[0].lives];
            for (int j = 0; j < game_param.level_params[0].lives; j++){
                player_item[i].HPs[j] = Instantiate(item_prefabs["HP"]);
            }

            ongame_ui_manager.init_player_ui(player_item[i], i);
        }

        ball_manager.initialize(this, item_prefabs["Ball"], game_param);
        ui_manager.initialize();

        // prepare level
        reload_level(StatManager.get_state().current_level);
    }

    public void reload_level(int level){
        Debug.Log(level);
        game_terrian.preload_level(level);
        game_terrian.instantiate_level(level);

        level_param = game_param.level_params[level];

        game_state.reload(level_param);
        brick_manager.reload(level_param);
        brick_manager.generate_map(game_terrian.get_grid());
        
        for (int i = 0; i < 2; i++){
            player_item[i].board.GetComponent<Board>().set_param(level_param);
            player_item[i].board.GetComponent<Board>().player_id = i;
            board_dragged(Vector2.zero, i);
            player_item[i].launcher.GetComponent<Launcher>().set_param(level_param, game_state.player_state[i], i);
        }
        ongame_ui_manager.reload(game_state);

        is_paused = false;
        is_finished = false;
        time_begin = Time.time;
        checkpoint_state = 0;
    }
    public void clear_level(){
        game_terrian.clear_level();
        ball_manager.clear();

        foreach (var item in ongame_items){
            Destroy(item);
        }
        ongame_items.Clear();
    }
    public void pause_game(){
        Time.timeScale = 0;
        is_paused = true;
    }
    public void resume_game(){
        Time.timeScale = 1;
        is_paused = false;
        //GameObject.Find("GameController").GetComponent<InputManager>().discard_flag = true;
    }
    public void restart_game(){
        resume_game();
        clear_level();
        reload_level(StatManager.get_state().current_level);
    }
    public void exit_game(){
        Debug.Log("exit");
        clear_level();
        SceneManager.LoadScene("Entry");
    }

    private void lose(int player_id){
        Debug.Log("Current=" + StatManager.get_state().current_level.ToString() + "total=" + game_param.level_params.Count);

        ui_manager.on_gameover(player_id != 0);
        pause_game();
        var g_state = StatManager.get_state();
        // if (player_id != 0) g_state.current_level += 1; !! This is used when there are multiple levels
        if (g_state.current_level == game_param.level_params.Count){
            g_state.current_level = 0;
        }
        StatManager.save_stat();
    }

    public void board_dragged(Vector2 pos, int player_id){
        //Debug.Log("Board dragged: " + pos.ToString());
        player_item[player_id].board.GetComponent<Board>().move_horizontal(pos.x);
    }

    public void board_touched(int player_id){
        Debug.Log("Board touched");
        if (game_state.player_state[player_id].active_bounce_tr <= 0){
            player_item[player_id].board.GetComponent<Board>().activate();
            game_state.player_state[player_id].active_bounce_tr = Mathf.Infinity;
        }
    }

    public void board_deactivated(int player_id){
        game_state.player_state[player_id].active_bounce_tr = 
        game_state.player_state[player_id].active_bounce_cd;
    }

    public void board_attacked(int player_id){
        Debug.Log("Board attacked");
        game_state.player_state[player_id].life--;
        ongame_ui_manager.update_hp_ui(player_id);

        if (game_state.player_state[player_id].life == 0) {
            lose(player_id);
        }
    }

    public void ball_recovered(int player_id){
        game_state.player_state[player_id].num_balls++;
        ongame_ui_manager.update_indicator_ui(player_id);
    }
    public void shoot_finished(int player_id){
        game_state.player_state[player_id].launch_tr = 
        game_state.player_state[player_id].launch_cd;
        game_state.player_state[player_id].num_balls += 1;
        ongame_ui_manager.update_indicator_ui(player_id);
    }
    public void brick_destroyed(Brick.BrickType brick_type, int player_id, Vector2 pos){
        game_state.num_bricks--;
        on_skill_effect(brick_type, player_id, pos);
    }

    public void on_skill_effect(Brick.BrickType brick_type, int player_id, Vector2 pos){
        Debug.Log("Skill used: " + brick_type.ToString());

        var ps = game_state.player_state[player_id];
        switch(brick_type){
            case Brick.BrickType.EXPLOSION: brick_manager.detonate(player_id, pos); break;
            case Brick.BrickType.BALL: {ps.num_balls++; ongame_ui_manager.update_indicator_ui(player_id);}; break;
            case Brick.BrickType.ROLL: ps.active_bounce_cd = Mathf.Clamp(
                ps.active_bounce_cd - level_param.roll_bounce_cd_dec, level_param.bounce_cd_min, Mathf.Infinity); 
                break;
            case Brick.BrickType.SANDGLASS: ps.launch_cd = Mathf.Clamp(
                ps.launch_cd - level_param.sandglass_launch_cd_dec, level_param.launch_cd_min, Mathf.Infinity); 
                break;
            default: break;
        }
    }

    public void ball_ignited(GameObject ball){
        var b = ball.GetComponent<Ball>();
        var rb = ball.GetComponent<Rigidbody2D>();
        if (!b.response_to_brick) return;   // avoid multiple acceleration

        var other_pid = 1 - b.player_id;
        var direction = (player_item[other_pid].board.GetComponent<Rigidbody2D>().position - rb.position).normalized;
        b.response_to_brick = false;
        ball_manager.ignore_bricks(ball);
        rb.velocity = level_param.fireball_speed * level_param.ball_speed * direction;
        Debug.Log("Ball Ignited with velocity=" + rb.velocity.ToString());
    }

    private void shoot(int player_id){
        Debug.Log("Shoot " + player_id.ToString());

        Func<Vector2> get_pos = () => player_item[player_id].launcher.transform.GetChild(0).transform.position;

        Func<Vector2> get_vec = () => {
            ongame_ui_manager.update_indicator_ui(player_id); // !! THIS IS A HACK!
            var angle = game_state.player_state[player_id].launcher_angle;
            float speed = player_id == 0 ? level_param.ball_speed : -level_param.ball_speed;
            return new Vector2(-Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        };

        StartCoroutine(ball_manager.spawn_sequence(
            player_id, get_pos, get_vec, game_state.player_state[player_id].num_balls, game_state.player_state[player_id]));
        
        // TODO UI
    }

    private void Update()
    {
        if (is_paused) return;

        for (int i = 0; i < 2; i++){
            var ps = game_state.player_state[i];
            ps.launch_tr -= Time.deltaTime;
            ps.active_bounce_tr = Mathf.Clamp(ps.active_bounce_tr - Time.deltaTime, 0, Mathf.Infinity);

            if (ps.launch_tr <= 0){
                shoot(i);
                ps.launch_tr = Mathf.Infinity;
            }

            player_item[i].board.GetComponent<Board>().set_active_length(Mathf.Clamp01(
                ps.active_bounce_tr / ps.active_bounce_cd
            ));

        }

        if (checkpoint_state < level_param.launcher_cd_checkpoints.Count && 
            Time.deltaTime - time_begin >= level_param.launcher_cd_checkpoints[checkpoint_state]){
                foreach (var ps in game_state.player_state){
                    ps.launch_cd = Mathf.Clamp(ps.launch_cd - level_param.launcher_cd_checkpoint_dec, level_param.launch_cd_min, Mathf.Infinity);
                }
                checkpoint_state++;
            }

        if (Input.GetKey(KeyCode.Space)){
            board_touched(0);
        }

    }
}