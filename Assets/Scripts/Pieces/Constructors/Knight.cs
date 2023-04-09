using System.Collections.Generic;
using UnityEngine;


// Các hằng số và phương thức cần cho phép "Knight" di chuyển
public class Knight
{
    #region Các biến cần thiết
    // Vị trí quân cờ
    readonly Vector2 position;
    // Màu sắc quân cờ
    readonly Pieces.Colour colour;

    public Knight(Vector2 position, Pieces.Colour colour)
    {
        this.position = position;
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
                // Nếu vua Trắng đang bị chiếu => loại bỏ các nước không bảo vệ vua

                if (Chess.WhiteKingInCheck)
                {
                    List<Vector2> tempList = GetMovePositionsWhite();

                    for (int i = 0; i < tempList.Count; i++)
                    {
                        if (!Chess.VerifyBlackMenacedPosition(tempList[i]))
                        {
                            tempList.Remove(tempList[i]);

                            i--;
                        }
                    }

                    return tempList;
                }

                // Nếu không sao thì trả danh sách di chuyển như ban đầu

                return GetMovePositionsWhite();
            }

            else
            {
                // Nếu vua Đen đang bị chiếu => loại bỏ các nước không bảo vệ vua

                if (Chess.BlackKingInCheck)
                {
                    List<Vector2> tempList = GetMovePositionsBlack();

                    for (int i = 0; i < tempList.Count; i++)
                    {
                        if (!Chess.VerifyWhiteMenacedPosition(tempList[i]))
                        {
                            tempList.Remove(tempList[i]);

                            i--;
                        }
                    }

                    return tempList;
                }

                // Nếu không sao thì trả danh sách di chuyển như ban đầu

                return GetMovePositionsBlack();
            }
        }
    }

    
    // Danh sách các vị trí mà quân cờ có thể bắt quân vua
    
    public List<Vector2> PositionsInCheck => (colour == Pieces.Colour.White) ? GetPositionsInCheckWhite() : GetPositionsInCheckBlack();

    
    // Danh sách các vị trí mà quân cờ có thể gây hại Vua địch
    
    public List<Vector2> MenacingPositions => (colour == Pieces.Colour.White) ? GetMenacingPositionsWhite() : GetMenacingPositionsBlack();

    
    // Giá trị của quân cờ vị trí hiện tại
    
    public int PositionValue => (colour == Pieces.Colour.White) ? GetPositionValueWhite() : GetPositionValueBlack();
    #endregion
    
    #region Danh sách các nước có thể thực hiện nhưng chưa block
    // Danh sách các nước quân Trắng có thể thực hiện
    // Danh sách các nước đi của quân cờ có thể thực hiện trước khi lọc ra các nước bị chặn
    List<Vector2> GetMovePositionsWhite()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x + 1, position.y + 2),
            new Vector2(position.x + 2, position.y + 1),
            new Vector2(position.x + 2, position.y - 1),
            new Vector2(position.x + 1, position.y - 2),
            new Vector2(position.x - 1, position.y - 2),
            new Vector2(position.x - 2, position.y - 1),
            new Vector2(position.x - 2, position.y + 1),
            new Vector2(position.x - 1, position.y + 2)
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
    // Danh sách các nước quân Đen có thể thực hiện
    // Danh sách các nước đi của quân cờ có thể thực hiện trước khi lọc ra các nước bị chặn
    List<Vector2> GetMovePositionsBlack()
    {
        List<Vector2> tempList = new List<Vector2>
        {
            new Vector2(position.x + 1, position.y + 2),
            new Vector2(position.x + 2, position.y + 1),
            new Vector2(position.x + 2, position.y - 1),
            new Vector2(position.x + 1, position.y - 2),
            new Vector2(position.x - 1, position.y - 2),
            new Vector2(position.x - 2, position.y - 1),
            new Vector2(position.x - 2, position.y + 1),
            new Vector2(position.x - 1, position.y + 2)
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
    
    #region Danh sách vị trí có thể bắt quân

    // Danh sách các vị trí mà quân cờ có thể bắt quân khác
    List<Vector2> GetPositionsInCheckWhite()
    {
        List<Vector2> tempList = GetMovePositionsWhite();

        return tempList;
    }
    

    // Danh sách các vị trí mà quân cờ có thể bắt quân khác
    List<Vector2> GetPositionsInCheckBlack()
    {
        List<Vector2> tempList = GetMovePositionsBlack();

        return tempList;
    }

    #endregion
    
    #region Vị trí quân có thể chiếu hết địch
    // Danh sách vị trí mà quân trắng có thể chiếu hết
    List<Vector2> GetMenacingPositionsWhite()
    {
        List<Vector2> tempList = GetMovePositionsWhite();

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
        List<Vector2> tempList = GetMovePositionsBlack();

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
            { 50, 40, 30, 30, 30, 30, 40, 50 },
            { 40, 20, 00, -05, -05, 00, 20, 40 },
            { 30, -05, -10, -15, -15, -10, -05, 30 },
            { 30, 00, -15, -20, -20, -15, 00, 30 },
            { 30, -05, -15, -20, -20, -15, -05, 30 },
            { 30, 00, -10, -15, -15, -10, 00, 30 },
            { 40, 20, 00, 00, 00, 00, 20, 40 },
            { 50, 40, 30, 30, 30, 30, 40, 50 }
        };

        return -320 + whiteValue[(int)position.y - 1, (int)position.x - 1];
    }

    
    // Giá trị quân đen vị trí hiện tại 
    int GetPositionValueBlack()
    {
        int[,] blackValue =
        {
            { -50, -40, -30, -30, -30, -30, -40, -50 },
            { -40, -20, 00, 00, 00, 00, -20, -40 },
            { -30, 00, 10, 15, 15, 10, 00, -30 },
            { -30, 05, 15, 20, 20, 15, 05, -30 },
            { -30, 00, 15, 20, 20, 15, 00, -30 },
            { -30, 05, 10, 15, 15, 10, 05, -30 },
            { -40, -20, 00, 05, 05, 00, -20, -40 },
            { -50, -40, -30, -30, -30, -30, -40, -50 }
        };

        return 320 + blackValue[(int)position.y - 1, (int)position.x - 1];
    }
    #endregion
}