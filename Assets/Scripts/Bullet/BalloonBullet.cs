using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for Shrimpoleon's ballon bullet
// This is a Legacy Shrimpoleon bullet, must be updated to push instead of leaving a waterpool behind
public class BalloonBullet : PlayerBullet
{
    public override float playerBulletSpeed { get{return 10f;} }
    [SerializeField] private GameObject waterPool;
    private bool canCollide = false;

    // Starts the movement of the bullet
    public override void StartBullet (int damage, Vector2 direction) {
        bulletDamage = 0;
        GetComponent<Rigidbody2D>().velocity = direction * playerBulletSpeed;
        StartCoroutine(EnableCollisions());
        
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (!canCollide) {
            return;
        }

        // Pop the ballon on collision with a wall, player, or enemy
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "MapBorders" || collision.gameObject.tag == "Player")
        {   
            Instantiate(waterPool, transform.position, transform.rotation);
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }

    void Start () {
        StartCoroutine(SelfDestruct());
    }

    // Waits for a moment before being able to explode
    // This prevents the ballon to explode on Shrimpoleon himself
    private IEnumerator EnableCollisions () {
        yield return new WaitForSeconds(0.02f);
        canCollide = true;
    }
    
    // If 2.5 seconds have ellapsed and the ballon has not popped, pop it
    private IEnumerator SelfDestruct () {
        yield return new WaitForSeconds(2.5f);
        if (this != null) {
            Instantiate(waterPool, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }


}
