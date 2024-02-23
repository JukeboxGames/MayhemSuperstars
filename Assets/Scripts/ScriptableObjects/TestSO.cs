using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Testing/Test Scriptable Object")]
public class TestSO : ScriptableObject
{
    public UnityEvent testEvent;

    public void triggerTestEvent(){
        testEvent.Invoke();
    }
}