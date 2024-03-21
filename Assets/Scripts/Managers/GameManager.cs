using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

// Game Manager class
// Manages the state of the game and player points
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


    void Awake()
    {
        // Singleton behaviour
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        this?.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
        state = GameState.Lobby;
        NetworkManager.SceneManager.OnSceneEvent += OnNetworkSceneLoaded;
    }

    // Join player to session. Returns its player number
    public int JoinPlayer(GameObject soul)
    {
        playerSoulArray[numberOfPlayers.Value] = soul;
        numberOfPlayers.Value++;
        changedNumberOfPlayers?.Invoke();
        return numberOfPlayers.Value;
    }

    // Checks if there are available spots to join
    public bool CheckAvailability()
    {
        if (numberOfPlayers.Value >= 4)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Updates Game State depending on the loaded scene
    void OnNetworkSceneLoaded(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                UpdateGameState(GameState.Lobby);
            }

            // mapsSO contains all the names of the maps
            // When a map is loaded, reset points and go to countdown
            if (Array.Exists(mapsSO.mapNames, element => element == SceneManager.GetActiveScene().name))
            {
                playerPoints = new int[4];
                UpdateGameState(GameState.Countdown);
            }
        }
    }

    // Update GameState variable and changes the timer
    void UpdateGameState(GameState newState)
    {
        if (IsServer)
        {
            state = newState;
            InvokeGameStateChangeClientRpc(newState);
            //ChangeState.Invoke(newState);

            switch (newState)
            {
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

    // Handle the timer on update
    void Update()
    {
        if (IsServer && recordTime)
        {
            timer.Value -= Time.deltaTime;
            if (timer.Value < 0f)
            {
                switch (state)
                {
                    case GameState.Countdown:
                        UpdateGameState(GameState.Round);
                        break;
                    case GameState.Round:
                        UpdateGameState(GameState.TimesUp);
                        break;
                    case GameState.TimesUp:
                        GivePoints();
                        UpdateGameState(GameState.Leaderboard);
                        break;
                    case GameState.Leaderboard:
                        if (CheckForWinner())
                        {
                            UpdateGameState(GameState.Endgame);
                        }
                        else
                        {
                            UpdateGameState(GameState.PurchasePhase);
                        }
                        break;
                    case GameState.PurchasePhase:
                        UpdateGameState(GameState.Countdown);
                        break;
                    default:
                        throw new System.ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }
        }
    }

    // Gives points to all alive player objects
    void GivePoints()
    {
        GameObject soul;
        for (int i = 0; i < numberOfPlayers.Value; i++)
        {
            soul = playerSoulArray[i];
            if (!soul.GetComponent<PlayerSoul>().vessel.GetComponent<PlayerController>().isDead)
            {
                playerPoints[i]++;
            }
        }
    }

    // Checks if any player has reached 5 points
    bool CheckForWinner()
    {
        foreach (int points in playerPoints)
        {
            if (points >= 5)
            {
                return true;
            }
        }

        // Hardcoded to return TRUE for debugging purposes
        //return false;
        return true;
    }

    // Change the state in every client
    [ClientRpc]
    void InvokeGameStateChangeClientRpc(GameState newState)
    {
        ChangeState.Invoke(newState);
    }

    // GameStates
    public enum GameState
    {
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
