using UnityEngine;

public class Sarge : PlayerController
{
    public override int characterSpeed { get{return 1;} }
    public override int characterMaxHealth { get{return 10;} }
    public override int characterFireRate { get{return 2;} }
    public override int characterDamage { get{return 4;} }
    public override float characterAbilityCooldown { get{return 20f;} }

    public override void CastSpecialAbility() {
        if ((Time.time - timeSinceLastAbility) > (currentAbilityCooldown)) {
            
            GetHit(2);

            if (!isDead) {
                RecordInvulnerability(5f);
            }

            // Actualizar tiempo de cooldown
            timeSinceLastAbility = Time.time;
        }
    }
}
