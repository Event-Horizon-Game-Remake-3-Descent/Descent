using UnityEngine;

public class EscapeDoor : Door
{
    new private void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Boss.OnBossDefeat +=  OpenDoor;
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

    private void OnDisable()
    {
        Boss.OnBossDefeat -= OpenDoor;
    }
}
