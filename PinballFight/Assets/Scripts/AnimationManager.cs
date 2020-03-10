using System.Collections.Generic;
using UnityEngine;

public class AnimationManager {

    Dictionary<string, GameObject> animation_prefabs = new Dictionary<string, GameObject>();
    Dictionary<string, float> animation_lengths = new Dictionary<string, float>();

    public void initialize(){
        animation_prefabs.Add("Explosion", ResManager.load_prefab("Explosion"));
        animation_lengths.Add("Explosion", 1.0f);
    }

    public void play_animation_at(string name, Vector2 pos){
        var g = GameObject.Instantiate(animation_prefabs[name], new Vector3(pos.x, pos.y, -0.08f), Quaternion.identity);
        GameObject.Destroy(g, animation_lengths[name]);
    }

}