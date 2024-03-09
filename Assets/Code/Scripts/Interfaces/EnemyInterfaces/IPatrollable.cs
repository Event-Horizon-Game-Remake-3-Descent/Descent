using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPatrollable
{
    [SerializeField] List<Transform> PatrollingPoints { get; set; }

    void PatrolToNextPoint() { }
}
