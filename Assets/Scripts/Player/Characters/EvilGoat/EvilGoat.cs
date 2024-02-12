using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilGoat : PlayerController
{
    public override int characterSpeed { get{return 2;} }
    public override int characterMaxHealth { get{return 6;} }
    public override int characterFireRate { get{return 2;} }
    public override int characterDamage { get{return 5;} }
    public override float characterAbilityCooldown { get{return 20f;} }

    [SerializeField] private GameObject bonfire;
    [SerializeField] private int numberOfBonfires;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float bonfireStartTime;

    public override void CastSpecialAbility() {
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            Vector3 placement;
            float angle;
            for (int i = 0; i < numberOfBonfires; i++) {
                angle = (360/numberOfBonfires) * i;
                placement = transform.position + Quaternion.Euler( 0, 0, angle) * Vector3.right * distanceFromPlayer;
                GameObject instance = Instantiate(bonfire, placement, transform.rotation);
                instance.GetComponent<EvilGoat_Bonfire>().damage = currentDamage;
                StartCoroutine(instance.GetComponent<EvilGoat_Bonfire>().StartBonfire(bonfireStartTime));
            }

            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }
}
