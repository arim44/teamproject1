using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Knob 기능은 삭제예정 : 다이얼 비슷한 기능
namespace OHGAR
{
    /// <summary>
    /// XRGrabInteractable의 기능을 사용하면
    /// 물체가 컨트롤러에 쥐어진 순간부터 따라서 움직이게 됨
    /// 축이 고정된 상태에서 회전만 되도록 처리하는 기능을 갖는 스크립트
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

        [Header("References(Grab 과 Pivot을 연결)")]
        [Tooltip("XR Grab Interactable 이 연결된 Transform")]
        [SerializeField] Transform _grab;
        [Tooltip("회전의 기준이 되는  transform. Rotation point (Rotates based on Grab local position).")]
        [SerializeField] Transform _pivot;

        // 일반적인 데이터와 유니티에서 제공하는 클래스는 public 로 선언해두면
        // 실행시점에서 메모리를 할당받게 됨
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
        // 현재 조이스틱의 값을 얻고자 한다면 이 델리게이트에 함수를 연결해주면 됨
        public static event System.Action<Vector3> Output;

        // 추가
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
            // 초기 회전값을 저장
            _initPivitRot = _pivot.rotation;
            // 조이스틱을 설정
            SetJoystick();

            if (_activateOnStart) Activate();
        }

        public void SetJoystick()
        {
            //_isKnob = false;

            //_grabRb.constraints &= ~RigidbodyConstraints.FreezeAll;
            // 하나의 저장소에 비트 값을 누적해서 저장한 이후에 비트 연산을 통해
            // 기능을 활성화 하거나 비활성화 할 수 있다
            // Rigidbody 에서 물리적인 힘에 의한 회전이 발생되지 않도록 처리(회전 고정!!)
            _grabRb.constraints = RigidbodyConstraints.FreezeRotation;

            // XRGtrabInteractable에서 위치만 추적할 수 있도록 처리
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
        /// 움직임이 커지지 않도록 제한하는 코드
        /// </summary>
        private void Clamp()
        {
            // Vector3.ClampMagnitude() : 벡터의 길이를 제한
            // 현재 값이 (3,0,0) 가정, 두번째 매개변수로 1을 넣어주게되면
            // 리턴되는 결과값은 (1,0,0)
            // _grab.position - transform.position : 현재 조이스틱의 위치 - 루트의 위치 = 조이스틱이 향하는 방향
            // 구한 길이값이 _maxPivotRadius보다 크면 _maxPivotRadius로 맞춤

            // 힘이 너무 세지지 않도록 이동성 제한(거리값을 최소화하기 위해)
            _grab.position = transform.position + Vector3.ClampMagnitude(_grab.position - transform.position, _maxPivotRadius);
        }

        // Clamp와 반대되는 코드
        public void DeadZoneCheck()
        {
            // 데드존을 사용하지 않는다면 함수를 종료
            if (!_useDeadZone) return;
            // 조이스틱과의 거리를 구함
            float distance = Vector3.Distance(transform.position, _grab.position);
            // 일정한 값 이하일 때 입력이 종료된 상태로 변경
            _isDeadZone = distance <= _deadZoneRadius;
        }

        /// <summary>
        /// 왼손좌표계 사용
        /// </summary>
        private void Move()
        {
            // (z축이 양수일땐 앞을 가리킴) z값을 x축에 대한 회전값으로 쓰겠다
            // _grab의 z축의 움직임을 기준으로 x축에 대한 회전값을 만들어내고
            // _grab의 x축의 움직임을 기준으로 z축에 대한 회전값을 만들어내는 코드
            _pivot.localRotation = new(_grab.localPosition.z * _sensitivity,
                                        _grab.localPosition.y,
                                        // 부호를 반대로 하는 이유는 방향을 맞추기 위함(봤을때 역방향이어서)
                                        -_grab.localPosition.x * _sensitivity, 1);
            // 조이스틱의 회전값을 처리한 이후에 콘솔창에 로그를 남기고 외부에 값을 전달
            ProcOutput();
        }

        /// <summary>
        /// 스크립트 연결안되어 있으면 끔
        /// </summary>
        private void NullReferenceCheck()
        {
            // XRGrabInteractable이 연결되어 있지 않거나
            // 조이스틱 모델이 연결되어 있지 않다면 스크립트 기능을 꺼줌
            if (_grab == null || _pivot == null)
            {
                enabled = false;
            }
        }

        public void Activate()
        {
            // 스크립트가 실행되었을때 _grab의 위치와 회전값을 현재 스크립트가 연결된
            // 게임오브젝트의 하단에 배치하고 현재회전값을 사용하도록 처리
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
            // 조이스틱의 움직임이 없다면 Vector3.zero를 전달
            if (_isDeadZone)
            {
                if (_useDelegate) Output?.Invoke(Vector3.zero);
            }
            // 조이스틱의 움직임이 있다면 움직이고 있는 위치를 전달
            else
            {
                // 그랩에 대한 로컬포지션
                Vector3 localPosition = _grab.localPosition;

                if (arcadeJoystick)
                {
                    localPosition.y = 0;
                    // 길이 값만 비교하려고 Abs 절대값으로 변경해서 비교처리(양수만 비교)
                    if (Mathf.Abs(localPosition.x) > Mathf.Abs(localPosition.z))
                        localPosition.z = 0;
                    else if (Mathf.Abs(localPosition.x) < Mathf.Abs(localPosition.z))
                        localPosition.x = 0;
                                        
                    // 조이스틸을 테스트했을 때 방향성을 역으로 맞춰야 게임을 진행할 수 있음
                    //(안그러면 거꾸로 움직일 꺼 같음)
                    if (localPosition.magnitude != 0)
                        localPosition = -localPosition;

                    localPosition.Normalize();
                }
                if (_useDelegate) Output?.Invoke(localPosition);
            }
            // 유니티 에디터 모드일 경우에만 콘솔창에 로그를 남김
#if UNITY_EDITOR
            string output = _isDeadZone ? Vector3.zero.ToString() : _grab.localPosition.ToString();
            if (_printOutput) Debug.Log($"{name}: Output = {output} | Dead Zone = {_isDeadZone}");
#endif
        }
    }
}