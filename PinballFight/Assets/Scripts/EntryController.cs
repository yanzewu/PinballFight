using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class EntryController : MonoBehaviour {
    private void Awake() {
        GameObject.Find("BtnStart").GetComponent<Button>().onClick.AddListener(this.on_click_start);
        GameObject.Find("BtnQuit").GetComponent<Button>().onClick.AddListener(this.on_click_quit);
    }

    void on_click_start(){
        SceneManager.LoadScene("Main");
    }

    void on_click_quit(){
        Application.Quit();
    }
}