using UnityEngine;

public class Launcher : MonoBehaviour {

    GameState.SinglePlayerState player_state;
    
    float angle_min;
    float angle_max;
    float angular_speed;

    GameObject barrel;
    float barrel_dr;
    float barrel_shift;
    public Vector2 target_pos;
    public RectHPBar hp_bar;

    private void Awake() {
        barrel = transform.GetChild(0).gameObject;
        var dr = barrel.transform.position - this.transform.position;
        barrel_dr = 0.0f;// Mathf.Sqrt(dr.x*dr.x + dr.y*dr.y);
        barrel_shift = 0.18f;

        hp_bar = transform.GetChild(0).GetComponent<RectHPBar>();
    }

    public void set_param(LevelParam param, GameState.SinglePlayerState state, int player_id){
        this.player_state = state;
        angle_max = param.launcher_max_angle;
        angle_min = param.launcher_min_angle;
        angular_speed = param.launcher_angular_speed * (2*Random.Range(0, 2) - 1);
        //barrel_dr = player_id == 0 ? barrel_dr : -barrel_dr;
    }

    public void set_active_length(float length){
        hp_bar.set_hp(length);
    }

    private void Update() {
        this.player_state.launcher_angle += angular_speed * Time.deltaTime;
        if (this.player_state.launcher_angle > angle_max) {
            angular_speed *= -1;
            this.player_state.launcher_angle = angle_max;
        }
        if (this.player_state.launcher_angle < angle_min){
            angular_speed *= -1;
            this.player_state.launcher_angle = angle_min;
        }

        target_pos = new Vector2(-Mathf.Cos(this.player_state.launcher_angle), Mathf.Sin(this.player_state.launcher_angle)) * barrel_dr;
        barrel.transform.localPosition = new Vector3(target_pos.x - 0.025f, target_pos.y + barrel_shift, 0.01f);
        barrel.transform.localEulerAngles = new Vector3(0, 0, 90-180/Mathf.PI * this.player_state.launcher_angle);
    }

}