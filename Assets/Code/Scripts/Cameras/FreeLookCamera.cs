using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [Header("Free Look Camera Settings")] 
    [SerializeField] Transform CameraTransform;
    [SerializeField] Camera Camera;
    [SerializeField] float MinDistance = 60f;
    [SerializeField] float MaxDistance = 350f;
    [SerializeField] float CamSmootingTime = 0.5f;
    [SerializeField] float ZoomSpeed = 0.025f;
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
        CameraTransform.position = CameraTransform.forward * MaxDistance;
    }

    private void Start()
    {
        UpdateCoroutine = StartCoroutine(ComputeFreeLookRotation());
    }

    private void Update()
    {
        //TODO: Remove
        if(Input.GetKey(KeyCode.W))
            ZoomIn();
        if (Input.GetKey(KeyCode.S))
            ZoomOut();
    }

    private IEnumerator ComputeFreeLookRotation()
    {
        while (true)
        {
            //update camera rotation
            CameraTransform.rotation = GetNewCameraRotation();
            transform.rotation *= GetFreeLookRotation();
            //update camera position
            CameraTransform.position = transform.forward * currentDist;
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
        float deltaX = InputManager.InputMap.Overworld.MouseX.ReadValue<float>() * GameManager.MouseSens * 0.02f;
        float deltaY = InputManager.InputMap.Overworld.MouseY.ReadValue<float>() * GameManager.MouseSens * 0.02f;

        X = Mathf.SmoothDamp(X, deltaY, ref ManagerRotationSpeed.x, CamSmootingTime);
        Y = Mathf.SmoothDamp(Y, deltaX, ref ManagerRotationSpeed.y, CamSmootingTime);

        Quaternion rotation = Quaternion.Euler(-X * Time.fixedDeltaTime, Y * Time.fixedDeltaTime, 0);

        return rotation;
    }

    private void ZoomIn()
    {
        currentDist += ZoomSpeed;
        currentDist = Mathf.Clamp(currentDist, MinDistance, MaxDistance);
    }

    private void ZoomOut()
    {
        currentDist -= ZoomSpeed;
        currentDist = Mathf.Clamp(currentDist, MinDistance, MaxDistance);
    }

    private void OnEnable()
    {
        Camera.enabled = true;
        //UpdateCoroutine = StartCoroutine(FreeLook());
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
