using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour {

    private bool IsHovered;
    public int _id = -1;
    private Color _color;

    //[SerializeField]
    public UnityEvent<GameObject> OnObjectHover;

    //[SerializeField]
    public UnityEvent<GameObject> OnClick;

    private MeshRenderer meshRenderer;
    
    private static readonly int MainColor = Shader.PropertyToID("_Color");

    void Start() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.SetColor(MainColor, _color);
    }

    public void Initialize(int id, Color color) {
        _id = id;
        _color = color;
    }

    // Update is called once per frame
    private void Update() {
        if (IsHovered && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
            OnClick?.Invoke(gameObject);
        }
    }

    public void SetHover(bool isHover) {
        IsHovered = isHover;
        if (isHover) {
            meshRenderer.material.SetColor(MainColor, Color.red);
            OnObjectHover?.Invoke(gameObject);
        } else {
            meshRenderer.material.SetColor(MainColor, _color);
        }
    }
}