using UnityEngine;


namespace Sokoban
{    public class InputManager : MonoBehaviour
    {
        // 게임매니저 스크립트
        private GameManager gameManager;

        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        // 입력 키 조정
        public void InputKey(Direction direction)
        {
            //입력된 키값을 받아옴
            //키 입력을 받아서 처리하는 내용
            switch (direction)
            {
                // x-1 캐릭터의 왼쪽                
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
