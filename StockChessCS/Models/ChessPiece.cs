using StockChessCS.Enums;      // Import các enum cho PieceColor và PieceType
using StockChessCS.Interfaces; // Import giao diện IBoardItem
using StockChessCS.ViewModels; // Import lớp ViewModelBase

namespace StockChessCS.Models
{
    // Lớp ChessPiece biểu diễn một quân cờ trên bảng cờ vua và triển khai giao diện IBoardItem.
    public class ChessPiece : ViewModelBase, IBoardItem
    {
        // Thuộc tính Color biểu diễn màu sắc của quân cờ (PieceColor: White hoặc Black).
        private PieceColor _color;
        public PieceColor Color
        {
            get { return _color; }   // Trả về giá trị màu sắc của quân cờ.
            set
            {
                _color = value;       // Đặt giá trị màu sắc của quân cờ.
                OnPropertyChanged();  // Thông báo rằng thuộc tính đã thay đổi.
            }
        }

        // Thuộc tính Piece biểu diễn loại của quân cờ (PieceType: King, Queen, Bishop, ...).
        private PieceType _piece;
        public PieceType Piece
        {
            get { return _piece; }   // Trả về giá trị loại của quân cờ.
            set
            {
                _piece = value;       // Đặt giá trị loại của quân cờ.
                OnPropertyChanged();  // Thông báo rằng thuộc tính đã thay đổi.
            }
        }

        // Thuộc tính Rank biểu diễn hàng (rank) của quân cờ trên bảng cờ vua.
        private int _rank;
        public int Rank
        {
            get { return _rank; }   // Trả về giá trị hàng của quân cờ.
            set
            {
                _rank = value;        // Đặt giá trị hàng của quân cờ.
                OnPropertyChanged();  // Thông báo rằng thuộc tính đã thay đổi.
            }
        }

        // Thuộc tính File biểu diễn cột (file) của quân cờ trên bảng cờ vua (bằng ký tự).
        private char _file;
        public char File
        {
            get { return _file; }   // Trả về giá trị cột của quân cờ.
            set
            {
                _file = value;        // Đặt giá trị cột của quân cờ.
                OnPropertyChanged();  // Thông báo rằng thuộc tính đã thay đổi.
            }
        }

        // Thuộc tính ItemType xác định loại mục (Piece) của quân cờ.
        public ChessBoardItem ItemType { get; set; }
    }
}
