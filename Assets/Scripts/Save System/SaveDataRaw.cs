using System.Collections.Generic;
using UnityEngine;

// Dữ liệu thô muốn lưu nhưng chưa được tuần tự hóa( serialized)
public class SaveDataRaw
{
    public Enums.Colours playerInTurn;

    // Vị trí của quân tốt bị thịt (lưu vị trí thật)
    public Vector2 enPassantDoublePosition;

    // Vị trí của quân tốt bị thịt (lưu vị trí ảo, mất sau khi hết lượt)
    public Vector2 enPassantPosition;

    public int movements;

    public List<PositionRecord> savedPositions;

    public List<GameObject> piecesWhite;

    public List<GameObject> piecesBlack;
}