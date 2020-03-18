using UnityEngine;

public class BounceBoard : MonoBehaviour {

    public float velocity_ampl;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Ball"){
            other.gameObject.GetComponent<Rigidbody2D>().velocity *= velocity_ampl;
        }
    }

}