using UnityEngine;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;

[CreateAssetMenu(menuName ="ScriptableObjects/Network/NetworkStarter")]
public class SO_NetworkStarter : ScriptableObject, ISerializationCallbackReceiver
{
    #region "Starting Values"
    private string defaultJoinCode = "";
    #endregion

    public string joinCode;

    // Provide starting values to variables
    public void OnAfterDeserialize() {
        joinCode = defaultJoinCode;
    }

    public void OnBeforeSerialize() { }

    public void StartLocalGame () {
        try {
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData("127.0.0.1", (ushort)7777);
			NetworkManager.Singleton.StartHost();
		} catch (RelayServiceException e){
			Debug.Log(e);
		}
    }

    public async void HostPrivateGame () {
        try {
			// Crear sesión de juego y obtener código de sala de juego
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
			string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			joinCode = relayCode;
			RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartHost();
		} catch (RelayServiceException e){
			Debug.Log(e);
		}
    }

    public async void JoinPrivateGame (GameObject textfield) {
        string joinCode = textfield.GetComponent<TMP_InputField>().text;
        try {
			// Entrar a sesión de juego
			joinCode = joinCode.ToUpper();
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
			RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
			NetworkManager.Singleton.StartClient();
		} catch (RelayServiceException e){
			Debug.Log(e);
		}
    }

    public void HostPublicGame () {
        // TODO
    }
    

    public void JoinPublicGame () {
        // TODO
    }
}
