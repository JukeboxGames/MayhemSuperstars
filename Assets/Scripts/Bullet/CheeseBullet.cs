using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseBullet : PlayerBullet
{
    public override float playerBulletSpeed { get{return 10f;} }

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

    private IEnumerator SelfDestruct () {
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}
