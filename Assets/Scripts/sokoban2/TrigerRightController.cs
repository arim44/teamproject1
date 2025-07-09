using UnityEngine;

public class TrigerRightController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Switch targetSwitch = other.GetComponent<Switch>();
        
        if(targetSwitch != null)
        {
            // 스위치 올리기
            targetSwitch.SetLeverRotation(-30);
        }
    }
}
