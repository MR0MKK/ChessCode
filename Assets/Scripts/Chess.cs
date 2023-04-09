using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// Các phương thức chính
public static class Chess
{
    #region Delegates

    // Kiểm soát màu sắc các ô
    /// <param name="piecePosition">Vị trí ô đã chọn</param>
    /// <param name="greenPositions">Vị trí ô có thể di chuyển</param>
    public delegate void NewColourDelegate(Vector2 piecePosition, List<Vector2> greenPositions);

    
    // Cập nhật màu các ô
    public static event NewColourDelegate UpdateColour;

    // Làm ô đổi sang đỏ
    public static event NewColourDelegate RedSquare;

    // Kiểm soát việc sử dụng ô
    public delegate void BoardsDelegate();

    // Trả ô về màu ban đầu
    public static event BoardsDelegate OriginalColour;

    // Cho phép lựa chọn ô
    public static event BoardsDelegate EnableSelection;

    // Khóa lựa chọn ô
    public static event BoardsDelegate DisableSelection;

    #endregion

    #region Players and game mode

    
    // Chọn màu trắng hay màu đen
    // Người chơi ko thể chọn quân khi chưa đến lượt
    // Chuẩn bị cho multiplayer  
    public static Enums.Colours PlayerColour { get; set; } = Enums.Colours.All;

    
    // Chơi vs AI hay Player
    static bool singlePlay;

    // Kiểm soát lượt người chơi
    static Enums.Colours playerInTurn;

    // Danh sách quân trắng
    public static List<GameObject> PiecesWhite { get; set; } = new List<GameObject>();

    // Danh sách quân đen
    public static List<GameObject> PiecesBlack { get; set; } = new List<GameObject>();

    // Vua trắng
    static GameObject whiteKing = null;

    // Vua đen
    static GameObject blackKing = null;

    // Chọn màu khi đấu với máy
    /// <param name="data">Load game="value", New game=null</param>
    public static void SelectColor(Enums.Colours colour, SaveData data)
    {
        // Dọn sạch bàn cờ
        CleanScene();
        // Màu người chơi 
        PlayerColour = colour;
        // Đánh với máy
        singlePlay = true;

        // Không dùng save game => new game
        if (data == null)
        {
            StartNewGame();
        }
        else
        {
            singlePlay = true;
            StartLoadedGame(data);
        }
    }

    
    /// NEW GAME
    public static void StartNewGame()
    {
        // Spawn các ô lên bảng

        InitialSpawn();

        // Xóa lịch sử của các ô ( nếu có)

        savedPositions.Clear();
        savedPositions.Add(new PositionRecord(PiecesWhite, PiecesBlack));

        //Các giá trị chính ban đầu

        stalemate = false;
        movements = 0;
        playerInTurn = Enums.Colours.White;
        WhiteKingInCheck = false;
        BlackKingInCheck = false;
        IsPlaying = true;

        // "Thông báo" trắng đi trước
        // Nút lưu trong menu để tạm dừng

        Interface.interfaceClass.SetWaitingMessage(Enums.Colours.White);
        Interface.interfaceClass.EnableOnlineSave();

        // Nếu AI + trắng

        if (singlePlay && PlayerColour == Enums.Colours.Black)
        {
            Interface.interfaceClass.EnableButtonPause(false);
            TimeEvents.timeEvents.StartWaitForAI();
        }

        // Khóa các ô để di chuyển

        EnableSelection();
        
    }

    
    // LOAD GAME  
    /// <param name="data">Tệp đã tải lên</param>
    public static void StartLoadedGame(SaveData data)
    {
        // Đưa dữ liệu đã lưu để khởi tạo

        playerInTurn = data.playerInTurn;
        enPassantPawnPosition = new Vector2(data.enPassantDoublePositionX, data.enPassantDoublePositionY);
        EnPassantPosition = new Vector2(data.enPassantPositionX, data.enPassantPositionY);
        EnPassantActive = enPassantPawnPosition != Vector2.zero;
        movements = data.movements;

        savedPositions.Clear();

        // Số lượng trạng thái được lưu
        // Màu sắc và quân cờ được lưu

        int numberOfPieces = data.numberOfPieces;
        List<Vector2> tempPositions = new List<Vector2>();
        List<Pieces.Piece> tempPieces = new List<Pieces.Piece>();
        List<Pieces.Colour> tempColours = new List<Pieces.Colour>();

        for (int i = 0; i < data.savedPositionsX.Length; i++)
        {
            tempPositions.Add(new Vector2(data.savedPositionsX[i], data.savedPositionsY[i]));
            tempPieces.Add(data.savedPieces[i]);
            tempColours.Add(data.savedColours[i]);

            numberOfPieces--;

            if (numberOfPieces == 0)
            {
                savedPositions.Add(new PositionRecord(tempPositions.ToArray(), tempPieces.ToArray(), tempColours.ToArray()));

                tempPositions.Clear();
                tempPieces.Clear();
                tempColours.Clear();

                numberOfPieces = data.numberOfPieces;
            }
        }

        // Lấy hình ảnh trong mục Prefabs cho quân cờs

        for (int i = 0; i < data.whitePositionsX.Length; i++)
        {
            string path = "";

            switch (data.whitePieces[i])
            {
                case Pieces.Piece.Bishop:
                    path = "Pieces/bishopW";
                    break;
                case Pieces.Piece.King:
                    path = "Pieces/kingW";
                    break;
                case Pieces.Piece.Knight:
                    path = "Pieces/knightW";
                    break;
                case Pieces.Piece.Pawn:
                    path = "Pieces/pawnW";
                    break;
                case Pieces.Piece.Queen:
                    path = "Pieces/queenW";
                    break;
                case Pieces.Piece.Rook:
                    path = "Pieces/rookW";
                    break;
            }

            GameObject piece = Object.Instantiate(Resources.Load<GameObject>(path), new Vector2(data.whitePositionsX[i], data.whitePositionsY[i]), Quaternion.identity);

            // Tính toán liệu các ô có bị di chuyển trước đó

            piece.GetComponent<PiecesMovement>().FirstMove = data.whiteFirstMove[i];

            // Thêm các quân cờ vào danh sách

            PiecesWhite.Add(piece);

            if (data.whitePieces[i] == Pieces.Piece.King)
            {
                whiteKing = PiecesWhite[i];
            }
        }

        // Làm lại tương tự với đội đen

        for (int i = 0; i < data.blackPositionsX.Length; i++)
        {
            string path = "";

            switch (data.blackPieces[i])
            {
                case Pieces.Piece.Bishop:
                    path = "Pieces/bishopB";
                    break;
                case Pieces.Piece.King:
                    path = "Pieces/kingB";
                    break;
                case Pieces.Piece.Knight:
                    path = "Pieces/knightB";
                    break;
                case Pieces.Piece.Pawn:
                    path = "Pieces/pawnB";
                    break;
                case Pieces.Piece.Queen:
                    path = "Pieces/queenB";
                    break;
                case Pieces.Piece.Rook:
                    path = "Pieces/rookB";
                    break;
            }

            GameObject piece = Object.Instantiate(Resources.Load<GameObject>(path), new Vector2(data.blackPositionsX[i], data.blackPositionsY[i]), Quaternion.identity);

            piece.GetComponent<PiecesMovement>().FirstMove = data.blackFirstMove[i];

            PiecesBlack.Add(piece);

            if (data.blackPieces[i] == Pieces.Piece.King)
            {
                blackKing = PiecesBlack[i];
            }
        }

        // Kiểm tra có vua nào đang bị chiếu

        stalemate = false;
        IsPlaying = true;

        CheckVerification();

        // Thông báo đang đến lượt chơi của ai

        Interface.interfaceClass.SetWaitingMessage(playerInTurn);

        // Nếu ko có lựa chọn nào ở trên => tiếp tục chơi

        ResetValues();

        // Nếu đang chơi với AI

        if (singlePlay)
        {
            Interface.interfaceClass.EnableButtonPause(false);
            TimeEvents.timeEvents.StartWaitForAI();
        }

        EnableSelection();
    }

    
    // Xóa các ô trên màn hình
    public static void CleanScene()
    {
        foreach (GameObject piece in PiecesWhite)
        {
            Object.Destroy(piece);
        }

        foreach (GameObject piece in PiecesBlack)
        {
            Object.Destroy(piece);
        }

        DeselectPosition();

        PiecesWhite.Clear();
        PiecesBlack.Clear();

        checkPositionsWhite.Clear();
        checkPositionsBlack.Clear();

        PlayerColour = Enums.Colours.All;

        IsPlaying = false;
        singlePlay = false;
    }

    
    // Đặt tất cả quân cờ lên bàn cờ
    static void InitialSpawn()
    {
        // Lấy tất cả các quân trắng từ Prefabs +xác định quân nào là vua

        whiteKing = Object.Instantiate(Resources.Load<GameObject>("Pieces/kingW"), new Vector2(5, 1), Quaternion.identity);
        PiecesWhite.Add(whiteKing);

        PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/rookW"), new Vector2(1, 1), Quaternion.identity));
        PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/rookW"), new Vector2(8, 1), Quaternion.identity));

        PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/knightW"), new Vector2(2, 1), Quaternion.identity));
        PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/knightW"), new Vector2(7, 1), Quaternion.identity));

        PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/bishopW"), new Vector2(3, 1), Quaternion.identity));
        PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/bishopW"), new Vector2(6, 1), Quaternion.identity));

        PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/queenW"), new Vector2(4, 1), Quaternion.identity));

        for (int i = 1; i <= 8; i++)
        {
            PiecesWhite.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/pawnW"), new Vector2(i, 2), Quaternion.identity));
        }

        // Tương tự với bên đen

        blackKing = Object.Instantiate(Resources.Load<GameObject>("Pieces/kingB"), new Vector2(5, 8), Quaternion.identity);
        PiecesBlack.Add(blackKing);

        PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/rookB"), new Vector2(1, 8), Quaternion.identity));
        PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/rookB"), new Vector2(8, 8), Quaternion.identity));

        PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/knightB"), new Vector2(2, 8), Quaternion.identity));
        PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/knightB"), new Vector2(7, 8), Quaternion.identity));

        PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/bishopB"), new Vector2(3, 8), Quaternion.identity));
        PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/bishopB"), new Vector2(6, 8), Quaternion.identity));

        PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/queenB"), new Vector2(4, 8), Quaternion.identity));

        for (int i = 1; i <= 8; i++)
        {
            PiecesBlack.Add(Object.Instantiate(Resources.Load<GameObject>("Pieces/pawnB"), new Vector2(i, 7), Quaternion.identity));
        }
    }

