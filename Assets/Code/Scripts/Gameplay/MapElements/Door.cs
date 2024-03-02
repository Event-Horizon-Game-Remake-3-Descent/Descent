using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Door : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] Transform UpperDoor;
    [SerializeField] Transform LowerDoor;
    [SerializeField] float OpenDistance = 5f;
    [SerializeField] float AnimationSpeed;
    [Header("SFX")]
    [SerializeField] AudioClip OpenSound;
    [SerializeField] AudioClip CloseSound;
}
