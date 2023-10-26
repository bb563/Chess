using System;
using System.IO;
using System.Reactive.Linq;
using System.Diagnostics;
using StockChessCS.Helpers;
using StockChessCS.Interfaces;

namespace StockChessCS.Services
{
    public class StockfishService : IEngineService
    {
        // Đối tượng để đọc dữ liệu từ engine Stockfish.
        private StreamReader strmReader;

        // Đối tượng để ghi dữ liệu đến engine Stockfish.
        private StreamWriter strmWriter;

        // Đối tượng đại diện cho tiến trình của engine Stockfish.
        private Process engineProcess;

        // Đối tượng dùng để lắng nghe thông điệp từ engine Stockfish.
        private IDisposable engineListener;

        // Sự kiện được kích hoạt khi có thông điệp từ engine Stockfish.
        public event Action<string> EngineMessage;

        // Phương thức để gửi lệnh đến engine Stockfish.
        public void SendCommand(string command)
        {
            // Kiểm tra xem strmWriter không null và command không phải là "uci".
            if (strmWriter != null && command != UciCommands.uci)
            {
                strmWriter.WriteLine(command);
            }
        }

        // Phương thức để dừng engine Stockfish.
        public void StopEngine()
        {
            // Kiểm tra xem engineProcess không null và không kết thúc.
            if (engineProcess != null && !engineProcess.HasExited)
            {
                // Hủy đăng ký lắng nghe thông điệp từ engine.
                engineListener.Dispose();

                // Đóng đối tượng strmReader và strmWriter.
                strmReader.Close();
                strmWriter.Close();
            }
        }

        // Phương thức để khởi động engine Stockfish.
        public void StartEngine()
        {
            // Xác định vị trí tệp thực thi của Stockfish.
            FileInfo engine = new FileInfo(Path.Combine(Environment.CurrentDirectory, "stockfish_8_x64.exe"));

            // Kiểm tra xem tệp tồn tại và có phải là tệp .exe.
            if (engine.Exists && engine.Extension == ".exe")
            {
                // Tạo một tiến trình để chạy engine Stockfish.
                engineProcess = new Process();

                // Cấu hình các thuộc tính của tiến trình.
                engineProcess.StartInfo.FileName = engine.FullName;
                engineProcess.StartInfo.UseShellExecute = false;
                engineProcess.StartInfo.RedirectStandardInput = true;
                engineProcess.StartInfo.RedirectStandardOutput = true;
                engineProcess.StartInfo.RedirectStandardError = true;
                engineProcess.StartInfo.CreateNoWindow = true;

                // Khởi động tiến trình.
                engineProcess.Start();

                // Lấy đối tượng StreamWriter để gửi lệnh đến engine.
                strmWriter = engineProcess.StandardInput;

                // Lấy đối tượng StreamReader để đọc kết quả từ engine.
                strmReader = engineProcess.StandardOutput;

                // Thiết lập lắng nghe các thông điệp từ engine bằng cách sử dụng Reactive Extensions.
                engineListener = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(1)).Subscribe(s => ReadEngineMessages());

                // Gửi các lệnh cần thiết để thiết lập engine theo giao thức UCI.
                strmWriter.WriteLine(UciCommands.uci);
                strmWriter.WriteLine(UciCommands.isready);
                strmWriter.WriteLine(UciCommands.ucinewgame);
            }
            else
            {
                // Nếu tệp không tồn tại hoặc không phải là tệp .exe, ném ngoại lệ FileNotFoundException.
                throw new FileNotFoundException();
            }
        }

        // Phương thức để đọc thông điệp từ engine Stockfish.
        private void ReadEngineMessages()
        {
            // Đọc một dòng thông điệp từ strmReader.
            var message = strmReader.ReadLine();

            // Kiểm tra xem thông điệp không rỗng.
            if (message != string.Empty)
            {
                // Kích hoạt sự kiện EngineMessage và truyền thông điệp đến các xử lý sự kiện (event handlers).
                EngineMessage?.Invoke(message);
            }
        }
    }
}
