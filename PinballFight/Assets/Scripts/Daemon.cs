using UnityEngine;

public class Daemon : MonoBehaviour {


    private static Daemon instance = null;
    public static Daemon Instance {
        get {return instance;}
    }

    private void Awake() {
        if (instance != null && instance != this){
            Destroy(this.gameObject);
            return;
        }
        else{
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}