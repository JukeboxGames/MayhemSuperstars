using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TO-DO: Change interactables from interface to abstract class
public abstract class Interactable : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!other.gameObject.GetComponent<PlayerController>().interactables.Contains(this.gameObject))
            {
                other.gameObject.GetComponent<PlayerController>().interactables.Add(this.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerController>().interactables.Contains(this.gameObject))
            {
                other.gameObject.GetComponent<PlayerController>().interactables.Remove(this.gameObject);
            }
        }
    }

    public abstract void Interact(GameObject vessel = null);
}
