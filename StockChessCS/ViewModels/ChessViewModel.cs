using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using StockChessCS.Interfaces;
using StockChessCS.Commands;
using StockChessCS.Enums;
using StockChessCS.Helpers;
using StockChessCS.Models;
using System.Windows.Input;

namespace StockChessCS.ViewModels
{
    public class ChessViewModel : ViewModelBase
    {
        private IEngineService engine;  // Dịch vụ động cơ (engine) để tích hợp với trò chơi cờ.
        private StringBuilder moves = new StringBuilder(); // StringBuilder để xây dựng danh sách các nước đi trong trò chơi.
        private short deepAnalysisTime = 5000; // Thời gian cho phép tính toán sâu hơn cho mỗi nước đi (mili giây).
        private short moveValidationTime = 1;  // Thời gian cho phép kiểm tra nước đi cơ bản (mili giây).
        private TaskFactory ctxTaskFactory;  // Đối tượng TaskFactory được sử dụng để thực hiện các tác vụ trên luồng giao diện người dùng (UI thread).

        public ChessViewModel(IEngineService es)
        {
            engine = es;  // Khởi tạo đối tượng engine bằng dịch vụ được cung cấp.
            BoardItems = Chess.BoardSetup();  // Khởi tạo bàn cờ bằng cách sử dụng phương thức BoardSetup từ lớp Chess.
            ctxTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());  // Khởi tạo TaskFactory cho luồng giao diện người dùng.
            engine.EngineMessage += EngineMessage;  // Đăng ký sự kiện xử lý thông điệp từ engine.
            engine.StartEngine();  // Khởi động engine.
        }

        private ObservableCollection<IBoardItem> _boardItems;  // Danh sách các ô cờ trên bàn cờ.
        public ObservableCollection<IBoardItem> BoardItems
        {
            get { return _boardItems; }
            set
            {
                _boardItems = value;
                OnPropertyChanged();  // Gọi sự kiện OnPropertyChanged để thông báo sự thay đổi.
            }
        }

        private bool _isEngineThinking;  // Biến kiểm tra xem engine đang tính toán hay không.
        public bool IsEngineThinking
        {
            get { return _isEngineThinking; }
            set
            {
                _isEngineThinking = value;
                OnPropertyChanged();  // Gọi sự kiện OnPropertyChanged để thông báo sự thay đổi.
            }
        }

        private bool _checkMate;  // Biến kiểm tra xem trò chơi đã kết thúc (bí mật) hay chưa.
        public bool CheckMate
        {
            get { return _checkMate; }
            set
            {
                _checkMate = value;
                OnPropertyChanged();  // Gọi sự kiện OnPropertyChanged để thông báo sự thay đổi.
            }
        }

        private PieceColor _playerColor;  // Màu của người chơi (Trắng hoặc Đen).
        public PieceColor PlayerColor
        {
            get { return _playerColor; }
            set
            {
                _playerColor = value;
                OnPropertyChanged();  // Gọi sự kiện OnPropertyChanged để thông báo sự thay đổi.
            }
        }

        private bool playerWantsToMovePiece;  // Biến kiểm tra xem người chơi muốn di chuyển quân cờ hay không.
        private bool playerWantsToCapturePiece;  // Biến kiểm tra xem người chơi muốn bắt quân cờ của đối phương hay không.
        private ChessPiece selectedPiece;  // Quân cờ mà người chơi đã chọn.
        private BoardSquare targetSquare;  // Ô cờ mà người chơi muốn di chuyển đến.
        private ChessPiece targetPiece;  // Quân cờ đối thủ mà người chơi muốn bắt.

        public IBoardItem SelectedBoardItem
        {
            set
            {
                if (value is ChessPiece)  // Nếu ô cờ được chọn là một quân cờ.
                {
                    var piece = (ChessPiece)value;
                    if (piece.Color == PlayerColor)  // Nếu người chơi chọn một quân cờ của mình.
                    {
                        selectedPiece = piece;  // Gán quân cờ đã chọn.
                    }
                    else if (piece.Color != PlayerColor && selectedPiece != null)
                    {
                        playerWantsToCapturePiece = true;
                        targetPiece = piece;
                        ValidateMove(targetPiece);  // Thực hiện kiểm tra và ghi nhớ nước đi.
                    }
                }
                else if (value is BoardSquare && selectedPiece != null)  // Nếu ô cờ được chọn là một ô trống và đã có quân cờ được chọn.
                {
                    playerWantsToMovePiece = true;
                    targetSquare = (BoardSquare)value;
                    ValidateMove(targetSquare);  // Thực hiện kiểm tra và ghi nhớ nước đi.
                }
            }
        }

