using UnityEngine;


public class BrickManager {

    float[] durability_probs;
    float[] probabilites;
    float grid_size = 0.9375f;

    public void reload(LevelParam param){
        probabilites = param.brick_probabilities;
        durability_probs = param.brick_durability_probs;
    }

    public void generate_map(GameObject grid){

        var cumsum_prob = _cumsum(probabilites);
        var durability_prob = _cumsum(durability_probs);
        foreach (Transform child in grid.transform){
            var br = child.gameObject.GetComponent<Brick>();
            br.brick_type = (Brick.BrickType)_pick_idx(Random.Range(0f, 1f), cumsum_prob);
            br.durability = _pick_idx(Random.Range(0f, 1f), durability_prob) + 1;
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