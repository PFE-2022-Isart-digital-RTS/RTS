using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionQueue : InstructionRunner
{
    InstructionWithNext lastTask = null;

    public void Clear()
    {
        StopCurrentInstruction();
        lastTask = null;
    }

    public void AddInstruction(InstructionWithNext newInstruction)
    {
        if (lastTask == null)
        {
            AssignNewInstruction(newInstruction);
        }
        else
        {
            lastTask.next = newInstruction;
        }

        lastTask = newInstruction;
    }

    public override void AssignNewInstruction(Instruction newInstruction)
    {
        //base.AssignNewInstruction(newInstruction);

        if (IsRunningInstruction())
        {
            currentInstruction.OnEnd();
            currentInstruction.taskRunner = null;
        }

        currentInstruction = newInstruction;
        if (IsRunningInstruction())
        {
            currentInstruction.taskRunner = this;
            currentInstruction.OnStart();
        }

        lastTask = IsRunningInstruction() ? (InstructionWithNext) newInstruction : null;
    }
}

public class InstructionRunner
{
    protected Instruction currentInstruction;
    public Instruction CurrentInstruction { get => currentInstruction; }

    public object blackboard;

    public bool IsInstructionRunning(Instruction task)
    {
        return currentInstruction == task;
    }

    public bool IsRunningInstruction()
    {
        return currentInstruction != null;
    }

    public void StopCurrentInstruction()
    {
        if (IsRunningInstruction())
        {
            currentInstruction.OnStop();
            currentInstruction.taskRunner = null;
            currentInstruction = null;
        }
    }

    public virtual void AssignNewInstruction(Instruction newInstruction)
    {
        if (IsRunningInstruction())
        {
            currentInstruction.OnStop();
            currentInstruction.taskRunner = null;
        }

        currentInstruction = newInstruction;
        if (IsRunningInstruction())
        {
            currentInstruction.taskRunner = this;
            currentInstruction.OnStart();
        }
    }

    public void UpdateCurrentInstruction()
    {
        if (IsRunningInstruction())
            currentInstruction.OnUpdate();
    }
}

public abstract class Instruction
{
    public InstructionRunner taskRunner;

    public abstract void OnStart();

    public virtual void OnEnd()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnStop()
    {
    }

    public abstract bool CanDoInstruction();
}

public abstract class InstructionWithNext : Instruction
{
    public Instruction next;

    protected void RunNextInstruction()
    {
        taskRunner.AssignNewInstruction(next);
    }
} 