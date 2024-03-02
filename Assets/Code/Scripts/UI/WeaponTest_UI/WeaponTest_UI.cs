using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTest_UI : MonoBehaviour
{
    [SerializeField] TMP_Text WeaponNamePrimary_Text;
    [SerializeField] TMP_Text WeaponNameSecondary_Text;
    [SerializeField] TMP_Text BulletLeftPrimary_Text;
    [SerializeField] TMP_Text BulletLeftSecondary_Text;
    [SerializeField] RawImage PrimaryWeaponImage;
    [SerializeField] RawImage SecondaryWeaponImage;
    [SerializeField] WeaponHUD_Rocket Rocket_HUD;

    private WeaponManager weaponManager;

    private string PrimaryWeaponName = "";
    private string SecondaryWeaponName = "";

    private float PrimaryBulletLeft;
    private float SecondaryBulletLeft;

    private void UpdatePrimaryBulletCount()
    {
        PrimaryBulletLeft = weaponManager.CurrentPrimary.WeaponMag.ProjectileLeft;
        BulletLeftPrimary_Text.SetText("Bullet Left Primary: " + Mathf.Round(PrimaryBulletLeft + 0.45f));
    }

    private void UpdateSecondaryBulletCount()
    {
        SecondaryBulletLeft = weaponManager.CurrentSecondary.WeaponMag.ProjectileLeft;
        BulletLeftSecondary_Text.SetText("Bullet Left Secondary: " + Mathf.Round(SecondaryBulletLeft + +0.45f));
        Rocket_HUD.UpdateHud(SecondaryBulletLeft);
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
        PrimaryWeaponImage.texture = weaponManager.CurrentPrimary.WeaponSprite;
    }

    private void UpdateSecondary()
    {
        SecondaryWeaponName = weaponManager.CurrentSecondary.WeaponName;
        WeaponNameSecondary_Text.text = "Secondary: " + SecondaryWeaponName;
        UpdateSecondaryBulletCount();
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
            Rocket_HUD.UpdateHud(0);
        };
    }

    private void OnDisable()
    {
        weaponManager.OnSecondaryFire -= UpdateBulletCount;
        weaponManager.OnPrimaryFire -= UpdateBulletCount;
        weaponManager.OnWeaponChanged -= UpdateInfo;
    }
}
