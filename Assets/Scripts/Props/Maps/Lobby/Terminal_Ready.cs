using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Terminal_Ready : Interactable
{
    public override void Interact(GameObject vessel = null)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("DebugScene", LoadSceneMode.Single);
    }
}
