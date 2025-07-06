using Newtonsoft.Json;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// 하나의 신에서 게임기기를 놓고, 여기에서 스테이지가 구성
/// 프리팹으로 구성해서 게임이 동작되도록 처리

/// <summary>
/// 스테이지를 구성하는 클래스
/// </summary>
public class EditorStages : MonoBehaviour
{
    // Reset 메뉴는 선택하면 public으로 노출된 요소도 다 초기화 됨
    // 그 이후에 Reset 함수가 호출됨
    // 벽이미지
    private SpriteRenderer Wall;
    // 아이템을 넣을 장소
    private SpriteRenderer Goal;
    // 아이템
    private SpriteRenderer Box;
    // 플레이어
    private SpriteRenderer Player;

    //스테이지
    private int stage = 2;


    private void Reset()
    {
        // 맵 블럭들 찾아오고 삭제한후 다시 만듬
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach(var render in renderers)
        {
            DestroyImmediate(render.gameObject);
        }

        // 프리펩 연결
        Wall = Resources.Load<SpriteRenderer>("Prefabs/Wall");
        Goal = Resources.Load<SpriteRenderer>("Prefabs/Goal");
        Box = Resources.Load<SpriteRenderer>("Prefabs/Box");
        Player = Resources.Load<SpriteRenderer>("Prefabs/Player");

        // 확장자 없이 파일 이름만 써도 됨 json파일 가져오기
        TextAsset asset = Resources.Load<TextAsset>("JsonFiles/Sokoban");
        var stages = JsonConvert.DeserializeObject<List<Sokoban_StageData>>(asset.text);

        // 지정한 스테이지의 행과 열의 값을 받음(배열의 크기값을 구하고)
        int height = stages[stage].Map.GetLength(0);
        int width = stages[stage].Map.GetLength(1);

        //배열을 할당
        var currentBoard = new int[height, width];
        //스테이지 데이터를 CurrtentBoard에복사함
        Array.Copy(stages[stage].Map, currentBoard, currentBoard.Length);

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                SpriteRenderer newObject = null;
                switch (currentBoard[r, c])
                {
                    //벽이라면
                    case 1: // 0,1,2,3,4,5,6,7
                        newObject= Instantiate(Wall, new Vector3(c, (height - r) - 1, 0), Quaternion.identity, transform);
                        break;
                    case 2:
                        newObject = Instantiate(Goal, new Vector3(c, (height - r) - 1, 0), Quaternion.identity, transform);
                        break;
                    case 3:
                        newObject = Instantiate(Box, new Vector3(c, (height - r) - 1, 0), Quaternion.identity, transform);
                        break;
                    case 4:
                        newObject = Instantiate(Player, new Vector3(c, (height - r) - 1, 0), Quaternion.identity, transform);
                        break;
                }
                if(newObject != null)
                {
                    // 배열의 행과 열로 접근가능
                    newObject.name = $"{r},{c}";
                }
            }
        }
    }
}
