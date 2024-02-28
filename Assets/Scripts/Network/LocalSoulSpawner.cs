using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine;
using Unity.Netcode;

public class LocalSoulSpawner : NetworkBehaviour
{

    public static LocalSoulSpawner Instance
    {
        get;
        private set;
    }

    [SerializeField] GameObject soulPrefab;

    void Awake () {

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

    void usedControl(InputControl inputControl, InputEventPtr eventptr) {
        // Only react to button presses on unpaired devices.
        if (!(inputControl is ButtonControl)) {
            return;
        }

        SpawnSoulServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnSoulServerRpc (ServerRpcParams serverRpcParams = default) {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        GameObject soul = Instantiate(soulPrefab);
        soul.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
    }
}
