using UnityEngine;
using System.Collections;

public class Sarge : PlayerController
{
    public override int characterSpeed { get{return 1;} }
    public override int characterMaxHealth { get{return 10;} }
    public override int characterFireRate { get{return 2;} }
    public override int characterDamage { get{return 4;} }
    public override float characterAbilityCooldown { get{return 5f;} }

    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldDistanceFromPlayer;
    [SerializeField] private float shieldDuration;

    public override void CastSpecialAbility() {
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            Vector2 direction = GetShieldDirection();
            if (direction == Vector2.zero) return;

            // Instantiate and position shield
            GameObject instance = Instantiate(shield,transform);
            instance.transform.localPosition = direction;

            // Rotate shield to desired angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90;
            instance.transform.eulerAngles = Vector3.forward * angle;

            StartCoroutine(RecordShieldTime(instance));

            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }

    IEnumerator RecordShieldTime (GameObject instance) {
        currentSpeed--;
        yield return new WaitForSeconds(shieldDuration);
        currentSpeed++;
        Destroy(instance);
    }

    Vector2 GetShieldDirection () {
        // Get Direction
        Vector2 direction;
        if (playerInput.currentControlScheme == "Keyboard") {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
            direction = worldMousePos - transform.position;
        } else {
            direction = input_ShootDirection;
        }
        direction.Normalize();
        direction = direction * shieldDistanceFromPlayer;
        return direction;
    }

    
}
