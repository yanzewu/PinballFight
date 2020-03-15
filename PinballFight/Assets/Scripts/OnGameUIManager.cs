using System;
using UnityEngine;
using UnityEngine.UI;

public class OnGameUIManager {

    GameController.PlayerItem[] player_items = new GameController.PlayerItem[2];
    GameState game_state;

    int num_players;


    public void initialize(GameController.PlayerItem[] player_items, int num_players){
        this.player_items = player_items;
        this.num_players = num_players;

        this.player_items[0].ball_indicator_text = GameObject.Find("BallIndicatorText");
        this.player_items[1].ball_indicator_text = GameObject.Instantiate(this.player_items[0].ball_indicator_text);
        this.player_items[1].ball_indicator_text.transform.SetParent(GameObject.Find("Canvas").transform);
        var op = this.player_items[1].ball_indicator_text.GetComponent<OnGamePosHelper>();
        if (num_players == 1){
            op.pos = new Vector3(100, 0, 0);
        }
        else{
            op.pos = new Vector3(-op.pos.x, -op.pos.y, op.pos.z);
        }

        for (int i = 0; i < 2; i++){
            init_player_ui(player_items[i], i);
        }

    }
    public void reload(GameState game_state){
        this.game_state = game_state;

        for (int i = 0; i < game_state.player_state.Length; i++){
            update_indicator_ui(i);
            update_hp_ui(i);
        }
    }

    public void init_player_ui(GameController.PlayerItem player_item, int player_id){

        player_item.board.GetComponent<SpriteHotLoader>().load(player_id);
        player_item.launcher.GetComponent<SpriteHotLoader>().load(player_id);
        player_item.ball_indicator.GetComponent<SpriteHotLoader>().load();
        player_item.bouncevalue_indicator.GetComponent<SpriteHotLoader>().load();   // TODO bvr = 4
        foreach (var hp in player_item.HPs){
            hp.GetComponent<SpriteHotLoader>().load();
        }
        var champion = player_item.board.transform.GetChild(0);
        champion.GetComponent<SpriteHotLoader>().load(player_id + StatManager.get_state().champion[player_id] * 2);

        Action<Transform, int, bool> convert_pos = (t, i, hide) => {
            if (i == 1) t.localPosition = new Vector3(-t.localPosition.x, -t.localPosition.y, hide ? 20 : t.localPosition.z);
        };

        Action<Transform> flip_pos = (t) => {
            t.Rotate(0, 0, 180);
        };
        
        convert_pos(player_item.board.transform, player_id, false);
        convert_pos(player_item.launcher.transform, player_id, false);
        convert_pos(player_item.ball_indicator.transform, player_id, num_players == 1 && player_id == 1);
        convert_pos(player_item.bouncevalue_indicator.transform, player_id, num_players == 1 && player_id == 1);
        convert_pos(champion.transform, player_id, false);

        
        for (int i = 0; i < player_item.HPs.Length; i++){
            convert_pos(player_item.HPs[i].transform, player_id, false);
            var pd = player_item.HPs[i].GetComponent<PosDuplicator>();
            pd.index = i;
            if (player_id == 1){
                pd.shift *= -1;
            }
        }

        if (num_players == 2 && player_id == 1){
            flip_pos(player_item.board.transform);
            flip_pos(player_item.ball_indicator.transform);
            flip_pos(player_item.ball_indicator_text.transform);
            flip_pos(player_item.bouncevalue_indicator.transform);
            player_item.bouncevalue_indicator.GetComponent<RectHPBar>().orientation = 3;
            player_item.board.GetComponent<RectHPBar>().orientation = 2;
            foreach (var hp in player_item.HPs){
                flip_pos(hp.transform);
            }
        }
    }

    public void update_indicator_ui(int player_id){
        player_items[player_id].ball_indicator_text.GetComponent<Text>().text = "x" + game_state.player_state[0].num_balls.ToString();
    }

    public void update_hp_ui(int player_id){
        for (int i = 0; i < game_state.player_state[player_id].life; i++){
            player_items[player_id].HPs[i].SetActive(true);
        }
        for (int i = game_state.player_state[player_id].life; i < player_items[player_id].HPs.Length; i++){
            player_items[player_id].HPs[i].SetActive(false);
        }
    }

    public void update_bouncevalue_ui(int player_id){
        player_items[player_id].bouncevalue_indicator.GetComponent<BounceValueIndicator>().set_length(
            (float)game_state.player_state[player_id].bounce_value / game_state.player_state[player_id].bounce_value_required
        );
    }

}