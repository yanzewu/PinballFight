using UnityEngine;


public class BrickManager {

    int default_durability;
    float[] probabilites;
    float grid_size = 0.9375f;

    public void reload(LevelParam param){
        default_durability = param.brick_durability;
        probabilites = param.brick_probabilities;
    }

    public void generate_map(GameObject grid){

        var cumsum_prob = _cumsum(probabilites);
        foreach (Transform child in grid.transform){
            var br = child.gameObject.GetComponent<Brick>();
            br.durability = default_durability;
            br.brick_type = (Brick.BrickType)_pick_idx(Random.Range(0f, 1f), cumsum_prob);
        }
    }

    public void detonate(int player_id, Vector2 pos){
        foreach (var b in GameObject.FindGameObjectsWithTag("Brick")){
            if ((b.GetComponent<Rigidbody2D>().position - pos).magnitude < 1.5 * grid_size){
                b.GetComponent<Brick>().hitten(player_id);
            }
        }
    }

    int _pick_idx(float v, float[] a){
        for (int i = 0; i < a.Length; i++){
            if (a[i] >= v){
                return i;
            }
        }
        return a.Length;
    }

    float[] _cumsum(float[] a){
        float[] r = new float[a.Length];
        float s = 0;
        for (int i = 0; i < a.Length; i++){
            s += a[i];
            r[i] = s;
        }
        return r;
    }

}