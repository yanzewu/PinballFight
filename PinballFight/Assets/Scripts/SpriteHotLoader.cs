using System.Collections.Generic;
using UnityEngine;

public class SpriteHotLoader : MonoBehaviour {

    public string filename;
    public float pfu = 100;
    public static Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public List<string> filenames = new List<string>();

    public void load(int idx, bool direct=false){
        filename = filenames[idx];
        load(direct);
    }

    public void load(bool direct=true){
        if (direct){
            load_direct();
        }
        else{
            if (!cache.ContainsKey(filename)){
                cache[filename] = load_sprite(filename, pfu);
            }
            apply(cache[filename]);
        }
    }

    public void load_direct(){
        Sprite newSprite = SpriteHotLoader.load_sprite(filename, pfu);

        // CHANGE TO ADDCOMPONENT to improve performance, once debug passed
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    public static Sprite load_sprite(string filename, float pfu){
        Texture2D spriteTexture = ResManager.load_runtime_image(filename);
        if (spriteTexture is null){
            Debug.Log(filename);
        }
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