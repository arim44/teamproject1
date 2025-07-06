using Newtonsoft.Json;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// �ϳ��� �ſ��� ���ӱ�⸦ ����, ���⿡�� ���������� ����
/// ���������� �����ؼ� ������ ���۵ǵ��� ó��

/// <summary>
/// ���������� �����ϴ� Ŭ����
/// </summary>
public class EditorStages : MonoBehaviour
{
    // Reset �޴��� �����ϸ� public���� ����� ��ҵ� �� �ʱ�ȭ ��
    // �� ���Ŀ� Reset �Լ��� ȣ���
    // ���̹���
    private SpriteRenderer Wall;
    // �������� ���� ���
    private SpriteRenderer Goal;
    // ������
    private SpriteRenderer Box;
    // �÷��̾�
    private SpriteRenderer Player;

    //��������
    private int stage = 2;


    private void Reset()
    {
        // �� ���� ã�ƿ��� �������� �ٽ� ����
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach(var render in renderers)
        {
            DestroyImmediate(render.gameObject);
        }

        // ������ ����
        Wall = Resources.Load<SpriteRenderer>("Prefabs/Wall");
        Goal = Resources.Load<SpriteRenderer>("Prefabs/Goal");
        Box = Resources.Load<SpriteRenderer>("Prefabs/Box");
        Player = Resources.Load<SpriteRenderer>("Prefabs/Player");

        // Ȯ���� ���� ���� �̸��� �ᵵ �� json���� ��������
        TextAsset asset = Resources.Load<TextAsset>("JsonFiles/Sokoban");
        var stages = JsonConvert.DeserializeObject<List<Sokoban_StageData>>(asset.text);

        // ������ ���������� ��� ���� ���� ����(�迭�� ũ�Ⱚ�� ���ϰ�)
        int height = stages[stage].Map.GetLength(0);
        int width = stages[stage].Map.GetLength(1);

        //�迭�� �Ҵ�
        var currentBoard = new int[height, width];
        //�������� �����͸� CurrtentBoard��������
        Array.Copy(stages[stage].Map, currentBoard, currentBoard.Length);

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                SpriteRenderer newObject = null;
                switch (currentBoard[r, c])
                {
                    //���̶��
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
                    // �迭�� ��� ���� ���ٰ���
                    newObject.name = $"{r},{c}";
                }
            }
        }
    }
}
