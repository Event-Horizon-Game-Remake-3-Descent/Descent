using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponTest_UI : MonoBehaviour
{
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] TMP_Text WeaponNamePrimary_Text;
    [SerializeField] TMP_Text WeaponNameSecondary_Text;
    [SerializeField] TMP_Text BulletLeftPrimary_Text;
    [SerializeField] TMP_Text BulletLeftSecondary_Text;

    private string PrimaryWeaponName = "";
    private string SecondaryWeaponName = "";

    private float PrimaryBulletLeft;
    private float SecondaryBulletLeft;

    private void Start()
    {
        UpdateInfo();
    }

    private void UpdatePrimaryBulletCount()
    {
        PrimaryBulletLeft = weaponManager.CurrentPrimary.WeaponMag.ProjectileLeft;
        BulletLeftPrimary_Text.SetText("Bullet Left Primary: " + PrimaryBulletLeft);
    }

    private void UpdateSecondaryBulletCount()
    {
        SecondaryBulletLeft = weaponManager.CurrentSecondary.WeaponMag.ProjectileLeft;
        BulletLeftSecondary_Text.SetText("Bullet Left Secondary: " + SecondaryBulletLeft);
    }

    private void UpdateBulletCount()
    {
        UpdatePrimaryBulletCount();
        UpdateSecondaryBulletCount();
    }

    private void UpdateInfo()
    {
        //Update selected weapon
        PrimaryWeaponName = weaponManager.CurrentPrimary.WeaponName;
        SecondaryWeaponName = weaponManager.CurrentSecondary.WeaponName;
        WeaponNamePrimary_Text.text = "Primary: " + PrimaryWeaponName;
        WeaponNameSecondary_Text.text = "Secondary: " + SecondaryWeaponName;
        //update bullet count
        UpdatePrimaryBulletCount();
        UpdateSecondaryBulletCount();
    }

    private void OnEnable()
    {
        weaponManager.OnSecondaryFire += UpdateBulletCount;
        weaponManager.OnPrimaryFire += UpdateBulletCount;
        weaponManager.OnWeaponChanged += UpdateInfo;
    }

    private void OnDisable()
    {
        weaponManager.OnSecondaryFire -= UpdateBulletCount;
        weaponManager.OnPrimaryFire -= UpdateBulletCount;
        weaponManager.OnWeaponChanged -= UpdateInfo;
    }
}
