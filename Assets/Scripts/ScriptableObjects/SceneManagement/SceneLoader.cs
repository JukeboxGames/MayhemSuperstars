using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

[CreateAssetMenu(menuName ="ScriptableObjects/SceneManagement/Scene Loader")]
public class SceneLoader : ScriptableObject
{
    public void NetworkLoadScene(string sceneName){
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void LocalLoadScene(string sceneName){
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
