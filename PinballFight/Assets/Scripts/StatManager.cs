using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public static class StatManager {

    private static bool loaded = false;
    public static GlobalState m_state = new GlobalState();

    public static void load_stat(){
        try {
            StreamReader reader = new StreamReader(ResManager.data_path());
            string json = reader.ReadToEnd();
            m_state = JsonUtility.FromJson<GlobalState>(json);
            reader.Close();
            loaded = true;
        }
        catch (Exception e){
            Debug.LogError(e);
        }
    }

    public static void save_stat(){
        StreamWriter writer = new StreamWriter(ResManager.data_path());
        writer.WriteLine(JsonUtility.ToJson(m_state));
        writer.Close();
    }

    public static bool has_loaded(){
        return loaded;
    }

    public static GlobalState get_state(){
        return m_state;
    }
}

