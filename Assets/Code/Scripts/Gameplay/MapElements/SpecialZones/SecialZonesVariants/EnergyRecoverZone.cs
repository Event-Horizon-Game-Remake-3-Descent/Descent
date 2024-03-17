using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnergyRecoverZone : SpecialZone
{
    public delegate void EnergyRecover(float amount);
    public static event EnergyRecover OnEnergyRecover = (float x) => { };

    [Header("Energy Recover Zone")]
    [SerializeField] float EnergyRecoverAmount = 0.5f;
    protected override void ApplyEffect(Collider collider)
    {
        OnEnergyRecover(EnergyRecoverAmount);
    }
}
