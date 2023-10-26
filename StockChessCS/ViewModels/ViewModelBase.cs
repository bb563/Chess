using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StockChessCS.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        // Khai báo sự kiện PropertyChanged để thông báo sự thay đổi thuộc tính.
        public event PropertyChangedEventHandler PropertyChanged;

        // Phương thức này được gọi để thông báo rằng một thuộc tính đã thay đổi.
        // CallerMemberNameAttribute được sử dụng để tự động lấy tên của thuộc tính gọi phương thức.
        public void OnPropertyChanged([CallerMemberName()] string name = null)
        {
            // Kiểm tra xem tên thuộc tính đã được truyền hay chưa.
            if (name != null)
            {
                // Gọi sự kiện PropertyChanged để thông báo sự thay đổi của thuộc tính.
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
