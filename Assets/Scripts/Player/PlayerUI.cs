using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour, IReactToGameState
{
    [Header("Scriptable Objects References")]
    [SerializeField] SO_GameState so_gameState;
    [SerializeField] SO_PlayerEvents[] playerEvents_SOs;
    private SO_PlayerEvents myPlayerEventSO;
    [SerializeField] SceneLoader so_SceneLoader;

    [Header("Elements")]
    [SerializeField] GameObject[] winScreenElements;
    [SerializeField] GameObject blackPanel;
    [SerializeField] TMP_Text healthText;
    [SerializeField] GameObject virtualCursor;

    void OnEnable(){
        so_gameState.gameState.AddListener(ReactToGameState);
    }

    public void SetPlayerEvents(int playerNumber){
        myPlayerEventSO = playerEvents_SOs[playerNumber-1];
        myPlayerEventSO.event_PlayerHealthChanged.AddListener(OnPlayerHealthChanged);
    }

    void OnPlayerHealthChanged(int health){
        healthText.text = "Health: " + health;
    }

    public void ReactToGameState(GameManager.GameState newState){
        switch (newState)
        {
            case GameManager.GameState.Lobby:
                healthText.gameObject.SetActive(false);
                blackPanel.SetActive(false);
                virtualCursor.SetActive(true);
                HideElements(winScreenElements);
                break;
            case GameManager.GameState.Countdown:
                healthText.gameObject.SetActive(true);
                virtualCursor.SetActive(false);
                break;
            case GameManager.GameState.PurchasePhase:
                virtualCursor.SetActive(true);
                break;
            case GameManager.GameState.Endgame:
                virtualCursor.SetActive(false);
                break;
            case GameManager.GameState.WinScreen:
                //blackPanel.SetActive(true);
                virtualCursor.SetActive(true);
                ShowElements(winScreenElements);
                break;
            default:
                break;
        }
    }

    void HideElements (GameObject[] array) {
        foreach (GameObject element in array)
        {
            element.SetActive(false);
        }
    }

    void ShowElements (GameObject[] array) {
        foreach (GameObject element in array)
        {
            element.SetActive(true);
        }
    }

    public void ReturnToLobby(){
        if (NetworkManager.Singleton.IsServer) so_SceneLoader.NetworkLoadScene("Lobby");
    }
}
