using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilGoat_Bonfire : MonoBehaviour
{
    public int damage;
    private bool enableDamage = false;

    public IEnumerator StartBonfire(float enableTime) {
        yield return new WaitForSeconds(enableTime);
        enableDamage = true;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.gameObject.tag == "MapBorders") {
            Destroy(this.gameObject);
        }

        if (!enableDamage) return;

        if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Enemy")
        {   
            collision.gameObject.GetComponent<IDamageable>()?.GetHit(damage);

            Destroy(this.gameObject);
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GetHit();
            Destroy(this.gameObject);
        }

    }
}
