using System.Collections.Generic;
using UnityEngine;

// Thuật toán MinMax cho AI
public static class MiniMax
{
    
    // Nước đi phù hợp nhất cho quân trắng
    public static AIMovePosition BestMovementWhite()
    {
        int value = 10000;
        List<AIMovePosition> selectedMove = new List<AIMovePosition>();

        for (int i = 0; i < Chess.PiecesWhite.Count; i++)
        {
            // Tính toán tất cả các nước hợp lý

            List<Vector2> greenPositions = Chess.PiecesWhite[i].GetComponent<PiecesMovement>().SearchGreenPositions();

            // Nếu quân ko có nước nào khả thi => quân tiếp theo

            if (greenPositions.Count == 0)
            {
                continue;
            }

            for (int j = 0; j < greenPositions.Count; j++)
            { 
                // Lưu các biến của quân cờ (vị trí và nếu nó có thể di chuyển) để có thể phục hồi 
 
                Vector2 startPosition = Chess.PiecesWhite[i].transform.position;
                bool hasMoved = Chess.PiecesWhite[i].GetComponent<PiecesMovement>().FirstMove;
 
                // Di chuyển mảnh đến vị trí có thể 
 
                Chess.PiecesWhite[i].transform.position = greenPositions[j];
                Chess.PiecesWhite[i].GetComponent<PiecesMovement>().FirstMove = true;

	            int currentValue = BoardValueWhite(greenPositions[j]);

                if (currentValue > value && value != 0)
                {
                    Chess.PiecesWhite[i].transform.position = startPosition;
                    Chess.PiecesWhite[i].GetComponent<PiecesMovement>().FirstMove = hasMoved;

                    continue;
                }

                int valueTemp = BestValueBlack(6, currentValue,-10000,10000);

                // Trắng tìm các giảm "giá trị của đen"
                // Nếu bé hơn giá trị ta tìm được => skip
                // Lớn hơn giá trị ta tìm được => thêm

                if (selectedMove.Count == 0 || valueTemp <= value)
                {                   
                    if( selectedMove.Count != 0)
                    {
                        selectedMove.Clear();
                    }
                    
                
                    value = valueTemp;

                    selectedMove.Add(new AIMovePosition(Chess.PiecesWhite[i], greenPositions[j]));
                }

                // Trả lại các quan về vị trí ban đầu

                Chess.PiecesWhite[i].transform.position = startPosition;
                Chess.PiecesWhite[i].GetComponent<PiecesMovement>().FirstMove = hasMoved;

                if (value == -10000)
                {
                    return selectedMove[Random.Range(0, selectedMove.Count)];
                }
            }
        }

        // Chỉ còn lại 1 nước đi ngẫu nhiên

        return selectedMove[Random.Range(0, selectedMove.Count)];
    }
    // Nước đi phù hợp nhất cho quân đen
    public static AIMovePosition BestMovementBlack()
    {

        int value = -10000;
        List<AIMovePosition> selectedMove = new List<AIMovePosition>();

        for (int i = 0; i < Chess.PiecesBlack.Count; i++)
        {
            // Tính toán tất cả các nước hợp lý

            List<Vector2> greenPositions = Chess.PiecesBlack[i].GetComponent<PiecesMovement>().SearchGreenPositions();

            // Nếu quân ko có nước nào khả thi => quân tiếp theo

            if (greenPositions.Count == 0)
            {
                continue;
            }
            
            
            for (int j = 0; j < greenPositions.Count; j++)
            {
                // Lưu các biến của quân cờ (vị trí và nếu nó có thể di chuyển) để có thể phục hồi

                Vector2 startPosition = Chess.PiecesBlack[i].transform.position;
                bool hasMoved = Chess.PiecesBlack[i].GetComponent<PiecesMovement>().FirstMove;

                // Di chuyển mảnh đến vị trí có thể

                Chess.PiecesBlack[i].transform.position = greenPositions[j];
                Chess.PiecesBlack[i].GetComponent<PiecesMovement>().FirstMove = true;

		        int currentValue = BoardValueBlack(greenPositions[j]);


                if (currentValue < value && value != 0)
                {
                    Chess.PiecesBlack[i].transform.position = startPosition;
                    Chess.PiecesBlack[i].GetComponent<PiecesMovement>().FirstMove = hasMoved;

                    continue;
                }

                int valueTemp = BestValueWhite(6, currentValue,-10000,10000);

                // Đen tìm các giảm "giá trị của trắng"
                // Nếu bé hơn giá trị ta tìm được => skip
                // Lớn hơn giá trị ta tìm được => thêm

                if (selectedMove.Count == 0 || valueTemp >= value)
                {
                    
                    if( selectedMove.Count != 0)
                    {
                        selectedMove.Clear();
                    }
                    
                    value = valueTemp;

                    selectedMove.Add(new AIMovePosition(Chess.PiecesBlack[i], greenPositions[j]));
                }

                // Trả lại các quan về vị trí ban đầu
                
                Chess.PiecesBlack[i].transform.position = startPosition;
                Chess.PiecesBlack[i].GetComponent<PiecesMovement>().FirstMove = hasMoved;
                Debug.Log(value);
                if (value == 10000)
                {
                    return selectedMove[Random.Range(0, selectedMove.Count)];
                }
            }
        }

        // Chỉ còn lại 1 nước đi ngẫu nhiên

        return selectedMove[Random.Range(0, selectedMove.Count)];
    }

