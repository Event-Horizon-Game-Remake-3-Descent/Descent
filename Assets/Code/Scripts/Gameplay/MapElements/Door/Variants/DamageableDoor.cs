using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableDoor : Door
{
    [Header("Damageable Door Settings")]
    [SerializeField] private float ChangeStageAfterDamage = 10f;
    [SerializeField] List<Transform> DoorStages = new List<Transform>();

    private float DamageAmount = 0;
    private int CurrentStage = 0;

    private bool Destroyed = false;

    new private void Awake()
    {
        base.Awake();

        for (int i = 0; i < ListOfPanels.Count; i++)
            ListOfPanels[i].OnPanelTrigger += DamageDoor;

        for(int i = 1; i < DoorStages.Count;  i++)
        {
            DoorStages[i].gameObject.SetActive(false);
        }
    }

    private void DamageDoor(float damageTaken)
    {
        if (Destroyed)
            return;

        DamageAmount += damageTaken;

        if(DamageAmount%ChangeStageAfterDamage == 0)
        {
            DoorStages[CurrentStage].gameObject.SetActive(false);

            CurrentStage++;
            if (CurrentStage > DoorStages.Count - 1)
            {
                Destroyed = true;
                return;
            }

            DoorStages[CurrentStage].gameObject.SetActive(true);
        }
    }
}
