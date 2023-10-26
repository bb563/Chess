using Microsoft.Practices.Unity; // Sử dụng Unity Container để quản lý phụ thuộc
using StockChessCS.Interfaces;
using StockChessCS.Services;
using StockChessCS.ViewModels;

namespace StockChessCS.Helpers
{
    public class ViewModelLocator
    {
        private UnityContainer _container;

        public ViewModelLocator()
        {
            // Khởi tạo Unity Container để quản lý các đối tượng và phụ thuộc
            _container = new UnityContainer();

            // Đăng ký dịch vụ IEngineService với StockfishService trong container.
            // Điều này có nghĩa rằng khi cần một đối tượng IEngineService, Unity Container
            // sẽ tạo và cung cấp một đối tượng StockfishService.
            _container.RegisterType<IEngineService, StockfishService>();
        }

        public ChessViewModel ChessVM
        {
            get
            {
                // Giải quyết và trả về một đối tượng ChessViewModel từ Unity Container.
                // Điều này đảm bảo rằng ChessViewModel được tạo và sử dụng
                // đúng phiên bản của IEngineService (StockfishService) đã đăng ký.
                return _container.Resolve<ChessViewModel>();
            }
        }
    }
}