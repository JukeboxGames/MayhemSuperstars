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
    [SerializeField] SO_Props so_Props; 

    [Header("Elements")]
    [SerializeField] GameObject[] winScreenElements;
    [SerializeField] GameObject blackPanel;
    [SerializeField] TMP_Text healthText;
    [SerializeField] GameObject virtualCursor;

    private GameObject ghostObject; 


    void OnEnable()
    {
        so_gameState.gameState.AddListener(ReactToGameState);
    }

    // Subscription to player events
    public void SetPlayerEvents(int playerNumber)
    {
        myPlayerEventSO = playerEvents_SOs[playerNumber - 1];
        myPlayerEventSO.event_PlayerHealthChanged.AddListener(OnPlayerHealthChanged);
    }

    void OnPlayerHealthChanged(int health)
    {
        healthText.text = "Health: " + health;
    }

    public void ReactToGameState(GameManager.GameState newState)
    {
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
                //sigue el objeto
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

    // General function to hide UI elements
    void HideElements(GameObject[] array)
    {
        foreach (GameObject element in array)
        {
            element.SetActive(false);
        }
    }

    // General function to show UI elements
    void ShowElements(GameObject[] array)
    {
        foreach (GameObject element in array)
        {
            element.SetActive(true);
        }
    }

    public void ReturnToLobby()
    {
        if (NetworkManager.Singleton.IsServer) so_SceneLoader.NetworkLoadScene("Lobby");
    }
    //call on the press of a button 
    public void SpawnObject(int propId){
        //Instantiate Scriptable.[i].GhostProp at pos = virtualCurosr.mouse.position
        //TO-DO SPAWN IN NETWORK
        ghostObject = so_Props.props[propId].ghostProp;
        if(virtualCursor != null){
            Instantiate(ghostObject, virtualCursor.GetComponent<VirtualCursor>().worldPositionVirtualMouse, Quaternion.identity);
        } else {
            // TO-DO Mouse normal
        }
        

    }
    public void MoveObject(){ // call on Update
        if(ghostObject != null){
            if(virtualCursor != null){
                ghostObject.transform.position = virtualCursor.GetComponent<VirtualCursor>().worldPositionVirtualMouse;
            } else { //TO-DO Mouse Normal

            }
        }
    }
}
