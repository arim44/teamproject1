using UnityEngine;

namespace RetroSokoban
{
    public class IOManager : MonoBehaviour
    {
        // ���ڹݸŴ��� ��ũ��Ʈ
        private SokobanManager sokobanManager;

        private void Awake()
        {
            sokobanManager = FindFirstObjectByType<SokobanManager>();
        }

        // �Է� Ű ����
        public void InputMoveKey(Direction direction)
        {
            //�Էµ� Ű���� �޾ƿ�
            //Ű �Է��� �޾Ƽ� ó���ϴ� ����
            switch (direction)
            {
                // x-1 ĳ������ ����                
                case Direction.Left:
                    // ĳ������ ����ó��
                    sokobanManager.MoveHorizontal(-1, -2);
                    break;
                case Direction.Right:
                    // ĳ������ ������ó��
                    sokobanManager.MoveHorizontal(1, 2);
                    break;
                case Direction.Up:
                    sokobanManager.MoveVertical(-1, -2);
                    break;
                case Direction.Down:
                    sokobanManager.MoveVertical(1, 2);
                    break;
            }
        }
    }
}
