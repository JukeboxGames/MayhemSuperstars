using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBullet : MonoBehaviour
{
    protected int bulletDamage;
    public abstract float playerBulletSpeed { get; }

    public virtual void StartBullet (int damage, Vector2 direction) {
        bulletDamage = damage;
        GetComponent<Rigidbody2D>().velocity = (direction) * (playerBulletSpeed);
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "MapBorders")
        {
            if (collision.gameObject.GetComponent<IDamageable>() != null) {
                collision.gameObject.GetComponent<IDamageable>().GetHit(bulletDamage);
            }
            Destroy(this.gameObject);
        }
    }
}
