using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;


public abstract class Prop : NetworkBehaviour, IReactToGameState
{
    // protected bool enabled = false;
    [SerializeField] private SO_GameState sO_GameState;

    public void MostrarHitbox()
    {
        // TODO: show player hitbox
        Debug.Log("CROSS THE ROAD");
    }
    void Start() {
        sO_GameState.gameState.AddListener(ReactToGameState);
    }
    public virtual void ReactToGameState(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Lobby:
                OnLobby();
                break;
            case GameManager.GameState.Countdown:
                OnCountDown(); 
                break;
            case GameManager.GameState.Round:
                OnRound(); 
                break;
            case GameManager.GameState.TimesUp:
                OnTimesUp(); 
                break;
            case GameManager.GameState.Leaderboard:
                OnLeaderboard(); 
                break;
            case GameManager.GameState.PurchasePhase:
                OnPurchasePhase(); 
                break;
            case GameManager.GameState.Endgame:
                OnEndgame(); 
                break;
            case GameManager.GameState.WinScreen:
                OnWinScreen(); 
                break;
            default:
                Debug.Log("Kill yourself");
                StopAllCoroutines();
                break;
        }
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

    public abstract void OnLobby(); 
    public abstract void OnCountDown(); 
    public abstract void OnRound();
    public abstract void OnTimesUp();
    public abstract void OnLeaderboard();
    public abstract void OnPurchasePhase();
    public abstract void OnEndgame();
    public abstract void OnWinScreen();      
}
