using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [Tooltip("List Of Primary weapons usable by the player")]
    [SerializeField] private List<Weapon> PrimaryWeaponList;
    [Tooltip("List Of Secondary weapons usable by the player")]
    [SerializeField] private List<Weapon> SecondaryWeaponList;
    [SerializeField] private Weapon FlareWeapon;
    [SerializeField] private Weapon BombWeapon;
    [Space]
    [Header("Exposed Mags")]
    [SerializeField] private Mag _EnergyMag;
    [SerializeField] private Mag _BombsMag;
    [Header("Change Weapon")]
    [SerializeField] private float ChangeWeaponCooldown;
    [Space]
    [Header("SFX")]
    [Tooltip("Sound reproduced when player can swich weapon again -- NOT ACTIVE")]
    [SerializeField] private AudioClip WeaponReadyAudoClip;

    //public using weapon
    [HideInInspector] public Weapon CurrentPrimary{ get; private set; }
    [HideInInspector] public int CurrentPrimaryIndex{ get; private set; }
    [HideInInspector] public Weapon CurrentSecondary{ get; private set; }
    [HideInInspector] public Mag EnergyMag{ get; private set; }
    [HideInInspector] public Mag BombsMag { get; private set; }

    //Indexers
    private int PrimaryIndex = 0;
    private int SecondaryIndex = 0;
    //Used for delay
    private bool CanChange = true;
    //SFX
    private AudioSource AudioSource;

    private void Awake()
    {
        CurrentPrimary = PrimaryWeaponList[PrimaryIndex];
        CurrentSecondary = SecondaryWeaponList[SecondaryIndex];

        EnergyMag = _EnergyMag;
        BombsMag = _BombsMag;

        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = WeaponReadyAudoClip;
        AudioSource.playOnAwake = false;
    }

    private void Start()
    {
        OnManagerReady(this);
    }

    

    //Use The next index weapon
    private void ChangeWeapon(ref int Index, List<Weapon> WeaponList)
    {
        do
        {
            Index++;
            if (Index > WeaponList.Count - 1)
                Index = 0;

        } while (!WeaponList[Index].IsUnlocked);
    }

    void ChangePrimary()
    {
        if (CanChange)
        {
            CanChange = false;
            ChangeWeapon(ref PrimaryIndex, PrimaryWeaponList);
            CurrentPrimary = PrimaryWeaponList[PrimaryIndex];
            CurrentPrimaryIndex = PrimaryIndex;
            //call OnWeaponChanged event
            OnWeaponChanged();
        }
    }

    void ChangeSecondary()
    {
        if (CanChange)
        {
            CanChange = false;
            ChangeWeapon(ref SecondaryIndex, SecondaryWeaponList);
            CurrentSecondary = SecondaryWeaponList[SecondaryIndex];
            //call OnWeaponChanged event
            OnWeaponChanged();
        }
    }

    //shoot functions
    private void ShootPrimary()
    {
        if (PrimaryWeaponList[PrimaryIndex].Shoot())
        {
            OnPrimaryFire();
        }
    }
    
    private void ShootSecondary()
    {
        if (SecondaryWeaponList[SecondaryIndex].Shoot())
        {
            OnSecondaryFire();
        }
    }
    
    private void ShootBomb()
    {
        BombWeapon.Shoot();
    }

    private void ShootFlare()
    {
        if (FlareWeapon.Shoot())
            OnPrimaryFire();
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
        OnWeaponChanged += () => StartCoroutine(StartCooldown());

        InputManager.OnPrimaryCalled += ShootPrimary;
        InputManager.OnSecondaryCalled += ShootSecondary;
        InputManager.OnLaunchingBomb += ShootBomb;
        PlayerController.OnPlayerDead += OnDeath;
        InputManager.OnSwitchPrimary += ChangePrimary;
        InputManager.OnSwitchSecondary += ChangeSecondary;
        InputManager.OnLaunchingFlare += ShootFlare;
    }

    //Disable all connected events
    private void OnDisable()
    {
        OnManagerReady -= OnManagerReady;
        OnWeaponChanged -= OnWeaponChanged;
        OnPrimaryFire -= OnPrimaryFire;
        OnSecondaryFire -= OnSecondaryFire;

        InputManager.OnPrimaryCalled -= ShootPrimary;
        InputManager.OnSecondaryCalled -= ShootSecondary;
        InputManager.OnLaunchingBomb -= ShootBomb;
        InputManager.OnSwitchPrimary -= ChangePrimary;
        InputManager.OnSwitchSecondary -= ChangeSecondary;
        InputManager.OnLaunchingFlare -= ShootFlare;

        PlayerController.OnPlayerDead -= OnDeath;
    }

    private void ResetManager()
    {
        for(int i = 1; i< PrimaryWeaponList.Count; i++)
        {
            PrimaryWeaponList[i].ResetWeapon();
            PrimaryWeaponList[i].IsUnlocked = false;
        }

        for (int i = 0; i < SecondaryWeaponList.Count; i++)
        {
            SecondaryWeaponList[i].ResetWeapon();
        }
    }

    private void OnDeath()
    {
        ResetManager();
        //TODO: DROP COLLECTIBLES
    }
}
