using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    // ����Ʋ ĵ������ ����� �Լ�
    public void OnPlayClicked()
    {
        gameObject.SetActive(false);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
