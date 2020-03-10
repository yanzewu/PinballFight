using System;
using UnityEngine;
using UnityEngine.UI;

public class OnGameUIManager {

    GameObject ball_indicator_text;
    GameObject[][] HPs = new GameObject[2][];
    GameState game_state;


    public void initialize(){
        ball_indicator_text = GameObject.Find("BallIndicatorText");
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
        if (player_id == 0){
            player_item.ball_indicator.GetComponent<SpriteHotLoader>().load();
            player_item.bouncevalue_indicator.GetComponent<SpriteHotLoader>().load();   // TODO bvr = 4
        }
        foreach (var hp in player_item.HPs){
            hp.GetComponent<SpriteHotLoader>().load();
        }
        var champion = player_item.board.transform.GetChild(0);
        champion.GetComponent<SpriteHotLoader>().load(player_id + StatManager.get_state().champion[player_id] * 2);


        Func<Vector3, int, Vector3> convert_pos = (pos, i) => {
            return i == 0 ? pos : new Vector3(-pos.x, -pos.y, pos.z);
        };
        

        player_item.board.transform.position = convert_pos(player_item.board.transform.position, player_id);
        player_item.launcher.transform.position = convert_pos(player_item.launcher.transform.position, player_id);
        champion.localPosition = convert_pos(champion.localPosition, player_id);
        
        for (int i = 0; i < player_item.HPs.Length; i++){
            player_item.HPs[i].transform.position = convert_pos(player_item.HPs[i].transform.position, player_id);
            var pd = player_item.HPs[i].GetComponent<PosDuplicator>();
            pd.index = i;
            if (player_id == 1){
                pd.shift *= -1;
            }
        }

        HPs[player_id] = player_item.HPs;
    }

    public void update_indicator_ui(int player_id){
        if (player_id != 0) return;
        ball_indicator_text.GetComponent<Text>().text = "x" + game_state.player_state[0].num_balls.ToString();
    }

    public void update_hp_ui(int player_id){
        for (int i = 0; i < game_state.player_state[player_id].life; i++){
            HPs[player_id][i].SetActive(true);
        }
        for (int i = game_state.player_state[player_id].life; i < HPs[player_id].Length; i++){
            HPs[player_id][i].SetActive(false);
        }
    }

}