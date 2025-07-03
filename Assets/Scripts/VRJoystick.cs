using Unity.VisualScripting;
using UnityEngine;
// ���ͷ��ͺ� ���� Ŭ������ ����ϱ� ���� �߰���
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRJoystick : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Transform using XR Grab Interactable.")]
    [SerializeField] Transform _grab;
    [Tooltip("Rotation point (Rotates based on Grab local position).")]
    [SerializeField] Transform _pivot;

    // _grabRb������ ������ٵ� ã�� ������ ����
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
