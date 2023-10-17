using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour {

    [SerializeField] private GameObject leftEye;
    [SerializeField] private GameObject rightEye;

    [SerializeField] private float rayDistance = 100f;

    [SerializeField] private float rayWidth = 0.01f;

    [SerializeField] private LayerMask layersToInclude;

    [SerializeField] private Color rayColor = Color.red;

    private const float weight = 0.06f;

    Vector3 oldForward;
    bool firstFrame = true;

    private LineRenderer _lineRenderer;
    private EyeInteractable _objectOnFocus;

    void Start() {
        _lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay() {
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.positionCount = 2;
        _lineRenderer.startWidth = rayWidth;
        _lineRenderer.endWidth = rayWidth;
        _lineRenderer.startColor = rayColor;
        _lineRenderer.endColor = rayColor;
        var position = transform.position;
        _lineRenderer.SetPosition(0, position);
        _lineRenderer.SetPosition(1, new Vector3(position.x, position.y, position.z + rayDistance));
    }

    private void Update() {

        Transform leftEyeTransform = leftEye.transform;
        Transform rightEyeTransform = rightEye.transform;

        transform.position = Vector3.Lerp(leftEyeTransform.position, rightEyeTransform.position, 0.5f);
        transform.rotation = Quaternion.Slerp(leftEyeTransform.rotation, rightEyeTransform.rotation, 0.5f);

        Vector3 rayCastDirection;

        if (firstFrame) {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            oldForward = forward;
            rayCastDirection = forward * rayDistance;
            firstFrame = false;
        } else {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 filteredForward =  ((forward * weight) + (oldForward * (1-weight))).normalized;
            rayCastDirection = filteredForward * rayDistance;
            oldForward = filteredForward;
        }

        RaycastHit hit;
        
        ClearFocus();

        if (Physics.Raycast(transform.position, rayCastDirection, out hit, Mathf.Infinity, layersToInclude)) {
             rayColor.a = 1f;
            _lineRenderer.startColor = rayColor;
            _lineRenderer.endColor = rayColor;
            _objectOnFocus = hit.transform.GetComponent<EyeInteractable>();
            _objectOnFocus.SetHover(true);
        } else {
            rayColor.a = 0f;
            _lineRenderer.startColor = rayColor;
            _lineRenderer.endColor = rayColor;
        }
    }
    
    public void ClearFocus() {
        if (_objectOnFocus == null) return;
        _objectOnFocus.SetHover(false);
        _objectOnFocus = null;
    }
}