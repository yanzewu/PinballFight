using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Entry of Event Dispath
public class GameController : MonoBehaviour {

    public class PlayerItem {
        public GameObject board;
        public GameObject launcher;
    };

    GameParam game_param;
    LevelParam level_param;
    GameState game_state = new GameState();

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
            "Ball", "Board", "Launcher"
        };

        item_prefabs = new Dictionary<string, GameObject>();
        foreach (var item_name in item_prefab_list){
            item_prefabs.Add(item_name, ResManager.load_prefab(item_name));
        }

        //game_param = ParamManager.load_param(0);
        game_param = new GameParam();
        ParamManager.save_param(0, game_param);
        game_state.initialize(game_param);

        float[] sign = new float[2] {1, -1};

        for (int i = 0; i < 2; i++){
            player_item[i].board = Instantiate(item_prefabs["Board"], item_prefabs["Board"].transform.position * sign[i], Quaternion.identity);
            player_item[i].launcher = Instantiate(item_prefabs["Launcher"], item_prefabs["Launcher"].transform.position * sign[i], Quaternion.identity);
        }

        ball_manager.initialize(this, item_prefabs["Ball"], game_param);

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
            player_item[i].board.GetComponent<SpriteRenderer>().color = level_param.colors[i];
            player_item[i].launcher.GetComponent<Launcher>().set_param(level_param, game_state.player_state[i]);
        }

        is_paused = false;
        is_finished = false;
        time_begin = Time.time;
        checkpoint_state = 0;
    }
    public void clear_level(){
        game_terrian.clear_level();

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
        clear_level();
        reload_level(StatManager.get_state().current_level);
    }
    public void exit_game(){
        Debug.Log("exit");
        clear_level();
        SceneManager.LoadScene("Start");
    }
    public void finish_level(){
        Debug.Log("Current=" + StatManager.get_state().current_level.ToString() + "total=" + game_param.level_params.Count);
        
        var g_state = StatManager.get_state();
        g_state.current_level += 1;
        if (g_state.current_level == game_param.level_params.Count){
            g_state.current_level = 0;
            
        }
        else{
            clear_level();
            reload_level(g_state.current_level);
        }
        StatManager.save_stat();
    }

    private void lose(int player_id){
        finish_level();
    }

    public void board_dragged(Vector2 pos, int player_id){
        Debug.Log("Board dragged: " + pos.ToString());
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

        if (game_state.player_state[player_id].life < 0) {
            lose(player_id);
        }
    }

    public void ball_recovered(int player_id){
        game_state.player_state[player_id].num_balls++;
    }
    public void shoot_finished(int player_id){
        game_state.player_state[player_id].launch_tr = 
        game_state.player_state[player_id].launch_cd;
        game_state.player_state[player_id].num_balls += 1;
    }
    public void brick_destroyed(){
        game_state.num_bricks--;
    }
    public void ball_ignited(GameObject ball){
        ball.GetComponent<Rigidbody2D>().velocity *= level_param.fireball_speed;
    }

    private void shoot(int player_id){
        Debug.Log("Shoot " + player_id.ToString());

        var pos = player_item[player_id].launcher.transform.position;

        Func<Vector2> get_vec = () => {
            var angle = game_state.player_state[player_id].launcher_angle;
            float speed = player_id == 0 ? level_param.ball_speed : -level_param.ball_speed;
            return new Vector2(-Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        };

        StartCoroutine(ball_manager.spawn_sequence(
            player_id, pos, get_vec, game_state.player_state[player_id].num_balls, game_state.player_state[player_id]));
        
        // TODO UI
    }

    private void Update()
    {
        if (is_paused) return;

        for (int i = 0; i < 2; i++){
            game_state.player_state[i].launch_tr -= Time.deltaTime;
            game_state.player_state[i].active_bounce_tr -= Time.deltaTime;

            if (game_state.player_state[i].launch_tr <= 0){
                shoot(i);
                game_state.player_state[i].launch_tr = Mathf.Infinity;
            }

            player_item[i].board.GetComponent<Board>().set_active_length(Mathf.Clamp01(
                game_state.player_state[i].active_bounce_tr / game_state.player_state[i].active_bounce_cd
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
            shoot(0);
        }

    }
}