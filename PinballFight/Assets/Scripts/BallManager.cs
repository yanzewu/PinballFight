using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager {

    GameController controller;
    GameObject ball_prefab;
    float spawn_dt;
    List<Vector4> colors;

    public void initialize(GameController controller, GameObject ball_prefab, GameParam param){
        this.controller = controller;
        this.ball_prefab = ball_prefab;
        this.spawn_dt = 1.0f/ param.level_params[0].launch_rate;
        this.colors = param.level_params[0].colors;
    }

    public void clear(){
        foreach (var b in GameObject.FindGameObjectsWithTag("Ball")){
            GameObject.Destroy(b.gameObject);
        }
    }

    public void ignore_bricks(GameObject ball){
        foreach (var b in GameObject.FindGameObjectsWithTag("Brick")){
            Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), ball.GetComponent<CircleCollider2D>());
        }
    }

    public void acclerate_all(int player_id, float ratio){
        foreach (var b in GameObject.FindGameObjectsWithTag("Ball")){
            if (b.GetComponent<Ball>().player_id == player_id){
                b.GetComponent<Rigidbody2D>().velocity *= ratio;
            }
        }
    }

    public IEnumerator spawn_sequence(int player_id, Func<Vector2> pos_gen, Func<Vector2> velocity_gen, int number, GameState.SinglePlayerState state) {

        Debug.Log("Spawn" + number.ToString());

        for (int i = 0; i < number; i++){
            spawn_single(player_id, pos_gen(), velocity_gen());
            state.num_balls--;
            yield return new WaitForSeconds(spawn_dt);
        }
        controller.shoot_finished(player_id);
    }

    void spawn_single(int player_id, Vector2 pos, Vector2 velocity){
        var g = GameObject.Instantiate(ball_prefab, pos, Quaternion.identity);

        g.GetComponent<Rigidbody2D>().velocity = velocity;
        g.GetComponent<Ball>().player_id = player_id;
        foreach (var b in GameObject.FindGameObjectsWithTag("Ball")){
            Physics2D.IgnoreCollision(b.GetComponent<CircleCollider2D>(), g.GetComponent<CircleCollider2D>());
        }
    }

}