using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
