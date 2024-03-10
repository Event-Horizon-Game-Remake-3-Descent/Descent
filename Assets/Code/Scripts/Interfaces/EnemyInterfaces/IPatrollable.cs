using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IPatrollable
{
    public PatrollingData PatrollingPoints { get; set; }
    public float PatrollingSpeed { get; set; }
    public float PatrollingThreshold { get; set; }
    public float PatrollingDecelerationDistance { get; set; }
    public int PatrollingIndex { get; set; }
    public void Patrol() { }
}
