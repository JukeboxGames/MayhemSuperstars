using UnityEngine;

public interface IInteractable
{
    // Receives optional parameter vessel
    public void Interact (GameObject vessel = null);
}