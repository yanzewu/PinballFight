
using System.IO;
using UnityEngine;

public class ParamManager {

    public static GameParam load_param (int level){
        // level path is
        
        TextAsset text = ResManager.load_text(ResManager.param_path(level));
        if (text == null) return null;

        return JsonUtility.FromJson<GameParam>(text.text);
    }
    public static GameParam load_param (string name){
        // level path is
        
        TextAsset text = ResManager.load_text(ResManager.param_path(name));
        if (text == null) return null;

        return JsonUtility.FromJson<GameParam>(text.text);
    }

    // only for Editor!
    public static void save_param(int level, GameParam param){
        string json = JsonUtility.ToJson(param, true);

        string filePath = ResManager.expand_res(ResManager.param_path(level));
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine(json);
        writer.Close();
    }
}

