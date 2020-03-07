using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager {

    GameController controller;

    GameObject gameover_panel;

    public void initialize(){
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        GameObject.Find("BtnRestart2").GetComponent<Button>().onClick.AddListener(this.on_click_restart);

        gameover_panel = GameObject.Find("PanelGameOver");
        gameover_panel.SetActive(false);
    }


    public void on_gameover(bool win){
        gameover_panel.SetActive(true);
        if (win){
            GameObject.Find("TextGameOver").GetComponent<Text>().text = "You win!";
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

}