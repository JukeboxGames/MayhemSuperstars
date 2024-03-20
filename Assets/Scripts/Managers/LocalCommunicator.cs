using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

// Local communicator: relays the information of the
// Game Manager to Unity Scriptable objects in every client
public class LocalCommunicator : MonoBehaviour
{
    public static LocalCommunicator instance;
    private GameManager gm;
    [SerializeField] private SO_GameState so_GameState;
    bool foundGM = false;

    void Awake()
    {
        // Singleton behaviour
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        StartCoroutine(SearchForGameManager());
    }

    // Repeat coroutine until GameManager is found
    // Could be better
    IEnumerator SearchForGameManager()
    {
        while (!foundGM)
        {
            yield return new WaitForSeconds(0.1f);
            if (GameObject.FindGameObjectWithTag("GameManager") != null)
            {
                gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                gm.ChangeState.AddListener(CommunicateGameState);
                foundGM = true;
            }
        }
    }

    // Calls Scriptable Object event to update game state
    void CommunicateGameState(GameManager.GameState newState)
    {
        so_GameState.TriggerGameStateEvent(newState);
    }
}
