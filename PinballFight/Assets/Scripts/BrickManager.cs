using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class BrickManager {

    float[] durability_probs;
    float[] probabilites;
    float triangluar_iron_brick_probability;
    float grid_size;

    public void reload(LevelParam param){
        probabilites = param.brick_probabilities;
        durability_probs = param.brick_durability_probs;
        triangluar_iron_brick_probability = param.triangluar_iron_brick_probability;
    }

    public void generate_map(GameObject grid){
/*
        var cumsum_prob = _cumsum(probabilites);
        var durability_prob = _cumsum(durability_probs);
        foreach (Transform child in grid.transform){
            var br = child.gameObject.GetComponent<Brick>();
            br.brick_type = (Brick.BrickType)_pick_idx(Random.Range(0f, 1f), cumsum_prob);
            br.durability = _pick_idx(Random.Range(0f, 1f), durability_prob) + 1;
        }*/
        grid_size = grid.GetComponent<Grid>().cellSize.x / 2;
        generate_map_advanced(grid, 2);
    }

    public void generate_map_advanced(GameObject grid, int mode=0){

        List<(GameObject, Brick, float)> down_half = new List<(GameObject, Brick, float)>();
        List<(GameObject, Brick, float)> up_half = new List<(GameObject, Brick, float)>();
        List<(GameObject, Brick, float)> mid_half = new List<(GameObject, Brick, float)>();

        var c_prob = _cumsum(durability_probs);

        foreach (Transform child in grid.transform){
            Brick b = child.gameObject.GetComponent<Brick>();
            if (b.is_triangular) {
                if (UnityEngine.Random.Range(0f, 1f) < triangluar_iron_brick_probability){
                    b.brick_type = Brick.BrickType.IRON;
                }
                else{
                    b.durability = _pick_idx(UnityEngine.Random.Range(0f, 1f), c_prob) + 1;
                }
                continue;
            }

            float c = child.position.x + child.position.y*20;
            if (c < 0){
                down_half.Add((child.gameObject, b, -c));
            }
            else if (c > 0){
                up_half.Add((child.gameObject, b, c));
            }
            else{
                mid_half.Add((child.gameObject, b, c));
            }
        }
        up_half.Sort((x, y)=>x.Item3.CompareTo(y.Item3));
        down_half.Sort((x, y)=>x.Item3.CompareTo(y.Item3));

        // we always random generate one side
        List<(Brick.BrickType, int)> down_result = generate_list(down_half.Count);
        List<(Brick.BrickType, int)> up_result;
        List<(Brick.BrickType, int)> mid_result = generate_list(mid_half.Count);

        if (mode == 0){
            up_result = generate_list(up_half.Count);
        }
        else if (mode == 1){
            up_result = down_result;
        }
        else if (mode == 2){
            up_result = generate_list(up_half.Count);
            int d_tot_down = down_result.Sum(x=>x.Item2), d_tot_up = up_result.Sum(x=>x.Item2);
            int t_tot_down = down_result.Sum(x=>(int)x.Item1 > 1?1:0), t_tot_up = up_result.Sum(x=>(int)x.Item1 > 1?1:0);   // not NONE/IRON

            var r = new System.Random();
            
            List<(Brick.BrickType, int)> d_lower_list, d_higher_list;
            if (d_tot_down < d_tot_up){
                d_lower_list = down_result; d_higher_list = up_result;
            }
            else{
                d_lower_list = up_result; d_higher_list = down_result;
            }

            foreach (var idx in Enumerable.Range(0, d_lower_list.Count)
                .Where(x => d_lower_list[x].Item2 < durability_probs.Length-1)
                .OrderBy(x=>r.Next())
                .Take(Mathf.Abs((d_tot_down - d_tot_up)/2))){
                    d_lower_list[idx] = (d_lower_list[idx].Item1, d_lower_list[idx].Item2+1);
                }
            foreach (var idx in Enumerable.Range(0, d_higher_list.Count)
                .Where(x => d_higher_list[x].Item2 > 1)
                .OrderBy(x=>r.Next())
                .Take(Mathf.Abs((d_tot_down - d_tot_up)/2))){
                    d_higher_list[idx] = (d_higher_list[idx].Item1, d_higher_list[idx].Item2-1);
                }
            
            List<(Brick.BrickType, int)> t_lower_list, t_higher_list;
            if (t_tot_down < t_tot_up){
                t_lower_list = down_result; t_higher_list = up_result;
            }
            else{
                t_lower_list = up_result; t_higher_list = down_result;
            }

            List<Brick.BrickType> discarded_bricks = new List<Brick.BrickType>();
            foreach (var idx in Enumerable.Range(0, t_higher_list.Count)
                .Where(x => (int)t_higher_list[x].Item1 > 1)
                .OrderBy(x=>r.Next())
                .Take(Mathf.Abs((t_tot_down - t_tot_up)/2))){
                    discarded_bricks.Add(t_higher_list[idx].Item1);
                    t_higher_list[idx] = (Brick.BrickType.NONE, t_higher_list[idx].Item2);
                }

            foreach (var idx in Enumerable.Range(0, t_lower_list.Count)
                .Where(x => t_lower_list[x].Item1 == Brick.BrickType.NONE)
                .OrderBy(x=>r.Next())
                .Take(Mathf.Abs((t_tot_down - t_tot_up)/2))){
                    t_lower_list[idx] = (discarded_bricks.Last(), t_lower_list[idx].Item2);
                    discarded_bricks.RemoveAt(discarded_bricks.Count-1);
                }
        }
        else{
            return;
        }

        // Special conditions
        var overall_list = down_result.Concat(up_result).Concat(mid_result);
        if (overall_list.Where(x=>x.Item1 == Brick.BrickType.EXPLOSION).Count() == 0 ||
            overall_list.Where(x=>x.Item1 == Brick.BrickType.IRON).Count() == 0
        ){
            Debug.Log("Failed to meet number requirement");
            generate_map_advanced(grid, mode);
            return;
        }
        if (Enumerable.Range(0, down_result.Count).Where(
            x => (
                down_half[x].Item3 > down_half.Last().Item3 - 10) && 
                down_result[x].Item1 == Brick.BrickType.IRON).Count() > 0 ||
            Enumerable.Range(0, down_result.Count).Where(
            x => (
                down_half[x].Item3 > down_half.Last().Item3 - 10) && 
                down_result[x].Item1 == Brick.BrickType.IRON).Count() > 0){
                Debug.Log("Failed to meet iron requirement");
                generate_map_advanced(grid, mode);
                return;
            }

        Debug.Log(System.String.Format("d_down={0}, d_up={1}", down_result.Sum(x=>x.Item2), up_result.Sum(x=>x.Item2)));
        Debug.Log(System.String.Format("t_down={0}, t_up={1}", down_result.Sum(x=>x.Item1!=0?1:0), up_result.Sum(x=>x.Item1!=0?1:0)));

        apply_generate_result(down_result, down_half);
        apply_generate_result(up_result, up_half);
        apply_generate_result(mid_result, mid_half);
    }

    private List<(Brick.BrickType, int)> generate_list(int count){
        var cumsum_prob = _cumsum(probabilites);
        var durability_prob = _cumsum(durability_probs);

        var ret = new List<(Brick.BrickType, int)>();
        for (int i = 0; i < count; i++){
            ret.Add((
                (Brick.BrickType)_pick_idx(UnityEngine.Random.Range(0f, 1f), cumsum_prob), 
                _pick_idx(UnityEngine.Random.Range(0f, 1f), durability_prob) + 1));
        }
        return ret;
    }

    private void apply_generate_result(List<(Brick.BrickType, int)> paramlist, List<(GameObject, Brick, float)> targets){
        foreach (var pt in paramlist.Zip(targets, (p, t)=>new {p = p, t = t})){
            pt.t.Item2.brick_type = pt.p.Item1;
            pt.t.Item2.durability = pt.p.Item2;
        }
    }

    public void detonate(int player_id, Vector2 pos){
        foreach (var b in GameObject.FindGameObjectsWithTag("Brick")){
            if ((b.GetComponent<Rigidbody2D>().position - pos).magnitude < 1.5 * grid_size){
                b.GetComponent<Brick>().hitten(player_id);
            }
        }
    }

    int _factorial(int i){
        return i > 1 ? i * _factorial(i-1) : 1;
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