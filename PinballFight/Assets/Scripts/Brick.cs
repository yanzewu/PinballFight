using UnityEngine;

public class Brick : MonoBehaviour {

    public enum BrickType {
        NONE, IRON, EXPLOSION,
        BALL, ROLL, SANDGLASS, LIGHTNTING
    };

    public BrickType brick_type = BrickType.NONE;
    public bool is_triangular = false;
    public int durability;
    GameController controller;

    private void Awake() {
        // load 9 graphs

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        GetComponent<SpriteHotLoader>().load((int)brick_type);
    }

    public void hitten(int player_id){
        // ! this will be overwritten by subclasses

        if (brick_type == BrickType.IRON) return;

        durability--;

        if (durability == 0){
            Destroy(this.gameObject);
            controller.brick_destroyed(brick_type, player_id, GetComponent<Rigidbody2D>().position);
        }
        else{
            update_ui();
        }
    }

    public void update_ui(){
        var sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, durability / 9.0f);
    }

}