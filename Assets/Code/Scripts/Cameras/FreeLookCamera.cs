using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookCamera : MonoBehaviour
{
    [Header("Free Look Camera Settings")] 
    [SerializeField] Transform CameraTransform;
    [SerializeField] Camera Camera;
    [SerializeField] float MinDistance = 60f;
    [SerializeField] float MaxDistance = 350f;
    [SerializeField] float CamSmootingTime = 0.5f;
    [SerializeField] float ZoomSpeed = 0.025f;
    [SerializeField] float MaxSpeed = 15;
    [Space]
    [Header("Gizmos Settings")]
    [SerializeField] bool DrawGizmos = true;
    [SerializeField] Color MinDistanceColor = new Color(0f, 0.509f, 0.486f, 0.5f);
    [SerializeField] Color MaxDistanceColor = new Color(0f, 0.827f, 0.486f, 0.7f);

    private Coroutine UpdateCoroutine = null;
    private float currentDist;

    private Vector3 ManagerRotationSpeed = Vector3.zero;
    private float X = 0f;
    private float Y = 0f;

    private void Awake()
    {
        currentDist = MaxDistance;
        CameraTransform.localPosition = Vector3.back*MaxDistance;
    }

  
    private IEnumerator ComputeFreeLookRotation()
    {
        while (true)
        {
            if (InputManager.IsZooming(out Vector3 direction)) 
            {  
                Zoom(-direction.normalized.z);
            }
            //update camera rotation
            CameraTransform.rotation = GetNewCameraRotation();
            transform.rotation *= GetFreeLookRotation();
            //wait one frame
            yield return null;
        }
    }

    private Quaternion GetNewCameraRotation()
    {
        return Quaternion.LookRotation((transform.position - CameraTransform.position).normalized);
    }

    private Quaternion GetFreeLookRotation()
    {
        //get deltas in movement
        float deltaX = InputManager.InputMap.MiniMap.MouseX.ReadValue<float>() * GameManager.MouseSens * 0.02f;
        float deltaY = InputManager.InputMap.MiniMap.MouseY.ReadValue<float>() * GameManager.MouseSens * 0.02f;

        X = Mathf.SmoothDamp(X, deltaY, ref ManagerRotationSpeed.x, CamSmootingTime, MaxSpeed, 0.02f);
        Y = Mathf.SmoothDamp(Y, deltaX, ref ManagerRotationSpeed.y, CamSmootingTime, MaxSpeed, 0.02f);

        Quaternion rotation = Quaternion.Euler(-X * 0.02f, Y * 0.02f, 0);

        return rotation;
    }

    
    private void Zoom(float direction)
    {
        currentDist += ZoomSpeed * direction;
        currentDist = Mathf.Clamp(currentDist, MinDistance, MaxDistance);

        CameraTransform.localPosition = Vector3.back * currentDist;
    }

    private void OnEnable()
    {
        Camera.enabled = true;
        UpdateCoroutine = StartCoroutine(ComputeFreeLookRotation());
    }

    private void OnDisable()
    {
        StopCoroutine(UpdateCoroutine);
        UpdateCoroutine = null;
        Camera.enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(!DrawGizmos)
            return;
        //draw min distance
        Gizmos.color = MinDistanceColor;
        Gizmos.DrawWireSphere(transform.position, MinDistance);
        //draw max distance
        Gizmos.color = MaxDistanceColor;
        Gizmos.DrawWireSphere(transform.position, MaxDistance);

    }
#endif
}
