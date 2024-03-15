using UnityEngine;

[System.Serializable]
public class RandomData
{
    [SerializeField] public Transform ObjectToGenerate;
    [SerializeField][Range(0f, 1f)] public float Probability;
}
