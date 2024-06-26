using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("Primay Weapon")]
    [SerializeField] TMP_Text WeaponNamePrimary_Text;
    [SerializeField] TMP_Text BulletLeftPrimary_Text;
    [SerializeField] RawImage PrimaryWeaponImage;
    [Header("Secondary Weapon")]
    [SerializeField] TMP_Text WeaponNameSecondary_Text;
    [SerializeField] TMP_Text BulletLeftSecondary_Text;
    [SerializeField] RawImage SecondaryWeaponImage;
    [Header("Generic Weapon")]
    [SerializeField] TMP_Text Energy_Text;
    [Space]
    [Header("HUD")]
    [SerializeField] RectTransform SingleShootingPoint;
    [SerializeField] MultipleShootingPoints_HUD MultipleShootingPoints;
    [SerializeField] Texture DefaultWeaponImage;
    [SerializeField] Image EnergyBarLeft;
    [SerializeField] Image EnergyBarRight;

    private WeaponManager weaponManager;

    private string PrimaryWeaponName = "";
    private string SecondaryWeaponName = "";

    private float PrimaryBulletLeft;
    private float EnergyLeft;
    private float SecondaryBulletLeft;

    private void UpdatePrimaryBulletCount()
    {
        PrimaryBulletLeft = weaponManager.CurrentPrimary.WeaponMag.ProjectileLeft;
        BulletLeftPrimary_Text.text = Mathf.Ceil(PrimaryBulletLeft).ToString();

        if (weaponManager.CurrentPrimaryIndex == 0)
            BulletLeftPrimary_Text.alpha = 0f;
        else
            BulletLeftPrimary_Text.alpha = 1f;

        EnergyLeft = weaponManager.EnergyMag.ProjectileLeft;

        Energy_Text.text = Mathf.Ceil(weaponManager.EnergyMag.ProjectileLeft).ToString();
    }

    private void UpdateSecondaryBulletCount()
    {
        SecondaryBulletLeft = weaponManager.CurrentSecondary.WeaponMag.ProjectileLeft;
        BulletLeftSecondary_Text.text = Mathf.Ceil(SecondaryBulletLeft).ToString();
        MultipleShootingPoints.UpdateHud(SecondaryBulletLeft);
    }

    private void UpdateBulletCount()
    {
        UpdatePrimaryBulletCount();
        UpdateSecondaryBulletCount();
        HandleVisualMag();
    }

    private void UpdatePrimary()
    {
        PrimaryWeaponName = weaponManager.CurrentPrimary.WeaponName;
        WeaponNamePrimary_Text.text = "Primary: " + PrimaryWeaponName;
        UpdatePrimaryBulletCount();

        //apply default sprite if null
        if (!weaponManager.CurrentPrimary.WeaponSprite)
        {
            PrimaryWeaponImage.texture = DefaultWeaponImage;
            return;
        }

        PrimaryWeaponImage.texture = weaponManager.CurrentPrimary.WeaponSprite;
    }

    private void UpdateSecondary()
    {
        SecondaryWeaponName = weaponManager.CurrentSecondary.WeaponName;
        WeaponNameSecondary_Text.text = "Secondary: " + SecondaryWeaponName;
        if(weaponManager.CurrentSecondary.ShootingPointsAmount == 2)
        {
            MultipleShootingPoints.gameObject.SetActive(true);
            SingleShootingPoint.gameObject.SetActive(false);
        }
        else
        {
            MultipleShootingPoints.gameObject.SetActive(false);
            SingleShootingPoint.gameObject.SetActive(true);
        }
        UpdateSecondaryBulletCount();
        //apply default sprite if null
        if(!weaponManager.CurrentSecondary.WeaponSprite)
        {
            SecondaryWeaponImage.texture = DefaultWeaponImage;
            return;
        }

        SecondaryWeaponImage.texture = weaponManager.CurrentSecondary.WeaponSprite;
    }

    private void UpdateInfo()
    {
        UpdatePrimary();
        UpdateSecondary();
        UpdateBulletCount();
    }

    void HandleVisualMag()
    {
        if (EnergyLeft <= 100)
        {
            EnergyBarLeft.fillAmount = EnergyLeft / 100;
            EnergyBarRight.fillAmount = EnergyLeft / 100;
        }
        else if (EnergyLeft > 100)
        {
            EnergyBarLeft.fillAmount = 1f;
            EnergyBarRight.fillAmount = 1f;
        }
    }

    private void SetUpManager(WeaponManager Manager)
    {
        //get weapon manager
        weaponManager = Manager;
        //connect all events
        weaponManager.OnSecondaryFire += UpdateBulletCount;
        weaponManager.OnPrimaryFire += UpdateBulletCount;
        weaponManager.OnWeaponChanged += UpdateInfo;
        //update UI
        UpdateInfo();
        MultipleShootingPoints.UpdateHud(0);
        weaponManager.OnPrimaryFire += HandleVisualMag;
        HandleVisualMag();
    }

    private void RecoverEnergyUI(float x)
    {
        UpdateBulletCount();
        HandleVisualMag();
    }

    private void DeathWeaponUI()
    {
        UpdateBulletCount();
        UpdateInfo();
        HandleVisualMag();
    }

    void DisableUI(InputAction.CallbackContext ui)
    {
        Energy_Text.gameObject.SetActive(false);
        SingleShootingPoint.gameObject.SetActive(false);
        MultipleShootingPoints.gameObject.SetActive(false);
        EnergyBarLeft.gameObject.SetActive(false);
        EnergyBarRight.gameObject.SetActive(false); 
    }

    void EnableUI(InputAction.CallbackContext noui)
    {
        Energy_Text.gameObject.SetActive(true);
        SingleShootingPoint.gameObject.SetActive(true);
        MultipleShootingPoints.gameObject.SetActive(true);
        EnergyBarLeft.gameObject.SetActive(true);
        EnergyBarRight.gameObject.SetActive(true);
    }

    private void Start()
    {
        InputManager.InputMap.Overworld.SwitchCamera.started += DisableUI;
        InputManager.InputMap.Overworld.SwitchCamera.canceled += EnableUI;
    }

    private void OnEnable()
    {
        WeaponManager.OnManagerReady += SetUpManager;
        Collectible.OnUpdateUI += UpdateInfo;
        EnergyRecoverZone.OnEnergyRecover += RecoverEnergyUI;
        PlayerController.OnPlayerDead += DeathWeaponUI;
        
    }

    private void OnDisable()
    {
        WeaponManager.OnManagerReady -= SetUpManager;
        Collectible.OnUpdateUI -= UpdateInfo;
        EnergyRecoverZone.OnEnergyRecover -= RecoverEnergyUI;
        PlayerController.OnPlayerDead -= DeathWeaponUI;
        InputManager.InputMap.Overworld.SwitchCamera.started -= DisableUI;
        InputManager.InputMap.Overworld.SwitchCamera.canceled -= EnableUI;
    }
}
