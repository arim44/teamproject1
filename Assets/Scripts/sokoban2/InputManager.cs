using UnityEngine;


namespace Sokoban
{    public class InputManager : MonoBehaviour
    {
        // ���ӸŴ��� ��ũ��Ʈ
        private GameManager gameManager;

        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        // �Է� Ű ����
        public void InputKey(Direction direction)
        {
            //�Էµ� Ű���� �޾ƿ�
            //Ű �Է��� �޾Ƽ� ó���ϴ� ����
            switch (direction)
            {
                // x-1 ĳ������ ����                
                case Direction.Left:
                    break;
                case Direction.Right:
                    break;
                case Direction.Up:
                    break;
                case Direction.Down:
                    break;
            }
        }
    }
}
