using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Door
{
    [Header("Proximity Door Setting")]
    [Tooltip("Seconds to wait after the player has exit the trigger zone")]
    [SerializeField] private float CloseAfter = 5f;

    private Coroutine CheckForStay = null;

    private bool IsOpening = false;
    private bool IsOpen = false;
    private bool CanClose = false;

    new private void Awake()
    {
        base.Awake();

        for (int i = 0; i < ListOfPanels.Count; i++)
        {
            ListOfPanels[i].OnPanelTrigger += (float damage) => OpenDoor();
            ListOfPanels[i].OnPanelOpen += () =>
            {
                IsOpening = false;
                IsOpen = true;

                StartCoroutine(CloseCoroutine());
            };
        }
    }

    private void OpenDoor()
    {
        //if is open or is opening don't open the door
        if (IsOpening || IsOpen)
            return;

        OnDoorOpen();

        SFX_Source.clip = OpenSFX;
        SFX_Source.Play();

        for (int i = 0; i < ListOfPanels.Count; i++)
        {
            ListOfPanels[i].MovePanel(1, 1f);
        }

        IsOpening = true;
    }

    private void CloseDoor()
    {
        SFX_Source.clip = CloseSFX;
        SFX_Source.Play();

        IsOpen = false;

        for (int i = 0; i < ListOfPanels.Count; i++)
        {
            ListOfPanels[i].MovePanel(-1, 0f);
        }
        OnDoorClosed();
    }

    private IEnumerator CloseCoroutine()
    {
        yield return new WaitForSeconds(CloseAfter);
        yield return new WaitUntil(() => CanClose);
        CloseDoor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOpen)
            if (other.CompareTag("Player"))
            {
                OpenDoor();
                CheckForStay = StartCoroutine(OnTriggerStay(other));
            }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CloseDoor();
            if (CheckForStay != null)
                StopCoroutine(CheckForStay);
        }
    }

    private IEnumerator OnTriggerStay(Collider other)
    {
        CanClose = !other.CompareTag("Player");
        yield return null;
    }
}
