using UnityEngine;

// Class for Black Charro's slash's hitbox
public class BlackCharro_Slash : MonoBehaviour
{
    public int damage;

    // Damage enemies and destroy enemy bullets
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")
        {   
            if (collision.gameObject.GetComponent<IDamageable>() != null) {
                collision.gameObject.GetComponent<IDamageable>().GetHit(damage);
            }
        }

        if (collision.gameObject.tag == "Enemy Bullet")
        {   
            Destroy(collision.gameObject);
        }

    }
}
