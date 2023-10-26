using System;
using System.Linq;
using System.Globalization;
using System.Windows.Data;

namespace StockChessCS.Converters
{
    // Lớp FileColumnConverter được sử dụng để chuyển đổi giá trị cột từ ký tự (a-h) sang số tương ứng (0-7).
    public class FileColumnConverter : IValueConverter
    {
        // Phương thức Convert chuyển đổi giá trị cột từ ký tự (a-h) sang số tương ứng (0-7).
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Ép kiểu giá trị vào ký tự.
            var file = (char)value;

            // Tạo một mảng chứa các ký tự cột (a-h).
            var files = ("abcdefgh").ToArray();

            // Tìm vị trí của ký tự trong mảng và trả về số cột tương ứng.
            var column = files.ToList().IndexOf(file);

            return column;
        }

        // Phương thức ConvertBack không được sử dụng trong trường hợp này, trả về Binding.DoNothing.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
