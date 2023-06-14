using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// Dữ liệu trò chơi khác nhau sẽ được lưu trong một tệp nối tiếp
[Serializable]
public class SaveData
{
    
    // Chuỗi lưu ngày giờ
    public readonly string saveDate;
    // Người đến lượt chơi
    public readonly Enums.Colours playerInTurn;
    // Vị trí trục X của quân tốt bị thịt (lưu vị trí thật)
    public readonly int enPassantDoublePositionX;
    // Vị trí trục Y của quân tốt bị thịt (lưu vị trí thật)
    public readonly int enPassantDoublePositionY;
    // Vị trí trục X của quân tốt bị thịt (lưu vị trí ảo, mất sau khi hết lượt)
    public readonly int enPassantPositionX;
    // Vị trí trục Y của quân tốt bị thịt (lưu vị trí ảo, mất sau khi hết lượt)
    public readonly int enPassantPositionY;


    // Số lần di chuyển được thực hiện mà không bắt quân hoặc di chuyển quân tốt
    public readonly int movements;


    
    // Số trạng thái trước đó sẽ được lưu
    public readonly int positionsSaved;    
    // Số lượng các quân được lưu
    public int numberOfPieces;    
    // Các vị trí trên trục X của tất cả các quân đã lưu
    public int[] savedPositionsX;
    // Các vị trí trên trục Y của tất cả các quân đã lưu
    public int[] savedPositionsY;

    
    // Loại của tất cả các quân đã lưu
    public Pieces.Piece[] savedPieces;   
    // Màu sắc của tất cả quân đã được lưu
    public Pieces.Colour[] savedColours;


    // Tất cả các vị trí trục X của quân trắng trên bàn cờ.
    public int[] whitePositionsX;
    // Tất cả các vị trí trục Y của quân trắng trên bàn cờ.
    public int[] whitePositionsY;
    // Tất cả các vị trí trục X của quân đen trên bàn cờ.
    public int[] blackPositionsX;
    // Tất cả các vị trí trục Y của quân đen trên bàn cờ
    public int[] blackPositionsY;

    
    // Danh sách các quân trắng trên bàn cờ
    public Pieces.Piece[] whitePieces;
    // Danh sách các quân đen trên bàn cờ
    public Pieces.Piece[] blackPieces;    
    // Danh sách Boolean xét các quân trắng di chuyển lần đầu không?
    public bool[] whiteFirstMove;
    // Danh sách Boolean xét các quân đen di chuyển lần đầu không?
    public bool[] blackFirstMove;
    

    // Constructor của SaveDataRaw.cs => tuần tự hóa
    
    /// <param name="data">Dữ liệu thô ko thế sắp xếp theo thứ tự</param>
    public SaveData(SaveDataRaw data)
    {
        // Nhận ngày và thời gian hiện tại

        saveDate = SetDate();

        // Lưu các dữ liệu cần thiết

        playerInTurn = data.playerInTurn;
        enPassantDoublePositionX = (int)data.enPassantDoublePosition.x;
        enPassantDoublePositionY = (int)data.enPassantDoublePosition.y;
        enPassantPositionX = (int)data.enPassantPosition.x;
        enPassantPositionY = (int)data.enPassantPosition.y;
        movements = data.movements;

        // Lưu dữ liệu của các bước đi cuối

        positionsSaved = data.savedPositions.Count;
 

        // Lưu trạng thái của bảng

        SetPositions(data.piecesWhite, data.piecesBlack);
        SetPieces(data.piecesWhite, data.piecesBlack);
        SetFirstMove(data.piecesWhite, data.piecesBlack);
    }
    
    /// <returns>Chuỗi có dạng "DD-MM-AAAA  HH:MM:SS".</returns>
    string SetDate()
    {
        DateTime time = DateTime.Now;

        return time.ToString("dd-MM-yyyy  HH:mm:ss");
    }
    

    // <param name="piecesWhite">Danh sách tất cả các quân trắng trên bàn cờ.</param>
    // <param name="piecesBlack">Danh sách tất cả các quân đen trên bàn cờ.</param>
    void SetPositions(List<GameObject> piecesWhite, List<GameObject> piecesBlack)
    {
        // Tạo hai danh sách cho các vị trí đã lưu X, Y => lưu dưới dạng mạng

        List<int> tempListX = new List<int>();
        List<int> tempListY = new List<int>();

        for (int i = 0; i < piecesWhite.Count; i++)
        {
            tempListX.Add((int)piecesWhite[i].transform.position.x);
            tempListY.Add((int)piecesWhite[i].transform.position.y);
        }

        whitePositionsX = tempListX.ToArray();
        whitePositionsY = tempListY.ToArray();

        // Tạo hai danh sách cho các vị trí đã lưu X, Y => lưu dưới dạng mạng

        tempListX.Clear();
        tempListY.Clear();

        for (int i = 0; i < piecesBlack.Count; i++)
        {
            tempListX.Add((int)piecesBlack[i].transform.position.x);
            tempListY.Add((int)piecesBlack[i].transform.position.y);
        }

        blackPositionsX = tempListX.ToArray();
        blackPositionsY = tempListY.ToArray();
    }
    
    // <param name="piecesWhite">Danh sách tất cả các quân trắng trên bàn cờ.</param>
    // <param name="piecesBlack">Danh sách tất cả các quân đen trên bàn cờ.</param>
    void SetPieces(List<GameObject> piecesWhite, List<GameObject> piecesBlack)
    {
        // Tạo danh sách lưu quân trắng trên bàn

        List<Pieces.Piece> tempList = new List<Pieces.Piece>();

        for (int i = 0; i < piecesWhite.Count; i++)
        {
            tempList.Add(piecesWhite[i].GetComponent<PiecesMovement>().PieceType);
        }

        whitePieces = tempList.ToArray();

        // Tạo danh sách lưu quân đen trên bàn

        tempList.Clear();

        for (int i = 0; i < piecesBlack.Count; i++)
        {
            tempList.Add(piecesBlack[i].GetComponent<PiecesMovement>().PieceType);

            blackPieces = tempList.ToArray();
        }
    }    
    // <param name="piecesWhite">Danh sách tất cả các quân trắng trên bàn cờ.</param>
    // <param name="piecesBlack">Danh sách tất cả các quân đen trên bàn cờ.</param>
    void SetFirstMove(List<GameObject> piecesWhite, List<GameObject> piecesBlack)
    {
        // Tạo danh sách lưu các nước "đầu tiên" của quân trắng

        List<bool> tempList = new List<bool>();

        for (int i = 0; i < piecesWhite.Count; i++)
        {
            tempList.Add(piecesWhite[i].GetComponent<PiecesMovement>().FirstMove);
        }

        whiteFirstMove = tempList.ToArray();

        // Tạo danh sách lưu các nước "đầu tiên" của quân đen

        tempList.Clear();

        for (int i = 0; i < piecesBlack.Count; i++)
        {
            tempList.Add(piecesBlack[i].GetComponent<PiecesMovement>().FirstMove);

            blackFirstMove = tempList.ToArray();
        }
    }
}