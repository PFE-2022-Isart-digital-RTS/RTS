using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedGameManager : SingletonMonoBehaviour<SharedGameManager>
{
    public Action<ETeam, Entity> onRegisterEntity;
    public Action<ETeam, Entity> onUnregisterEntity;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
