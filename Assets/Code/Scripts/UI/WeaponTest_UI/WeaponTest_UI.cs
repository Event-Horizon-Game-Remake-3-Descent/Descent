using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class WeaponTest_UI : MonoBehaviour
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
        
        (Mathf.Round(PrimaryBulletLeft + 0.45f)).ToString();
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
    }

    private void OnEnable()
    {
        WeaponManager.OnManagerReady += (WeaponManager Mangager) =>
        {
            //get weapon manager
            weaponManager = Mangager;
            //connect all events
            weaponManager.OnSecondaryFire += UpdateBulletCount;
            weaponManager.OnPrimaryFire += UpdateBulletCount;
            weaponManager.OnWeaponChanged += UpdateInfo;
            //update UI
            UpdateInfo();
            MultipleShootingPoints.UpdateHud(0);
        };

        Collectible.OnUpdateUI += UpdateInfo;
    }

    private void OnDisable()
    {
        //weaponManager.OnSecondaryFire -= UpdateBulletCount;
        //weaponManager.OnPrimaryFire -= UpdateBulletCount;
        //weaponManager.OnWeaponChanged -= UpdateInfo;
    }
}
