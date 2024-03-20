using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cheeseman : PlayerController
{
    // Character stats
    public override int characterSpeed { get { return 3; } }
    public override int characterMaxHealth { get { return 6; } }
    public override int characterFireRate { get { return 3; } }
    public override int characterDamage { get { return 3; } }
    public override float characterAbilityCooldown { get { return 5f; } }
    int cheeseBulletIndex = 1;

    [SerializeField] private GameObject cheeseBulletPrefab;

    public override void CastSpecialAbility()
    {
        // If cooldown has ellapsed
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown))
        {
            // Get shoot direction
            Vector2 direction;
            if (playerInput.currentControlScheme == "Keyboard")
            {
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
                direction = worldMousePos - transform.position;
            }
            else
            {
                direction = input_ShootDirection;
            }

            // Return if there's no shooting input
            if (direction == Vector2.zero) return;
            direction.Normalize();

            SpawnCheeseBulletServerRpc(direction, transform.position);

            // Update cooldown time
            timeSinceLastAbility = Time.time;
        }
    }

    // Call server to spawn cheese bullet
    // TODO: Make client authoritative
    [ServerRpc]
    void SpawnCheeseBulletServerRpc(Vector2 direction, Vector2 startPos)
    {
        GameObject cheeseBulletInstance;
        cheeseBulletInstance = Instantiate(so_Bullets.bulletPrefabs[cheeseBulletIndex], transform.position, Quaternion.identity);
        cheeseBulletInstance.GetComponent<PlayerBullet>().StartBullet(10, direction);
        cheeseBulletInstance.GetComponent<NetworkObject>().Spawn();
    }
}
