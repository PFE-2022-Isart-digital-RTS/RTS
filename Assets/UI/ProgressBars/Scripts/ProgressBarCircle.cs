using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarCircle : UnityEngine.UI.Slider, IProgressBar
{
    public override float value
    {
        get => base.value;
        set
        {
            base.value = value;
            UpdateUI(base.value);
        }
    }

    void UpdateUI(float newValue)
    {
        // TODO : Code UI if necessary
    }
}
