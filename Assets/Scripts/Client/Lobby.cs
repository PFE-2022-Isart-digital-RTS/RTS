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
        loadSceneButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            NetworkManager.Singleton.SceneManager.OnLoadComplete += LoadMap;
        });
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
    
    void LoadMap(ulong id,string sceneName, LoadSceneMode mode)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Map", LoadSceneMode.Additive);
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= LoadMap;
    }
}