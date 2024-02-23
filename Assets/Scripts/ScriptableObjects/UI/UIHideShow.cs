using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/Network/UIHideShow")]
public class UIHideShow : ScriptableObject
{
    public void HideUIElement(GameObject obj){
        obj.SetActive(false);
    }

    public void ShowUIElement(GameObject obj){
        obj.SetActive(true);
    }
}