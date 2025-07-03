using UnityEngine;

public class TitleCanvas : MonoBehaviour
{
    // 터이틀 캔버스에 연결될 함수
    public void OnPlayClicked()
    {
        gameObject.SetActive(false);
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
