using UnityEngine;

[CreateAssetMenu(menuName ="SO_UIHideShow")]
public class UIHideShow : ScriptableObject
{
    public void HideUIElement(GameObject obj){
        obj.SetActive(false);
    }

    public void ShowUIElement(GameObject obj){
        obj.SetActive(true);
    }
}