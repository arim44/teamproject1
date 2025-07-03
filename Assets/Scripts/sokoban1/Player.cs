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
        // ����, �¿츦 ����Ű�� Ű�� ���ÿ� �Էµǰ� �ִٸ� ���ϸ� ����Ű�� Ű�� ����
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
