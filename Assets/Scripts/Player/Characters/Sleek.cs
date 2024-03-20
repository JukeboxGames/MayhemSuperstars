using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleek : PlayerController
{
    // character stats
    public override int characterSpeed { get{return 5;} }
    public override int characterMaxHealth { get{return 2;} }
    public override int characterFireRate { get{return 3;} }
    public override int characterDamage { get{return 2;} }
    public override float characterAbilityCooldown { get{return 0.5f;} }

    [SerializeField] private float dashDistance;
    [SerializeField] private float dashTime;
    private bool isDashing = false;
    private Vector2 dashDirection;

    public override void CastSpecialAbility() {
        // If cooldown has ellapsed
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            if (isDashing) return;
            // Encontrar direccion de dash
            Vector2 direction = input_Movement;
            direction.Normalize();

            // return if there is no movement input
            if (direction == Vector2.zero) return;

            // Check where Sleek will go and deny the dash if
            // it would cause her to get stuck on a wall
            RaycastHit2D[] hits;
            hits = Physics2D.RaycastAll(transform.position, direction, dashDistance);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.tag == "MapBorders")
                {
                    return;
                }    
            }

            dashDirection = direction;

            // Dash
            StartCoroutine(SleekDashTimer());
            RecordInvulnerability(dashTime);
            

            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }

    // Dash for an amount of time
    IEnumerator SleekDashTimer(){
        isDashing = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        GetComponent<CapsuleCollider2D>().enabled = true;
    }

    // Lock movement during dash
    public override void FixedUpdate(){
        base.FixedUpdate();
        if (isDashing) {
            rig.velocity = dashDirection * (dashDistance/dashTime);
        }
    }
}
