using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Lưu giữ trạng thái của các nước cờ
public class PositionRecord
{
    // Vị trí tất cả các quân trên bàn
    public readonly List<Vector2> positions;

    // Hình của tất cả các quân trên bàn
    public readonly List<Pieces.Piece> pieces;

    // Màu sắc của tất cả các quân trên bàn
    public readonly List<Pieces.Colour> colours;

    
    // Trình lưu trạng thái
    /// <param name="whitePieces">Số quân cờ trắng trên bàn</param>
    /// <param name="blackPieces">Số quân cờ đen trên bàn</param>
    public PositionRecord(List<GameObject> whitePieces, List<GameObject> blackPieces)
    {
        // Tạo 3 save slots

        List<Vector2> tempPositions = new List<Vector2>();
        List<Pieces.Piece> tempPieces = new List<Pieces.Piece>();
        List<Pieces.Colour> tempColours = new List<Pieces.Colour>();

        // Lưu danh sách quân trên bàn của 2 đội

        for (int i = 0; i < whitePieces.Count; i++)
        {
            tempPositions.Add(whitePieces[i].transform.position);
            tempPieces.Add(whitePieces[i].GetComponent<PiecesMovement>().PieceType);
            tempColours.Add(Pieces.Colour.White);
        }

        for (int i = 0; i < blackPieces.Count; i++)
        {
            tempPositions.Add(blackPieces[i].transform.position);
            tempPieces.Add(blackPieces[i].GetComponent<PiecesMovement>().PieceType);
            tempColours.Add(Pieces.Colour.Black);
        }

        // Lưu các giá trị của nước đi gần nhất

        positions = tempPositions;
        pieces = tempPieces;
        colours = tempColours;
    }

    
    // Truy xuất save slots
    /// <param name="savedPositions">Vị trí các quân trên</param>
    /// <param name="savedPieces">Số liệu các quân trên bàn</param>
    /// <param name="savedColours">Màu sắc tất cả các quân</param>
    public PositionRecord(Vector2[] savedPositions, Pieces.Piece[] savedPieces, Pieces.Colour[] savedColours)
    {
        // Lưu giá trị vào danh sách

        positions = savedPositions.ToList();
        pieces = savedPieces.ToList();
        colours = savedColours.ToList();
    }

    
    // Lấy danh sách tất cá các vị trí theo chiều dọc
    /// <returns>Danh sách vị trí có quân( vì không thể sắp xếp thứ tự có vector</returns>
    public List<int> GetPositionsX()
    {
        List<int> tempList = new List<int>();

        for (int i = 0; i < positions.Count; i++)
        {
            tempList.Add((int)positions[i].x);
        }

        return tempList;
    }

    
    // Lấy danh sách tất cá các vị trí theo chiều ngang
    /// <returns>Danh sách vị trí có quân( vì không thể sắp xếp thứ tự có vector</returns>
    public List<int> GetPositionsY()
    {
        List<int> tempList = new List<int>();

        for (int i = 0; i < positions.Count; i++)
        {
            tempList.Add((int)positions[i].y);
        }

        return tempList;
    }

    
    // Kiểm tra 2 trạng thái lưu có giống nhau
    /// <param name="other">Trạng thái cần so sánh với các trạng thái đã lưu</param>
    /// <returns>True nếu xảy ra, False nếu ko xảy ra</returns>
    public bool Equals(PositionRecord other)
    {
        if (positions.Count != other.positions.Count)
        {
            return false;
        }

        for (int i = 0; i < positions.Count; i++)
        {
            if (positions[i] != other.positions[i] || pieces[i] != other.pieces[i] || colours[i] != other.colours[i])
            {

                return false;
            }
        }

        return true;
    }
}