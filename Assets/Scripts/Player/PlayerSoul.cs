using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerSoul : NetworkBehaviour
{
    private GameObject vessel;
    [SerializeField] private SO_Characters characterSO;
    [SerializeField] private SO_Maps mapsSO;
    private int debugCharacterIndex = 0;
    Vector2 currentPosition;
    
    private void OnEnable () {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (!IsOwner) {
            Destroy(gameObject.GetComponent<PlayerInput>());
        }
        StartCoroutine(WaitToChangeCharacter(0));
    }



    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: Add MAP LIST to call for CHANGE CHARACTER on scene loaded
        if (Array.Exists(mapsSO.mapNames, element => element == scene.name)) {
            StartCoroutine(WaitToChangeCharacter(0));
        }
    }

    private IEnumerator WaitToChangeCharacter (int i) {
        yield return new WaitForEndOfFrame();
        ChangeCharacter(i);
    }

    public void ChangeCharacter (int index) {
        if (IsOwner) {
            if (vessel != null) {
                currentPosition = vessel.transform.position;
                SpawnVesselServerRpc(GetComponent<NetworkObject>().OwnerClientId, index, vessel, currentPosition.x, currentPosition.y);
            } else {
                currentPosition = Vector2.zero;
                SpawnVesselServerRpc(GetComponent<NetworkObject>().OwnerClientId, index, currentPosition.x, currentPosition.y);
            }
        }
    }

    private void Update() {
    if (!IsOwner) {
            return;
        }
        if (Input.GetKeyDown("e")) {
            if (debugCharacterIndex == 5) {
                debugCharacterIndex = 0;
            } else {
                debugCharacterIndex++;
            }
            StartCoroutine(WaitToChangeCharacter(debugCharacterIndex));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVesselServerRpc (ulong clientId, int index, float x, float y) {
        GameObject clientVessel = Instantiate(characterSO.characterPrefabs[index], currentPosition, Quaternion.identity);
        clientVessel.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        clientVessel.transform.position = new Vector2(x,y);
        GetControlClientRpc(clientVessel, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVesselServerRpc (ulong clientId, int index, NetworkObjectReference vess, float x, float y) {
        GameObject clientVessel = Instantiate(characterSO.characterPrefabs[index], currentPosition, Quaternion.identity);
        clientVessel.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        clientVessel.transform.position = new Vector2(x,y);
        GetControlClientRpc(clientVessel, clientId);
        if (vess.TryGet(out NetworkObject obj))
        {
            obj.Despawn();
        }
    }

    [ClientRpc]
    void GetControlClientRpc (NetworkObjectReference vess, ulong clientId) {
        // TODO: Check whose local player this is
        if (GetComponent<NetworkObject>().OwnerClientId != clientId || !IsOwner) {
            return;
        }

        if (vess.TryGet(out NetworkObject obj))
        {
            vessel = obj.gameObject;
            vessel.transform.position = currentPosition;
            obj.gameObject.GetComponent<PlayerController>().InitializeInput(GetComponent<PlayerInput>());
        }
    }  
}
