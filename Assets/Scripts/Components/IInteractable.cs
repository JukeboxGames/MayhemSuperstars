using UnityEngine;

// Used for objects able to be interacted with
// will be replaced with the abstract class Interactable
public interface IInteractable
{
    // Receives optional parameter vessel
    public void Interact (GameObject vessel = null);
}