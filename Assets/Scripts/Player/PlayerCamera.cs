using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour, IReactToGameState
{
    [HideInInspector] public GameObject camera_target;
    [SerializeField] private SO_GameState so_GameState;
    private bool camera_followTarget = true;
    private Vector3 camera_newPos;

    void OnEnable () {
        so_GameState.gameState.AddListener(ReactToGameState);
    }

    void OnDisable () {
        so_GameState.gameState.RemoveListener(ReactToGameState);
    }

    public void ChangeCameraTarget (GameObject newTarget) {
        camera_target = newTarget;
    }

    public void ReactToGameState(GameManager.GameState newState){
        if (newState == GameManager.GameState.Lobby) {
            camera_followTarget = true;
        } 

        if (newState == GameManager.GameState.Countdown) {
            camera_followTarget = true;
        } 

        if (newState == GameManager.GameState.Leaderboard) {
            camera_followTarget = false;
        }
    }

    void Update () {
        if (camera_target != null) {
            if (camera_followTarget) {
                Transform camera_targetTransform = camera_target.transform;
                camera_newPos = new Vector3(camera_targetTransform.position.x, camera_targetTransform.position.y, -10f);
            } else {
                camera_newPos = new Vector3(0f, 0f, -10f);
            }
        }

        transform.position = camera_newPos;
    }
}
