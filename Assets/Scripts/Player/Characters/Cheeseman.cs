using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cheeseman : PlayerController
{
    public override int characterSpeed { get{return 3;} }
    public override int characterMaxHealth { get{return 6;} }
    public override int characterFireRate { get{return 3;} }
    public override int characterDamage { get{return 3;} }
    public override float characterAbilityCooldown { get{return 5f;} }
    int cheeseBulletIndex = 1;

    [SerializeField] private GameObject cheeseBulletPrefab;

    public override void CastSpecialAbility() {
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            Vector2 direction;
            if (playerInput.currentControlScheme == "Keyboard") {
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
                direction = worldMousePos - transform.position;
            } else {
                direction = input_ShootDirection;
            }

            if (direction == Vector2.zero) return;

            // Encontrar dirección de disparo
            direction.Normalize();

            SpawnCheeseBulletServerRpc(direction, transform.position);

            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }

    [ServerRpc]
    void SpawnCheeseBulletServerRpc(Vector2 direction, Vector2 startPos){
        GameObject cheeseBulletInstance;
        cheeseBulletInstance = Instantiate(so_Bullets.bulletPrefabs[cheeseBulletIndex], transform.position, Quaternion.identity);
        cheeseBulletInstance.GetComponent<PlayerBullet>().StartBullet(10, direction);
        cheeseBulletInstance.GetComponent<NetworkObject>().Spawn();
    }
}
