using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class SpecialZone : MonoBehaviour
{
    [Header("Special Zone Settings")]
    [SerializeField] private float EffectAppliedFrequency = 1f;

    private WaitForSeconds WaitTime;
    private Coroutine EffectCoroutine;

    protected void Awake()
    {
        WaitTime = new WaitForSeconds(1/EffectAppliedFrequency);
        EffectCoroutine = null;
    }

    protected virtual void ApplyEffect(Collider collider)
    {

    }

    private IEnumerator EffectEnumerator(Collider collider)
    {
        while(true)
        {
            yield return WaitTime;
            ApplyEffect(collider);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        EffectCoroutine = StartCoroutine(EffectEnumerator(collider));
    }

    private void OnTriggerExit(Collider collider)
    {
        StopCoroutine(EffectCoroutine);
    }
}
