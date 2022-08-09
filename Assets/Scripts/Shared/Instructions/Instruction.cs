using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionQueue : InstructionRunner
{
    InstructionWithNext lastTask;

    public void Clear()
    {
        StopCurrentInstruction();
        lastTask = null;
    }

    public void AddInstruction(InstructionWithNext newInstruction)
    {
        lastTask.next = newInstruction;
        lastTask = newInstruction;
    }
}

public class InstructionRunner
{
    protected Instruction currentInstruction;

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
            currentInstruction.OnEnd();
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