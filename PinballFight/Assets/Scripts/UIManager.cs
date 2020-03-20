using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager {

    GameController controller;

    GameObject gameover_panel;
    GameObject info_panel;

    public void initialize(){
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        GameObject.Find("BtnQuit").GetComponent<Button>().onClick.AddListener(this.on_click_quit);
        GameObject.Find("BtnQuit2").GetComponent<Button>().onClick.AddListener(this.on_click_quit);
        GameObject.Find("BtnRestart").GetComponent<Button>().onClick.AddListener(this.on_click_restart);
        GameObject.Find("BtnRestart2").GetComponent<Button>().onClick.AddListener(this.on_click_restart);
        GameObject.Find("BtnInfo").GetComponent<Button>().onClick.AddListener(this.on_click_info);
        GameObject.Find("BtnCloseInfo").GetComponent<Button>().onClick.AddListener(this.on_click_closeinfo);

        gameover_panel = GameObject.Find("PanelGameOver");
        gameover_panel.SetActive(false);

        info_panel = GameObject.Find("PanelInfo");
        info_panel.SetActive(false);

        // aspect
        if (Screen.height > 16/9.0f * Screen.width){
            Camera.main.orthographicSize = 5.0f * Screen.height / (float)Screen.width * (9/16.0f);
        }
        
    }


    public void on_gameover(bool win){
        gameover_panel.SetActive(true);
        if (win){
            GameObject.Find("TextGameOver").GetComponent<Text>().text = "Victory!";
        }
        else{
            GameObject.Find("TextGameOver").GetComponent<Text>().text = "Game over!";
        }
    }

    public void on_click_restart(){
        gameover_panel.SetActive(false);
        controller.restart_game();
    }

    public void on_click_pause(){

    }

    public void on_click_quit(){
        controller.exit_game();
    }

    public void on_click_info(){
        info_panel.SetActive(true);
        controller.pause_game();
    }
    public void on_click_closeinfo(){
        info_panel.SetActive(false);
        controller.resume_game();
    }

}