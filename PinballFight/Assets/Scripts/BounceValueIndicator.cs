using UnityEngine;

public class BounceValueIndicator : MonoBehaviour {

    private void Start() {
        set_length(0);
    }

    public void set_length(float length){
        GetComponent<RectHPBar>().set_hp(length);
    }

    private void Update() {
        
    }

}