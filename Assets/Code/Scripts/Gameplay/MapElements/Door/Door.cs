using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Door : MonoBehaviour
{
    public delegate void DoorDelegate();
    public DoorDelegate OnDoorOpen = () => { };
    public DoorDelegate OnDoorClosed = () => { };

    [Header("Door Settings")]
    [SerializeField] protected List<DoorPanel> ListOfPanels;
    [Header("SFX Settings")]
    [SerializeField] protected AudioClip OpenSFX;
    [SerializeField] protected AudioClip CloseSFX;

    protected AudioSource SFX_Source;

    protected void Awake()
    {
        SFX_Source = GetComponent<AudioSource>();
    }
}
