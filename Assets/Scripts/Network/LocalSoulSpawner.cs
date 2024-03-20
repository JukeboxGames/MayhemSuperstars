using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

// Class that checks for controller input to join players
public class LocalSoulSpawner : NetworkBehaviour
{
    public static LocalSoulSpawner Instance
    {
        get;
        private set;
    }

    [SerializeField] GameObject soulPrefab;

    void Awake()
    {
        // Singleton behaviour
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Activate support for listening to device activity.
        ++InputUser.listenForUnpairedDeviceActivity;

        // When a button on an unpaired device is pressed, pair the device to a new
        // or existing user.
        InputUser.onUnpairedDeviceUsed += usedControl;
    }

    void usedControl(InputControl inputControl, InputEventPtr eventptr)
    {
        // Only react to button presses on unpaired devices.
        if (!(inputControl is ButtonControl))
        {
            return;
        }

        // Only join player if the game is in the lobby
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            return;
        }

        if (GameObject.FindGameObjectWithTag("GameManager") == null)
        {
            return;
        }

        GameObject gm = GameObject.FindGameObjectWithTag("GameManager");

        // Do not join player if there is no player slots
        if (!gm.GetComponent<GameManager>().CheckAvailability())
        {
            return;
        }

        SpawnSoulServerRpc();
    }

    // Spawn Player Soul
    [ServerRpc(RequireOwnership = false)]
    void SpawnSoulServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        GameObject soul = Instantiate(soulPrefab);
        soul.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
