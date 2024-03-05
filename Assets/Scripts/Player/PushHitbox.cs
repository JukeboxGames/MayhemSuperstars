using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushHitbox : MonoBehaviour
{
    public Vector2 direction;

    void Awake () {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), transform.parent.GetComponent<Collider2D>());
        StartCoroutine(SelfDestruct());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {   
            collision.gameObject.GetComponent<PlayerController>().AddInstantForce(direction * 10f, 0.3f);
        }
    }

    IEnumerator SelfDestruct () {
        yield return new WaitForSeconds(0.1f);
        Destroy(this.gameObject);
    }
}
