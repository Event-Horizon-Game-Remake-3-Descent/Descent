using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour, IDamageable
{
    public delegate void DoorDelegate();
    public DoorDelegate OnDoorOpen = () => { };
    public DoorDelegate OnDoorClosed = () => { };

    Coroutine IsOccupied = null;

    private enum TriggerType
    {
        None,
        Damage,
        Key,
        Proximity,
    }

    [Header("Door Setting")]
    [SerializeField] private TriggerType TypeOfTrigger = TriggerType.None;
    [SerializeField] private float DoorHP = 20f;
    public float HP { get; set; }

    [Header("Animation Settings")]
    [SerializeField] private List<Transform> ObjectsToMove;
    [SerializeField] float OpenDistance;
    [SerializeField] float OpenSpeed = 1;
    [SerializeField] float CloseSpeed = 0.5f;
    [Header("SFX")]
    [SerializeField] AudioClip OpenSound;
    [SerializeField] AudioClip CloseSound;

    private AudioSource AudioSource;
    private float PercentageOpen = 0f;

    private Vector3[] StartPos;
    private Vector3[] EndPos;

    private void Awake()
    {
        HP = DoorHP;
        AudioSource = GetComponent<AudioSource>();

        //Get Start Pos
        StartPos = new Vector3[ObjectsToMove.Count];
        for (int i = 0; i < ObjectsToMove.Count; i++)
            StartPos[i] = ObjectsToMove[i].position;


        //Get End Pos
        EndPos = new Vector3[ObjectsToMove.Count];
        for (int i = 0; i < ObjectsToMove.Count; i++)
            EndPos[i] = ObjectsToMove[i].position + (ObjectsToMove[i].forward * OpenDistance);
    }

    #region ANIMATIONS
    private IEnumerator SetDoorProgress(float x)
    {
        x = math.clamp(x, 0f, 1f);

        int dir = 1;

        if (x < PercentageOpen)
            dir = -1;

        Vector3[] startPos = new Vector3[ObjectsToMove.Count];
        Vector3[] endPos = new Vector3[ObjectsToMove.Count];

        //Get Start Pos
        for (int i = 0; i < ObjectsToMove.Count; i++)
            startPos[i] = ObjectsToMove[i].position;
        //Get EndPos
        for (int i = 0; i < ObjectsToMove.Count; i++)
            endPos[i] = ObjectsToMove[i].position + (ObjectsToMove[i].forward * OpenDistance * x * dir);

        float progress = 0;
        while (progress < 1)
        {
            //Move Door
            for (int i = 0; i < ObjectsToMove.Count; i++)
            {
                Vector3 newPos = Vector3.Lerp(startPos[i], endPos[i], progress);
                ObjectsToMove[i].position = newPos;
            }
            //Update progress
            progress += Time.fixedDeltaTime * OpenSpeed;
            yield return null;
        }
        PercentageOpen = x;
        IsOccupied = null;
    }

    private IEnumerator OpenDoor()
    {
        OnDoorOpen();
        this.AudioSource.clip = OpenSound;
        this.AudioSource.Play();

        float progress = 0;

        while (progress < 1)
        {
            //Move Door
            for (int i = 0; i < ObjectsToMove.Count; i++)
            {
                Vector3 newPos = Vector3.Lerp(StartPos[i], EndPos[i], progress);
                ObjectsToMove[i].position = newPos;
            }

            progress += Time.fixedDeltaTime * OpenSpeed;
            yield return null;
        }

        PercentageOpen = 1f;
        IsOccupied = null;
    }

    private IEnumerator CloseDoor()
    {
        this.AudioSource.clip = CloseSound;
        this.AudioSource.Play();

        float progress = 0;

        while (progress < 1)
        {
            //Move Door
            for (int i = 0; i < ObjectsToMove.Count; i++)
            {
                Vector3 newPos = Vector3.Lerp(EndPos[i], StartPos[i], progress);
                ObjectsToMove[i].position = newPos;
            }
            //Update Timers
            progress += Time.fixedDeltaTime * CloseSpeed;
            yield return null;
        }

        PercentageOpen = 0f;
        OnDoorClosed();
        IsOccupied = null;
    }
    #endregion

    public void TakeDamage(float Damage)
    {
        HP -= Damage;

        if (TypeOfTrigger == TriggerType.Damage)
        {
            if(HP > 0)
                IsOccupied = StartCoroutine(SetDoorProgress((DoorHP - HP) / DoorHP));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (TypeOfTrigger == TriggerType.Proximity)
            {
                if(IsOccupied == null)
                    IsOccupied = StartCoroutine(OpenDoor());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TypeOfTrigger == TriggerType.Proximity)
            {
                if (IsOccupied == null)
                    IsOccupied = StartCoroutine(CloseDoor());
            }
        }
    }
}
