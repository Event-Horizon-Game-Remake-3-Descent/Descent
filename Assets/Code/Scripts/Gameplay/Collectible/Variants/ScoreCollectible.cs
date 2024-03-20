using System;
using UnityEngine;

public class ScoreCollectible : Collectible
{
    public static Action<float> OnIncreaseScore;

    [Header("Score Collectible Setting")]
    [SerializeField] float IncreaseScoreValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 6)
            return;

        OnIncreaseScore(IncreaseScoreValue);

        base.TriggerEvents();
        Destroy(this.gameObject);
    }
}
