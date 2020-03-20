using UnityEngine;

public class BounceBoard : MonoBehaviour {

    public float velocity_ampl;
    public float velocity_max;

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Ball"){
            var rb = other.gameObject.GetComponent<Rigidbody2D>();
            if (rb.velocity.magnitude < velocity_max / velocity_ampl){
                rb.velocity *= velocity_ampl;
            }
        }
    }

}