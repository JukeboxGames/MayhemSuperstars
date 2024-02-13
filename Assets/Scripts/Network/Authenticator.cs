using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Networking.Transport.Relay;

using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

using Unity.Services.Core;
using Unity.Services.Authentication;

public class Authenticator : MonoBehaviour
{
    private async void Start() {
        await UnityServices.InitializeAsync();
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
