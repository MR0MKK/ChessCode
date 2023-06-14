using System.Collections.Generic;
using UnityEngine;


// Các hằng số và phương thức cần cho phép "Queen" di chuyển
public class Queen
{
    
    #region Các biến cần thiết
    // Vị trí quân cờ
    readonly Vector2 position;
    // Màu sắc quân cờ
    readonly Pieces.Colour colour;

    public Queen(Vector2 position, Pieces.Colour colour)
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

    
    // Chặn nước đi của quân khiến Vua bị chiếu hết
    public void ActivateForbiddenPositions()
    {
        if (colour == Pieces.Colour.White)
        {
            SetForbiddenPositionsWhite();
        }

        else
        {
            SetForbiddenPositionsBlack();
        }
    }

    
    // Giá trị của quân cờ vị trí hiện tại
    public int PositionValue => (colour == Pieces.Colour.White) ? GetPositionValueWhite() : GetPositionValueBlack();
    #endregion
    
    #region Danh sách các nước có thể thực hiện nhưng chưa block
    // Danh sách các nước quân Trắng có thể thực hiện
    // Danh sách các nước đi của quân cờ có thể thực hiện trước khi lọc ra các nước bị chặn
    List<Vector2> GetMovePositionsWhite()
    {
        List<Vector2> tempList = new List<Vector2>();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }
        }

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }
        }

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }
        }

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }
        }

        int loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)) || position.y + loops > 8)
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }
        }

        loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)) || position.y - loops < 1)
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)) || position.y + loops > 8)
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)) || position.y - loops < 1)
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }
        }

        return tempList;
    }

    
    // Danh sách các nước quân Đen có thể thực hiện
    // Danh sách các nước đi của quân cờ có thể thực hiện trước khi lọc ra các nước bị chặn
    List<Vector2> GetMovePositionsBlack()
    {
        List<Vector2> tempList = new List<Vector2>();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }
        }

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }
        }

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }
        }

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }
        }

        int loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)) || position.y + loops > 8)
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }
        }

        loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)) || position.y - loops < 1)
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)) || position.y + loops > 8)
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)) || position.y - loops < 1)
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }
        }

        return tempList;
    }
    #endregion
    
    #region Danh sách vị trí có thể bắt quân

    // Danh sách các vị trí mà quân cờ có thể bắt quân khác
    List<Vector2> GetPositionsInCheckWhite()
    {
        List<Vector2> tempList = new List<Vector2>();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                if (Chess.BlackKingPosition != new Vector2(i, position.y))
                {
                    break;
                }
            }
        }

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                if (Chess.BlackKingPosition != new Vector2(i, position.y))
                {
                    break;
                }
            }
        }

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                if (Chess.BlackKingPosition != new Vector2(position.x, i))
                {
                    break;
                }
            }
        }

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                if (Chess.BlackKingPosition != new Vector2(position.x, i))
                {
                    break;
                }
            }
        }

        int loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                if (Chess.BlackKingPosition != new Vector2(i, position.y + loops))
                {
                    break;
                }
            }
        }

        loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                if (Chess.BlackKingPosition != new Vector2(i, position.y - loops))
                {
                    break;
                }
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                if (Chess.BlackKingPosition != new Vector2(i, position.y + loops))
                {
                    break;
                }
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                if (Chess.BlackKingPosition != new Vector2(i, position.y - loops))
                {
                    break;
                }
            }
        }

        return tempList;
    }

    
    // Danh sách các vị trí mà quân cờ có thể bắt quân khác
    List<Vector2> GetPositionsInCheckBlack()
    {
        List<Vector2> tempList = new List<Vector2>();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                if (Chess.WhiteKingPosition != new Vector2(i, position.y))
                {
                    break;
                }
            }
        }

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y));

                if (Chess.WhiteKingPosition != new Vector2(i, position.y))
                {
                    break;
                }
            }
        }

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                if (Chess.WhiteKingPosition != new Vector2(position.x, i))
                {
                    break;
                }
            }
        }

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                tempList.Add(new Vector2(position.x, i));

                if (Chess.WhiteKingPosition != new Vector2(position.x, i))
                {
                    break;
                }
            }
        }

        int loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                if (Chess.WhiteKingPosition != new Vector2(i, position.y + loops))
                {
                    break;
                }
            }
        }

        loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                if (Chess.WhiteKingPosition != new Vector2(i, position.y - loops))
                {
                    break;
                }
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y + loops));

                if (Chess.WhiteKingPosition != new Vector2(i, position.y + loops))
                {
                    break;
                }
            }
        }

        loops = 0;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));

                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                tempList.Add(new Vector2(i, position.y - loops));

                if (Chess.WhiteKingPosition != new Vector2(i, position.y - loops))
                {
                    break;
                }
            }
        }

        return tempList;
    }
    #endregion
    
    #region Vị trí quân có thể chiếu hết địch
    // Danh sách vị trí mà quân trắng có thể chiếu hết
    List<Vector2> GetMenacingPositionsWhite()
    {
        List<Vector2> tempList = new List<Vector2>();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(i, position.y))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(i, position.y))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(position.x, i))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(position.x, i))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        int loops = 0;
        tempList.Clear();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(i, position.y + loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        loops = 0;
        tempList.Clear();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(i, position.y - loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        loops = 0;
        tempList.Clear();

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(i, position.y + loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        loops = 0;
        tempList.Clear();

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (!Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                if (Chess.BlackKingPosition == new Vector2(i, position.y - loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        return tempList;
    }

    
    // Danh sách vị trí mà quân đen có thể chiếu hết
    List<Vector2> GetMenacingPositionsBlack()
    {
        List<Vector2> tempList = new List<Vector2>();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(i, position.y))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                tempList.Add(new Vector2(i, position.y));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(i, position.y))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(position.x, i))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                tempList.Add(new Vector2(position.x, i));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(position.x, i))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        int loops = 0;

        tempList.Clear();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(i, position.y + loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        loops = 0;

        tempList.Clear();

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(i, position.y - loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        loops = 0;

        tempList.Clear();

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                tempList.Add(new Vector2(i, position.y + loops));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(i, position.y + loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        loops = 0;

        tempList.Clear();

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (!Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                tempList.Add(new Vector2(i, position.y - loops));
            }

            else
            {
                if (Chess.WhiteKingPosition == new Vector2(i, position.y - loops))
                {
                    tempList.Add(position);

                    return tempList;
                }
            }
        }

        tempList.Clear();

        return tempList;
    }
    #endregion
    
    #region Chặn nước gây chiếu 
    // Nếu các nước đi quân đên gây Vua bị chiếu => chặn
            
    void SetForbiddenPositionsWhite()
    {
        bool firstJump = false;
        GameObject selectedPiece = null;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(i, position.y));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Right);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Top);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Bottom);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(i, position.y));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Left);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Top);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Bottom);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(position.x, i));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Top);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(position.x, i) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Left);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Right);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(position.x, i));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Bottom);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(position.x, i) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Left);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Right);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;
        int loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(i, position.y + loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopRight);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y + loops) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopLeft);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomRight);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        loops = 0;
        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(i, position.y - loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomRight);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y - loops) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopRight);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomLeft);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        loops = 0;
        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(i, position.y + loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopLeft);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y + loops) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopRight);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomLeft);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        loops = 0;
        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceBlackInPosition(new Vector2(i, position.y - loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomLeft);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y - loops) == Chess.BlackKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopLeft);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomRight);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    
    // Nếu các nước đi quân đên gây Vua bị chiếu => chặn
            
    void SetForbiddenPositionsBlack()
    {
        bool firstJump = false;
        GameObject selectedPiece = null;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(i, position.y));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Right);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Top);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Bottom);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(i, position.y)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(i, position.y)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(i, position.y));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Left);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Top);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Bottom);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.y + 1; i <= 8; i++)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(position.x, i));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Top);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(position.x, i) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Left);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Right);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.y - 1; i >= 1; i--)
        {
            if (Chess.CheckSquareBlack(new Vector2(position.x, i)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(position.x, i)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(position.x, i));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Bottom);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(position.x, i) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Left);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.Right);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        firstJump = false;
        selectedPiece = null;
        int loops = 0;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(i, position.y + loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopRight);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y + loops) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopLeft);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomRight);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        loops = 0;
        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x + 1; i <= 8; i++)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(i, position.y - loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomRight);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y - loops) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopRight);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomLeft);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        loops = 0;
        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y + loops)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(i, position.y + loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(i, position.y + loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopLeft);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y + loops) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopRight);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomLeft);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

        loops = 0;
        firstJump = false;
        selectedPiece = null;

        for (int i = (int)position.x - 1; i >= 1; i--)
        {
            loops++;

            if (Chess.CheckSquareBlack(new Vector2(i, position.y - loops)))
            {
                break;
            }

            if (Chess.CheckSquareWhite(new Vector2(i, position.y - loops)))
            {
                if (!firstJump)
                {
                    selectedPiece = Chess.GetPieceWhiteInPosition(new Vector2(i, position.y - loops));

                    if ((Vector2)selectedPiece.transform.position == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomLeft);

                        return;
                    }

                    else
                    {
                        firstJump = true;
                    }
                }

                else
                {
                    if (new Vector2(i, position.y - loops) == Chess.WhiteKingPosition)
                    {
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.TopLeft);
                        selectedPiece.GetComponent<PiecesMovement>().EnableLock(Pieces.Directions.BottomRight);
                    }

                    else
                    {
                        break;
                    }
                }
            }
        }

    }
    #endregion
    
    #region Giá trị quân cờ vị trí hiện tại
    // Giá trị quân trắng vị trí hiện tại 
    int GetPositionValueWhite()
    {
        int[,] whiteValue =
        {
            { 100, 60, 60, 30, 30, 60, 60, 100 },
            { 60, 00, -30, 00, 00, 00, 00, 10 },
            { 60, -30, -30, -30, -30, -30, 00, 10 },
            { 00, 00, -30, -30, -30, -30, 00, 05 },
            { 30, 00, -30, -30, -30, -30, 00, 05 },
            { 60, 00, -30, -30, -30, -30, 00, 10 },
            { 60, 00, 00, 00, 00, 00, 00, 10 },
            { 100, 60, 60, 30, 30, 60, 60, 100 }
        };

        return -900 + whiteValue[(int)position.y - 1, (int)position.x - 1];
    }

    
    // Giá trị quân đen vị trí hiện tại 
    int GetPositionValueBlack()
    {
        int[,] blackValue =
        {
            { -100, -60, -60, -30, -30, -60, -60, -100 },
            { -60, 00, 00, 00, 00, 00, 00, -10 },
            { -60, 00, 30, 30, 30, 30, 00, 10 },
            { -30, 00, 30, 30, 30, 30, 00, -05 },
            { 00, 00, 30, 30, 30, 30, 00, -05 },
            { -60, 30, 30, 30, 30, 30, 00, -10 },
            { -60, 00, 30, 00, 00, 00, 00, -10 },
            { -100, -60, -60, -30, -30, -60, -60, -100 }
        };

        return 900 + blackValue[(int)position.y - 1, (int)position.x - 1];
    }
    #endregion

}