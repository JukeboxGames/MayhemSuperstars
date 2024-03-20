using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Do not destroy this object when loading new scenes
public class DoNotDestroyOnLoad : MonoBehaviour
{
    void Awake (){
        DontDestroyOnLoad(this.gameObject);
    }
}
