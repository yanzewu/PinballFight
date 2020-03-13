using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapPickController : MonoBehaviour {

    private void Awake() {
        
        GameObject.Find("BtnQuit").GetComponent<Button>().onClick.AddListener(delegate{SceneManager.LoadScene("Entry");});
        GameObject.Find("BtnContinue").GetComponent<Button>().onClick.AddListener(delegate{SceneManager.LoadScene("Main");});

        var ps = GameObject.Find("PanelSlider");
        var pu0 = GameObject.Find("PanelUnit");
        var w = pu0.GetComponent<RectTransform>().rect.width;

        pu0.GetComponent<Button>().onClick.AddListener(delegate{map_choosed(0);});

        for (int i = 1; i < 2; i++){
            var pu_new = Instantiate(pu0);
            pu_new.transform.SetParent(ps.transform, false);
            pu_new.transform.position = pu0.transform.position + new Vector3(w, 0, 0);
            int j = i;

            pu_new.GetComponent<Button>().onClick.AddListener(delegate{map_choosed(j);});
            pu_new.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Map " + (i+1).ToString();
            var img = pu_new.transform.GetChild(1).gameObject.GetComponent<Image>();

            var texture = ResManager.load_runtime_image("background/Background_Rem");
            img.sprite = Sprite.Create(
                texture, new Rect(0, 0, texture.width, texture.height), img.sprite.pivot, img.sprite.pixelsPerUnit
            );
        }
    }

    private void map_choosed(int index){
        StatManager.get_state().current_map = index;
    }

}