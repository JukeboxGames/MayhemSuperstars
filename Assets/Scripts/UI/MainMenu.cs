using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void HideUIElement(GameObject obj){
        obj.SetActive(false);
    }

    public void ShowUIElement(GameObject obj){
        obj.SetActive(true);
    }
}
