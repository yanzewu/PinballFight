using UnityEngine;

public class Launcher : MonoBehaviour {

    GameState.SinglePlayerState player_state;
    
    float angle_min;
    float angle_max;
    float angular_speed;

    GameObject barrel;
    float barrel_dr;
    public Vector2 target_pos;

    private void Awake() {
        barrel = transform.GetChild(0).gameObject;
        var dr = barrel.transform.position - this.transform.position;
        barrel_dr = Mathf.Sqrt(dr.x*dr.x + dr.y*dr.y);
    }

    public void set_param(LevelParam param, GameState.SinglePlayerState state, int player_id){
        this.player_state = state;
        angle_max = param.launcher_max_angle;
        angle_min = param.launcher_min_angle;
        angular_speed = param.launcher_angular_speed * (2*Random.Range(0, 2) - 1);
        barrel_dr = player_id == 0 ? barrel_dr : -barrel_dr;
    }
    private void Update() {
        this.player_state.launcher_angle += angular_speed * Time.deltaTime;
        if (this.player_state.launcher_angle > angle_max || this.player_state.launcher_angle < angle_min){
            angular_speed *= -1;
        }

        target_pos = new Vector2(-Mathf.Cos(this.player_state.launcher_angle), Mathf.Sin(this.player_state.launcher_angle)) * barrel_dr;
        barrel.transform.localPosition = new Vector3(target_pos.x, target_pos.y, -0.01f);
        barrel.transform.eulerAngles = new Vector3(0, 0, -180/Mathf.PI * this.player_state.launcher_angle);
    }

}