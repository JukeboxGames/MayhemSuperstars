using System;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.VisualScripting;

public class PlayerSoul : NetworkBehaviour
{
    [HideInInspector] public GameObject vessel;
    [SerializeField] private SO_Characters characterSO;
    [SerializeField] private SO_Maps mapsSO;
    [SerializeField] private PlayerCamera playerCameraComponent;
    [SerializeField] private PlayerUI playerUiComponent;
    Vector2 currentPosition;
    [HideInInspector] public int characterIndex = 0;
    private bool joined = false;
    public NetworkVariable<int> playerNumber = new NetworkVariable<int>();
    
    private void OnEnable () {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        if (IsOwner) {
            StartCoroutine(WaitToJoin());
        } else {
            Destroy(gameObject.GetComponent<PlayerInput>());
        }
    }

    IEnumerator WaitToJoin () {
        while (!joined) {
            yield return new WaitForSeconds(0.1f);
            if (GameObject.FindGameObjectWithTag("GameManager") != null ) {
                joined = true;
                JoinPlayerServerRpc(gameObject);
            }
        }
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Ejecutar funciones de escena de juego
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // TODO: Add MAP LIST to call for CHANGE CHARACTER on scene loaded
        if (Array.Exists(mapsSO.mapNames, element => element == scene.name)) {
            StartCoroutine(WaitToChangeCharacter(characterIndex));
        }

        if (scene.name == "Lobby") {
            StartCoroutine(WaitToChangeCharacter(0));
        }
    }

    private IEnumerator WaitToChangeCharacter (int i) {
        while (!joined){
            yield return new WaitForEndOfFrame();
        }
        
        ChangeCharacter(i);
    }

    public void ChangeCharacter (int index) {
        if (IsOwner) {
            if (vessel != null) {
                currentPosition = vessel.transform.position;
                SpawnVesselServerRpc(gameObject, GetComponent<NetworkObject>().OwnerClientId, index, vessel, currentPosition.x, currentPosition.y);
            } else {
                currentPosition = Vector2.zero;
                SpawnVesselServerRpc(gameObject, GetComponent<NetworkObject>().OwnerClientId, index, currentPosition.x, currentPosition.y);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void JoinPlayerServerRpc (NetworkObjectReference soul) {
        GameManager gm = GameObject.FindGameObjectWithTag("GameManager")?.GetComponent<GameManager>();
        int pNum = gm.JoinPlayer(soul);
        playerNumber.Value = pNum;
        if (soul.TryGet(out NetworkObject obj)){
            ConfirmPlayerNumberClientRpc(obj, pNum);
        }
        
    }

    [ClientRpc]
    void ConfirmPlayerNumberClientRpc(NetworkObjectReference soul, int pNum){
        if (soul.TryGet(out NetworkObject obj)){
            StartCoroutine(obj.gameObject.GetComponent<PlayerSoul>().WaitToChangeCharacter(0));
            playerUiComponent.SetPlayerEvents(pNum);
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVesselServerRpc (NetworkObjectReference soul, ulong clientId, int index, float x, float y) {
        GameObject clientVessel = Instantiate(characterSO.characterPrefabs[index], currentPosition, Quaternion.identity);
        clientVessel.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        clientVessel.transform.position = new Vector2(x,y);
        GetControlClientRpc(clientVessel, clientId);
        if (soul.TryGet(out NetworkObject soulGo))
        {
            soulGo.gameObject.GetComponent<PlayerSoul>().vessel = clientVessel;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnVesselServerRpc (NetworkObjectReference soul, ulong clientId, int index, NetworkObjectReference vess, float x, float y) {
        GameObject clientVessel = Instantiate(characterSO.characterPrefabs[index], currentPosition, Quaternion.identity);
        clientVessel.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
        clientVessel.transform.position = new Vector2(x,y);
        GetControlClientRpc(clientVessel, clientId);
        if (soul.TryGet(out NetworkObject soulGo))
        {
            soulGo.gameObject.GetComponent<PlayerSoul>().vessel = clientVessel;
        }

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
            playerCameraComponent.ChangeCameraTarget(vessel);
            vessel.transform.position = currentPosition;
            obj.gameObject.GetComponent<PlayerController>().InitializeInput(GetComponent<PlayerInput>());
        }
    }
}
