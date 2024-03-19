using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;


public abstract class Prop : NetworkBehaviour, IReactToGameState
{
    // protected bool enabled = false;

    public virtual void ReactToGameState (GameManager.GameState newState){

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
}
