using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSubscribeComponent : MonoBehaviour
{
    [SerializeField]
    float speedMultiplier = 1f;

    public float SpeedMultiplier { get => speedMultiplier; }
}
