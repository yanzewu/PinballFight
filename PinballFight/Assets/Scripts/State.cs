using System;
using UnityEngine;
using System.Collections.Generic;


// global game state
[Serializable]
public class GlobalState {
    public int current_level = 0;
    public int current_map = 0;
    public int game_mode = 0;   // ai; 1->two players
    public bool is_first_time = true;
    public int bot_level = 0;
    public int[] champion = new int [2]{0, 0};

}


// local game state
[Serializable]
public class GameState
{

    [Serializable]
    public class SinglePlayerState {
        public int life;
        public int board_color;
        public float active_bounce_cd;
        public float active_bounce_tr;
        public float launch_cd;
        public float launch_tr;
        public float launcher_angle;
        public int bounce_value;
        public int bounce_value_required;
        public int num_balls;
    };

    public SinglePlayerState[] player_state = new SinglePlayerState[2]{new SinglePlayerState(), new SinglePlayerState()};
    public int num_bricks;

    public void initialize(GameParam param){

    }

    public void reload(LevelParam param) {
        for (int i = 0; i < 2; i++){
            player_state[i].life = param.lives;
            player_state[i].active_bounce_cd = param.bounce_cd_init;
            player_state[i].active_bounce_tr = 0;
            player_state[i].launch_cd = param.launch_cd_init;
            player_state[i].launch_tr = 0;
            player_state[i].launcher_angle = UnityEngine.Random.Range(
                param.launcher_min_angle, param.launcher_max_angle);
            player_state[i].bounce_value = 0;
            player_state[i].bounce_value_required = param.bounce_value_required;
            player_state[i].num_balls = 1;
            
            int champion = StatManager.get_state().champion[i];

            if (champion == 1){
                player_state[i].active_bounce_cd -= param.championA_bounce_cd_dec;
            }
            else if (champion == 2){
                player_state[i].launch_cd -= param.championB_launch_cd_dec;
            }
            else if (champion == 3){
                player_state[i].bounce_value_required = param.championC_bounce_value_required;
            }
        }

    }
}
