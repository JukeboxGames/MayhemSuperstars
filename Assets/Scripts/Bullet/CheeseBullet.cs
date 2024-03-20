using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for Cheeseman's cheese bullet
public class CheeseBullet : PlayerBullet
{
    public override float playerBulletSpeed { get{return 10f;} }

    // When hitting an enemy, damage it
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {   
            if (collision.gameObject.GetComponent<IDamageable>() != null) {
                collision.gameObject.GetComponent<IDamageable>().GetHit(bulletDamage);
            }
        }
    }

    void Start () {
        StartCoroutine(SelfDestruct());
    }

    // After 5 seconds have ellapsed, destroy the bullet
    private IEnumerator SelfDestruct () {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}
