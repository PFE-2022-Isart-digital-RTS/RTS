using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarHorizontal : UnityEngine.UI.Slider, IProgressBar // inheritance can be changed in the future
{
    [property: SerializeField]
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
