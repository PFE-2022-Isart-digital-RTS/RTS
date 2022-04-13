using System.Collections.Generic;

public class ServerGameManager : SingletonMonoBehaviour<ServerGameManager>
{
    private List<Entity>[] m_teamsUnits = new List<Entity>[(int) ETeam.TeamCount];

    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        SharedGameManager.Instance.onRegisterEntity += RegisterEntity;
        SharedGameManager.Instance.onUnregisterEntity += UnregisterEntity;

        for (var index = 0; index < m_teamsUnits.Length; index++)
        {
            m_teamsUnits[index] = new List<Entity>();
        }
    }

    /// <summary>
    /// Need to be called in OnEnable
    /// </summary>
    /// <example>
    ///private void OnEnable()
    ///{
    ///    GameManager.Instance.RegisterUnit(team, this);
    ///}
    /// </example>
    /// <param name="team"></param>
    public void RegisterEntity(ETeam team, Entity unit)
    {
        m_teamsUnits[(int) team].Add(unit);
    }

    /// <summary>
    /// Need to be called in OnDisable
    /// </summary>
    /// <example>
    ///private void OnDisable()
    ///{
    ///    if(gameObject.scene.isLoaded)
    ///        GameManager.Instance.UnregisterUnit(team, this);
    ///}
    /// </example>
    /// <param name="team"></param>
    public void UnregisterEntity(ETeam team, Entity unit)
    {
        m_teamsUnits[(int) team].Remove(unit);
    }
}