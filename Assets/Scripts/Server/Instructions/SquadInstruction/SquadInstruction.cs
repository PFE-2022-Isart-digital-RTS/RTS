using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SquadInstruction 
{
    //public Squad squad;
    public List<GameObject> units = new List<GameObject>();

    private int nbPrev = 0;

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
                value.nbPrev++;
            }

            if (oldNext != null)
            {
                oldNext.nbPrev--;
                oldNext.CheckEnd();
            }
        }

        get
        {
            return (SquadInstruction)next;
        }
    }

    protected void CheckEnd()
    {
        if (units.Count == 0 && nbPrev == 0)
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

    public void End()
    {
        hasEnded = true;
        OnEnd();
        RunNextInstruction();
    }

    protected virtual void OnEnd()
    {
    }

    public virtual void OnClean()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnStop()
    {
        foreach (GameObject unit in units)
        {
            OnUnitStop(unit);
        }
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