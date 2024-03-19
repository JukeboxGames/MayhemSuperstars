using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Terminal_Ready : MonoBehaviour, IInteractable
{
    public void Interact(GameObject vessel = null)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("DebugScene", LoadSceneMode.Single);
    }

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
