using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    void Awake () {
        if (instance != null && instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            instance = this; 
        } 
    }
}
