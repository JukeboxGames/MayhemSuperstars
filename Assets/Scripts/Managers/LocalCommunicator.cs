using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalCommunicator : MonoBehaviour
{
    private GameManager gm;
    [SerializeField] private SO_GameState so_GameState;
    bool foundGM = false;
    void Awake () {
        StartCoroutine(SearchForGameManager());
    }

    IEnumerator SearchForGameManager () {
        while (!foundGM) {
            yield return new WaitForSeconds(0.1f);
            if (GameObject.FindGameObjectWithTag("GameManager") != null) {
                gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                gm.ChangeState.AddListener(CommunicateGameState);
                foundGM = true;
            }
        }
    }

    void CommunicateGameState(GameManager.GameState newState){
        so_GameState.TriggerGameStateEvent(newState);
    }
}
