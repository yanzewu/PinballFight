using UnityEngine;


public class Transporter : MonoBehaviour {

    public float shift = 0.1f;
    private void OnTriggerEnter2D(Collider2D other) {
        var rb = other.gameObject.GetComponent<Rigidbody2D>();
        rb.position = new Vector2(-rb.position.x + (rb.position.x > 0 ? 1:-1) * shift, -rb.position.y);
    }
}