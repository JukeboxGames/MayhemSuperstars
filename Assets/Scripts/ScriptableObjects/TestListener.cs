using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListener : MonoBehaviour
{
    public TestSO scriptable;

    private void OnEnable() {
        scriptable.testEvent.AddListener(WriteInConsole);
    }

    private void OnDisable() {
        scriptable.testEvent.RemoveListener(WriteInConsole);
    }

    private void WriteInConsole(){
        Debug.Log("Hello! This works!");
    }
}
