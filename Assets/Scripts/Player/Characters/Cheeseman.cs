using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cheeseman : PlayerController
{
    public override int characterSpeed { get{return 3;} }
    public override int characterMaxHealth { get{return 6;} }
    public override int characterFireRate { get{return 3;} }
    public override int characterDamage { get{return 3;} }
    public override float characterAbilityCooldown { get{return 5f;} }

    [SerializeField] private GameObject cheeseBulletPrefab;

    public override void CastSpecialAbility() {
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            Vector2 direction;
            if (GetComponent<PlayerInput>().devices[0].ToString() == "Keyboard:/Keyboard" || GetComponent<PlayerInput>().devices[0].ToString() == "Mouse:/Mouse") {
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
                direction = worldMousePos - transform.position;
            } else {
                direction = input_ShootDirection;
                if (direction == Vector2.zero) {
                    return;
                }
            }

            // Encontrar dirección de disparo
            direction.Normalize();

            GameObject cheeseBulletInstance;
            cheeseBulletInstance = Instantiate(cheeseBulletPrefab, transform.position, transform.rotation);
            cheeseBulletInstance.GetComponent<PlayerBullet>().StartBullet(10, direction);

            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }
}
