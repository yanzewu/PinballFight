using UnityEngine;

public class BounceValueIndicator : MonoBehaviour {

    public int player_id;
    SpriteHotLoader sr;

    private void Awake() {
        sr = GetComponent<SpriteHotLoader>();
    }

    public void set_length(int length){
        //GetComponent<RectHPBar>().set_hp(length);
        sr.load(player_id * 6 + length);
        
    }

    private void Update() {
        
    }

}