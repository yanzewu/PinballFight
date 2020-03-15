using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EntryController : MonoBehaviour {
    private void Awake() {
        GameObject.Find("BtnStart").GetComponent<Button>().onClick.AddListener(this.on_click_start);
        GameObject.Find("BtnQuit").GetComponent<Button>().onClick.AddListener(this.on_click_quit);

        var panel_gamemode = GameObject.Find("PanelGameMode");
        

        
        GameObject.Find("BtnClosePanel").GetComponent<Button>().onClick.AddListener(delegate{panel_gamemode.SetActive(false);});

        StatManager.load_stat();

        var sp = GameObject.Find("BtnSinglePlayer").GetComponent<Toggle>();
        var mp = GameObject.Find("BtnMultiPlayer").GetComponent<Toggle>();
        var easy = GameObject.Find("BtnEasy").GetComponent<Toggle>();
        var hard = GameObject.Find("BtnHard").GetComponent<Toggle>();

        Debug.Log(StatManager.get_state().game_mode);
        Debug.Log(StatManager.get_state().bot_level);

        sp.onValueChanged.AddListener(b=>{if(b) StatManager.get_state().game_mode = 0;});
        mp.onValueChanged.AddListener(b=>{if(b) StatManager.get_state().game_mode = 1;});
        easy.onValueChanged.AddListener(b=>{if(b) StatManager.get_state().bot_level = 0;});
        hard.onValueChanged.AddListener(b=>{if(b) StatManager.get_state().bot_level = 2;});

        GameObject.Find("BtnGameMode").GetComponent<Button>().onClick.AddListener(() => {
            panel_gamemode.SetActive(true);
            if (StatManager.get_state().game_mode == 0) sp.isOn = true; else mp.isOn = true;
            if (StatManager.get_state().bot_level == 2) hard.isOn = true; else easy.isOn = true;
        });

        panel_gamemode.SetActive(false);
    }

    void on_click_start(){
        SceneManager.LoadScene("MapPick");
    }

    void on_click_quit(){
        Application.Quit();
    }
}