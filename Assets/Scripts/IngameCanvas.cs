using UnityEngine;

public class IngameCanvas : MonoBehaviour
{
    private TitleCanvas titleCanvas;

    private void Awake()
    {
        titleCanvas = FindAnyObjectByType<TitleCanvas>(FindObjectsInactive.Include);
    }

    // 인게임 UI에서 뒤로가기 버튼을 클릭했을 때 실행될 함수
    public void OnBackClicked()
    {        
        titleCanvas?.SetActive(true);
        //gameObject.SetActive(false);        
    }
}
