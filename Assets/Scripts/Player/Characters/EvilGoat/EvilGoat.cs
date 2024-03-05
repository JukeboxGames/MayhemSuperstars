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
    [SerializeField] private float outwardSpeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float bonfireStartTime;

    private List<GameObject> bonfires = new List<GameObject>();
    private List<float> bonfireAngles = new List<float>();
    private bool wasCasting = false;
    private float distanceFromPlayer = 0;
    private int tempSpeed;

    public override void CastSpecialAbility() {
        if (!wasCasting) {
            if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
                tempSpeed = currentSpeed;
                currentSpeed = 0;
                wasCasting = true;
                Vector3 placement;
                float angle;
                distanceFromPlayer = 0;
                for (int i = 0; i < numberOfBonfires; i++) {
                    angle = 360/numberOfBonfires * i;
                    bonfireAngles.Add(angle);
                    placement = transform.position; // + Quaternion.Euler( 0, 0, angle) * Vector3.right * distanceFromPlayer;
                    GameObject instance = Instantiate(bonfire, placement, transform.rotation);
                    bonfires.Add(instance);
                    instance.GetComponent<EvilGoat_Bonfire>().damage = currentDamage;
                }
            }
        } else {
            distanceFromPlayer += outwardSpeed * Time.deltaTime;
            for (int i = 0; i < numberOfBonfires; i++){
                bonfireAngles[i] += turnSpeed * Time.deltaTime;
                bonfires[i].transform.position += Quaternion.Euler( 0, 0, bonfireAngles[i]) * Vector3.right * distanceFromPlayer;
            }
        }
    }

    private void EndCasting(){
        currentSpeed = tempSpeed;
        foreach (GameObject bonfire in bonfires)
        {
            StartCoroutine(bonfire.GetComponent<EvilGoat_Bonfire>().StartBonfire(bonfireStartTime));
        }

        bonfires.Clear();
        bonfireAngles.Clear();
        wasCasting = false;
        // Actualizar tiempo de cooldown
        timeSinceLastAbility = Time.time;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!input_Special && wasCasting) {
            EndCasting();
        }
    }
}
