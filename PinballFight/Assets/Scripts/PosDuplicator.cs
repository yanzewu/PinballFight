using UnityEngine;

public class PosDuplicator : MonoBehaviour {

    public int index = 0;
    public Vector3 shift = Vector3.zero;

    private void Start() {
        transform.position += shift * index;
    }
}