    #endregion

    #region Movement

    
    // Kiểm tra có game nào đang được chơi
    
    public static bool IsPlaying { get; set; }

    
    // Ô được chọn để di chuyển
    
    public static GameObject ActivePiece { get; set; } = null;

    
    // Vị trí ô được chọn => Vị trí
    
    public static Vector2 ActivePiecePosition => ActivePiece.transform.position;

    
    // Di chuyển quân cờ đến vị trí chỉ định
    
    // <param name="position">Vị trí chỉ định</param>
    public static void MovePiece(Vector2 position)
    {
        // Trả ô vuông về màu ban đầu của nó

        ResetColour();

        // Tăng số lần di chuyển( quá 50 nước chưa)
        movements++;

        // Tùy quân cờ mà có sự khác nhau về ô có thể đi

        MovementPeculiarities(position);

        // Di chuyển quân cờ đến vị trí mới 

        ActivePiece.transform.position = position;

        // Thêm âm thanh khi di chuyển quân

        Interface.interfaceClass.PlayMoveSound();

        // Tô "đỏ" ô đích

        ActivateRed(position);

        // Nếu có quân địch => thịt nó
        Pieces.Colour colour = ActivePiece.GetComponent<PiecesMovement>().PieceColour;

        if (colour == Pieces.Colour.White)
        {
            if (CheckSquareBlack(position))
            {
                // Loại bỏ quân bị tiêu diệt + xóa khỏi danh sách

                GameObject capturedPiece = GetPieceBlackInPosition(position);

                PiecesBlack.Remove(capturedPiece);

                Object.Destroy(capturedPiece);
                Debug.Log(capturedPiece);
                if(capturedPiece==blackKing)
                {
                    Interface.interfaceClass.ActivateCheckmateMessage(Enums.Colours.White);
                    ResetColour();
                    DisableSelection();
                    DeleteAutoSave();
                }

                // Một quân cờ bị bắt => bộ đếm di chuyển reset

                movements = 0;

                // Vì số lượng quân cờ ít hơn số lượng trạng thái, vì vậy loại bỏ chúng.

                savedPositions.Clear();
            }
        }

        else
        {
            if (CheckSquareWhite(position))
            {
                GameObject capturedPiece = GetPieceWhiteInPosition(position);

                PiecesWhite.Remove(capturedPiece);
                Debug.Log(capturedPiece);
                Object.Destroy(capturedPiece);

                if(capturedPiece==whiteKing)
                {
                    Interface.interfaceClass.ActivateCheckmateMessage(Enums.Colours.Black);
                    ResetColour();
                    DisableSelection();
                    DeleteAutoSave();

                }

                movements = 0;

                savedPositions.Clear();
            }
        }

        // Thăng cấp cho tốt

        if (activePromotion)
        {
            // Xác định màu quân tốt

            if (ActivePiece.GetComponent<PiecesMovement>().PieceColour == Pieces.Colour.White)
            {
                if (singlePlay && PlayerColour == Enums.Colours.Black)
                {
                    PieceSelectedToPromotion(Enums.PromotablePieces.Queen, Pieces.Colour.White);

                    return;
                }

                else
                {
                    DisableSelection();

                    Interface.interfaceClass.ActivatePromotionWhite(CheckTurn());
                }
            }

            else
            {
                if (singlePlay && PlayerColour == Enums.Colours.White)
                {
                    PieceSelectedToPromotion(Enums.PromotablePieces.Queen, Pieces.Colour.Black);

                    return;
                }

                else
                {
                    DisableSelection();

                    Interface.interfaceClass.ActivatePromotionBlack(CheckTurn());
                }
            }

            // Hiện thông báo phong hậu => ko cho người chơi chọn ô

            return;
        }

        // Lưu trạn thái các vị trí mới => kiếm tra lặp lại trong tương lai
        savedPositions.Add(new PositionRecord(PiecesWhite, PiecesBlack));

        // Đổi lượt chơi

        NextTurn();
    }

    
    // Kiểm tra các quân có nước đi đặc biệt
    /// <param name="position">Vị trí các mảnh sẽ được di chuyển</param>
    static void MovementPeculiarities(Vector2 position)
    {
        // Xác định quân cờ

        Pieces.Piece kindOfPiece = ActivePiece.GetComponent<PiecesMovement>().PieceType;

        switch (kindOfPiece)
        {
            // Kiểm tra "BẮT TỐT QUA ĐƯỜNG + THĂNG CẤP"
            // Đảm bảo tốt đã di chuyển + reset lại chuyển động

            case Pieces.Piece.Pawn:
                CheckEnPassant(position);
                ActivateEnPassant(position);
                VerifyPromotion(position);
                ActivePiece.GetComponent<PiecesMovement>().FirstMove = true;
                movements = 0;
                break;

            // Rook di chuyển => không thể nhập thành

            case Pieces.Piece.Rook:
                ActivePiece.GetComponent<PiecesMovement>().FirstMove = true;
                break;

            // Kiểm tra King di chuyển

            case Pieces.Piece.King:
                CheckCastling(position);
                ActivePiece.GetComponent<PiecesMovement>().FirstMove = true;
                break;
        }
    }

    
    // Không cho di chuyển khi Vua Trắng bị chiếu
    /// <param name="unblock">True = tắt khóa, False = khóa</param>
    static void BlockMovementsWhite(bool unblock)
    {
        // Block các nước Trắng + Unblock nước Đen

        if (unblock)
        {
            foreach (GameObject piece in PiecesWhite)
            {
                piece.GetComponent<PiecesMovement>().DisableLock();
            }
        }

        else
        {
            foreach (GameObject piece in PiecesBlack)
            {
                piece.GetComponent<PiecesMovement>().ActivateForbiddenPositions();
            }
        }
    }

    
    // Không cho di chuyển khi Vua Đen bị chiếu
    /// <param name="unblock">True = tắt khóa, False = khóa</param>
    static void BlockMovementsBlack(bool unblock)
    {
        // Block các nước Đen + Unblock nước Trắng

        if (unblock)
        {
            foreach (GameObject piece in PiecesBlack)
            {
                piece.GetComponent<PiecesMovement>().DisableLock();
            }
        }

        else
        {
            foreach (GameObject piece in PiecesWhite)
            {
                piece.GetComponent<PiecesMovement>().ActivateForbiddenPositions();
            }
        }
    }

