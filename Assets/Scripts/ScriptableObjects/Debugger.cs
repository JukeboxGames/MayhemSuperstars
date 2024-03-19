using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Debug/Debugger")]
public class Debugger : ScriptableObject
{
    public void DebugMessage(string message) {
        Debug.Log(message);
    }
}
