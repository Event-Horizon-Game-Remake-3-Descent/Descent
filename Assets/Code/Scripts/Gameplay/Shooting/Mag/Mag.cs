using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mag : MonoBehaviour
{
    [SerializeField] public int MagSize;
    [HideInInspector] public float ProjectileLeft;

    private void Awake()
    {
        ProjectileLeft = MagSize;
    }

}
