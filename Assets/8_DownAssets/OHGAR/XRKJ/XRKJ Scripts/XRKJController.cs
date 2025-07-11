using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Knob ����� �������� : ���̾� ����� ���
namespace OHGAR
{
    /// <summary>
    /// XRGrabInteractable�� ����� ����ϸ�
    /// ��ü�� ��Ʈ�ѷ��� ����� �������� ���� �����̰� ��
    /// ���� ������ ���¿��� ȸ���� �ǵ��� ó���ϴ� ����� ���� ��ũ��Ʈ
    /// </summary>
    public class XRKJController : MonoBehaviour
    {
        [Header("Automation")]
        [Tooltip("If true, rigid body and grab interactable settings will be changed OnStart via SetJoystick & SetKnob methods (Determined by Is Knob in knob parameters).")]
        [SerializeField] bool _autoSettings = true;
        [Tooltip("Use this to test joystick movent without XR (Move the grab interactable game object in the editor to see movement).\n\nWARNING! OnDeactivate won't be called if moving obj in editor.")]
        [SerializeField] bool _activateOnStart = false;

        [Header("Output")]
        [Tooltip("Will Debug.Log the group, output, and Dead Zone status.")]
        [SerializeField] bool _printOutput = false;
        //[Tooltip("Use this to pair joysticks to specific receivers.")]
        //[SerializeField] int _group = 0;

        [Space]
        [Tooltip("Required if using receiver component, otherwise optional.")]
        [SerializeField] bool _useDelegate = true;
        //[Tooltip("Optional: Use this to avoid using the output Delegate, receiver componenet, and groups. A unique scriptable object will need to be created for each joystick.\n\nNOTE: Can be used in addition to Delegate.")]
        //[SerializeField] XRKJOutputSO _outputScriptableObject;
        [Header("Joystick Parameters")]
        [Tooltip("How fast the pivot rotates (Not used as knob).")]
        [SerializeField][Min(0f)] float _sensitivity = 4f;
        [Tooltip("The amount the pivot can rotate.")]
        [SerializeField][Min(0f)] float _maxPivotRadius = .04f;

        [Space]
        [Tooltip("If true, output will equal Vector3.zero if Grab distance from this position is less than Dead Zone Radius")]
        [SerializeField] bool _useDeadZone = true;
        [SerializeField][Min(0.0001f)] float _deadZoneRadius;

        //[Header("Knob Parameters")]
        //[Tooltip("Will Output Grab Interactable's local euler angles.")]
        //[SerializeField] bool _isKnob = false;

        [Header("References(Grab �� Pivot�� ����)")]
        [Tooltip("XR Grab Interactable �� ����� Transform")]
        [SerializeField] Transform _grab;
        [Tooltip("ȸ���� ������ �Ǵ�  transform. Rotation point (Rotates based on Grab local position).")]
        [SerializeField] Transform _pivot;

        // �Ϲ����� �����Ϳ� ����Ƽ���� �����ϴ� Ŭ������ public �� �����صθ�
        // ����������� �޸𸮸� �Ҵ�ް� ��
        [Space]
        public UnityEvent OnActivate;
        public UnityEvent OnDeactivate;

        bool _isActive;
        //bool _isNullRef;
        bool _isDeadZone;

        Quaternion _initPivitRot;

        Rigidbody _grabRb;
        XRGrabInteractable _grabInteractable;

        //public delegate void XRKinematicJoystickOutputDelegate(int group, Vector3 output);
        // ���� ���̽�ƽ�� ���� ����� �Ѵٸ� �� ��������Ʈ�� �Լ��� �������ָ� ��
        public static event System.Action<Vector3> Output;

        // �߰�
        public bool arcadeJoystick = true;

        private void Awake()
        {
            _grabRb = _grab.GetComponent<Rigidbody>();
            _grabInteractable = _grab.GetComponent<XRGrabInteractable>();
        }

        private void Start()
        {
            NullReferenceCheck();
            Initialize();
        }

        private void Update()
        {
            if (_isActive)
            {
                Clamp();
                DeadZoneCheck();
                Move();
            }
        }

        private void Initialize()
        {
            // �ʱ� ȸ������ ����
            _initPivitRot = _pivot.rotation;
            // ���̽�ƽ�� ����
            SetJoystick();

            if (_activateOnStart) Activate();
        }

        public void SetJoystick()
        {
            //_isKnob = false;

            //_grabRb.constraints &= ~RigidbodyConstraints.FreezeAll;
            // �ϳ��� ����ҿ� ��Ʈ ���� �����ؼ� ������ ���Ŀ� ��Ʈ ������ ����
            // ����� Ȱ��ȭ �ϰų� ��Ȱ��ȭ �� �� �ִ�
            // Rigidbody ���� �������� ���� ���� ȸ���� �߻����� �ʵ��� ó��(ȸ�� ����!!)
            _grabRb.constraints = RigidbodyConstraints.FreezeRotation;

            // XRGtrabInteractable���� ��ġ�� ������ �� �ֵ��� ó��
            if (_grabInteractable != null)
            {
                _grabInteractable.trackPosition = true;
                _grabInteractable.trackRotation = false;
            }
        }

        //public void SetKnob()
        //{
        //    _isKnob = true;

        // 126
        //    _grabRb.constraints = RigidbodyConstraints.FreezeAll;
        //    _grabRb.constraints &= ~RigidbodyConstraints.FreezeRotationY;

