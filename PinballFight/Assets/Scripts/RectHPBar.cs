using UnityEngine;

public class RectHPBar : MonoBehaviour {

    Texture2D empty_texture;
    public Color color = Color.white;
    public int orientation = 0;
    public Vector2 padding = new Vector2(0.1f, 0.1f);
    public bool front = true;
    GameObject ghost;
    Vector2 size;


    private void Start() {
        size = GetComponent<SpriteRenderer>().sprite.rect.size;
        empty_texture = ResManager.load_runtime_image("empty");
        
        ghost = new GameObject("HPGhost");
        var sr = ghost.AddComponent<SpriteRenderer>();
        sr.color = color;
        sr.sprite = Sprite.Create(
            empty_texture,
            new Rect(0, 0, size.x, size.y),
            new Vector2(0.5f, 0.5f),
            100
        );
        ghost.transform.SetParent(this.gameObject.transform);
        if (front){
            ghost.transform.localPosition = new Vector3(0, 0, -0.01f);
        }
        else{
            ghost.transform.localPosition = new Vector3(0, 0, 0.01f);
        } 
    }

    public void set_hp(float hp){
        if (orientation == 0){
            ghost.transform.localPosition = new Vector3(
                (-0.5f+hp/2) * size.x/100, 0f, ghost.transform.localPosition.z
            );
            ghost.transform.localScale = new Vector3(hp, 1, 1);
        }
        else if (orientation == 1){
            ghost.transform.localPosition = new Vector3(
                0f, (-0.5f+hp/2) * size.y/100, ghost.transform.localPosition.z
            );
            ghost.transform.localScale = new Vector3(1, hp, 1);
        }
    }

}