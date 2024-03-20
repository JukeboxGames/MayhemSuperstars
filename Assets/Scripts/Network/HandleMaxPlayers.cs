using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// Legacy
public class HandleMaxPlayers : MonoBehaviour
{

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = DefaultApprovalCheck;
    }

    public void SearchGM()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().changedNumberOfPlayers.AddListener(ChangeApprovalCheck);
    }

    private void ChangeApprovalCheck()
    {
        if (GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().numberOfPlayers.Value >= 4)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = GameFullApprovalCheck;
        }
    }


    private void DefaultApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = true;
        /*if (NetworkManager.Singleton.IsServer) {
            response.Approved = true;
        } else {
            if (GameObject.FindGameObjectWithTag("GameManager") != null) {
                response.Approved = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().CheckAvailability();
            }
        }*/


        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Max number of players has been reached";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }

    private void GameFullApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;

        // Your approval logic determines the following values
        response.Approved = false;
        /*if (NetworkManager.Singleton.IsServer) {
            response.Approved = true;
        } else {
            if (GameObject.FindGameObjectWithTag("GameManager") != null) {
                response.Approved = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().CheckAvailability();
            }
        }*/


        response.CreatePlayerObject = false;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

        // Position to spawn the player object (if null it uses default of Vector3.zero)
        response.Position = Vector3.zero;

        // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
        response.Rotation = Quaternion.identity;

        // If response.Approved is false, you can provide a message that explains the reason why via ConnectionApprovalResponse.Reason
        // On the client-side, NetworkManager.DisconnectReason will be populated with this message via DisconnectReasonMessage
        response.Reason = "Max number of players has been reached";

        // If additional approval steps are needed, set this to true until the additional steps are complete
        // once it transitions from true to false the connection approval response will be processed.
        response.Pending = false;
    }
}
