using System.Collections.Generic;
using UnityEngine;

public interface ISpawnable
{
    [SerializeField] public List<RandomData> ObjectsToSpawn { set; get; }
    public void SpawnObject();
}
