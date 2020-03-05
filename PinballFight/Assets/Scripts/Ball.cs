using UnityEngine;

public class Ball : MonoBehaviour {

    public int player_id;

    GameController controller;

    private void Awake() {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
        else if (other.gameObject.tag == "Brick"){
            other.gameObject.GetComponent<Brick>().hitten();
        }
    }

    public void ignite(){
        controller.ball_ignited(this.gameObject);
    }
}