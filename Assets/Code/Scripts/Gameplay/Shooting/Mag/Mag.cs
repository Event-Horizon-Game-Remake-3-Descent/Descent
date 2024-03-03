using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mag : MonoBehaviour
{
    public enum MagType
    {
        Energy,
        Vulkan,
        Spread,
        Concussion_Rocket,
        HeatSeeking_Rocket,
        PlasmaCannon,
        Bomb,
    };

    [Header("Mag Settings")]
    [SerializeField] public int InitialMagSize;
    [SerializeField] public MagType MagMagType;

    [HideInInspector] public float ProjectileLeft;

    private void Awake()
    {
        ProjectileLeft = InitialMagSize;
    }

    private void OnAmmoTaken(MagType type, float amount)
    {
        if (type == MagMagType)
            return;
        //ProjectileLeft + cazzo palle
    }

}
