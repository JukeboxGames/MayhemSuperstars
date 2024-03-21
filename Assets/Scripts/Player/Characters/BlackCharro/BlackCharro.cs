using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackCharro : PlayerController
{
    // Character stats
    public override int characterSpeed { get{return 4;} }
    public override int characterMaxHealth { get{return 4;} }
    public override int characterFireRate { get{return 2;} }
    public override int characterDamage { get{return 4;} }
    public override float characterAbilityCooldown { get{return 1f;} }

    [SerializeField] private GameObject slash;
    [SerializeField] private float slashDistanceFromPlayer;
    [SerializeField] private float slashDuration;


    // TODO: Netcode
    public override void CastSpecialAbility() {
        // if cooldown has ellapsed, use special ability
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            // The player must have a direction input to use the ability
            Vector2 direction = GetSlashDirection();
            if (direction == Vector2.zero) return;

            // Instantiate and position slash
            GameObject instance = Instantiate(slash,transform);
            instance.transform.localPosition = direction;
            instance.GetComponent<BlackCharro_Slash>().damage = currentDamage;

            // Rotate slash to desired angle
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg -90;
            instance.transform.eulerAngles = Vector3.forward * angle;

            StartCoroutine(RecordSlashTime(instance));
            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }

    // Destroy Slash after some time
    IEnumerator RecordSlashTime (GameObject instance) {
        yield return new WaitForSeconds(slashDuration);
        Destroy(instance);
    }

    Vector2 GetSlashDirection () {
        // Get Direction
        Vector2 direction;
        if (playerInput.currentControlScheme == "Keyboard") {
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(input_ShootDirection);
            direction = worldMousePos - transform.position;
        } else {
            direction = input_ShootDirection;
        }
        direction.Normalize();
        direction = direction * slashDistanceFromPlayer;
        return direction;
    }
}
