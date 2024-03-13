using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableDoor : Door
{
    [Header("Damageable Door Settings")]
    [SerializeField] private float DoorHealthPoints = 20f;

    private float StartingHP;

    new private void Awake()
    {
        base.Awake();

        for (int i = 0; i < ListOfPanels.Count; i++)
        {
            ListOfPanels[i].OnPanelTrigger += DamageDoor;
        }

        StartingHP = DoorHealthPoints;
    }

    private void DamageDoor(float damageTaken)
    {
        DoorHealthPoints -= damageTaken;
        for (int i = 0; i < ListOfPanels.Count; i++)
        {
            ListOfPanels[i].MovePanel(1, (StartingHP-DoorHealthPoints)/StartingHP);
            Debug.Log((StartingHP - DoorHealthPoints) / StartingHP);
        }
    }
}