        private ICommand _newGameCommand;  // Lệnh bắt đầu trò chơi mới.
        public ICommand NewGameCommand
        {
            get
            {
                if (_newGameCommand == null) _newGameCommand = new RelayCommand(o => NewGame());  // Tạo một đối tượng lệnh mới nếu chưa có.
                return _newGameCommand;
            }
        }

        private void NewGame()
        {
            BoardItems = Chess.BoardSetup();  // Khởi tạo bàn cờ mới.
            if (moves.Length > 0) moves.Clear();  // Xóa danh sách nước đi.
            if (CheckMate) CheckMate = false;  // Đặt lại trạng thái kết thúc trò chơi nếu có.
            if (IsEngineThinking) IsEngineThinking = false;  // Đặt lại trạng thái tính toán của engine nếu đang tính toán.
            ResetSomeMembers();  // Đặt lại các biến thành viên.
            engine.SendCommand(UciCommands.ucinewgame);  // Gửi lệnh cho engine bắt đầu trò chơi mới.

            if (PlayerColor == PieceColor.Black)  // Nếu người chơi là màu Đen, engine sẽ chọn nước đi đầu tiên.
            {
                engine.SendCommand(UciCommands.position);  // Gửi lệnh vị trí hiện tại của bàn cờ cho engine.
                engine.SendCommand(UciCommands.go_movetime + " " + deepAnalysisTime.ToString());  // Gửi lệnh cho engine tính toán nước đi theo thời gian deepAnalysisTime.
                IsEngineThinking = true;  // Đặt trạng thái tính toán của engine.
            }
        }

        private ICommand _stopEngineCommand;  // Lệnh dừng engine Stockfish.
        public ICommand StopEngineCommand
        {
            get
            {
                if (_stopEngineCommand == null) _stopEngineCommand = new RelayCommand(o => StopEngine());  // Tạo một đối tượng lệnh mới nếu chưa có.
                return _stopEngineCommand;
            }
        }

        private void StopEngine()
        {
            engine.EngineMessage -= EngineMessage;  // Hủy đăng ký sự kiện xử lý thông điệp từ engine.
            engine.StopEngine();  // Dừng engine.
        }

        private ICommand _changePlayerColorCommand;  // Lệnh thay đổi màu của người chơi (Trắng hoặc Đen).
        public ICommand ChangePlayerColorCommand
        {
            get
            {
                if (_changePlayerColorCommand == null)
                {
                    _changePlayerColorCommand = new RelayCommand<PieceColor>(ChangePlayerColor);  // Tạo một đối tượng lệnh mới nếu chưa có.
                }
                return _changePlayerColorCommand;
            }
        }

        private void ChangePlayerColor(PieceColor color)
        {
            PlayerColor = color;  // Thay đổi màu của người chơi.
            NewGame();  // Bắt đầu trò chơi mới.
        }

