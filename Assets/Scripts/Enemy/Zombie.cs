using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    private IEnumerator walkLeft()
    {
        while (true) {
            gameObject.transform.position = gameObject.transform.position - gameObject.transform.right * _moveSpeed;
            Debug.Log("Moved");
            yield return new WaitForSeconds(1.0f);
        }
    }
    protected override void StartMoveCycle()
    {
        StartCoroutine(walkLeft());
    }
    protected override void StartAttackCycle()
    {
        Debug.Log("Hello");
    }
    protected override void Start()
    {
        base.Start();
        setStats("alien");
    }
}
