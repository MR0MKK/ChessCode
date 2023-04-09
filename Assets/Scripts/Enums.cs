// Danh sách các Enums dùng cho lớp khác
public static class Enums
{
    // Màu của quân cờ 2 đội
    public enum Colours {
        Black,
        White,
        // Khi chơi multiplayer
        All 
    };

    // Khi tốt được thăng cấp
    public enum PromotablePieces {
        Rook,
        Knight,
        Bishop,
        Queen
    }

    // Các cách để một trò chơi hòa
    public enum DrawModes {
        Stalemate,
        Impossibility,
        Move75,
        ThreefoldRepetition
    }
}