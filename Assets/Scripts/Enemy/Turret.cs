using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy
{
    [SerializeField] private GameObject bullet;
    private IEnumerator ShootCycle() {
        while (true) {
            Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(target);
            Instantiate(bullet, target, Quaternion.identity);
            yield return new WaitForSeconds(_attackSpeed);
        }
    }
    private IEnumerator WalkLeft()
    {
        while (true) {
            gameObject.transform.position = gameObject.transform.position - gameObject.transform.right * _moveSpeed;
            yield return new WaitForSeconds(1.0f);
        }
    }
    protected override void StartMoveCycle()
    {
        StartCoroutine(WalkLeft());
    }
    protected override void StartAttackCycle()
    {
        Debug.Log(_attackSpeed);
        StartCoroutine(ShootCycle());
    }
    void Start()
    {
        Initialize("turret");
    }
}
