using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager {

    GameController controller;
    Dictionary<string, GameObject> animation_prefabs = new Dictionary<string, GameObject>();
    Dictionary<string, float> animation_lengths = new Dictionary<string, float>();

    public void initialize(){
        add_animation("Explosion", 1.0f);
        add_animation("SelfExplosion", 1.0f);

        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void add_animation(string name, float duration){
        animation_prefabs.Add(name, ResManager.load_prefab(name));
        animation_lengths.Add(name, 1.0f);
    }

    public void play_animation_at(string name, Vector2 pos){
        var g = GameObject.Instantiate(animation_prefabs[name], new Vector3(pos.x, pos.y, -0.08f), Quaternion.identity);
        controller.StartCoroutine(destory_future(g, animation_lengths[name]));
    }

    public void clear(){
        foreach (var g in GameObject.FindGameObjectsWithTag("Animation")){
            GameObject.Destroy(g);
        }
    }

    IEnumerator destory_future(GameObject g, float time){
        yield return new WaitForSeconds(time);
        if (g.gameObject != null){
            GameObject.Destroy(g);
        }
    }

}