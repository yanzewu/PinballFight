using System.IO;
using UnityEngine;

public class ResManager {

    public static GameObject load_obj(string name){
        return Resources.Load(name) as GameObject;
    }

    public static GameObject load_prefab(string name){
        return Resources.Load("Prefabs/" + name) as GameObject;
    }

    public static TextAsset load_text(string name){
        string path = Path.Combine(Path.GetDirectoryName(name), Path.GetFileNameWithoutExtension(name));
        return Resources.Load(path) as TextAsset;
    }

    public static Texture2D load_runtime_image(string name){
        return Resources.Load<Texture2D>("Images/" + name.ToString());
    }

    public static string terrian_path(int level){
        return "Terrains/Grid" + level.ToString();
    }

    public static string param_path(int level){
        return "Params/" + level.ToString() + ".json";
    }
    public static string param_path(string name){
        return "Params/" + name + ".json";
    }

    public static string expand_res(string path){
        return Path.Combine(Application.dataPath, "Resources", path);
    }

    public static string data_path(){
        Debug.Log(Application.persistentDataPath);
        return Path.Combine(Application.persistentDataPath, "player.save");
    }

}