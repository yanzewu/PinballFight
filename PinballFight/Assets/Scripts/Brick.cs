using UnityEngine;

public class Brick : MonoBehaviour {

    public enum BrickType {
        NONE, IRON, EXPLOSION,
        BALL, ROLL, SANDGLASS, LIGHTNTING
    };

    public BrickType type;
    public int durability;
    GameController controller;

    private void Awake() {
        // load 9 graphs

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    public void set_param(LevelParam param){
        durability = param.brick_durability;
        update_ui();
    }

    public void hitten(){
        // ! this will be overwritten by subclasses

        durability--;

        if (durability == 0){
            Destroy(this.gameObject);
            controller.brick_destroyed();
        }
        else{
            update_ui();
        }
    }

    public void update_ui(){

    }

}