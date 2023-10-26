using StockChessCS.Interfaces;
using StockChessCS.Enums;

namespace StockChessCS.Models
{
    // Lớp BoardSquare triển khai giao diện IBoardItem để biểu diễn một ô vuông trên bảng cờ vua.
    public class BoardSquare : IBoardItem
    {
        public int Rank { get; set; }          // Hàng (rank) của ô vuông
        public char File { get; set; }         // Cột (file) của ô vuông
        public ChessBoardItem ItemType { get; set; } // Loại mục (Square) của ô vuông
    }
}
