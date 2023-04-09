using System.Collections.Generic;
using UnityEngine;


// Các hằng số và phương thức cần cho phép "King" di chuyển
public class King
{

    #region Các biến cần thiết
    // Vị trí quân cờ
    readonly Vector2 position;
    // Quân cờ di chuyển lần đầu tiên chưa ? 
    readonly bool firstMove;
    // Màu sắc quân cờ
    readonly Pieces.Colour colour;
    public King(Vector2 position, bool firstMove, Pieces.Colour colour)
    {
        this.position = position;
        this.firstMove = firstMove;
        this.colour = colour;
    }
    #endregion

    
    #region Chức năng chung

    // Danh sách các nước đi của quân cờ có thể thực hiện trước khi lọc ra các nước bị chặn
    
    public List<Vector2> MovePositions
    {
        get
        {
            if (colour == Pieces.Colour.White)
            {
                List<Vector2> tempList = GetMovePositionsWhite();

                for (int i = 0; i < tempList.Count; i++)
                {
                    if (Chess.VerifyBlackCheckPosition(tempList[i]))
                    {
                        tempList.Remove(tempList[i]);

                        i--;
                    }
                }

                return tempList;
            }

            else
            {
                List<Vector2> tempList = GetMovePositionsBlack();

                for (int i = 0; i < tempList.Count; i++)
                {
                    if (Chess.VerifyWhiteCheckPosition(tempList[i]))
                    {
                        tempList.Remove(tempList[i]);

                        i--;
                    }
                }

                return tempList;
            }
        }
    }
    
    // Danh sách các vị trí mà quân cờ có thể bắt quân vua
    public List<Vector2> PositionsInCheck => (colour == Pieces.Colour.White) ? GetPositionsInCheckWhite() : GetPositionsInCheckBlack();

    
    // Danh sách các vị trí mà quân cờ có thể gây hại Vua địch
    public List<Vector2> MenacingPositions => (colour == Pieces.Colour.White) ? GetMenacingPositionsWhite() : GetMenacingPositionsBlack();

    // Giá trị của quân cờ vị trí hiện tại
    public int PositionValue
    {
        get
        {
            bool whiteQueen = false;
            bool blackQueen = false;
            int whitePieces = 0;
            int blackPieces = 0;

            for (int i = 0; i < Chess.PiecesWhite.Count; i++)
            {
                if (Chess.PiecesWhite[i].activeSelf)
                {
                    whitePieces++;

                    if (Chess.PiecesWhite[i].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Queen)
                    {
                        whiteQueen = true;
                    }
                }
            }

            for (int i = 0; i < Chess.PiecesBlack.Count; i++)
            {
                if (Chess.PiecesBlack[i].activeSelf)
                {
                    blackPieces++;

                    if (Chess.PiecesBlack[i].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Queen)
                    {
                        blackQueen = true;
                    }
                }
            }

            if (colour == Pieces.Colour.White)
            {
                if ((!whiteQueen & !blackQueen) || (whiteQueen && whitePieces <= 3))
                {
                    return GetPositionValueWhiteEnd();
                }

                return GetPositionValueWhite();
            }

            else
            {
                if ((!whiteQueen & !blackQueen) || (blackQueen && blackPieces <= 3))
                {
                    return GetPositionValueBlackEnd();
                }

                return GetPositionValueBlack();
            }
        }
    }
    #endregion
    
