using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
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

    public abstract void Interact(GameObject vessel = null);
}
