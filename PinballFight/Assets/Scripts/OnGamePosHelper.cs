
using UnityEngine;

public class OnGamePosHelper : MonoBehaviour {

    public Vector3 pos;
    private void Start() {
        var screen_pos = Camera.main.WorldToScreenPoint(pos);
        transform.position = new Vector3(screen_pos.x, screen_pos.y, pos.z);
    }
}