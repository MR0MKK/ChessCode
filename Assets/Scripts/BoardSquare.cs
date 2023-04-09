using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// Tạo ô vuông trên bảng
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class BoardSquare : MonoBehaviour, IPointerClickHandler
{
    // Kiểm tra ô có được chọn?
    bool isSelected = false;    
    // Kiểm tra ô có thể chọn?
    bool selectable = true;
    // Unable interact ô không thể chọn
    bool locked = true;    
    // Color ô    
    Color initialColour;    
    // Thêm hình cho quân cờ    
    SpriteRenderer sr = null;
    private void Awake()
    {
        // Lưu hình quân cờ ngay từ đầu
        sr = GetComponent<SpriteRenderer>();
        initialColour = sr.color;
    }

    private void OnEnable()
    {
        // Game manager tô màu cho chữ 
        Chess.UpdateColour += UpdateColour;
        Chess.RedSquare += ActivateRedColour;
        Chess.OriginalColour += ResetColour;
        Chess.EnableSelection += UnlockSquare;
        Chess.DisableSelection += LockSquare;
    }

    
    // Kích hoạt khi click chuột     
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Bị khóa hay chưa đến lượt => ko cho tương tác
        if (locked || !Chess.CheckTurn())
        {
            return;
        }

        // Nếu chọn được
        if (selectable && Chess.SelectPiece(transform.position))
        {
            selectable = false;
            isSelected = true;
        }
        // Chọn hộp được chọn => bỏ chọn
        else if (isSelected)
        {
            Chess.DeselectPosition();
        }

        // Di chuyển quân đến "Available Move"
        else if (sr.color == Color.green)
        {
            // Chơi với máy

            if (!NetworkManager.manager.IsConnected)
            {
                Chess.MovePiece(transform.position);
            }
            // Chơi online => gửi dữ liệu đến server
            else
            {
                NetworkManager.manager.MovePiece(Chess.ActivePiecePosition, transform.position);
            }
        }
    }    
    // Unable interact ô    
    void LockSquare()
    {
        locked = true;
    }
    // Mở khóa để có thể chọn    
    void UnlockSquare()
    {
        locked = false;
    }    
    // Ô trở về màu ban đầu    
    void ResetColour()
    {
        sr.color = initialColour;
        isSelected = false;
        selectable = true;
    }    
    // Cập nhập ô màu
    /// <param name="piecePosition">Vị trí ô được chọn</param>
    /// <param name="greenPositions">Các vị trí có thể chọn</param>
    void UpdateColour(Vector2 piecePosition, List<Vector2> greenPositions)
    {
        // Tô "vàng" ô quân cờ đang đứng

        if (piecePosition == (Vector2)transform.position)
        {
            sr.color = Color.yellow;

            return;
        }

        // Nếu ô được chọn = có thể di chuyển => tô màu xanh lục

        for (int i = 0; i < greenPositions.Count; i++)
        {
            if (greenPositions[i] == (Vector2)transform.position)
            {
                sr.color = Color.green;

                return;
            }
        }
    }    
    // Màu "đỏ" cho ô quân cờ đến
    // Làm mất màu đỏ vừa tô    
    /// <param name="position">Vị trí ô được di chuyển</param>
    public void ActivateRedColour(Vector2 position, List<Vector2> list)
    {
        if (transform.position.Equals(position))
        {
            StartCoroutine(RedColour());
        }
    }    
    // Tô đỏ 1s rồi biến mất    
    /// <returns></returns>
    IEnumerator RedColour()
    {
        sr.color = Color.red;

        yield return new WaitForSeconds(0.5f);

        sr.color = initialColour;
    }
}