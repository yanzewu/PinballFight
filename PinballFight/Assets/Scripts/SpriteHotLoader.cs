using UnityEngine;

public class SpriteHotLoader : MonoBehaviour {

    public string filename;
    public float pfu = 100;

    public void load(){
        
        Texture2D spriteTexture = ResManager.load_runtime_image(filename);
        Sprite newSprite = Sprite.Create(
            spriteTexture,
            new Rect(0, 0, spriteTexture.width, spriteTexture.height),
            new Vector2(0.5f, 0.5f),
            pfu);

        // CHANGE TO ADDCOMPONENT to improve performance, once debug passed
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

}