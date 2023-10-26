using System;
using System.Globalization;
using System.Windows.Data;

namespace StockChessCS.Converters
{
    // Lớp RankRowConverter được sử dụng để chuyển đổi giá trị hàng từ số nguyên (1-8) sang số tương ứng (7-0).
    public class RankRowConverter : IValueConverter
    {
        // Phương thức Convert chuyển đổi giá trị hàng từ số nguyên (1-8) sang số tương ứng (7-0).
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Ép kiểu giá trị thành số nguyên.
            var rank = (int)value;

            // Tính toán hàng tương ứng bằng cách lấy 8 trừ đi giá trị rank.
            var row = 8 - rank;

            return row;
        }

        // Phương thức ConvertBack không được sử dụng trong trường hợp này, trả về Binding.DoNothing.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
