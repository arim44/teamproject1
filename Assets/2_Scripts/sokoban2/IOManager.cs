using UnityEngine;

namespace RetroSokoban
{
    public class IOManager : MonoBehaviour
    {
        // 소코반매니저 스크립트
        private SokobanManager sokobanManager;

        private void Awake()
        {
            sokobanManager = FindFirstObjectByType<SokobanManager>();
        }

        // 입력 키 조정
        public void InputMoveKey(Direction direction)
        {
            //입력된 키값을 받아옴
            //키 입력을 받아서 처리하는 내용
            switch (direction)
            {
                // x-1 캐릭터의 왼쪽                
                case Direction.Left:
                    // 캐릭터의 왼쪽처리
                    sokobanManager.MoveHorizontal(-1, -2);
                    break;
                case Direction.Right:
                    // 캐릭터의 오른쪽처리
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
