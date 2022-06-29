using FogOfWarPackage;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public Button loadSceneButton;
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    public InputField IpInputField;

    // Start is called before the first frame update
    void Start()
    {
        loadSceneButton.enabled = false;
        loadSceneButton.onClick.AddListener(LoadGameScene);
        hostButton.onClick.AddListener(() =>
        {
            Debug.Log("StartHost success");

            if (NetworkManager.Singleton.StartHost())
            {
                loadSceneButton.enabled = true;
            }
            else
            {
                SceneManager.LoadScene("Menu");
                Debug.Log("StartHost fail");
            }
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
                loadSceneButton.enabled = true;
            }
            else
            {
                Debug.Log("serverButton fail");
            }
        });

        IpInputField.text = NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress;
        IpInputField.onValueChanged.AddListener(ipAdress =>
        {
            NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ipAdress;
        });
    }

    void LoadGameScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += LoadAdditiveScene;
    }
    
    void LoadAdditiveScene(ulong id,string sceneName, LoadSceneMode mode)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= LoadAdditiveScene;
        NetworkManager.Singleton.SceneManager.OnLoadComplete += StartGame;
    }

    void StartGame(ulong id, string sceneName, LoadSceneMode mode)
    {
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= StartGame;
        RTSGameMode gameMode = GameObject.FindObjectOfType<RTSGameMode>();
        if (gameMode)
        {
            RTSGameMode.RTSGameStartData gameStartData = new RTSGameMode.RTSGameStartData();
            int teamID = 0;
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                gameStartData.playersStartData.Add(new RTSGameMode.RTSPlayerStartData
                {
                    client = client,
                    teamID = teamID
                });
                teamID++;
            }
            gameMode.StartGame(gameStartData);
        }
        else
        {
            Debug.LogWarning("No GameMode has been found");
        }
    }
}