    static int BestValueWhite(int depth, int previousValue, int alpha, int beta)
    {
        List<GameObject> capturedPieces = new List<GameObject>();

        foreach (GameObject piece in Chess.PiecesWhite)
        {
            if (Chess.CheckSquareBlack(piece.transform.position))
            {
                if (Chess.BlackKingPosition == (Vector2)piece.transform.position)
                {
                    foreach (GameObject capturedPiece in capturedPieces)
                    {
                        capturedPiece.SetActive(true);
                    }

                    return -10000;
                }

                capturedPieces.Add(piece);
                piece.SetActive(false);
            }
        }

        int value = 10000;
        Chess.CheckVerification();

        bool foundMove = false; // Track if any valid move is found

        for (int i = 0; i < Chess.PiecesWhite.Count; i++)
        {
            GameObject piece = Chess.PiecesWhite[i];
            PiecesMovement pieceMovement = piece.GetComponent<PiecesMovement>();

            if (!pieceMovement)
            {
                continue;
            }

            List<Vector2> greenPositions = pieceMovement.SearchGreenPositions();

            foreach (Vector2 position in greenPositions)
            {
                Vector2 startPosition = piece.transform.position;
                bool hasMoved = pieceMovement.FirstMove;

                piece.transform.position = position;
                pieceMovement.FirstMove = true;

                int currentValue = BoardValueWhite(position);

                if (currentValue > previousValue)
                {
                    piece.transform.position = startPosition;
                    pieceMovement.FirstMove = hasMoved;
                    continue;
                }

                int valueTemp;

                if (depth == 0)
                {
                    valueTemp = currentValue;
                }
                else
                {
                    depth--;
                    valueTemp = BestValueBlack(depth, currentValue, alpha, beta);

                    if (valueTemp < beta)
                    {
                        beta = valueTemp;

                        if (beta <= alpha)
                        {
                            piece.transform.position = startPosition;
                            pieceMovement.FirstMove = hasMoved;
                            goto EndLoop; // Alpha-beta pruning
                        }
                    }
                }

                foundMove = true; // Valid move found
                if (valueTemp < value)
                {
                    value = valueTemp;
                }

                EndLoop:
                piece.transform.position = startPosition;
                pieceMovement.FirstMove = hasMoved;
            }
        }

        foreach (GameObject piece in capturedPieces)
        {
            piece.SetActive(true);
        }

        if (!foundMove)
        {
            return -10000; // No valid move found, return a very low value
        }

        return value;
    }


   
  
