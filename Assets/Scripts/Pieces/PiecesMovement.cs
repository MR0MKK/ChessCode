using System.Collections.Generic;
using UnityEngine;


// Class di chuyển các quân

public class PiecesMovement : MonoBehaviour
{
    
    // Màu của quân
    public Pieces.Colour PieceColour { get; private set; }
    // Loại của quân
    public Pieces.Piece PieceType { get; private set; }
    // Quân đã di chuyển lần đầu chưa?
    public bool FirstMove { get; set; } = false;
    
    [SerializeField] Pieces variables;

    
    // Chuyển động đi lên bị chặn
    bool lockTop = false;
    // Chuyển động sang bên phải bị chặn
    bool lockRight = false;
    // Chuyển động đi xuống bị chặn
    bool lockBottom = false;
    // Chuyển động sang trái bị chặn
    bool lockLeft = false;
    // Chuyển động chéo lên bên phải bị chặn
    bool lockTopRight = false;
    // Di chuyển chéo lên trên bên trái bị chặn
    bool lockTopLeft = false;
    // Di chuyển chéo xuống bên phải bị chặn
    bool lockBottomRight = false;
    // Di chuyển chéo xuống bên trái bị chặn
    bool lockBottomLeft = false;

    void OnEnable()
    {
        PieceColour = variables.GetColour();
        PieceType = variables.GetPiece();
    }

    
    // Giá trị của quân ở vị trí hiện tại của nó
    
    public int Value
    {
        get
        {
            if (!gameObject.activeSelf)
            {
                return -variables.GetValue(transform.position, FirstMove);
            }

            return variables.GetValue(transform.position, FirstMove);
        }
    }

    
    // Chặn chuyển động của quân cờ theo hướng chỉ định
    public void EnableLock(Pieces.Directions direction)
    {
        switch (direction)
        {
            case Pieces.Directions.Top:
                lockTop = true;
                break;
            case Pieces.Directions.Right:
                lockRight = true;
                break;
            case Pieces.Directions.Bottom:
                lockBottom = true;
                break;
            case Pieces.Directions.Left:
                lockLeft = true;
                break;
            case Pieces.Directions.TopRight:
                lockTopRight = true;
                break;
            case Pieces.Directions.TopLeft:
                lockTopLeft = true;
                break;
            case Pieces.Directions.BottomRight:
                lockBottomRight = true;
                break;
            case Pieces.Directions.BottomLeft:
                lockBottomLeft = true;
                break;
        }
    }

    
    // Mở khóa các di chuyển bị khóa
    
    public void DisableLock()
    {
        lockTop = false;
        lockRight = false;
        lockBottom = false;
        lockLeft = false;
        lockTopLeft = false;
        lockTopRight = false;
        lockBottomLeft = false;
        lockBottomRight = false;
    }

    
    /// Tính tất cả các nước đi hợp lệ mà quân cờ có thể thực hiện.
    public List<Vector2> SearchGreenPositions()
    {
        // Nếu quân cờ bị vô hiệu hóa , sẽ không thể thực hiện được nước đi nào => danh sach rong

        if (!gameObject.activeSelf)
        {
            return new List<Vector2>();
        }

        // Danh sách các nước có thể loại quân cờ

        List<Vector2> tempList = variables.GetMovePositions(transform.position, FirstMove);

        // Danh sách thu được,loại bỏ các vị trí bị chặn

        for (int i = 0; i < tempList.Count; i++)
        {
            if (lockTop && tempList[i].y > transform.position.y && tempList[i].x == transform.position.x)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }

            if (lockRight && tempList[i].x > transform.position.x && tempList[i].y == transform.position.y)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }

            if (lockBottom && tempList[i].y < transform.position.y && tempList[i].x == transform.position.x)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }

            if (lockLeft && tempList[i].x < transform.position.x && tempList[i].y == transform.position.y)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }

            if (lockTopRight && tempList[i].x > transform.position.x && tempList[i].y > transform.position.y)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }

            if (lockTopLeft && tempList[i].x < transform.position.x && tempList[i].y > transform.position.y)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }

            if (lockBottomRight && tempList[i].x > transform.position.x && tempList[i].y < transform.position.y)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }

            if (lockBottomLeft && tempList[i].x < transform.position.x && tempList[i].y < transform.position.y)
            {
                tempList.Remove(tempList[i]);
                i--;

                continue;
            }
        }

        return tempList;
    }

    
    // Tính toán tất cả các vị trí mà quân cờ có thể bắt
    public List<Vector2> GetPositionsInCheck()
    {
        // Nếu quân bị diệt => ko có di chuyển => return danh sách rỗng

        if (!gameObject.activeSelf)
        {
            return new List<Vector2>();
        }

        return variables.GetPositionsInCheck(transform.position, FirstMove);
    }

    
    // Tính toán các vị trí mà quân cờ có thể chiếu vua địch
    public List<Vector2> GetMenacingPositions()
    {
        // 

        if (!gameObject.activeSelf)
        {
            return new List<Vector2>();
        }

        return variables.GetMenacingPositions(transform.position, FirstMove);
    }

    
    // Loại bỏ các di chuyển khiến cho vua của đội bị chiếu
    
    public void ActivateForbiddenPositions()
    {
        // Nếu quân bị diệt => ko có di chuyển => return danh sách rỗng

        if (!gameObject.activeSelf)
        {
            return;
        }

        variables.ActivateForbiddenPosition(transform.position);
    }
}