using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : Link each PlayerController to the PlayerState through network?

// Local to each client, responsible of the ui and the selection, 
// and sends the inputs to the server.
public class PlayerController : MonoBehaviour
{
    [SerializeField, EditInPlayModeOnly]
    PlayerState playerState;

    // List<ICanBeSelected> selectedEntities;

    // TODO : UI
}
