using System;

namespace StockChessCS.Interfaces
{
    // Giao diện IEngineService định nghĩa các phương thức và sự kiện
    // cần thiết để tương tác với một dịch vụ engine cờ vua.
    public interface IEngineService
    {
        // Khởi động dịch vụ engine.
        void StartEngine();

        // Dừng dịch vụ engine.
        void StopEngine();

        // Gửi lệnh tới engine.
        void SendCommand(string command);

        // Sự kiện để lắng nghe tin nhắn từ engine.
        event Action<string> EngineMessage;
    }
}
