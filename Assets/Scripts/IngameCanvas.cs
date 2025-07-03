using UnityEngine;

public class IngameCanvas : MonoBehaviour
{
    private TitleCanvas titleCanvas;

    private void Awake()
    {
        titleCanvas = FindAnyObjectByType<TitleCanvas>(FindObjectsInactive.Include);
    }

    // �ΰ��� UI���� �ڷΰ��� ��ư�� Ŭ������ �� ����� �Լ�
    public void OnBackClicked()
    {        
        titleCanvas?.SetActive(true);
        //gameObject.SetActive(false);        
    }
}
