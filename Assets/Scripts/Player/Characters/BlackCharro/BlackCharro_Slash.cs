using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackCharro_Slash : MonoBehaviour
{
    public int damage;

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
