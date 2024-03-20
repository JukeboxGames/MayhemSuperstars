using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal_ChangeCharacter : MonoBehaviour, IInteractable
{
    [SerializeField] private SO_Characters characterSO;

    // Change player character
    public void Interact (GameObject vessel = null){
        PlayerSoul soul = vessel?.GetComponent<PlayerController>().playerSoul;
        if (soul != null) {
            soul.characterIndex++;
            if (soul.characterIndex >= characterSO.characterPrefabs.Length) {
                soul.characterIndex = 0;
            }
            soul.ChangeCharacter(soul.characterIndex);
        }
    }

    // Subscribe or desubscribe from the players interactable list
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            if (!other.gameObject.GetComponent<PlayerController>().interactables.Contains(this.gameObject)) {
                other.gameObject.GetComponent<PlayerController>().interactables.Add(this.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            if (other.gameObject.GetComponent<PlayerController>().interactables.Contains(this.gameObject)) {
                other.gameObject.GetComponent<PlayerController>().interactables.Remove(this.gameObject);
            }
        }
    }
}
