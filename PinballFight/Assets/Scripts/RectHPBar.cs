using UnityEngine;

public class RectHPBar : MonoBehaviour {

    Texture2D empty_texture;
    public Color color = Color.white;
    public int orientation = 0;
    public Vector2 padding = new Vector2(0.1f, 0.1f);
    public bool front = true;
    public float pfu = 100;
    public string filename = "empty";
    GameObject ghost;
    Vector2 size;

    private void Awake() {
        empty_texture = ResManager.load_runtime_image(filename);
    }

    private void Start() {
        size = GetComponent<SpriteRenderer>().sprite.rect.size;
        
        ghost = new GameObject("HPGhost");
        var sr = ghost.AddComponent<SpriteRenderer>();
        sr.color = color;

        Vector2 pivot = Vector2.zero;
        if (orientation == 0 || orientation == 2) pivot = new Vector2(0, 0.5f);
        else if (orientation == 1 || orientation == 3) pivot = new Vector2(0.5f, 0);

        sr.sprite = Sprite.Create(
            empty_texture,
            new Rect(0, 0, size.x, size.y),
            pivot,
            pfu
        );
        ghost.transform.SetParent(this.gameObject.transform);
        if (orientation > 1) ghost.transform.Rotate(0, 0, 180);
        float zindex = front ? -0.01f:0.01f;

        if (orientation == 0 || orientation == 2){
            ghost.transform.localPosition = new Vector3(-size.x / 2 / pfu * (1 - 2 * padding.x), 0, zindex);
        }
        else if (orientation == 1 || orientation == 3){
            ghost.transform.localPosition = new Vector3(0, -size.y / 2 / pfu * (1 - 2 * padding.y), zindex);
        }
    }

    public void set_hp(float hp){
        if (orientation == 0 || orientation == 2){
            /*
            ghost.transform.localPosition = new Vector3(
                (-0.5f+hp/2) * size.x/100, 0f, ghost.transform.localPosition.z
            );*/
            ghost.transform.localScale = new Vector3(hp * (1- 2* padding.x), 1 - padding.y, 1);
        }
        else if (orientation == 1 || orientation == 3){
            /*ghost.transform.localPosition = new Vector3(
                0f, (-0.5f+hp/2) * size.y/100, ghost.transform.localPosition.z
            );*/
            ghost.transform.localScale = new Vector3(1 - padding.x*2, hp * (1 - 2* padding.y), 1);
        }
    }

}