using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoor : Door
{
    [Header("Enemy Door Setting")]
    [Tooltip("Seconds to wait after the player has exit the trigger zone")]
    [SerializeField] private float CloseAfter = 5f;
    [SerializeField] Boss BossReference;

    new private void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (BossReference != null)
            BossReference.OnEnemyDead += () => OpenDoor();
        else
            Debug.LogError("ENEMY REQUESTED FOR THIS DOOR");
    }

    private void OpenDoor()
    {
        OnDoorOpen();

        SFX_Source.clip = OpenSFX;
        SFX_Source.Play();

        for (int i = 0; i < ListOfPanels.Count; i++)
        {
            ListOfPanels[i].MovePanel(1, 1f);
        }
    }
}
