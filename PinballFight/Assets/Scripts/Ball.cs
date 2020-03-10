using UnityEngine;

public class Ball : MonoBehaviour {

    public int player_id;
    public bool response_to_brick = true;
    GameController controller;

    private void Awake() {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        GetComponent<SpriteHotLoader>().load(player_id);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Softwall"){
            Destroy(this.gameObject);
            controller.ball_recovered(player_id);
        }
        else if (other.gameObject.tag == "Ball"){
            if (other.gameObject.GetComponent<Ball>().player_id != player_id){
                // 
            }
        }
        else if (other.gameObject.tag == "Brick" && response_to_brick){
            other.gameObject.GetComponent<Brick>().hitten(player_id);
        }
    }

    public void ignite(){
        controller.ball_ignited(this.gameObject);
        controller.bounce_value_obtained(player_id);
        transform.GetChild(0).GetComponent<SpriteHotLoader>().load(player_id);
    }
}