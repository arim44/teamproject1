using UnityEngine;
using UnityEngine.InputSystem;

public class RightHandTriggerInput : MonoBehaviour
{
    // ���¿��� ������ InputActionAsset�� �����Ϳ��� �巡���ؼ� ����
    public InputActionAsset inpuActions;

    private InputAction triggerPressAction;

    private void OnEnable()
    {
        // RightHand �׼Ǹʿ��� Ʈ���������� �׼� ��������
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
            Debug.Log("������ Ʈ���� ��ư ����");
        }
    }
}
