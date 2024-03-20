using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for player bullets
public abstract class PlayerBullet : MonoBehaviour
{
    protected int bulletDamage;
    // Each bullet sets its speed
    public abstract float playerBulletSpeed { get; }

    // Starts moving the bullet once instantiated, must be called
    // on the script that instantiates
    public virtual void StartBullet (int damage, Vector2 direction) {
        bulletDamage = damage;
        GetComponent<Rigidbody2D>().velocity = (direction) * (playerBulletSpeed);
    }

    // Base collisions for bullet
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
