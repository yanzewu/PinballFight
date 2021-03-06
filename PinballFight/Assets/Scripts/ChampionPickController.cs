using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ChampionPickController : MonoBehaviour {

    public int item_count = 3;
    List<GameObject> buttons = new List<GameObject>();

    public string[] image_filenames;

    public string[] texts;

    private void Awake() {
        
        for (int i = 0; i < texts.Length; i++){
            texts[i] =  texts[i].Replace('&', '\n');
        }

        GameObject.Find("BtnQuit").GetComponent<Button>().onClick.AddListener(delegate{SceneManager.LoadScene("Entry");});
        GameObject.Find("BtnContinue").GetComponent<Button>().onClick.AddListener(delegate{SceneManager.LoadScene("MapPick");});

        var ph = GameObject.Find("PanelHost");
        var pu0 = GameObject.Find("PanelUnit");
        var pu0_rt = pu0.GetComponent<RectTransform>();
        //pu0_rt.sizeDelta = new Vector2(pu0_rt.sizeDelta.x, -(float)ph.GetComponent<RectTransform>().sizeDelta.y/item_count);
        var h = pu0.GetComponent<RectTransform>().rect.height;

        for (int i = 0; i < item_count; i++){
            GameObject pu_new = Instantiate(pu0);

            pu_new.transform.SetParent(ph.transform, false);
            pu_new.transform.localPosition += new Vector3(0, -i*h, 0);
            int j = i;

            pu_new.GetComponent<Toggle>().onValueChanged.AddListener(b => {if (b) champion_choosed(j+1);});
            pu_new.transform.GetChild(0).gameObject.GetComponent<Text>().text = texts[i];
            var img = pu_new.transform.GetChild(1).gameObject.GetComponent<Image>();

            var texture = ResManager.load_runtime_image(image_filenames[i]);
            img.sprite = Sprite.Create(
                texture, new Rect(0, 0, texture.width, texture.height), img.sprite.pivot, img.sprite.pixelsPerUnit
            );

            buttons.Add(pu_new);
        }
        buttons[StatManager.get_state().champion[0]-1].GetComponent<Toggle>().Select();

        pu0.SetActive(false);
    }

    private void champion_choosed(int index){
        Debug.Log(index);
        StatManager.get_state().champion[0] = index;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
            SceneManager.LoadScene("Entry");
        }

        if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.tag != "Selectable"){
            EventSystem.current.SetSelectedGameObject(buttons[StatManager.get_state().champion[0]-1]);
        }
    }

}