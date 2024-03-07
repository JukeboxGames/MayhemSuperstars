using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

[CreateAssetMenu(menuName ="ScriptableObjects/Game/GameState")]
public class SO_GameState : ScriptableObject
{

    public UnityEvent<GameManager.GameState> gameState;

    public void TriggerGameStateEvent (GameManager.GameState newState) {
        gameState.Invoke(newState);
    }
}
