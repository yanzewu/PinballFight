using UnityEngine;
public class Bot : MonoBehaviour {

    public float speed;
    public float target_x = 0;
    int player_id;

    Transform tf;
    Rigidbody2D rb;
    GameController controller;

    private void Awake() {
        tf = this.transform;
        rb = this.GetComponent<Rigidbody2D>();

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        player_id = GetComponent<Board>().player_id;
    }

    public void set_speed(float v){
        speed = v;
    }

    public void set_target(float x){
        Debug.Log("target set: " + x.ToString());
        target_x = x;
    }
    public float get_pos(){
        return this.rb.position.x;
    }

    public void bounce(){
        controller.board_touched(player_id);
    }

    private void Update() {
        var sgn = (target_x - rb.position.x >= 0 ? 1.0f : -1.0f);
        if (Mathf.Abs(rb.position.x - target_x) > speed * Time.deltaTime){
            controller.board_dragged(rb.position + new Vector2(sgn * speed * Time.deltaTime, 0), player_id);
        }
        else if (rb.position.x == target_x){

        }
        else{
            controller.board_dragged(new Vector2(target_x, rb.position.y), player_id);
        }
    }

}