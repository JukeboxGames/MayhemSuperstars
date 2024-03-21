using UnityEngine;

[System.Serializable]
public class PairedProp {
    public GameObject placedProp; 
    public GameObject ghostProp; 

    public PairedProp(GameObject first, GameObject second){
        placedProp = first; 
        ghostProp = second; 
    }
}

[CreateAssetMenu(menuName ="ScriptableObjects/Containers/Props")]

public class SO_Props : ScriptableObject
{
    public PairedProp[] props; 
}
