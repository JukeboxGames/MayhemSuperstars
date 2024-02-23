using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerSoul : NetworkBehaviour
{
    private GameObject vessel;
    [SerializeField] private SO_Characters characterSO;
    [SerializeField] private SO_Maps mapsSO;
    private int debugCharacterIndex = 0;
    [SerializeField] private GameObject si;

    private void OnEnable() {
        //ChangeCharacter(0);
        //Instantiate(si);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: Add MAP LIST to call for CHANGE CHARACTER on scene loaded
        if (Array.Exists(mapsSO.mapNames, element => element == scene.name)) {
            ChangeCharacter(0);
        }
    }

    public void ChangeCharacter (int index) {
        Vector2 currentPosition;
        if (vessel != null) {
            currentPosition = vessel.transform.position;
            Destroy(vessel.gameObject);
        } else {
            currentPosition = Vector2.zero;
        }

        vessel = Instantiate(characterSO.characterPrefabs[index], currentPosition, Quaternion.identity);
        vessel.GetComponent<PlayerController>().InitializeInput(GetComponent<PlayerInput>());
        SpawnVesselServerRpc();
    }

    private void Update() {
        if (Input.GetKeyDown("e")) {
            if (debugCharacterIndex == 5) {
                debugCharacterIndex = 0;
            } else {
                debugCharacterIndex++;
            }
            ChangeCharacter(debugCharacterIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVesselServerRpc () {
        vessel.GetComponent<NetworkObject>().Spawn();
    }
}
