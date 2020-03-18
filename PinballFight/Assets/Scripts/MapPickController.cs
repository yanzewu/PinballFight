using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class MapPickController : MonoBehaviour {

    ScrollRect scrollRect;
    public int item_count = 2;
    public string[] image_filenames;
    public string[] texts;

    List<GameObject> buttons = new List<GameObject>();

    private void Awake() {
        
        GameObject.Find("BtnQuit").GetComponent<Button>().onClick.AddListener(delegate{SceneManager.LoadScene("ChampionPick");});
        GameObject.Find("BtnContinue").GetComponent<Button>().onClick.AddListener(delegate{SceneManager.LoadScene("Main");});

        
        var pu0 = GameObject.Find("PanelUnit");
        var w = pu0.GetComponent<RectTransform>().rect.width;

        var ps = GameObject.Find("PanelSlider");
        var ps_rt = ps.GetComponent<RectTransform>();
        ps_rt.sizeDelta = new Vector2(w * item_count, ps_rt.sizeDelta.y);

        scrollRect = GameObject.Find("PanelHost").GetComponent<ScrollRect>();

        for (int i = 0; i < item_count; i++){
            GameObject pu_new = Instantiate(pu0);

            pu_new.transform.SetParent(ps.transform, false);
            pu_new.transform.localPosition += new Vector3(i*w, 0, 0);
            int j = i;

            pu_new.GetComponent<Toggle>().onValueChanged.AddListener(b => {if (b) map_choosed(j);});
            pu_new.transform.GetChild(0).gameObject.GetComponent<Text>().text = texts[i];
            var img = pu_new.transform.GetChild(1).gameObject.GetComponent<Image>();

            var texture = ResManager.load_runtime_image(image_filenames[i]);
            img.sprite = Sprite.Create(
                texture, new Rect(0, 0, texture.width, texture.height), img.sprite.pivot, img.sprite.pixelsPerUnit
            );

            buttons.Add(pu_new);
        }

        int cm = StatManager.get_state().current_map;
        buttons[cm].GetComponent<Toggle>().Select();
        scrollRect.normalizedPosition = new Vector2((float)cm/(item_count-1), 0);

        pu0.SetActive(false);
    }

    private void map_choosed(int index){
        StatManager.get_state().current_map = index;
        scrollRect.normalizedPosition = new Vector2((float)index/(item_count-1), 0);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("ChampionPick");
        }

        if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.tag != "Selectable"){
            EventSystem.current.SetSelectedGameObject(buttons[StatManager.get_state().current_map]);
        }
    }

}