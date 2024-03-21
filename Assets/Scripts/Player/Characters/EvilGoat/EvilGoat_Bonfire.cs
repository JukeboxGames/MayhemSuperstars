using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for Evil Goat's fire
public class EvilGoat_Bonfire : MonoBehaviour
{
    public int damage;
    private bool enableDamage = false;

    // Starts the bonfire
    public IEnumerator StartBonfire(float enableTime)
    {
        // Waits for a moment to start damaging (for telegraphing)
        yield return new WaitForSeconds(enableTime);
        enableDamage = true;
        // Resets the collider to check for collisions
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MapBorders")
        {
            Destroy(this.gameObject);
        }

        if (!enableDamage) return;

        // Damage players and enemies
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
