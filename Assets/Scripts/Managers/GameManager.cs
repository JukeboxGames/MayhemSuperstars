using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    
    public NetworkVariable<float> timer = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone);
    private GameState state;
    public UnityEvent changedNumberOfPlayers;
    public UnityEvent<GameState> ChangeState;
    [SerializeField] private SO_Maps mapsSO;
    private bool recordTime = false;

    public GameObject[] playerSoulArray = new GameObject[4];
    private int[] playerPoints = new int[4];
    public NetworkVariable<int> numberOfPlayers = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone);

    void Awake () {
        if (!NetworkManager.Singleton.IsServer) {
            return;
        }

        if (instance != null && instance != this) { 
            Destroy(this.gameObject); 
        }
        else { 
            instance = this; 
        }

        this?.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
        state = GameState.Lobby;
        NetworkManager.SceneManager.OnSceneEvent += OnNetworkSceneLoaded;
    }

    public int JoinPlayer (GameObject soul) {
        playerSoulArray[numberOfPlayers.Value] = soul;
        numberOfPlayers.Value++;
        changedNumberOfPlayers?.Invoke();
        return numberOfPlayers.Value;
    }

    public bool CheckAvailability () {
        if (numberOfPlayers.Value >= 4) {
            return false;
        } else {
            return true;
        }
    }

    // Ejecutar funciones de escena de juego
    void OnNetworkSceneLoaded(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted){
            if (SceneManager.GetActiveScene().name == "Lobby") {
                UpdateGameState(GameState.Lobby);
            }

            if (Array.Exists(mapsSO.mapNames, element => element == SceneManager.GetActiveScene().name)) {
                playerPoints = new int[4];
                UpdateGameState(GameState.Countdown);
            }
        }
    }

    void UpdateGameState (GameState newState) {
        if (IsServer) {
            state = newState;
            InvokeGameStateChangeClientRpc(newState);
            //ChangeState.Invoke(newState);

            switch(newState){
                case GameState.Lobby:
                    recordTime = false;
                    break;
                case GameState.Countdown:
                    recordTime = true;
                    timer.Value = 3f;
                    break;
                case GameState.Round:
                    timer.Value = 30f;
                    break;
                case GameState.TimesUp:
                    timer.Value = 3f;
                    break;
                case GameState.Leaderboard:
                    timer.Value = 5f;
                    break;
                case GameState.PurchasePhase:
                    timer.Value = 45f;
                    break;
                case GameState.Endgame:
                    // Endgame logic here
                    UpdateGameState(GameState.WinScreen);
                    break;
                case GameState.WinScreen:
                    recordTime = false;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }
    }

    void Update () {
        if (IsServer && recordTime) {
            timer.Value -= Time.deltaTime;
            if (timer.Value < 0f) {
                switch(state){
                    case GameState.Countdown:
                        UpdateGameState(GameState.Round);
                        break;
                    case GameState.Round:
                        UpdateGameState(GameState.TimesUp);
                        break;
                    case GameState.TimesUp:
                        GivePoints ();
                        UpdateGameState(GameState.Leaderboard);
                        break;
                    case GameState.Leaderboard:
                        if (CheckForWinner()) {
                            UpdateGameState(GameState.Endgame);
                        } else {
                            UpdateGameState(GameState.PurchasePhase);
                        }
                        break;
                    case GameState.PurchasePhase:
                        UpdateGameState(GameState.Countdown);
                        break;
                    default:
                        Debug.Log("Hola");
                        throw new System.ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }
        }   
    }

    void GivePoints () {
        GameObject soul;
        for(int i = 0; i < numberOfPlayers.Value; i++) {
            soul = playerSoulArray[i];
            if (!soul.GetComponent<PlayerSoul>().vessel.GetComponent<PlayerController>().isDead) {
                playerPoints[i]++;
            }
        }
    }

    bool CheckForWinner () {
        foreach (int points in playerPoints)
        {
            if (points >= 5) {
                return true;
            }
        }
        //return false;
        return true;
    }

    [ClientRpc]
    void InvokeGameStateChangeClientRpc (GameState newState) {
        ChangeState.Invoke(newState);
    }

    // Estados de juego
    public enum GameState{
        Lobby,
        Countdown,
        Round,
        TimesUp,
        Leaderboard,
        PurchasePhase,
        Endgame,
        WinScreen
    };
}
