using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    public TitleCanvas titleCanvas;
    private GameScene gameScene;


    private void Awake()
    {
        gameScene = FindAnyObjectByType<GameScene>(FindObjectsInactive.Include);
        animator = GetComponent<Animator>();
        titleCanvas = FindAnyObjectByType<TitleCanvas>(FindObjectsInactive.Include);
    }

    public void SetAnimation(float horizontal, float vertical)
    {
        // 상하, 좌우를 가리키는 키가 동시에 입력되고 있다면 상하를 가리키는 키를 무시
        if (vertical != 0 && horizontal != 0)
        {
            vertical = 0;
        }

        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }

    public void SetPosition(int x, int y)
    {
        transform.position = new Vector3(x, y, 0);
    }
}
