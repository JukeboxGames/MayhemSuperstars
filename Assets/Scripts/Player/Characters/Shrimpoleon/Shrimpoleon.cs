using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrimpoleon : PlayerController
{
    // character stats
    public override int characterSpeed { get{return 3;} }
    public override int characterMaxHealth { get{return 6;} }
    public override int characterFireRate { get{return 5;} }
    public override int characterDamage { get{return 1;} }
    public override float characterAbilityCooldown { get{return 10f;} }

    [SerializeField] private GameObject balloon;

    public override void CastSpecialAbility() {
        // If cooldown has ellapsed
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {

            // Get shooting direction
            Vector2 direction;
            if (playerInput.currentControlScheme == "Keyboard") {
                Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
                direction = worldMousePos - transform.position;
            } else {
                direction = input_ShootDirection;
            }

            // return if there is no shooting direction input
            if (direction == Vector2.zero) return;
            direction.Normalize();

            // Spawn a ballon bullet
            GameObject instance = Instantiate(balloon, transform.position, transform.rotation);
            Physics2D.IgnoreCollision(instance.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            instance.GetComponent<PlayerBullet>().StartBullet(0, direction);

            // Update cooldown time
            timeSinceLastAbility = Time.time;
        }
    }
}
