using UnityEngine;
using UnityEngine.InputSystem;

public class RightHandTriggerInput : MonoBehaviour
{
    // 에셋에서 생성한 InputActionAsset을 데이터에서 드래그해서 넣음
    public InputActionAsset inpuActions;

    private InputAction triggerPressAction;

    private void OnEnable()
    {
        // RightHand 액션맵에서 트리거프레스 액션 가져오기
        var rightHandMap = inpuActions.FindActionMap("RightHand");
        triggerPressAction = rightHandMap.FindAction("TriggerPress");
        triggerPressAction.Enable();
    }

    private void OnDisable()
    {
        triggerPressAction.Disable();
    }

    private void Update()
    {
        if(triggerPressAction.WasPressedThisFrame())
        {
            Debug.Log("오른손 트리거 버튼 눌림");
        }
    }
}
