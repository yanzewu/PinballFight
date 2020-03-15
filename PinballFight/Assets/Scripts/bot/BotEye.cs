using UnityEngine;
public class BotEye : MonoBehaviour {
    
    GameController controller;
    public int player_id = 1;

    private void Awake() {
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Ball"){
            controller.bot_ball_sensed(other.gameObject, player_id);
        }
    }

}