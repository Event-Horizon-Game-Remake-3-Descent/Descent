using UnityEngine;

public class EscapeDoor : Door
{
    [Header("Escape Door Setting")]
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
