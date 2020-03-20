using System.Collections.Generic;
using UnityEngine;

public class SoundManager {

    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

    AudioSource speaker;

    public void initialize(){

        add_sound("brick_bonus");
        add_sound("brick_disappear");
        add_sound("explosion_brick");
        add_sound("explosion_hero");
        add_sound("hit_board");
        add_sound("hit_bounce");
        add_sound("lose_game over");
        add_sound("win_victory");

        speaker = GameObject.Find("Speaker").GetComponent<AudioSource>();

        GameObject.Find("BGM").GetComponent<AudioSource>().Stop();

    }

    public void add_sound(string name, string filename=null){
        if (filename == null) filename = name;
        sounds.Add(name, ResManager.load_runtime_sound(name));
    }

    public void play_sound(string name){
        speaker.PlayOneShot(sounds[name]);
    }

    public void finalize(){
        GameObject.Find("BGM").GetComponent<AudioSource>().Play();
    }

}