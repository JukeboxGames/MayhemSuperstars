using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;


public class DestructibleWall : Prop, IDamageable
{
    private int health = 5;
    public void Die()
    {
        Destroy(gameObject);
    }

    public void GetHit(int dmg)
    {
        health -= dmg;
        if(health <= 0) Die(); 
    }
    //On

    /* Lobby,
        Countdown,
        Round,
        TimesUp,
        Leaderboard,
        PurchasePhase,
        Endgame,
        WinScreen */

    public override void OnLobby()
    {
        Debug.Log("1");
        //throw new NotImplementedException();
    }
    public override void OnCountDown()
    {
        Debug.Log("2");
        //throw new NotImplementedException();
    }
    public override void OnLeaderboard()
    {
        Debug.Log("3");
        //throw new NotImplementedException();
    }
    public override void OnRound()
    {
        Debug.Log("4");
        //throw new NotImplementedException();
    }
    public override void OnEndgame()
    {
        Debug.Log("5");
        //throw new NotImplementedException();
    }
    public override void OnPurchasePhase()
    {
        Debug.Log("6");
        //throw new NotImplementedException();
    }
    public override void OnTimesUp()
    {
        Debug.Log("7");
        //throw new NotImplementedException();
    }
    public override void OnWinScreen()
    {
        Debug.Log("8");
        //throw new NotImplementedException();
    }
}
