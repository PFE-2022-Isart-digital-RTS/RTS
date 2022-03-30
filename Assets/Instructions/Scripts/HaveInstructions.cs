using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// HaveInstructions is a component (MonoBehaviour)being able to store multiple instructions, that the entity will try to do, one at a time.
// Each function adding a new instruction *must* be called on the serv (i.e. a previous function must be a Server RPC).
// Instructions can be replicated to clients.
// When the Server RPC is called, it is to the server to determine if the instruction is valid or not (e.g. attacking an enemy not in sight of the player is impossible).
public class HaveInstructions : MonoBehaviour
{
    List<Instruction> instructions = new List<Instruction>();

    private void Update()
    {
        
    }
}
