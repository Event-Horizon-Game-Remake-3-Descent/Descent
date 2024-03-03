using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponManager : MonoBehaviour
{
    //Weapon Manager ready
    public delegate void WeaponManagerReady(WeaponManager weaponManager);
    public static event WeaponManagerReady OnManagerReady = (WeaponManager weaponManager) => { };

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
        ////Disable primary weapons except for the first one
        //for (int i = 1; i < PrimaryWeaponList.Count; i++)
        //    PrimaryWeaponList[i].gameObject.SetActive(false);
        ////Disable secondary weapons except for the first one
        //for (int i = 1; i < SecondaryWeaponList.Count; i++)
        //    SecondaryWeaponList[i].gameObject.SetActive(false);

        CurrentPrimary = PrimaryWeaponList[PrimaryIndex];
        CurrentSecondary = SecondaryWeaponList[SecondaryIndex];

        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = WeaponReadyAudoClip;
        AudioSource.playOnAwake = false;
    }

    private void Start()
    {
        OnManagerReady(this);
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

    [Space]
    [Header("SPECIAL TO REMOVE")]
    public KeyCode FLARE_SHOOTKEY;

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

        if (Input.GetKey(FLARE_SHOOTKEY))
        {
            if (FlareWeapon.Shoot())
                OnPrimaryFire();
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

        if (!WeaponList[Index].Unlocked)
        {
            Debug.Log(WeaponList[Index].WeaponName + " Is Locked");
            Index--;
            if (Index < 0)
                Index = WeaponList.Count - 1;
            return;
        }

        ////enable new weapon
        //WeaponList[Index].gameObject.SetActive(true);
        ////disable unused weapon
        //for (int i = 0; i < WeaponList.Count; i++)
        //{
        //    if (i == Index)
        //        continue;
        //    else
        //        WeaponList[i].gameObject.SetActive(false);
        //}
    }

    private IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(ChangeWeaponCooldown);
        CanChange = true;
        //AudioSource.Play();
    }

    //Get Current weapon
    public Weapon GetPrimaryWeapon() { return PrimaryWeaponList[PrimaryIndex]; }
    public Weapon GetSecondaryWeapon() { return SecondaryWeaponList[SecondaryIndex]; }

    //subscribe to events
    private void OnEnable()
    {
        OnWeaponChanged += () =>
        {
            //StopCoroutine(StartCooldown());
            StartCoroutine(StartCooldown());
        };
    }

    //Disable all connected events
    private void OnDisable()
    {
        OnManagerReady -= OnManagerReady;
        OnWeaponChanged -= OnWeaponChanged;
        OnPrimaryFire -= OnPrimaryFire;
        OnSecondaryFire -= OnSecondaryFire;
    }

    private void OnDeath()
    {

    }

}
