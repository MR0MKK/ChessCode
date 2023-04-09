using System.Collections.Generic;
using UnityEngine;

// Thuật toán MinMax cho AI
public static class MiniMax
{
    
    // Nước đi phù hợp nhất cho quân trắng
    public static AIMovePosition BestMovementWhite()
    {
        int value = 0;
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

                int valueTemp = BestValueBlack(10, 0,-10000,10000);

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

        int value = 0;
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

                int valueTemp = BestValueWhite(10, 0,-10000,10000);

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
Debug.Log(Chess.PiecesBlack[i].GetComponent<PiecesMovement>().PieceType);
            // Debug.Log(greenPositions.Count);
            Debug.Log(value);
                    selectedMove.Add(new AIMovePosition(Chess.PiecesBlack[i], greenPositions[j]));
                    Debug.Log(greenPositions[j]);
                }

                // Trả lại các quan về vị trí ban đầu
                
                Chess.PiecesBlack[i].transform.position = startPosition;
                Chess.PiecesBlack[i].GetComponent<PiecesMovement>().FirstMove = hasMoved;

                if (value == 10000)
                {
                    return selectedMove[Random.Range(0, selectedMove.Count)];
                }
            }
        }
        Debug.Log("ENDENDENDENDENDEND");

        // Chỉ còn lại 1 nước đi ngẫu nhiên

        return selectedMove[Random.Range(0, selectedMove.Count)];
    }

    // Tính toán giá trị tốt nhất của Trắng tại 1 thời điểm
    static int BestValueWhite(int depth, int previousValue,int alpha,int beta)
    {
        // Kiểm tra xem không có quân đen và trắng nào trên cùng một ô vuông.
        // Quân trắng bị quân đen bắt
        // Xóa quân cờ bị bắt để thuật toán ko tính quân đó

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

        // Gía trị ban đầu

        int value = 10000;
        Chess.CheckVerification();

        for (int i = 0; i < Chess.PiecesWhite.Count; i++)
        {
            // Mỗi phần, cập nhật danh sách các vị trí và kiểm tra các nước đi hợp pháp.

                

            // List<Vector2> greenPositions = Chess.PiecesWhite[i].GetComponent<PiecesMovement>().SearchGreenPositions();
            GameObject piece = Chess.PiecesWhite[i];
            PiecesMovement pieceMovement = piece.GetComponent<PiecesMovement>();
            if (!pieceMovement)
            {
                continue;
            }
            List<Vector2> greenPositions = pieceMovement.SearchGreenPositions();

            // Quần không thể thực hiện bất kỳ chuyển động nào, bỏ qua nó và thử chuyển động tiếp theo

            if (greenPositions.Count == 0)
                continue;

            for (int j = 0; j < greenPositions.Count; j++)
            {
                // Lưu vị trí ban đầu để có thể truy xuất nó sau này

                Vector2 startPosition = Chess.PiecesWhite[i].transform.position;
                bool hasMoved = Chess.PiecesWhite[i].GetComponent<PiecesMovement>().FirstMove;

                // Di chuyển quân đến vị trí mới và thu được giá trị

                Chess.PiecesWhite[i].transform.position = greenPositions[j];
                Chess.PiecesWhite[i].GetComponent<PiecesMovement>().FirstMove = true;

                int currentValue = BoardValueWhite(greenPositions[j]);

                if (currentValue > previousValue)
                {
                    
                    piece.transform.position = startPosition;
                    pieceMovement.FirstMove = hasMoved;
                    continue;
                }

                int valueTemp;

                // Nếu ta ở mức độ sâu nhất -> trả về 

                if (depth == 0)
                {
                    valueTemp = currentValue;
                }

                // Chưa sâu nhaast => tăng 1 mức và tính toán cho màu đối thủ
                else
                {
                    depth--;

                    valueTemp = BestValueBlack(depth, currentValue,alpha,beta);
                    
                    // giá trị tốt nhất cho Trắng, tìm kiếm giá trị thấp nhất cho Đen

                }

                    if (valueTemp < value)
                        value = valueTemp;
                    
                    if (value >= alpha) // alpha-beta pruning
                    {
                        alpha=value;   
                    }

                    if (value < beta)
                        beta = value;
                    
                    

           
                // Trả lại các quân về vị trí ban đầu của chúng.
                piece.transform.position = startPosition;
                pieceMovement.FirstMove = hasMoved;
               
            }
        }

        // Đưa quân về trạn thái ban đầu để tiến hành tính toán mới

        foreach (GameObject piece in capturedPieces)
        {
            piece.SetActive(true);
        }

        return value;
    }

   
  
    static int BestValueBlack(int depth, int previousValue,int alpha,int beta)
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
        for (int i = 0; i < Chess.PiecesBlack.Count; i++)
        {

            
      
            // List<Vector2> greenPositions = Chess.PiecesBlack[i].GetComponent<PiecesMovement>().SearchGreenPositions();

            // if (greenPositions.Count == 0)
                // continue;
            GameObject piece = Chess.PiecesBlack[i];

            PiecesMovement pieceMovement = piece.GetComponent<PiecesMovement>();

            if (!pieceMovement)
            {
                continue;
            }

            List<Vector2> greenPositions = pieceMovement.SearchGreenPositions();

            for (int j = 0; j < greenPositions.Count; j++)
            {
                Vector2 startPosition = Chess.PiecesBlack[i].transform.position;
                bool hasMoved = Chess.PiecesBlack[i].GetComponent<PiecesMovement>().FirstMove;

                Chess.PiecesBlack[i].transform.position = greenPositions[j];
                Chess.PiecesBlack[i].GetComponent<PiecesMovement>().FirstMove = true;

                int currentValue = BoardValueBlack(greenPositions[j]);

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
                    valueTemp = BestValueWhite(depth, currentValue,alpha,beta);
                    
                  
                }

                if (valueTemp > value)
                {
                    value = valueTemp;
                }

                if (value > alpha)
                {
                    alpha = value;
                }

                if (value > beta)
                    beta = value;
                          
                piece.transform.position = startPosition;
                pieceMovement.FirstMove = hasMoved;
            
            }
        }

        foreach (GameObject piece in Chess.PiecesBlack)
        {
            piece.SetActive(true);
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