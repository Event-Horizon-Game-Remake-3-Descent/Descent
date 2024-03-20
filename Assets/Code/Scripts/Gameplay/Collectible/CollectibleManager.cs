using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private CollectibleManager Instance;

    [Header("Collectible Manager Settings")]
    [SerializeField] private float RespawnMaxSpeed = 0.75f;
    [SerializeField] private float RespawnMinSpeed = 0.8f;

    private List<WeaponCollectible> PickedUpWeapon = new List<WeaponCollectible>();
    private Transform PlayerTransform;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void AddWeaponCollectible(WeaponCollectible collectible)
    {
        //move to origin
        collectible.transform.position = transform.position;
        //add collectible
        PickedUpWeapon.Add(collectible);
    }

    private void SetUp(PlayerController Player)
    {
        PlayerTransform = Player.transform;
    }

    private void RecoverCollectibles()
    {
        for(int i = 0; i < PickedUpWeapon.Count; i++)
        {
            PickedUpWeapon[i].transform.position = PlayerTransform.position;
            PickedUpWeapon[i].ApplyForce(Random.insideUnitSphere, Random.Range(RespawnMinSpeed, RespawnMaxSpeed), ForceMode.Impulse);
        }
    }

    private void OnEnable()
    {
        WeaponCollectible.OnWeaponCollectiblePickedUp += AddWeaponCollectible;
        PlayerController.OnPlayerDead += RecoverCollectibles;
        PlayerController.OnPlayerReady += SetUp;
    }

    private void OnDisable()
    {
        WeaponCollectible.OnWeaponCollectiblePickedUp -= AddWeaponCollectible;
        PlayerController.OnPlayerDead -= RecoverCollectibles;
        PlayerController.OnPlayerReady -= SetUp;
    }
}