    #endregion

    #region Castling

    
    // Nhập thành xa
    static GameObject castlingLeftRook = null;

    // Nhập thành gần
    static GameObject castlingRightRook = null;
    
    // Vị trí Vua khi Nhập thành xa
    static Vector2 castlingLeftDestination;

    // Vị trí Vua khi Nhập thành gần
    static Vector2 castlingRightDestination;

    
    // Vị trí mà quân nhập thành xa nên di chuyển đến.
    
    static Vector2 castlingLeftPosition;

    
    // Vị trí mà quân nhập thành gần nên di chuyển đến.
    
    static Vector2 castlingRightPosition;

    
    // Kiểm tra Vua bị chiếu khi nhập thành
    /// <param name="position">Vị trí vua sẽ di chuyển.</param>
    static void CheckCastling(Vector2 position)
    {
        // Nhập thành xa diễn ra => di chuyển xe đến vị trí

        if (castlingLeftDestination == position)
        {
            castlingLeftRook.transform.position = castlingLeftPosition;
        }

        // Nhập thành gần diễn ra => di chuyển xe đến vị trí

        else if (castlingRightDestination == position)
        {
            castlingRightRook.transform.position = castlingRightPosition;
        }
    }

    
    // Cho biết nhập thành xa đang thực hiện
    /// <param name="colour">Màu sắc ô đang di chuyển</param>
    /// <returns>Nếu chuyển động thực hiện là một nhập thành dài.</returns>
    public static bool CastlingLeft(Pieces.Colour colour)
    {
        if (colour == Pieces.Colour.White && CheckSquareWhite(new Vector2(1, 1)))
        {
            // Vua trắng đang di chuyển + kiểm tra Xe tại A1 

            GameObject capturedPiece = GetPieceWhiteInPosition(new Vector2(1, 1));

            if (capturedPiece.GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Rook &&
                !capturedPiece.GetComponent<PiecesMovement>().FirstMove)
            {
                // Không có xe => ko nhập thành

                if (!CheckSquareEmpty(new Vector2(2, 1)))
                {
                    return false;
                }

                if (VerifyBlackCheckPosition(new Vector2(3, 1)) || !CheckSquareEmpty(new Vector2(3, 1)))
                {
                    return false;
                }

                if (VerifyBlackCheckPosition(new Vector2(4, 1)) || !CheckSquareEmpty(new Vector2(4, 1)))
                {
                    return false;
                }

                if (VerifyBlackCheckPosition(new Vector2(5, 1)))
                {
                    return false;
                }

                // Nếu PASS qua các điều kiện => cho phép nhập thành

                else
                {
                    castlingLeftRook = capturedPiece;
                    castlingLeftPosition = new Vector2(4, 1);
                    castlingLeftDestination = new Vector2(3, 1);

                    return true;
                }
            }

            else
            {
                return false;
            }
        }

        if (colour == Pieces.Colour.Black && CheckSquareBlack(new Vector2(1, 8)))
        {
            // 

            GameObject capturedPiece = GetPieceBlackInPosition(new Vector2(1, 8));

            if (capturedPiece.GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Rook &&
                !capturedPiece.GetComponent<PiecesMovement>().FirstMove)
            {
                // Vua đên đang di chuyển + kiểm tra Xe tại A8

                if (CheckSquareBlack(new Vector2(2, 8)) || CheckSquareWhite(new Vector2(2, 8)))
                {
                    return false;
                }

                if (VerifyWhiteCheckPosition(new Vector2(3, 8)) || CheckSquareBlack(new Vector2(3, 8)) || CheckSquareWhite(new Vector2(3, 8)))
                {
                    return false;
                }

                if (VerifyWhiteCheckPosition(new Vector2(4, 8)) || CheckSquareBlack(new Vector2(4, 8)) || CheckSquareWhite(new Vector2(4, 8)))
                {
                    return false;
                }

                if (VerifyWhiteCheckPosition(new Vector2(5, 8)))
                {
                    return false;
                }

                // Nếu PASS qua các điều kiện => cho phép nhập thành

                else
                {
                    castlingLeftRook = capturedPiece;
                    castlingLeftPosition = new Vector2(4, 8);
                    castlingLeftDestination = new Vector2(3, 8);

                    return true;
                }
            }

            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }
    }

    
    //Cho biết nhập thành gần đang thực hiện
    