    #region Danh sách các nước có thể thực hiện nhưng chưa block
    // Danh sách các nước quân Trắng có thể thực hiện
    // Danh sách các nước đi của quân cờ có thể thực hiện trước khi lọc ra các nước bị chặn
    List<Vector2> GetMovePositionsWhite()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x, position.y + 1),
            new Vector2(position.x + 1, position.y + 1),
            new Vector2(position.x + 1, position.y),
            new Vector2(position.x + 1, position.y - 1),
            new Vector2(position.x, position.y - 1),
            new Vector2(position.x - 1, position.y - 1),
            new Vector2(position.x - 1, position.y),
            new Vector2(position.x - 1, position.y + 1)
        };

        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i].x < 1 || tempList[i].x > 8 || tempList[i].y < 1 || tempList[i].y > 8 || Chess.CheckSquareWhite(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        if (!firstMove)
        {
            if (Chess.CastlingLeft(colour))
            {
                tempList.Add(new Vector2(3, 1));
            }

            if (Chess.CastlingRight(colour))
            {
                tempList.Add(new Vector2(7, 1));
            }
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            if (Chess.VerifyBlackCheckPosition(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        return tempList;
    }

    
    // Danh sách các nước quân Đen có thể thực hiện
    // Danh sách các nước đi của quân cờ có thể thực hiện trước khi lọc ra các nước bị chặn
    List<Vector2> GetMovePositionsBlack()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x, position.y + 1),
            new Vector2(position.x + 1, position.y + 1),
            new Vector2(position.x + 1, position.y),
            new Vector2(position.x + 1, position.y - 1),
            new Vector2(position.x, position.y - 1),
            new Vector2(position.x - 1, position.y - 1),
            new Vector2(position.x - 1, position.y),
            new Vector2(position.x - 1, position.y + 1)
        };

        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i].x < 1 || tempList[i].x > 8 || tempList[i].y < 1 || tempList[i].y > 8 || Chess.CheckSquareBlack(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        if (!firstMove)
        {
            if (Chess.CastlingLeft(colour))
            {
                tempList.Add(new Vector2(3, 8));
            }

            if (Chess.CastlingRight(colour))
            {
                tempList.Add(new Vector2(7, 8));
            }
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            if (Chess.VerifyWhiteCheckPosition(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        return tempList;
    }
    #endregion
    
    #region Danh sách vị trí có thể bắt quân

    // Danh sách các vị trí mà quân cờ có thể bắt quân khác
    List<Vector2> GetPositionsInCheckWhite()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x, position.y + 1),
            new Vector2(position.x + 1, position.y + 1),
            new Vector2(position.x + 1, position.y),
            new Vector2(position.x + 1, position.y - 1),
            new Vector2(position.x, position.y - 1),
            new Vector2(position.x - 1, position.y - 1),
            new Vector2(position.x - 1, position.y),
            new Vector2(position.x - 1, position.y + 1)
        };

        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i].x < 1 || tempList[i].x > 8 || tempList[i].y < 1 || tempList[i].y > 8 || Chess.CheckSquareWhite(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        return tempList;
    }

    
    // Danh sách các vị trí mà quân cờ có thể bắt quân khác
    List<Vector2> GetPositionsInCheckBlack()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x, position.y + 1),
            new Vector2(position.x + 1, position.y + 1),
            new Vector2(position.x + 1, position.y),
            new Vector2(position.x + 1, position.y - 1),
            new Vector2(position.x, position.y - 1),
            new Vector2(position.x - 1, position.y - 1),
            new Vector2(position.x - 1, position.y),
            new Vector2(position.x - 1, position.y + 1)
        };

        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i].x < 1 || tempList[i].x > 8 || tempList[i].y < 1 || tempList[i].y > 8 || Chess.CheckSquareBlack(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        return tempList;
    }
    #endregion
    
    #region Vị trí quân có thể chiếu hết địch
    
    // Danh sách vị trí mà quân trắng có thể chiếu hết
    List<Vector2> GetMenacingPositionsWhite()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x, position.y + 1),
            new Vector2(position.x + 1, position.y + 1),
            new Vector2(position.x + 1, position.y),
            new Vector2(position.x + 1, position.y - 1),
            new Vector2(position.x, position.y - 1),
            new Vector2(position.x - 1, position.y - 1),
            new Vector2(position.x - 1, position.y),
            new Vector2(position.x - 1, position.y + 1)
        };

        for (int i = 0; i < tempList.Count; i++)
        {
            if (Chess.BlackKingPosition == tempList[i])
            {
                return new List<Vector2> { position };
            }
        }

        return new List<Vector2>();
    }

    
    // Danh sách vị trí mà quân đen có thể chiếu hết
    List<Vector2> GetMenacingPositionsBlack()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x, position.y + 1),
            new Vector2(position.x + 1, position.y + 1),
            new Vector2(position.x + 1, position.y),
            new Vector2(position.x + 1, position.y - 1),
            new Vector2(position.x, position.y - 1),
            new Vector2(position.x - 1, position.y - 1),
            new Vector2(position.x - 1, position.y),
            new Vector2(position.x - 1, position.y + 1)
        };

        for (int i = 0; i < tempList.Count; i++)
        {
            if (Chess.WhiteKingPosition == tempList[i])
            {
                return new List<Vector2> { position };
            }
        }

        return new List<Vector2>();
    }
    #endregion
      
    #region Giá trị quân cờ vị trí hiện tại

    // Giá trị quân trắng vị trí hiện tại 
    int GetPositionValueWhite()
    {
        int[,] whiteValue =
        {
            { -20, -30, -10, 00, 00, -10, -30, -20 },
            { -20, -20, 00, 00, 00, 00, -20, -20 },
            { 10, 20, 20, 20, 20, 20, 20, 10 },
            { 20, 30, 30, 40, 40, 30, 30, 20 },
            { 30, 40, 40, 50, 50, 40, 40, 30 },
            { 30, 40, 40, 50, 50, 40, 40, 30 },
            { 30, 40, 40, 0, 0, 0, 40, 30 },
            { 30, 40, 70, 50, 50, 40, 70, 30 }
        };

        return -20000 + whiteValue[(int)position.y - 1, (int)position.x - 1];
    }

    
     // Giá trị quân đen vị trí hiện tại  
    int GetPositionValueBlack()
    {
        int[,] blackValue =
        {
            { 30, 40, 70, 50, 50, 40, 70, 30 },
            { 30, 40, 40, 0, 0, 0, 40, 30 },
            { 30, 40, 40, 50, 50, 40, 40, 30 },
            { 30, 40, 40, 50, 50, 40, 40, 30 },
            { 20, 30, 30, 40, 40, 30, 30, 20 },
            { 10, 20, 20, 20, 20, 20, 20, 10 },
            { -20, -20, 00, 00, 00, 00, -20, -20 },
            { -20, -30, -10, 00, 00, -10, -30, -20 }
        };

        return 20000 + blackValue[(int)position.y - 1, (int)position.x - 1];
    }

    
    // Giá trị quân trắng gai đoạn sau trận đấu
    int GetPositionValueWhiteEnd()
    {
        int[,] whiteValueEnd =
        {
            { -50, -30, -30, -30, -30, -30, -30, -50 },
            { -30, -30, 00, 00, 00, 00, -30, -30 },
            { -30, -10, 20, 30, 30, 20, -10, -30 },
            { -30, -10, 30, 40, 40, 30, -10, -30 },
            { -30, -10, 30, 40, 40, 30, -10, -30 },
            { -30, -10, 20, 30, 30, 20, -10, -30 },
            { -30, -20, -10, 00, 00, -10, -20, -30 },
            { -50, -40, -30, -20, -20, -30, -40, -50 }
        };

        return -20000 + whiteValueEnd[(int)position.y - 1, (int)position.x - 1];
    }

   
    // Giá trị quân đen gai đoạn sau trận đấu
    int GetPositionValueBlackEnd()
    {
        int[,] blackValueEnd =
        {
            { 50, 40, 30, 20, 20, 30, 40, 50 },
            { 30, 20, 10, 00, 00, 10, 20, 30 },
            { 30, 10, -20, -30, -30, -20, 10, 30 },
            { 30, 10, -30, -40, -40, -30, 10, 30 },
            { 30, 10, -30, -40, -40, -30, 10, 30 },
            { 30, 10, -20, -30, -30, -20, 10, 30 },
            { 30, 30, 00, 00, 00, 00, 30, 30 },
            { 50, 30, 30, 30, 30, 30, 30, 50 }
        };

        return 20000 + blackValueEnd[(int)position.y - 1, (int)position.x - 1];
    }
    #endregion

}