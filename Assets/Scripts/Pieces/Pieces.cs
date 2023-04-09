using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Pieces : ScriptableObject
{

    public enum Colour {
        Black,
        White
    };
    public enum Piece {
        Bishop,
        King,
        Knight,
        Pawn,
        Queen,
        Rook
    };

    public enum Directions {
        Top,
        Right,
        Bottom,
        Left,
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft
    };

    
    
    [SerializeField] Colour colour;    
    [SerializeField] Piece piece;

    public Colour GetColour()
    {
        return colour;
    }

    public Piece GetPiece()
    {
        return piece;
    }

    
    // Lấy danh sách các nước đi hợp lệ mà quân cờ có thể thực hiện từ vị trí hiện tại của nó

    public List<Vector2> GetMovePositions(Vector2 position, bool firstMove)
    {
        if (piece == Piece.Pawn)
        {
            Pawn constructor = new Pawn(position, firstMove, colour);

            return constructor.MovePositions;
        }

        else if (piece == Piece.Rook)
        {
            Rook constructor = new Rook(position, colour);

            return constructor.MovePositions;
        }

        else if (piece == Piece.Bishop)
        {
            Bishop constructor = new Bishop(position, colour);

            return constructor.MovePositions;
        }

        else if (piece == Piece.Queen)
        {
            Queen constructor = new Queen(position, colour);

            return constructor.MovePositions;
        }

        else if (piece == Piece.Knight)
        {
            Knight constructor = new Knight(position, colour);

            return constructor.MovePositions;
        }

        else
        {
            King constructor = new King(position, firstMove, colour);

            return constructor.MovePositions;
        }
    }

    
    // Tính toán danh sách các vị trí mà quân cờ có thể bắt quân từ vị trí hiện tại của nó

    public List<Vector2> GetPositionsInCheck(Vector2 position, bool firstMove)
    {
        if (piece == Piece.Pawn)
        {
            Pawn constructor = new Pawn(position, firstMove, colour);

            return constructor.PositionsInCheck;
        }

        else if (piece == Piece.Rook)
        {
            Rook constructor = new Rook(position, colour);

            return constructor.PositionsInCheck;
        }

        else if (piece == Piece.Bishop)
        {
            Bishop constructor = new Bishop(position, colour);

            return constructor.PositionsInCheck;
        }

        else if (piece == Piece.Queen)
        {
            Queen constructor = new Queen(position, colour);

            return constructor.PositionsInCheck;
        }

        else if (piece == Piece.Knight)
        {
            Knight constructor = new Knight(position, colour);

            return constructor.PositionsInCheck;
        }

        else
        {
            King constructor = new King(position, firstMove, colour);

            return constructor.PositionsInCheck;
        }
    }

    
    // Danh sách các vị trí mà quân cờ là mối đe dọa khi chiếu, thường dùng khi vua ko thể đi đến chỗ chết

    public List<Vector2> GetMenacingPositions(Vector2 position, bool firstMove)
    {
        if (piece == Piece.Pawn)
        {
            Pawn constructor = new Pawn(position, firstMove, colour);

            return constructor.MenacingPositions;
        }

        else if (piece == Piece.Rook)
        {
            Rook constructor = new Rook(position, colour);

            return constructor.MenacingPositions;
        }

        else if (piece == Piece.Bishop)
        {
            Bishop constructor = new Bishop(position, colour);

            return constructor.MenacingPositions;
        }

        else if (piece == Piece.Queen)
        {
            Queen constructor = new Queen(position, colour);

            return constructor.MenacingPositions;
        }

        else if (piece == Piece.Knight)
        {
            Knight constructor = new Knight(position, colour);

            return constructor.MenacingPositions;
        }

        else
        {
            King constructor = new King(position, firstMove, colour);

            return constructor.MenacingPositions;
        }
    }

    
    // Chặn các di chuyển làm vua bị chiếu
    public void ActivateForbiddenPosition(Vector2 position)
    {
        if (piece == Piece.Pawn)
        {
            return;
        }

        else if (piece == Piece.Rook)
        {
            Rook constructor = new Rook(position, colour);

            constructor.ActivateForbiddenPositions();
        }

        else if (piece == Piece.Bishop)
        {
            Bishop constructor = new Bishop(position, colour);

            constructor.ActivateForbiddenPositions();
        }

        else if (piece == Piece.Queen)
        {
            Queen constructor = new Queen(position, colour);

            constructor.ActivateForbiddenPositions();
        }

        else if (piece == Piece.Knight)
        {
            return;
        }

        else
        {
            return;
        }
    }

    
    // Tìm giá trị tương đối của mảnh ở vị trí hiện tại => phục vụ AI chọn nước đi
    public int GetValue(Vector2 position, bool firstMove)
    {
        if (piece == Piece.Pawn)
        {
            Pawn constructor = new Pawn(position, firstMove, colour);

            return constructor.PositionValue;
        }

        else if (piece == Piece.Rook)
        {
            Rook constructor = new Rook(position, colour);

            return constructor.PositionValue;
        }

        else if (piece == Piece.Bishop)
        {
            Bishop constructor = new Bishop(position, colour);

            return constructor.PositionValue;
        }

        else if (piece == Piece.Queen)
        {
            Queen constructor = new Queen(position, colour);

            return constructor.PositionValue;
        }

        else if (piece == Piece.Knight)
        {
            Knight constructor = new Knight(position, colour);

            return constructor.PositionValue;
        }

        else
        {
            King constructor = new King(position, firstMove, colour);

            return constructor.PositionValue;
        }
    }
}