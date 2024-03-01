using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponManager : MonoBehaviour
{
    //weapon changed event
    public delegate void WeaponChanged();
    public event WeaponChanged OnWeaponChanged = () => {};

    //weapon shoot event
    public delegate void WeaponShoot();
    public event WeaponShoot OnPrimaryFire = () => { };
    public event WeaponShoot OnSecondaryFire = () => { };

    [Header("Weapons")]
    [SerializeField] private List<Weapon> PrimaryWeaponList;
    [SerializeField] private List<Weapon> SecondaryWeaponList;
    [SerializeField] private Weapon FlareWeapon;
    [SerializeField] private Weapon BombWeapon;
    [Space]
    [Header("Change Weapon")]
    [SerializeField] private float ChangeWeaponCooldown;
    [Space]
    [Header("SFX")]
    [SerializeField] private AudioClip WeaponReadyAudoClip;

    //public using weapon
    [HideInInspector] public Weapon CurrentPrimary{ get; private set; }
    [HideInInspector] public Weapon CurrentSecondary{ get; private set; }

    //Indexers
    private int PrimaryIndex = 0;
    private int SecondaryIndex = 0;
    //Used for delay
    private bool CanChange = true;
    //SFX
    private AudioSource AudioSource;

    private void Awake()
    {
        //Disable primary weapons except for the first one
        for (int i = 1; i < PrimaryWeaponList.Count; i++)
            PrimaryWeaponList[i].gameObject.SetActive(false);
        //Disable secondary weapons except for the first one
        for (int i = 1; i < SecondaryWeaponList.Count; i++)
            SecondaryWeaponList[i].gameObject.SetActive(false);

        CurrentPrimary = PrimaryWeaponList[PrimaryIndex];
        CurrentSecondary = SecondaryWeaponList[SecondaryIndex];

        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = WeaponReadyAudoClip;
        AudioSource.playOnAwake = false;
    }

    #region TO REMOVE
    //////////TO REMOVE//////////////////////
    [Space]
    [Header("TO REMOVE CHANGE KEYS")]
    public KeyCode CHANGE_PRIMARY;
    public KeyCode CHANGE_SECONDARY;

    [Space]
    [Header("TO REMOVE SHOT KEYS")]
    public KeyCode PRIMARY_SHOOTKEY;
    public KeyCode SECONDARY_SHOOTKEY;

    private void Update()
    {
        if (Input.GetKeyDown(CHANGE_PRIMARY))
        {
            if(CanChange)
            {
                CanChange = false;
                ChangeWeapon(ref PrimaryIndex, PrimaryWeaponList);
                CurrentPrimary = PrimaryWeaponList[PrimaryIndex];
                //call OnWeaponChanged event
                OnWeaponChanged();
            }
        }
        if (Input.GetKeyDown(CHANGE_SECONDARY))
        {
            if(CanChange)
            {
                CanChange = false;
                ChangeWeapon(ref SecondaryIndex, SecondaryWeaponList);
                CurrentSecondary = SecondaryWeaponList[SecondaryIndex];
                //call OnWeaponChanged event
                OnWeaponChanged();
            }
        }
        if (Input.GetKey(PRIMARY_SHOOTKEY))
        {
            if(PrimaryWeaponList[PrimaryIndex].Shoot())
                OnPrimaryFire();
        }
        if (Input.GetKey(SECONDARY_SHOOTKEY))
        {
            if(SecondaryWeaponList[SecondaryIndex].Shoot())
                OnSecondaryFire();
        }
    }
    /////////////////////////////////////////
    #endregion

    //Use The next index weapon
    private void ChangeWeapon(ref int Index, List<Weapon> WeaponList)
    {
        Index++;
        if (Index > WeaponList.Count -1)
            Index = 0;

        //enable new weapon
        WeaponList[Index].gameObject.SetActive(true);
        //disable unused weapon
        for (int i = 0; i < WeaponList.Count; i++)
        {
            if (i == Index)
                continue;
            else
                WeaponList[i].gameObject.SetActive(false);
        }
    }

    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(ChangeWeaponCooldown);
        CanChange = true;
        //AudioSource.Play();
    }

    //Unlock a new weapon
    private void AddToPrimary(Weapon Weapon)
    {
        PrimaryWeaponList.Add(Weapon);
    }

    private void AddToSecondary(Weapon Weapon)
    {
        SecondaryWeaponList.Add(Weapon);
    }

    //Get Current weapon
    public Weapon GetPrimaryWeapon() { return PrimaryWeaponList[PrimaryIndex]; }
    public Weapon GetSecondaryWeapon() { return SecondaryWeaponList[SecondaryIndex]; }

    //subscribe to events
    private void OnEnable()
    {
        OnWeaponChanged += () =>
        {
            StopCoroutine(StartCooldown());
            StartCoroutine(StartCooldown());
        };
    }

    //Disable all connected events
    private void OnDisable()
    {
        //disconnect connected OnWeaponChanged function
        OnWeaponChanged -= OnWeaponChanged;
        OnPrimaryFire -= OnPrimaryFire;
    }

}