    static int BestValueBlack(int depth, int previousValue, int alpha, int beta)
    {
        List<GameObject> capturedPieces = new List<GameObject>();

        foreach (GameObject piece in Chess.PiecesBlack)
        {
            if (Chess.CheckSquareWhite(piece.transform.position))
            {
                if (Chess.WhiteKingPosition == (Vector2)piece.transform.position)
                {
                    foreach (GameObject capturedPiece in capturedPieces)
                    {
                        capturedPiece.SetActive(true);
                    }

                    return 10000;
                }

                capturedPieces.Add(piece);
                piece.SetActive(false);
            }
        }

        int value = -10000;
        Chess.CheckVerification();

        bool foundMove = false; // Track if any valid move is found

        for (int i = 0; i < Chess.PiecesBlack.Count; i++)
        {
            GameObject piece = Chess.PiecesBlack[i];
            PiecesMovement pieceMovement = piece.GetComponent<PiecesMovement>();

            if (!pieceMovement)
            {
                continue;
            }

            List<Vector2> greenPositions = pieceMovement.SearchGreenPositions();

            foreach (Vector2 position in greenPositions)
            {
                Vector2 startPosition = piece.transform.position;
                bool hasMoved = pieceMovement.FirstMove;

                piece.transform.position = position;
                pieceMovement.FirstMove = true;

                int currentValue = BoardValueBlack(position);

                if (currentValue < previousValue)
                {
                    piece.transform.position = startPosition;
                    pieceMovement.FirstMove = hasMoved;
                    continue;
                }

                int valueTemp;

                if (depth == 0)
                {
                    valueTemp = currentValue;
                }
                else
                {
                    depth--;
                    valueTemp = BestValueWhite(depth, currentValue, alpha, beta);

                    if (valueTemp > alpha)
                    {
                        alpha = valueTemp;

                        if (alpha >= beta)
                        {
                            piece.transform.position = startPosition;
                            pieceMovement.FirstMove = hasMoved;
                            goto EndLoop; // Alpha-beta pruning
                        }
                    }
                }

                foundMove = true; // Valid move found
                if (valueTemp > value)
                {
                    value = valueTemp;
                }

                EndLoop:
                piece.transform.position = startPosition;
                pieceMovement.FirstMove = hasMoved;
            }
        }

        foreach (GameObject piece in capturedPieces)
        {
            piece.SetActive(true);
        }

        if (!foundMove)
        {
            return 10000; // No valid move found, return a very high value
        }

        return value;
    }


   
    // Giá trị hiện tại trên bảng cho các quân trắng

    static int BoardValueWhite(Vector2 position)
    {
        int value = 0;

        // Loại bỏ quân cờ đen nếu bị bắt trong nước đi

        GameObject pieceInPosition = Chess.GetPieceBlackInPosition(position);

        if (pieceInPosition != null)
        {
            pieceInPosition.SetActive(false);
        }

        // Bước tiếp theo là cộng giá trị của từng quân cờ trên bàn cờ của cả hai màu

        for (int i = 0; i < Chess.PiecesWhite.Count; i++)
        {
            value += Chess.PiecesWhite[i].GetComponent<PiecesMovement>().Value;
        }

        for (int i = 0; i < Chess.PiecesBlack.Count; i++)
        {
            value += Chess.PiecesBlack[i].GetComponent<PiecesMovement>().Value;
        }

        // Cuối cùng, nếu trong bước đầu tiên ta đã loại bỏ bất kỳ phần nào, ta sẽ khôi phục phần đó.

        if (pieceInPosition != null)
        {
            pieceInPosition.SetActive(true);
        }

        return value;
    }

   
    static int BoardValueBlack(Vector2 position)
    {
        int value = 0;
        GameObject pieceInPosition = Chess.GetPieceWhiteInPosition(position);

        if (pieceInPosition != null)
        {
            pieceInPosition.SetActive(false);
        }

        for (int i = 0; i < Chess.PiecesWhite.Count; i++)
        {
            value += Chess.PiecesWhite[i].GetComponent<PiecesMovement>().Value;
        }

        for (int i = 0; i < Chess.PiecesBlack.Count; i++)
        {
            value += Chess.PiecesBlack[i].GetComponent<PiecesMovement>().Value;
        }

        if (pieceInPosition != null)
        {
            pieceInPosition.SetActive(true);
        }

        return value;
    }
}