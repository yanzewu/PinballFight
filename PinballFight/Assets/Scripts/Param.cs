using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameParam {

    public int map_number = 1;
    public List<LevelParam> level_params = new List<LevelParam>(){new LevelParam()};
}

[Serializable]
public class LevelParam {

    public int lives = 3;
    public float ball_speed = 2.5f;
    public float bounce_cd_init = 6.0f;
    public float bounce_cd_min = 3.0f;
    public float bounce_dy = 0.3125f;
    public float bounce_dt = 0.1f;
    public float bounce_dt2 = 0.1f;
    public float hit_dy = 0.05f;
    public float hit_dt = 0.025f;
    public float touch_dt = 0.2f;   // duration that a buttonup is identified as touch
    public float fireball_speed = 2.0f;
    public float launch_cd_init = 5.0f; // need to be adjusted for hero.
    public float launch_cd_min = 2.0f;
    public float launch_rate = 20f;
    public float launcher_min_angle = (float)Math.PI / 6;
    public float launcher_max_angle = (float)Math.PI * 5/12;
    public float launcher_angular_speed = (float)Math.PI/5;
    public List<float> launcher_cd_checkpoints = new List<float>(){15, 30, 45, 60};
    public float launcher_cd_checkpoint_dec = 0.3f;
    public float[] brick_durability_probs = new float[9]{
        0.03917f, 0.06458f, 0.09526f, 0.12576f, 0.14857f,
        0.15706f, 0.14857f, 0.12576f, 0.09526f
    };
    
    public float[] brick_probabilities = new float[7]{
        0.5f, 0.1f, 0.1f, 0.1f, 0.1f, 0, 0.1f
    };
    public float triangluar_iron_brick_probability = 0.4f;

    public int bounce_value_required = 5;

    // SPECIAL SKILLS

    public int explode_damage = 3;
    public float roll_bounce_cd_dec = 0.5f;
    public float sandglass_launch_cd_dec = 0.3f;
    public float lightening_accelerate_ratio = 1.2f;
    public float championA_bounce_cd_dec = 1.0f;
    public float championB_launch_cd_dec = 0.6f;
    public int championC_bounce_value_required = 4;

    public List<Vector4> colors = new List<Vector4>(){new Vector4(0, 0, 255, 255), new Vector4(255, 0, 0, 255)};


    // AI

    public float defence_buffer = 0.07f;
    public float attack_buffer = 0.1f;
    public float bounce_activate_range_rel = 0.8f;
    public float[] bot_speed = new float[3]{4.0f, 10.0f, 20.0f};
    public int[] bot_prediction = new int[3]{1, 10, 10};
    public bool[] does_attack = new bool[3]{false, true, true};
}
