using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public GameObject[] scenePanel;

    public void TurnOnScene(GameObject sceneTurnOn){
        for(int i = 0; i < scenePanel.Length; i++){
            if(scenePanel[i] != sceneTurnOn){
                scenePanel[i].SetActive(false);
            } else {
                scenePanel[i].SetActive(true);
            }
        }
    }

    public void turnOffAllScene(){
        for(int i = 0; i < scenePanel.Length; i++){
            scenePanel[i].SetActive(false);
        }
    }
}
