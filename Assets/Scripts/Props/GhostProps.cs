using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostProps : MonoBehaviour
{
    private int propId {get; set;}
    private Collider2D collider2D;
    private SpriteRenderer[] sprites; 
    private Color[] colors; 
    [SerializeField] private SO_Props so_Props; 

    /*
    Awake 
        intatiete oneself in the world
        getCollider
        getSprites
        saveoriginalColor
    OnTriggerEnter
        if(tag is !GhostBox)
            For each turn sprites red
    OnTriggerLeave
        if(tag is !GhostBox)
            For each turn back sprites from red 
    OnPlace
        getCoincidingGameObject from scirptable and instantiate it
    */
    void OnPlace(){
        GameObject placedProp  = so_Props.props[propId].placedProp;
        Instantiate(placedProp, transform.position, transform.rotation);
    }
}
