using System.Collections;
using UnityEditor;
using UnityEngine;

public class DoorPanel : MonoBehaviour, IDamageable
{
    public delegate void PanelDamaged(float damageTaken);
    public event PanelDamaged OnPanelTrigger = (float damage) => { };

    public delegate void PanelOpen();
    public event PanelOpen OnPanelOpen = () => { };
    
    public delegate void PanelClose();
    public event PanelClose OnPanelClose = () => { };

    [Header("Panel Settings")]
    [SerializeField] Vector3 Direction = Vector3.forward;
    [SerializeField] float Distance = 0f;
    [SerializeField] float OpenSpeed = 1f;
    [SerializeField] float CloseSpeed = 1f;

    [Header("Gizmos Settings")]
    [SerializeField] private bool DrawGizmos = true;
    [SerializeField] private Color DistanceColor = Color.white;
    [SerializeField] private Color DirectionColor = Color.red;

    private Coroutine MovingCoroutine;

    private float PercentageOpen = 0f;

    public float HP { get; set; } = 0;

    Vector3 StartPos;
    Vector3 EndPos;

    private void Awake()
    {
        StartPos = transform.position;
        EndPos = StartPos + transform.TransformDirection(Direction.normalized * Distance);
    }
    
    public void TakeDamage(float damage)
    {
        OnPanelTrigger(damage);
    }

    public void MovePanel(int dir, float percentage)
    {
        MovingCoroutine = dir > 0 ? StartCoroutine(OpenCoroutine(percentage)) : StartCoroutine(CloseCoroutine(percentage));
    }

    private IEnumerator OpenCoroutine(float percentage)
    {
        float progress = PercentageOpen;
        while (progress < percentage)
        {
            Vector3 newPos;
            newPos = Vector3.Lerp(StartPos, EndPos, progress);

            transform.position = newPos;

            progress += Time.fixedDeltaTime * OpenSpeed;
            yield return null;
        }
        PercentageOpen = percentage;
        MovingCoroutine = null;
        OnPanelOpen();
    }

    private IEnumerator CloseCoroutine(float percentage)
    {
        float progress = PercentageOpen;
        while (progress > percentage)
        {
            Vector3 newPos;
            newPos = Vector3.Lerp(StartPos, EndPos, progress);

            transform.position = newPos;

            progress -= Time.fixedDeltaTime * CloseSpeed;
            yield return null;
        }
        PercentageOpen = percentage;
        MovingCoroutine = null;
        OnPanelClose();
    }

    private void OnDisable()
    {
        OnPanelOpen -= OnPanelOpen; 
        OnPanelClose -= OnPanelClose;
        OnPanelTrigger -= OnPanelTrigger;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!DrawGizmos)
            return;

        if (Application.isPlaying)
            return;

        Handles.color = DistanceColor;
        Handles.DrawWireDisc(transform.position, transform.up, Distance);

        Gizmos.color = DirectionColor;
        Gizmos.DrawSphere(transform.position + transform.TransformDirection(Direction.normalized * Distance), 0.2f);
    }
#endif
}
