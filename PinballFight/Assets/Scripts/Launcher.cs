using UnityEngine;

public class Launcher : MonoBehaviour {

    GameState.SinglePlayerState player_state;
    
    float angle_min;
    float angle_max;
    float angular_speed;

    private void Awake() {
    }

    public void set_param(LevelParam param, GameState.SinglePlayerState state){
        this.player_state = state;
        angle_max = param.launcher_max_angle;
        angle_min = param.launcher_min_angle;
        angular_speed = param.launcher_angular_speed * (2*Random.Range(0, 2) - 1);
    }

    private void Update() {
        this.player_state.launcher_angle += angular_speed * Time.deltaTime;
        if (this.player_state.launcher_angle > angle_max || this.player_state.launcher_angle < angle_min){
            angular_speed *= -1;
        }
    }

}