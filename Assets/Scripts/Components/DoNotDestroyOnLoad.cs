using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoNotDestroyOnLoad : MonoBehaviour
{
    void Awake (){
        DontDestroyOnLoad(this.gameObject);
    }
}
