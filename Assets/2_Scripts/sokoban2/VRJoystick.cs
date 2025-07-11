using Unity.VisualScripting;
using UnityEngine;
// 인터렉터블에 대한 클래스를 사용하기 위해 추가함
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRJoystick : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform using XR Grab Interactable.")]
    [SerializeField] Transform _grab;
    [Tooltip("Rotation point (Rotates based on Grab local position).")]
    [SerializeField] Transform _pivot;

    // _grabRb계층의 리지드바디를 찾아 저장할 변수
    private Rigidbody _grabRb;
    private XRGrabInteractable _grabInteractable;

    [SerializeField] float _sensitivity = 4f;


    private void Awake()
    {
        _grabRb = _grab.GetComponent<Rigidbody>();
        _grabInteractable = _grab.GetComponent<XRGrabInteractable>();
    }

    private void Move()
    {
        _pivot.localRotation = new(_grab.localPosition.z * _sensitivity, 
                                    _grab.localPosition.y, 
                                    -_grab.localPosition.x * _sensitivity, 1);
    }

    private void Update()
    {
        Move();
    }
}
