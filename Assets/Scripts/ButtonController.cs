using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public void TurnOffScene(GameObject scene){
        scene.SetActive(false);
    }

    public void TurnOnScene(GameObject scene){
        scene.SetActive(true);
    }
}
