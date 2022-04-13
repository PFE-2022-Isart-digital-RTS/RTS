using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Warning ! Singleton ise not DontDestroyOnLoad.
/// </summary>
/// <typeparam name="T"></typeparam>
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
                    GameObject newObj = new GameObject(typeof(T).Name);
                    m_Instance = newObj.AddComponent<T>();
                }
            }

            return m_Instance;
        }
    }
}