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
    SpriteHotLoader crack_sh;

    private void Awake() {
        // load 9 graphs

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        if (is_triangular) return;
        crack_sh = transform.GetChild(0).GetComponent<SpriteHotLoader>();
    }

    private void Start() {
        if (is_triangular){
            int shift = brick_type == BrickType.IRON ? 4:0;
            var a = this.transform.eulerAngles.z;
            if (Mathf.Abs(a) < 0.01f) GetComponent<SpriteHotLoader>().load(shift);
            else if (Mathf.Abs(a - 90) < 0.01f) GetComponent<SpriteHotLoader>().load(shift+1);
            else if (Mathf.Abs(a - 180) < 0.01f) GetComponent<SpriteHotLoader>().load(shift+2);
            else if (Mathf.Abs(a - 270) < 0.01f) GetComponent<SpriteHotLoader>().load(shift+3);
        }
        else{
            GetComponent<SpriteHotLoader>().load((int)brick_type);
        }
        update_ui();
    }

    public void hitten(int player_id, int damage=1){
        // ! this will be overwritten by subclasses

        if (brick_type == BrickType.IRON) return;

        durability -= damage;

        if (durability <= 0){
            Destroy(this.gameObject);
            controller.brick_destroyed(brick_type, player_id, GetComponent<Rigidbody2D>().position);
        }
        else{
            update_ui();
        }
    }

    public void update_ui(){
        if (is_triangular) return;
        if (brick_type == BrickType.IRON) return;
        if (durability == 9) return;
        else if (durability >= 7){
            crack_sh.load(2);
        }
        else if (durability >= 4){
            crack_sh.load(1);
        }
        else {
            crack_sh.load(0);
        }

        //var sr = GetComponent<SpriteRenderer>();
        //sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, durability / 9.0f);
    }

}