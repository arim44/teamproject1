using UnityEngine;
using UnityEngine.UIElements;

public class Slot : MonoBehaviour
{
    // ȭ���� �迭���� �� 0,0 ���Կ� ��������Ʈ ����
    private int row;
    private int column;
    private SpriteRenderer spriteRenderer;

    public Slot() { }
    public Slot(int row, int column)
    {

    }

    public Slot(int row, int column, SpriteRenderer spriteRenderer)
    {
        this.row = row;
        this.column = column;
        this.spriteRenderer = spriteRenderer;
    }

    public void SetPosition(int row, int column)
    {
       
    }

    //public bool Check(int checkNum)
    //{
    //    if (currentBoard[row, column] != checkNum)
    //        return false;
    //    else return true;
    //}
}
