using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal_ChangeCharacter : Interactable
{
    [SerializeField] private SO_Characters characterSO;

    public override void Interact (GameObject vessel = null){
        PlayerSoul soul = vessel?.GetComponent<PlayerController>().playerSoul;
        if (soul != null) {
            soul.characterIndex++;
            if (soul.characterIndex >= characterSO.characterPrefabs.Length) {
                soul.characterIndex = 0;
            }
            soul.ChangeCharacter(soul.characterIndex);
        }
        
    }
}
