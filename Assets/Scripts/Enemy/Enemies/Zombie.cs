using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.AI;

public class Zombie : Enemy
{
  // protected bool enabled = false;
  NavMeshAgent agent;

  protected override void Start()
  {
    base.Start();
    health = 1;
    moveSpeed = 1;
    atkSpeed = 1;
    atkDmg = 1;
    agent = gameObject.GetComponent<NavMeshAgent>(); 
    agent.destination = GameManager.instance.playerSoulArray[0].GetComponent<PlayerSoul>().vessel.transform.position; 
  }

  public override void OnLobby()
  {
    Debug.Log("1");
  }
  public override void OnCountDown()
  {
    Debug.Log("2");
  }
  public override void OnRound()
  {
    Debug.Log("3");
  }
  public override void OnTimesUp()
  {
    Debug.Log("4");
  }
  public override void OnLeaderboard()
  {
    Debug.Log("5");
  }
  public override void OnPurchasePhase()
  {
    Debug.Log("6");
  }
  public override void OnEndgame()
  {
    Debug.Log("7");
  }
  public override void OnWinScreen()
  {
    Debug.Log("8");
  }
  public override void GetHit(int dmg)
  {
    health -= dmg;
    if (health <= 0) Die();
  }
  public override void Die()
  {
    Destroy(gameObject);
  }

  void OnCollisionEnter2D(Collision2D other)
  {
    Debug.Log("Here");
    if (other.gameObject.tag == "Player")
    {
      other.gameObject.GetComponent<PlayerController>().GetHit(atkDmg);
    }
  }
}
