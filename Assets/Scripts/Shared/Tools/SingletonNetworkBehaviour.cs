using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Warning ! Singleton ise not DontDestroyOnLoad.
/// </summary>
/// <typeparam name="T"></typeparam>
[RequireComponent(typeof(NetworkObject))]
public class SingletonNetworkBehaviour<T> : NetworkBehaviour
    where T : Component
{
    private static T m_Instance = null;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                var objs = FindObjectsOfType<T>();
                if (objs.Length > 0)
                {
                    m_Instance = objs[0];
                }

                if (objs.Length > 1)
                {
                    Debug.LogError($"There is more than one {typeof(T).Name} in the scene.");
                }

                if (m_Instance == null)
                {
                    if (!NetworkManager.Singleton.IsHost)
                    {
                        GameObject newObj = new GameObject(typeof(T).Name);
                        m_Instance = newObj.AddComponent<T>();
                    }
                    else
                    {
                        Debug.LogError($"The client tries to get {typeof(T).Name} , but it hasn't been instantiated on the server yet.");
                    }
                }
            }

            return m_Instance;
        }
    }
}