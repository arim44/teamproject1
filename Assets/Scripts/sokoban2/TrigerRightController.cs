using UnityEngine;

public class TrigerRightController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Switch targetSwitch = other.GetComponent<Switch>();
        
        if(targetSwitch != null)
        {
            // ����ġ �ø���
            targetSwitch.SetLeverRotation(-30);
        }
    }
}
