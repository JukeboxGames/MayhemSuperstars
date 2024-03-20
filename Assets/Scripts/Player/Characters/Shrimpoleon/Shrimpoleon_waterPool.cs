using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for Shrimpoleon's waterpools
// Legacy
public class Shrimpoleon_waterPool : MonoBehaviour
{
    void Start () {
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct () {
        yield return new WaitForSeconds(10f);
        if (this != null) {
            Destroy(this.gameObject);
        }
    }
}
