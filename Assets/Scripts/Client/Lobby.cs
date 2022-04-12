using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;
    
    // Start is called before the first frame update
    void Start()
    {
        hostButton.onClick.AddListener(() =>
        {
            Debug.Log("StartHost success");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");
            asyncLoad.completed += operation =>
            {
                if (NetworkManager.Singleton.StartHost())
                {
                    SceneManager.LoadScene("Map", LoadSceneMode.Additive);
                }
                else
                {
                    SceneManager.LoadScene("Menu");
                    Debug.Log("StartHost fail");
                }
            };
        });
        
        clientButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("StartClient success");
            }
            else
            {
                Debug.Log("StartClient fail");
            }
        });
        
        serverButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartServer())
            {
                Debug.Log("serverButton success");
            }
            else
            {
                Debug.Log("serverButton fail");
            }
        });
    }
}