    /// <param name="colour">Màu sắc ô đang di chuyển</param>
    /// <returns>Nếu chuyển động thực hiện là một nhập thành gần.</returns>
    public static bool CastlingRight(Pieces.Colour colour)
    {
        if (colour == Pieces.Colour.White && CheckSquareWhite(new Vector2(8, 1)))
        {
            // Vua trắng đang di chuyển + kiểm tra Xe tại H1

            GameObject capturedPiece = GetPieceWhiteInPosition(new Vector2(8, 1));

            if (capturedPiece.GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Rook &&
                !capturedPiece.GetComponent<PiecesMovement>().FirstMove)
            {
                // Không có xe => ko nhập thành

                if (VerifyBlackCheckPosition(new Vector2(7, 1)) || CheckSquareWhite(new Vector2(7, 1)) || CheckSquareBlack(new Vector2(7, 1)))
                {
                    return false;
                }

                if (VerifyBlackCheckPosition(new Vector2(6, 1)) || CheckSquareWhite(new Vector2(6, 1)) || CheckSquareBlack(new Vector2(6, 1)))
                {
                    return false;
                }

                if (VerifyBlackCheckPosition(new Vector2(5, 1)))
                {
                    return false;
                }

                // Nếu PASS qua các điều kiện => cho phép nhập thành

                else
                {
                    castlingRightRook = capturedPiece;
                    castlingRightPosition = new Vector2(6, 1);
                    castlingRightDestination = new Vector2(7, 1);

                    return true;
                }
            }

            else
            {
                return false;
            }
        }

        if (colour == Pieces.Colour.Black && CheckSquareBlack(new Vector2(8, 8)))
        {
            // Vua trắng đang di chuyển + kiểm tra Xe tại H8

            GameObject capturedPiece = GetPieceBlackInPosition(new Vector2(8, 8));

            if (capturedPiece.GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Rook &&
                !capturedPiece.GetComponent<PiecesMovement>().FirstMove)
            {
               // Không có xe => ko nhập thành

                if (VerifyWhiteCheckPosition(new Vector2(7, 8)) || CheckSquareBlack(new Vector2(7, 8)) || CheckSquareWhite(new Vector2(7, 8)))
                {
                    return false;
                }

                if (VerifyWhiteCheckPosition(new Vector2(6, 8)) || CheckSquareBlack(new Vector2(6, 8)) || CheckSquareWhite(new Vector2(6, 8)))
                {
                    return false;
                }

                if (VerifyWhiteCheckPosition(new Vector2(5, 8)))
                {
                    return false;
                }

                // Nếu PASS qua các điều kiện => cho phép nhập thành

                else
                {
                    castlingRightRook = capturedPiece;
                    castlingRightPosition = new Vector2(6, 8);
                    castlingRightDestination = new Vector2(7, 8);

                    return true;
                }
            }

            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }
    }

    #endregion

    #region En Passant

    
    //Vị trí tốt bị "BẮT TỐT QUA ĐƯỜNG"
    
    static Vector2 enPassantPawnPosition;

    
    //Vị trí tốt di chuyển => có thể bắt tốt địch
    
    public static Vector2 EnPassantPosition { get; private set; }

    
    //Chỉ có quân tốt vừa tiến 2 ô
    
    static GameObject enPassantPiece;

    
    //Kiểm tra chức năng "BẮT TỐT QUA ĐƯỜNG" có hoạt động
    
    public static bool EnPassantActive { get; private set; }

    
    //Kích hoạt "BẮT TỐT QUA ĐƯỜNG"
    
    /// <param name="position">Vị trí của tốt sau khi "BẮT TỐT QUA ĐƯỜNG"</param>
    public static void ActivateEnPassant(Vector2 position)
    {
        // Xác định vị trí tốt

        if (ActivePiece.GetComponent<PiecesMovement>().FirstMove)
        {
            return;
        }

        if (position.y == 4)
        {
            EnPassantActive = true;
            enPassantPawnPosition = position;
            EnPassantPosition = new Vector2(enPassantPawnPosition.x, enPassantPawnPosition.y - 1);
            enPassantPiece = ActivePiece;
        }

        else if (position.y == 5)
        {
            EnPassantActive = true;
            enPassantPawnPosition = position;
            EnPassantPosition = new Vector2(enPassantPawnPosition.x, enPassantPawnPosition.y + 1);
            enPassantPiece = ActivePiece;
        }

        else
        {
            EnPassantActive = false;
            enPassantPawnPosition = Vector2.zero;
            EnPassantPosition = Vector2.zero;
            enPassantPiece = null;
        }
    }

    
    //Đặt lại các biến của "BẮT TỐT QUA ĐƯỜNG"
    
    static void DeactivateEnPassant()
    {
        EnPassantActive = false;

        enPassantPawnPosition = Vector2.zero;
        EnPassantPosition = Vector2.zero;
        enPassantPiece = null;
    }

    
    //Kiểm tra khi đi chuyển 1 con tốt, có con nào bị bắt ko
    
