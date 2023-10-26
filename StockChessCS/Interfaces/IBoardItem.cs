using StockChessCS.Enums;

namespace StockChessCS.Interfaces
{
    public interface IBoardItem
    {
        int Rank { get; set; }          // Hàng (rank) của mục trên bảng cờ vua
        char File { get; set; }         // Cột (file) của mục trên bảng cờ vua
        ChessBoardItem ItemType { get; set; } // Loại mục (Piece hoặc Square)
    }
}
