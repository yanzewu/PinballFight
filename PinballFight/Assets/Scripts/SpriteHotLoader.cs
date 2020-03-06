using UnityEngine;

public class SpriteHotLoader : MonoBehaviour {

    public string filename;
    public float pfu = 100;

    public void load(){
        Sprite newSprite = SpriteHotLoader.load_sprite(filename, pfu);

        // CHANGE TO ADDCOMPONENT to improve performance, once debug passed
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    public static Sprite load_sprite(string filename, float pfu){
        Texture2D spriteTexture = ResManager.load_runtime_image(filename);
        Sprite newSprite = Sprite.Create(
            spriteTexture,
            new Rect(0, 0, spriteTexture.width, spriteTexture.height),
            new Vector2(0.5f, 0.5f),
            pfu);
        return newSprite;
    }

    public void apply(Sprite newSprite){
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

}