    /// <param name="position">Vị trí quân cờ đã di chuyển</param>
    static void CheckEnPassant(Vector2 position)
    {
        // Nếu tốt đã di chuyển => ko bị gì

        if (!EnPassantActive)
        {
            return;
        }

        // Đủ điều kiện "BẮT TỐT QUA ĐƯỜNG"

        if (position == EnPassantPosition)
        {
            if (enPassantPiece.GetComponent<PiecesMovement>().PieceColour == Pieces.Colour.White)
            {
                PiecesWhite.Remove(enPassantPiece);
            }

            else if (enPassantPiece.GetComponent<PiecesMovement>().PieceColour == Pieces.Colour.Black)
            {
                PiecesBlack.Remove(enPassantPiece);
            }

            Object.Destroy(enPassantPiece);
            enPassantPiece = null;

            DeactivateEnPassant();
        }
    }

    #endregion

    #region Pawn Promotion

    
    ///Cho biết có con tốt nào được thăng cấp trong lượt 
    
    static bool activePromotion = false;

    
    //Kiểm tra có con tốt nào được thăng cấp trong lượt 
    
    /// <param name="position">Vị trí quân cờ dã di chuyển</param>
    static void VerifyPromotion(Vector2 position)
    {
        if (ActivePiece.GetComponent<PiecesMovement>().PieceColour == Pieces.Colour.White && position.y == 8)
        {
            activePromotion = true;
            savedPositions.Clear();
        }

        else if (ActivePiece.GetComponent<PiecesMovement>().PieceColour == Pieces.Colour.Black && position.y == 1)
        {
            activePromotion = true;
            savedPositions.Clear();
        }
    }

    
    //Thay thế quân tốt được thăng hạng bằng quân cờ đã chọn
    
    /// <param name="piece">Quân cờ mới được chọn</param>
    /// <param name="colour">Màu sắc quân cờ được chọn</param>
    public static void PieceSelectedToPromotion(Enums.PromotablePieces piece, Pieces.Colour colour)
    {
        Vector2 position = ActivePiece.transform.position;
        GameObject newPiece = null;

        if (colour == Pieces.Colour.White)
        {
            PiecesWhite.Remove(ActivePiece);
            Object.Destroy(ActivePiece);

            switch (piece)
            {
                case Enums.PromotablePieces.Rook:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/rookW"), position, Quaternion.identity);
                    break;
                case Enums.PromotablePieces.Knight:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/knightW"), position, Quaternion.identity);
                    break;
                case Enums.PromotablePieces.Bishop:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/bishopW"), position, Quaternion.identity);
                    break;
                case Enums.PromotablePieces.Queen:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/queenW"), position, Quaternion.identity);
                    break;
            }

            PiecesWhite.Add(newPiece);
            ActivePiece = newPiece;
        }

        else
        {
            PiecesBlack.Remove(ActivePiece);
            Object.Destroy(ActivePiece);

            switch (piece)
            {
                case Enums.PromotablePieces.Rook:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/rookB"), position, Quaternion.identity);
                    break;
                case Enums.PromotablePieces.Knight:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/knightB"), position, Quaternion.identity);
                    break;
                case Enums.PromotablePieces.Bishop:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/bishopB"), position, Quaternion.identity);
                    break;
                case Enums.PromotablePieces.Queen:
                    newPiece = Object.Instantiate(Resources.Load<GameObject>("Pieces/queenB"), position, Quaternion.identity);
                    break;
            }

            PiecesBlack.Add(newPiece);
            ActivePiece = newPiece;
        }

        Interface.interfaceClass.DisablePromotions();

        activePromotion = false;

        savedPositions.Add(new PositionRecord(PiecesWhite, PiecesBlack));

        EnableSelection();

        NextTurn();
    }

    #endregion

    #region Check

    
    //Danh sách vị trí quân trắng có thể kiểm tra.
    
    static List<Vector2> checkPositionsWhite = new List<Vector2>();

    
    //Danh sách vị trí quân đen có thể kiểm tra.
    
    static List<Vector2> checkPositionsBlack = new List<Vector2>();

    
    //Kiểm tra vua trắng có bị chiếu
    
    public static bool WhiteKingInCheck { get; private set; }

    
    //Kiểm tra vua đen có bị chiếu
    
    public static bool BlackKingInCheck { get; private set; }

    
    //Vị trí vua trắng trên bàn cờ
    
    public static Vector2 WhiteKingPosition => whiteKing.transform.position;

    
    //Vị trí vua đen trên bàn cờ
    
    public static Vector2 BlackKingPosition => blackKing.transform.position;

    
    //Danh sách vị trí đe dọa quân trắng
    
    static List<Vector2> menacingWhitePositions = new List<Vector2>();

    
    //Danh sách vị trí đe dọa quân đen
    
    static List<Vector2> menacingBlackPositions = new List<Vector2>();

    
    //Cập nhật danh sách các quân trắng có thể tiêu diệt
    
    static void SetCheckPositionsWhite()
    {
        checkPositionsWhite.Clear();

        List<Vector2> tempList = new List<Vector2>();

        foreach (GameObject piece in PiecesWhite)
        {
            tempList = tempList.Concat(piece.GetComponent<PiecesMovement>().GetPositionsInCheck()).ToList();
        }

        checkPositionsWhite = tempList.Distinct().ToList();
    }

    
    //Cập nhật danh sách các quân đen có thể tiêu diệt
    
    static void SetCheckPositionsBlack()
    {
        checkPositionsBlack.Clear();

        List<Vector2> tempList = new List<Vector2>();

        foreach (GameObject piece in PiecesBlack)
        {
            tempList = tempList.Concat(piece.GetComponent<PiecesMovement>().GetPositionsInCheck()).ToList();
        }

        checkPositionsBlack = tempList.Distinct().ToList();
    }

    
    //Cập nhật danh sách  có thể tiêu diệt quân đen
    
    static void SetMenacingPositionsWhite()
    {
        menacingWhitePositions.Clear();

        List<Vector2> tempList = new List<Vector2>();

        foreach (GameObject piece in PiecesWhite)
        {
            tempList = tempList.Concat(piece.GetComponent<PiecesMovement>().GetMenacingPositions()).ToList();
        }

        menacingWhitePositions = tempList.Distinct().ToList();
    }

    
    //Cập nhật danh sách  có thể tiêu diệt quân trắng
    
    static void SetMenacingPositionsBlack()
    {
        menacingBlackPositions.Clear();

        List<Vector2> tempList = new List<Vector2>();

        foreach (GameObject piece in PiecesBlack)
        {
            tempList = tempList.Concat(piece.GetComponent<PiecesMovement>().GetMenacingPositions()).ToList();
        }

        menacingBlackPositions = tempList.Distinct().ToList();
    }

    
    //Kiểm tra xem khi đến lượt vua có bị chiếu không
    
