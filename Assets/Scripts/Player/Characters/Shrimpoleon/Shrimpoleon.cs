using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrimpoleon : PlayerController
{
    public override int characterSpeed { get{return 3;} }
    public override int characterMaxHealth { get{return 6;} }
    public override int characterFireRate { get{return 5;} }
    public override int characterDamage { get{return 1;} }
    public override float characterAbilityCooldown { get{return 10f;} }

    [SerializeField] private GameObject balloon;

    public override void CastSpecialAbility() {
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            Vector2 direction;
            if (playerInput.devices[0].ToString() == "Keyboard:/Keyboard" || playerInput.devices[0].ToString() == "Mouse:/Mouse") {
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

            GameObject instance = Instantiate(balloon, transform.position, transform.rotation);
            instance.GetComponent<PlayerBullet>().StartBullet(0, direction);

            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }
}
