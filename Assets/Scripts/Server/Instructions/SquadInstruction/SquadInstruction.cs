using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SquadInstruction 
{
    //public Squad squad;
    public List<GameObject> units = new List<GameObject>();

    public int PreviousTasksCount { get; private set; }

    SquadInstruction next;
    public object blackboard;
    private bool hasStarted = false;
    private bool hasEnded = false;
    protected bool HasStarted
    {
        get => hasStarted;
    }
    protected bool HasEnded
    {
        get => hasEnded;
    }
    public InstructionsManager instructionManager;

    public SquadInstruction Next
    {
        set
        {
            SquadInstruction oldNext = Next;

            next = value;

            if (value != null)
            {
                value.OnPreviousTaskAdd(value);
            }

            if (oldNext != null)
            {
                oldNext.OnPreviousTaskRemove(oldNext);
            }
        }

        get
        {
            return (SquadInstruction)next;
        }
    }

    public virtual void OnPreviousTaskAdd(SquadInstruction instructionToAdd)
    {
        PreviousTasksCount++;
    }

    public virtual void OnPreviousTaskRemove(SquadInstruction instructionToRemove)
    {
        PreviousTasksCount--;
        CheckEnd();
    }

    protected virtual void CheckEnd()
    {
        if (PreviousTasksCount == 0 && units.Count == 0)
        {
            if (!hasEnded)
            {
                hasEnded = true;
                OnEnd();
            }
            OnClean();
        }
    }

    public void TryStart()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            OnStart();
        }
    }

    protected virtual void OnStart()
    {

    }

    public void TryEnd()
    {
        if (!hasEnded)
        {
            End();
        }
    }

    private void End()
    {
        hasEnded = true;
        OnEnd();
        RunNextInstruction();
        Next = null;
    }

    protected virtual void OnEnd()
    {
    }

    public virtual void OnClean()
    {
        if (instructionManager != null)
            instructionManager.RemoveUpdate(this);
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void UnitStart(GameObject unit)
    {
        units.Add(unit);

        if (HasEnded)
        {
            RunNextInstruction(unit);
        }

        TryStart();

        OnUnitStart(unit);
    }

    protected virtual void OnUnitStart(GameObject unit)
    {

    }

    public virtual void UnitStop(GameObject unit)
    {
        OnUnitStop(unit);

        units.Remove(unit);
        CheckEnd();
    }

    protected virtual void OnUnitStop(GameObject unit)
    {

    }

    public void RunNextInstruction()
    {
        List<GameObject> unitsToRemove = new List<GameObject>(units);
        foreach (GameObject unit in unitsToRemove)
        {
            RunNextInstruction(unit);
        }
    }

    public void RunNextInstruction(GameObject unit)
    {
        instructionManager.AssignInstruction(unit, Next);
    }
}