        //    if (_grabInteractable != null)
        //    {
        //        _grabInteractable.trackPosition = false;
        //        _grabInteractable.trackRotation = true;
        //    }
        //}

        /// <summary>
        /// �������� Ŀ���� �ʵ��� �����ϴ� �ڵ�
        /// </summary>
        private void Clamp()
        {
            // Vector3.ClampMagnitude() : ������ ���̸� ����
            // ���� ���� (3,0,0) ����, �ι�° �Ű������� 1�� �־��ְԵǸ�
            // ���ϵǴ� ������� (1,0,0)
            // _grab.position - transform.position : ���� ���̽�ƽ�� ��ġ - ��Ʈ�� ��ġ = ���̽�ƽ�� ���ϴ� ����
            // ���� ���̰��� _maxPivotRadius���� ũ�� _maxPivotRadius�� ����

            // ���� �ʹ� ������ �ʵ��� �̵��� ����(�Ÿ����� �ּ�ȭ�ϱ� ����)
            _grab.position = transform.position + Vector3.ClampMagnitude(_grab.position - transform.position, _maxPivotRadius);
        }

        // Clamp�� �ݴ�Ǵ� �ڵ�
        public void DeadZoneCheck()
        {
            // �������� ������� �ʴ´ٸ� �Լ��� ����
            if (!_useDeadZone) return;
            // ���̽�ƽ���� �Ÿ��� ����
            float distance = Vector3.Distance(transform.position, _grab.position);
            // ������ �� ������ �� �Է��� ����� ���·� ����
            _isDeadZone = distance <= _deadZoneRadius;
        }

        /// <summary>
        /// �޼���ǥ�� ���
        /// </summary>
        private void Move()
        {
            // (z���� ����϶� ���� ����Ŵ) z���� x�࿡ ���� ȸ�������� ���ڴ�
            // _grab�� z���� �������� �������� x�࿡ ���� ȸ������ ������
            // _grab�� x���� �������� �������� z�࿡ ���� ȸ������ ������ �ڵ�
            _pivot.localRotation = new(_grab.localPosition.z * _sensitivity,
                                        _grab.localPosition.y,
                                        // ��ȣ�� �ݴ�� �ϴ� ������ ������ ���߱� ����(������ �������̾)
                                        -_grab.localPosition.x * _sensitivity, 1);
            // ���̽�ƽ�� ȸ������ ó���� ���Ŀ� �ܼ�â�� �α׸� ����� �ܺο� ���� ����
            ProcOutput();
        }

        /// <summary>
        /// ��ũ��Ʈ ����ȵǾ� ������ ��
        /// </summary>
        private void NullReferenceCheck()
        {
            // XRGrabInteractable�� ����Ǿ� ���� �ʰų�
            // ���̽�ƽ ���� ����Ǿ� ���� �ʴٸ� ��ũ��Ʈ ����� ����
            if (_grab == null || _pivot == null)
            {
                enabled = false;
            }
        }

        public void Activate()
        {
            // ��ũ��Ʈ�� ����Ǿ����� _grab�� ��ġ�� ȸ������ ���� ��ũ��Ʈ�� �����
            // ���ӿ�����Ʈ�� �ϴܿ� ��ġ�ϰ� ����ȸ������ ����ϵ��� ó��
            _grab.parent = transform;
            _grab.rotation = transform.rotation;
            _isActive = true;
            OnActivate.Invoke();
        }

        public void Deactivate()
        {
            _grab.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ProcOutput();
            _pivot.rotation = _initPivitRot;
            _isActive = false;
            OnDeactivate.Invoke();
        }

        private void ProcOutput()
        {
            // ���̽�ƽ�� �������� ���ٸ� Vector3.zero�� ����
            if (_isDeadZone)
            {
                if (_useDelegate) Output?.Invoke(Vector3.zero);
            }
            // ���̽�ƽ�� �������� �ִٸ� �����̰� �ִ� ��ġ�� ����
            else
            {
                // �׷��� ���� ����������
                Vector3 localPosition = _grab.localPosition;

                if (arcadeJoystick)
                {
                    localPosition.y = 0;
                    // ���� ���� ���Ϸ��� Abs ���밪���� �����ؼ� ��ó��(����� ��)
                    if (Mathf.Abs(localPosition.x) > Mathf.Abs(localPosition.z))
                        localPosition.z = 0;
                    else if (Mathf.Abs(localPosition.x) < Mathf.Abs(localPosition.z))
                        localPosition.x = 0;
                                        
                    // ���̽�ƿ�� �׽�Ʈ���� �� ���⼺�� ������ ����� ������ ������ �� ����
                    //(�ȱ׷��� �Ųٷ� ������ �� ����)
                    if (localPosition.magnitude != 0)
                        localPosition = -localPosition;

                    localPosition.Normalize();
                }
                if (_useDelegate) Output?.Invoke(localPosition);
            }
            // ����Ƽ ������ ����� ��쿡�� �ܼ�â�� �α׸� ����
#if UNITY_EDITOR
            string output = _isDeadZone ? Vector3.zero.ToString() : _grab.localPosition.ToString();
            if (_printOutput) Debug.Log($"{name}: Output = {output} | Dead Zone = {_isDeadZone}");
#endif
        }
    }
}