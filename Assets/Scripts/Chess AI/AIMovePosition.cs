using UnityEngine;


public class AIMovePosition
{
   
    // Quẩn để di chuyển
    public readonly GameObject piece;
    
    // Vị trí mà quân sẽ di chuyển
    public readonly Vector2 position;

    public AIMovePosition(GameObject piece, Vector2 position)
    {
        this.piece = piece;
        this.position = position;
    }
}