        private void EngineMessage(string message)
        {
            if (message.Contains(UciCommands.bestmove))  // Nếu thông điệp từ engine chứa nước đi tốt nhất.
            {
                if (!message.Contains("ponder")) CheckMate = true;  // Nếu không có ponder move, đánh dấu trò chơi kết thúc.

                var move = message.Split(' ').ElementAt(1);  // Tách nước đi từ thông điệp.
                var position1 = move.Take(2);  // Lấy vị trí đầu tiên.
                var position2 = move.Skip(2);  // Lấy vị trí thứ hai.
                var enginePiece = BoardItems.OfType<ChessPiece>().
                    Where(p => p.Rank == int.Parse(position1.ElementAt(1).ToString()) &
                    p.File == position1.ElementAt(0)).Single();  // Tìm quân cờ trên bàn cờ tương ứng với nước đi.

                if (enginePiece.Color == PlayerColor)  // Nếu người chơi đã thực hiện nước đi không hợp lệ.
                {
                    RemoveLastMove();  // Xóa nước đi cuối cùng.
                    ResetSomeMembers();  // Đặt lại các biến thành viên.
                }
                else
                {
                    if (playerWantsToMovePiece)
                    {
                        ctxTaskFactory.StartNew(() => Chess.MovePiece(selectedPiece, targetSquare, BoardItems)).Wait();  // Thực hiện di chuyển quân cờ người chơi.
                        DeeperMoveAnalysis();  // Tiến hành kiểm tra nước đi sâu hơn.
                    }
                    else if (playerWantsToCapturePiece)
                    {
                        ctxTaskFactory.StartNew(() => Chess.CapturePiece(selectedPiece, targetPiece, BoardItems)).Wait();  // Thực hiện bắt quân cờ đối thủ.
                        DeeperMoveAnalysis();  // Tiến hành kiểm tra nước đi sâu hơn.
                    }
                    else  // Nếu đây là nước đi của engine.
                    {
                        moves.Append(" " + move);  // Ghi nhớ nước đi vào danh sách nước đi.
                        var pieceToCapture = BoardItems.OfType<ChessPiece>().
                            Where(p => p.Rank == int.Parse(position2.ElementAt(1).ToString()) &
                            p.File == position2.ElementAt(0)).SingleOrDefault();  // Tìm quân cờ cần bắt.

                        if (pieceToCapture != null)
                        {
                            ctxTaskFactory.StartNew(() => Chess.CapturePiece(enginePiece, pieceToCapture, BoardItems)).Wait();  // Thực hiện bắt quân cờ đối thủ.
                        }
                        else
                        {
                            targetSquare = BoardItems.OfType<BoardSquare>().
                                Where(s => s.Rank == int.Parse(position2.ElementAt(1).ToString()) &
                                s.File == position2.ElementAt(0)).SingleOrDefault();  // Tìm ô cờ cần di chuyển đến.
                            ctxTaskFactory.StartNew(() => Chess.MovePiece(enginePiece, targetSquare, BoardItems)).Wait();  // Thực hiện di chuyển quân cờ của engine.
                        }
                        IsEngineThinking = false;  // Đặt trạng thái tính toán của engine về false.
                    }
                }
            }
        }

        private void ResetSomeMembers()
        {
            if (selectedPiece != null) selectedPiece = null;  // Đặt lại quân cờ đã chọn về null.
            if (playerWantsToCapturePiece) playerWantsToCapturePiece = false;  // Đặt lại biến kiểm tra bắt quân cờ của đối thủ về false.
            if (playerWantsToMovePiece) playerWantsToMovePiece = false;  // Đặt lại biến kiểm tra di chuyển quân cờ của người chơi về false.
        }

        private void DeeperMoveAnalysis()
        {
            SendMovesToEngine(deepAnalysisTime);  // Tiến hành kiểm tra nước đi sâu hơn.
            IsEngineThinking = true;  // Đặt trạng thái tính toán của engine về true.
            ResetSomeMembers();  // Đặt lại các biến thành viên.
        }

        private void RemoveLastMove()
        {
            if (moves.Length > 0)
            {
                var length = moves.Length;
                var start = moves.Length - 5;
                moves.Remove(start, 5);  // Xóa nước đi cuối cùng khỏi danh sách nước đi.
            }
        }

        private void ValidateMove(IBoardItem item)
        {
            var position1 = selectedPiece.File.ToString() + selectedPiece.Rank.ToString();  // Lấy vị trí của quân cờ đã chọn.
            var position2 = item.File.ToString() + item.Rank.ToString();  // Lấy vị trí của ô cờ muốn di chuyển đến.

            var move = position1 + position2;  // Tạo nước đi từ hai vị trí.
            moves.Append(" " + move);  // Ghi nhớ nước đi vào danh sách nước đi.
            SendMovesToEngine(moveValidationTime);  // Gửi danh sách nước đi cho engine để kiểm tra.
        }

        private void SendMovesToEngine(short time)
        {
            var command = UciCommands.position + moves.ToString();  // Tạo lệnh đặt vị trí bàn cờ cho engine.
            engine.SendCommand(command);  // Gửi lệnh cho engine.
            command = UciCommands.go_movetime + " " + time.ToString();  // Tạo lệnh cho engine tính toán nước đi theo thời gian.
            engine.SendCommand(command);  // Gửi lệnh cho engine.
        }
    }
}
