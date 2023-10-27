using System.Collections.ObjectModel;
using System.Linq;
using StockChessCS.Enums;
using StockChessCS.Interfaces;
using StockChessCS.Models;

namespace StockChessCS.Helpers
{
    public static class Chess
    {
        // Phương thức tạo bố cục ban đầu của bảng cờ.
        public static ObservableCollection<IBoardItem> BoardSetup()
        {
            // Tạo một ObservableCollection để lưu trữ các thành phần trên bảng cờ.
            ObservableCollection<IBoardItem> items = new ObservableCollection<IBoardItem>();

            // Tạo một mảng chứa các chữ cái đại diện cho các cột trên bảng cờ.
            var files = ("abcdefgh").ToArray();

            // Bắt đầu vòng lặp để tạo các ô cờ trên bảng cờ.
            foreach (var fl in files)
            {
                for (int rank = 1; rank <= 8; rank++)
                {
                    // Tạo và thêm một ô cờ vào danh sách items.
                    // Mỗi ô cờ được đại diện bởi đối tượng BoardSquare.
                    // Nó có thông tin về dòng (rank) và cột (file) trên bảng cờ.
                    items.Add(new BoardSquare { Rank = rank, File = fl, ItemType = ChessBoardItem.Square });
                }
            }

            // Tạo các quân tốt (Pawns)
            foreach (var fl in files)
            {
                items.Add(new ChessPiece
                {
                    //Rank và file là vị trí của các quân cờ
                    ItemType = ChessBoardItem.Piece,
                    Color = PieceColor.Black,
                    Piece = PieceType.Pawn,
                    Rank = 7, // <=== Ví dụ: Rank = 7 là hàng 7
                    File = fl
                });
                items.Add(new ChessPiece
                {
                    ItemType = ChessBoardItem.Piece,
                    Color = PieceColor.White,
                    Piece = PieceType.Pawn,
                    Rank = 2,
                    File = fl
                });
            }
            // Tạo các quân đen (Black pieces)
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.Rook, // Thêm Xe
                Rank = 8, 
                File = 'a'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.Knight, //Thêm Mã
                Rank = 8,
                File = 'b'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.Bishop, //Thêm Tượng
                Rank = 8,
                File = 'c'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.Queen, //Thêm Hậu
                Rank = 8,
                File = 'd'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.King, //Thêm vua
                Rank = 8,
                File = 'e'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.Bishop,
                Rank = 8,
                File = 'f'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.Knight,
                Rank = 8,
                File = 'g'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.Black,
                Piece = PieceType.Rook,
                Rank = 8,
                File = 'h'
            });
            // Tạo các quân trắng (White pieces)
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.Rook,
                Rank = 1,
                File = 'a'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.Knight,
                Rank = 1,
                File = 'b'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.Bishop,
                Rank = 1,
                File = 'c'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.Queen,
                Rank = 1,
                File = 'd'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.King,
                Rank = 1,
                File = 'e'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.Bishop,
                Rank = 1,
                File = 'f'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.Knight,
                Rank = 1,
                File = 'g'
            });
            items.Add(new ChessPiece
            {
                ItemType = ChessBoardItem.Piece,
                Color = PieceColor.White,
                Piece = PieceType.Rook,
                Rank = 1,
                File = 'h'
            });

            return items;
        }

        // Phương thức di chuyển quân cờ.
        public static void MovePiece(ChessPiece selectedPiece, BoardSquare selectedSquare,
            ObservableCollection<IBoardItem> items)
        {
            switch (selectedPiece.Piece)
            {
                case PieceType.King:
                    KingMove(selectedPiece, selectedSquare, items);
                    break;
                case PieceType.Pawn:
                    PawnMove(selectedPiece, selectedSquare, items);
                    break;
                default:
                    Move(selectedPiece, selectedSquare);
                    break;
            }
        }

        private static void Move(ChessPiece piece, BoardSquare square)
        {
            // Di chuyển quân cờ đến ô cờ mới.
            piece.Rank = square.Rank;
            piece.File = square.File;
        }

        private static void KingMove(ChessPiece piece, BoardSquare targetSquare, ObservableCollection<IBoardItem> items)
        {
            if (piece.File == 'e' && targetSquare.File == 'g') // Nước đi nhập thành ngắn
            {
                // Lấy quân xe đen ở vị trí 'h'.
                var rook = items.OfType<ChessPiece>().Where(p => p.Color == piece.Color &&
                p.Piece == PieceType.Rook && p.File == 'h').FirstOrDefault();

                // Di chuyển quân Vua và quân Xe cho nước đi nhập thành ngắn
                piece.File = 'g';
                rook.File = 'f';
            }
            else if (piece.File == 'e' && targetSquare.File == 'c') // Nước đi nhập thành dài
            {
                // Lấy quân xe đen ở vị trí 'a'.
                var rook = items.OfType<ChessPiece>().Where(p => p.Color == piece.Color &&
                p.Piece == PieceType.Rook && p.File == 'a').FirstOrDefault();

                // Di chuyển quân Vua và quân Xe cho nước đi nhập thành dài
                piece.File = 'c';
                rook.File = 'd';
            }
            else { Move(piece, targetSquare); }
        }

        private static void PawnMove(ChessPiece piece, BoardSquare targetSquare, ObservableCollection<IBoardItem> items)
        {
            // Nâng cấp quân tốt nếu đạt đến hàng cuối cùng của đối phương.

            switch (piece.Color)
            {
                case PieceColor.Black:
                    if (piece.Rank == 1) piece.Piece = PieceType.Queen;
                    break;
                case PieceColor.White:
                    if (piece.Rank == 8) piece.Piece = PieceType.Queen;
                    break;
            }
            // Xử lý nước đi Bắt Tốt qua đường.
            if (piece.File != targetSquare.File)
            {
                // Tìm quân tốt đối phương ở hàng và vị trí đích của nước đi Bắt Tốt qua đường
                var opponentPawn = items.OfType<ChessPiece>().Where(p => p.Color != piece.Color &&
                p.Piece == PieceType.Pawn && p.Rank == piece.Rank && p.File == targetSquare.File).FirstOrDefault();

                // Loại bỏ quân tốt đối phương.
                items.Remove(opponentPawn);
            }

            Move(piece, targetSquare);
        }

        // Phương thức di chuyển và ăn quân.
        public static void CapturePiece(ChessPiece selectedPiece, ChessPiece otherPiece,
            ObservableCollection<IBoardItem> items)
        {
            // Di chuyển quân cờ đã chọn đến vị trí của quân cờ đối phương và loại bỏ quân đối phương.
            selectedPiece.Rank = otherPiece.Rank;
            selectedPiece.File = otherPiece.File;
            items.Remove(otherPiece);
        }
    }
}
