using UnityEngine;

public abstract class FakePlayerBullet : MonoBehaviour
{
    public abstract float playerBulletSpeed { get; }

    public virtual void StartBullet (Vector2 direction) {
        GetComponent<Rigidbody2D>().velocity = (direction) * (playerBulletSpeed);
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "MapBorders")
        {
            Destroy(this.gameObject);
        }
    }
}
