using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonBullet : PlayerBullet
{
    public override float playerBulletSpeed { get{return 10f;} }
    [SerializeField] private GameObject waterPool;
    private bool canCollide = false;

    public override void StartBullet (int damage, Vector2 direction) {
        bulletDamage = 0;
        GetComponent<Rigidbody2D>().velocity = direction * playerBulletSpeed;
        StartCoroutine(EnableCollisions());
        
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if (!canCollide) {
            return;
        }

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

    private IEnumerator EnableCollisions () {
        yield return new WaitForSeconds(0.02f);
        canCollide = true;
    }

    private IEnumerator SelfDestruct () {
        yield return new WaitForSeconds(2.5f);
        if (this != null) {
            Instantiate(waterPool, transform.position, transform.rotation);
            Destroy(this.gameObject);
        }
    }


}