    public static void CheckVerification()
    {
        // Cập nhật 2 danh sách các chuyển động

        SetCheckPositionsWhite();
        SetMenacingPositionsWhite();
        BlockMovementsBlack(false);
        BlockMovementsWhite(true);

        SetCheckPositionsBlack();
        SetMenacingPositionsBlack();
        BlockMovementsWhite(false);
        BlockMovementsBlack(true);


        // Kiểm tra có phải lượt người chơi

        if (playerInTurn == Enums.Colours.Black)
        {            
            for (int i = 0; i < checkPositionsWhite.Count; i++)
            {
                // Nếu vua trắng bị chiếu => cảnh báo

                if ((Vector2)blackKing.transform.position == checkPositionsWhite[i])
                {
                    BlackKingInCheck = true;
                    WhiteKingInCheck = false;

                    Interface.interfaceClass.ActivatePanelCheck(Enums.Colours.Black);

                    return;
                }

                // Không có => tăt cảnh báo

                else
                {
                    BlackKingInCheck = false;
                    WhiteKingInCheck = false;

                    Interface.interfaceClass.DeactivatePanelCheck();
                }
            }
        }

        else
        {
            for (int i = 0; i < checkPositionsBlack.Count; i++)
            {
                // Nếu vua đen bị chiếu => cảnh báo

                if ((Vector2)whiteKing.transform.position == checkPositionsBlack[i])
                {
                    WhiteKingInCheck = true;
                    BlackKingInCheck = false;

                    Interface.interfaceClass.ActivatePanelCheck(Enums.Colours.White);

                    return;
                }

                // Không có => tăt cảnh báo

                else
                {
                    WhiteKingInCheck = false;
                    BlackKingInCheck = false;

                    Interface.interfaceClass.DeactivatePanelCheck();
                }
            }
        }
    }

    
    //Kiểm tra Vua trắng có đang bị chiếu không
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns></returns>
    public static bool VerifyWhiteCheckPosition(Vector2 position)
    {
        for (int i = 0; i < checkPositionsWhite.Count; i++)
        {
            if (position == checkPositionsWhite[i])
            {
                return true;
            }
        }

        return false;
    }

    
    //Kiểm tra Vua đen có đang bị chiếu không
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns></returns>
    public static bool VerifyBlackCheckPosition(Vector2 position)
    {
        for (int i = 0; i < checkPositionsBlack.Count; i++)
        {
            if (position == checkPositionsBlack[i])
            {
                return true;
            }
        }

        return false;
    }

    
    //Kiểm tra xem một vị trí có phải là mối đe dọa đối với quân trắng hay không.
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns></returns>
    public static bool VerifyWhiteMenacedPosition(Vector2 position)
    {
        for (int i = 0; i < menacingWhitePositions.Count; i++)
        {
            if (position == menacingWhitePositions[i])
            {
                return true;
            }
        }

        return false;
    }

    
    //Kiểm tra xem một vị trí có phải là mối đe dọa đối với quân đen hay không.
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns></returns>
    public static bool VerifyBlackMenacedPosition(Vector2 position)
    {
        for (int i = 0; i < menacingBlackPositions.Count; i++)
        {
            if (position == menacingBlackPositions[i])
            {
                return true;
            }
        }

        return false;
    }

    
    //Lấy vị trí 1 quân trắng ở 1 vị trí
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns></returns>
    public static GameObject GetPieceWhiteInPosition(Vector2 position)
    {
        for (int i = 0; i < PiecesWhite.Count; i++)
        {
            if ((Vector2)PiecesWhite[i].transform.position == position)
            {
                return PiecesWhite[i];
            }
        }

        return null;
    }

    
    //Lấy vị trí 1 quân đen ở 1 vị trí
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns></returns>
    public static GameObject GetPieceBlackInPosition(Vector2 position)
    {
        for (int i = 0; i < PiecesBlack.Count; i++)
        {
            if ((Vector2)PiecesBlack[i].transform.position == position)
            {
                return PiecesBlack[i];
            }
        }

        return null;
    }

    #endregion

    #region Checkmate

    
    //Kiểm tra Vua Trắng đã chiếu hết Đen
    
