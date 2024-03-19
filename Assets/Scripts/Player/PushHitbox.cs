using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PushHitbox : NetworkBehaviour
{
    public NetworkVariable<Vector2> direction = new NetworkVariable<Vector2>();
    public NetworkVariable<int> playerNumber = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(SelfDestruct());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {   
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player.playerNumber.Value != playerNumber.Value) {
                player.AddInstantForce(direction.Value * 10f, 0.3f); 
            }
        } else if (collision.gameObject.GetComponent<IPushable>() != null) {
            collision.gameObject.GetComponent<IPushable>().AddInstantForce(direction.Value * 10f, 0.3f);
        }
    }

    IEnumerator SelfDestruct () {
        yield return new WaitForSeconds(0.3f);
        if (IsServer) {
            gameObject.GetComponent<NetworkObject>().Despawn();
        }
    }
}