    /// <returns>Nếu quân trắng đã thắng</returns>
    static bool CheckmateWhiteVerification()
    {
        // Tìm tất cả nước có thể đi của đen

        List<Vector2> tempList = new List<Vector2>();

        foreach (GameObject piece in PiecesBlack)
        {
            tempList = tempList.Concat(piece.GetComponent<PiecesMovement>().SearchGreenPositions()).ToList();
        }

        tempList.Distinct().ToList();

        // Làm sạch danh sách + loại bỏ các ô có quân đen

        for (int i = 0; i < tempList.Count; i++)
        {
            if (CheckSquareBlack(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        // Nếu không còn nước đi

        if (tempList.Count == 0)
        {
            // Nếu vua đen bị chiếu, trắng thắng

            if (BlackKingInCheck)
            {
                return true;
            }

            // Nếu không, trò chơi kết thúc với tỷ số hòa

            else
            {
                stalemate = true;

                return false;
            }
        }

        return false;
    }

    
    //Kiểm tra Vua Đen đã chiếu hết Trắng
    
    /// <returns>Nếu quân đen đã thắng</returns>
    static bool CheckmateBlackVerification()
    {
        // Tìm tất cả nước có thể đi của trắng

        List<Vector2> tempList = new List<Vector2>();

        foreach (GameObject piece in PiecesWhite)
        {
            tempList = tempList.Concat(piece.GetComponent<PiecesMovement>().SearchGreenPositions()).ToList();
        }

        tempList.Distinct().ToList();

        // Làm sạch danh sách + loại bỏ các ô có quân trắng

        for (int i = 0; i < tempList.Count; i++)
        {
            if (CheckSquareWhite(tempList[i]))
            {
                tempList.Remove(tempList[i]);

                i--;
            }
        }

        // Nếu không còn nước đi

        if (tempList.Count == 0)
        {
            // Nếu vua thắng bị chiếu, trắng đen

            if (WhiteKingInCheck)
            {
                return true;
            }

            // Nếu không, trò chơi kết thúc với tỷ số hòa

            else
            {
                stalemate = true;

                return false;
            }
        }

        return false;
    }

    #endregion

    #region Draw

    
    //Các điều kiện để kết quả hòa
    
    static bool stalemate = false;

    
    //Số lượt di chuyển
    
    static int movements = 0;

    
    //Kiểm tra có di chuyển quân cờ lặp lại
    
    static readonly List<PositionRecord> savedPositions = new List<PositionRecord>();

    
    //Thông báo kết quả hòa ra màn hình 
    
    //<param name="drawType">Lý do kết quả hòa</param>
    static void FinishWithDraw(Enums.DrawModes drawType)
    {
        Interface.interfaceClass.ActivateDrawMessage(drawType);

        // Hủy khả năng chọn ô

        DisableSelection();

        // Xóa khả năng lưu vị trí

        DeleteAutoSave();
    }

    
    //Liệu có thể hòa ko?
    
    /// <returns>True nếu ko đủ quân cờ để kết thúc</returns>
    static bool DrawByImpossibility()
    {
        // Các phép tính có thể xảy ra

        if (PiecesWhite.Count == 1 && PiecesWhite.Count == 1)
        {
            return true;
        }

        if (PiecesWhite.Count == 2 && PiecesBlack.Count == 1 && PiecesWhite[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Knight)
        {
            return true;
        }

        if (PiecesBlack.Count == 2 && PiecesWhite.Count == 1 && PiecesBlack[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Knight)
        {
            return true;
        }

        if (PiecesWhite.Count == 2 && PiecesBlack.Count == 1 && PiecesWhite[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Bishop)
        {
            return true;
        }

        if (PiecesBlack.Count == 2 && PiecesWhite.Count == 1 && PiecesBlack[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Bishop)
        {
            return true;
        }

        if (PiecesWhite.Count == 3 && PiecesBlack.Count == 1 && PiecesWhite[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Knight
            && PiecesWhite[2].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Knight)
        {
            return true;
        }

        if (PiecesBlack.Count == 3 && PiecesWhite.Count == 1 && PiecesBlack[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Knight
            && PiecesBlack[2].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Knight)
        {
            return true;
        }

        if (PiecesWhite.Count == 2 && PiecesBlack.Count == 2 && PiecesWhite[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Bishop
            && PiecesBlack[1].GetComponent<PiecesMovement>().PieceType == Pieces.Piece.Bishop
            && (PiecesWhite[1].transform.position.x + PiecesWhite[1].transform.position.y) % 2 == (PiecesBlack[1].transform.position.x + PiecesBlack[1].transform.position.y) % 2)
        {
            return true;
        }

        return false;
    }

    
    //Kiểm tra có chiếu 3 lần lặp lại
    
    /// <returns></returns>
    static bool VerifyRepeatedPositions()
    {
        for (int i = 0; i < savedPositions.Count - 1; i++)
        {
            int repetitions = 0;

            for (int j = i + 1; j < savedPositions.Count; j++)
            {
                if (savedPositions[i].Equals(savedPositions[j]))
                {
                    repetitions++;

                    if (repetitions == 2)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    #endregion

    #region Turns

    
    //Kiểm tra có chiếu hết hay hòa ko => đổi lượt
    
    static void NextTurn()
    {
        // Hiện thị thông báo đã đến lượt

        if (playerInTurn == Enums.Colours.White)
        {
            playerInTurn = Enums.Colours.Black;

            Interface.interfaceClass.SetWaitingMessage(Enums.Colours.Black);
        }

        else if (playerInTurn == Enums.Colours.Black)
        {
            playerInTurn = Enums.Colours.White;

            Interface.interfaceClass.SetWaitingMessage(Enums.Colours.White);
        }

        // Kiểm tra đã kết thúc bởi chiếu tướng chưa

        CheckVerification();

        if (playerInTurn == Enums.Colours.White)
        {
            if (CheckmateWhiteVerification())
            {
                Interface.interfaceClass.ActivateCheckmateMessage(Enums.Colours.White);
                ResetColour();
                DisableSelection();
                DeleteAutoSave();

                return;
            }

            if (CheckmateBlackVerification())
            {
                Interface.interfaceClass.ActivateCheckmateMessage(Enums.Colours.Black);
                ResetColour();
                DisableSelection();
                DeleteAutoSave();

                return;
            }
        }

        else if (playerInTurn == Enums.Colours.Black)
        {
            if (CheckmateBlackVerification())
            {
                Interface.interfaceClass.ActivateCheckmateMessage(Enums.Colours.Black);
                ResetColour();
                DisableSelection();
                DeleteAutoSave();

                return;
            }

            if (CheckmateWhiteVerification())
            {
                Interface.interfaceClass.ActivateCheckmateMessage(Enums.Colours.White);
                ResetColour();
                DisableSelection();
                DeleteAutoSave();

                return;
            }
        }

        // Tổng số nước đi mỗi đội là 75 => hòa

        if (movements == 150)
        {
            FinishWithDraw(Enums.DrawModes.Move75);

            return;
        }

        // Lặp lại 3 lần 1 nước đi

        if (savedPositions.Count > 5 && VerifyRepeatedPositions())
        {
            FinishWithDraw(Enums.DrawModes.ThreefoldRepetition);

            return;
        }

        // Kiểm tra không còn nước đi

        if (stalemate)
        {
            FinishWithDraw(Enums.DrawModes.Stalemate);

            return;
        }

        // Có  thể kết thúc trò chơi với các quân cờ trên bàn

        if (PiecesWhite.Count <= 3 && PiecesBlack.Count <= 3 && DrawByImpossibility())
        {
            FinishWithDraw(Enums.DrawModes.Impossibility);

            return;
        }

        // Không thỏa mãn điều kiện nào => tiếp tục trò chơi

        ResetValues();

        // Khi đấu với máy

        if (singlePlay)
        {
            Interface.interfaceClass.EnableButtonPause(false);
            TimeEvents.timeEvents.StartWaitForAI();
        }
    }

    
    //Khi đến lượt của người chơi
    public static bool CheckTurn()
    {
        if (PlayerColour == Enums.Colours.All || PlayerColour == playerInTurn)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    
    // Chọn ô cho nước đi tiếp theo
    
    /// <param name="position">Vị trí của ô</param>
    /// <returns>True nếu có 1 quân cờ + đang trong lượt</returns>
    public static bool SelectPiece(Vector2 position)
    {
        if (playerInTurn == Enums.Colours.White)
        {
            GameObject piece = GetPieceWhiteInPosition(position);

            if (piece != null)
            {
                ActivePiece = piece;

                // Ô hợp pháp => TÔ MÀU "XANH"

                ChangeColour(ActivePiece.transform.position, ActivePiece.GetComponent<PiecesMovement>().SearchGreenPositions());

                return true;
            }
        }

        else if (playerInTurn == Enums.Colours.Black)
        {
            GameObject piece = GetPieceBlackInPosition(position);

            if (piece != null)
            {
                ActivePiece = piece;

                // Ô hợp pháp => TÔ MÀU "XANH"

                ChangeColour(ActivePiece.transform.position, ActivePiece.GetComponent<PiecesMovement>().SearchGreenPositions());

                return true;
            }
        }

        return false;
    }

    
    // Bỏ chọn quân đã chọn => trả về màu cũ
    
    public static void DeselectPosition()
    {
        ActivePiece = null;

        ResetColour();
    }

    
    // Reset lại các biến cần thiết
    
    public static void ResetValues()
    {
        if (enPassantPawnPosition != Vector2.zero && ActivePiece != null)
        {
            if (enPassantPawnPosition != (Vector2)ActivePiece.transform.position)
            {
                DeactivateEnPassant();
            }
        }

        ActivePiece = null;

        castlingLeftRook = null;
        castlingRightRook = null;
        castlingLeftDestination = Vector2.zero;
        castlingRightDestination = Vector2.zero;
        castlingLeftPosition = Vector2.zero;
        castlingRightPosition = Vector2.zero;

        AutoSave();
    }

    #endregion

    #region Square Colours

    
    // Tô màu cho các ô trên bàn
    
    /// <param name="piecePosition">Vị trí ô đang được chọn => VÀNG</param>
    /// <param name="greenPositions">Vị trí quân cờ có thể đi => XANH</param>
    public static void ChangeColour(Vector2 piecePosition, List<Vector2> greenPositions)
    {
        ResetColour();

        if (CheckTurn())
        {
            UpdateColour(piecePosition, greenPositions);
        }
    }

    
    // Trả ô về màu ban đầu
    
    public static void ResetColour()
    {
        OriginalColour();
    }

    
    // Làm cho ô đich trở thành màu đỏ
    
    /// <param name="position">Vị trí của ô</param>
    static void ActivateRed(Vector2 position)
    {
        RedSquare(position, null);
    }

    #endregion

    #region Examine Squares

    
    // Kiểm tra ô chỉ định có trông hay không
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns>True nếu trống</returns>
    public static bool CheckSquareEmpty(Vector2 position)
    {
        for (int i = 0; i < PiecesWhite.Count; i++)
        {
            if ((Vector2)PiecesWhite[i].transform.position == position && PiecesWhite[i].activeSelf)
            {
                return false;
            }
        }

        for (int i = 0; i < PiecesBlack.Count; i++)
        {
            if ((Vector2)PiecesBlack[i].transform.position == position && PiecesBlack[i].activeSelf)
            {
                return false;
            }
        }

        return true;
    }

    
    // Kiểm tra có quân cờ Trắng tại vị trí chỉ định không?
    
    /// <param name="position">Vị trí cần kiểm tra</param>
    /// <returns>True nếu có quân Trắng</returns>
    public static bool CheckSquareWhite(Vector2 position)
    {
        for (int i = 0; i < PiecesWhite.Count; i++)
        {
            if ((Vector2)PiecesWhite[i].transform.position == position && PiecesWhite[i].activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    
    // Kiểm tra có quân cờ Đen tại vị trí chỉ định không?
    
    /// <param name="position">Vị trí cần kiểm tra.</param>
    /// <returns>True nếu có quân Đen</returns>
    public static bool CheckSquareBlack(Vector2 position)
    {
        for (int i = 0; i < PiecesBlack.Count; i++)
        {
            if ((Vector2)PiecesBlack[i].transform.position == position && PiecesBlack[i].activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Save

    
    // Tự động lưu trò chơi
    
    static void AutoSave()
    {
        SaveDataRaw data = new SaveDataRaw
        {
            playerInTurn = playerInTurn,
            enPassantDoublePosition = enPassantPawnPosition,
            enPassantPosition = EnPassantPosition,
            movements = movements,
            savedPositions = savedPositions,
            piecesWhite = PiecesWhite,
            piecesBlack = PiecesBlack
        };

        SaveManager.SaveGame(0, data);
    }

    
    // Xóa trò chơi được lưu tự động
    
    static void DeleteAutoSave()
    {
        SaveManager.DeleteAutoSave();

        Interface.interfaceClass.UpdateSaveDates();
    }

    
    // Lưu trò chơi vào slot mong muốn
    
    /// <param name="saveSlot">Slots để lưu trò chơi</param>
    public static void SaveGame(int saveSlot)
    {
        SaveDataRaw data = new SaveDataRaw
        {
            playerInTurn = playerInTurn,
            enPassantDoublePosition = enPassantPawnPosition,
            enPassantPosition = EnPassantPosition,
            movements = movements,
            savedPositions = savedPositions,
            piecesWhite = PiecesWhite,
            piecesBlack = PiecesBlack
        };

        SaveManager.SaveGame(saveSlot, data);
    }

    #endregion

    #region Pause

    
    // Làm biến mất các ô vuông + hiện thị lựa chọn khi tạm dừng
    
    /// <param name="activePause">True nếu hiện, False nếu ẩn</param>
    public static void PauseGame(bool activePause)
    {
        if (activePause)
        {
            foreach (GameObject piece in PiecesWhite)
            {
                piece.GetComponent<SpriteRenderer>().enabled = false;
            }

            foreach (GameObject piece in PiecesBlack)
            {
                piece.GetComponent<SpriteRenderer>().enabled = false;
            }

            DeselectPosition();
            DisableSelection();
        }

        else
        {
            foreach (GameObject piece in PiecesWhite)
            {
                piece.GetComponent<SpriteRenderer>().enabled = true;
            }

            foreach (GameObject piece in PiecesBlack)
            {
                piece.GetComponent<SpriteRenderer>().enabled = true;
            }

            EnableSelection();
        }
    }

    #endregion

    #region Artificial Intelligence

    
    // Bắt đầu di chuyển quân trong lượt AI
    
    public static void MoveAIPiece()
    {
        
        if (PlayerColour == Enums.Colours.White && playerInTurn == Enums.Colours.Black)
        {
            AIMovePosition bestMove = MiniMax.BestMovementBlack();
            ActivePiece = bestMove.piece;
            PiecesWhite.Reverse();
            PiecesBlack.Reverse();
            MovePiece(bestMove.position);
        }

        else if (PlayerColour == Enums.Colours.Black && playerInTurn == Enums.Colours.White)
        {
            AIMovePosition bestMove = MiniMax.BestMovementWhite();
            ActivePiece = bestMove.piece;
            PiecesWhite.Reverse();
            PiecesBlack.Reverse();
            MovePiece(bestMove.position);
        }
    }

    #endregion
}