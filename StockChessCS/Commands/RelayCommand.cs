using System;
using System.Diagnostics;
using System.Windows.Input;

namespace StockChessCS.Commands
{
    // Lớp RelayCommand cung cấp một ICommand để thực hiện hành động và kiểm tra điều kiện có thể thực thi.
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        // Constructor mặc định của RelayCommand.
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        // Constructor của RelayCommand với cả hành động thực hiện và kiểm tra điều kiện thực thi.
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        // Kiểm tra xem lệnh có thể thực thi hay không.
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        // Sự kiện được kích hoạt khi có sự thay đổi về trạng thái có thể thực thi của lệnh.
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Thực hiện lệnh.
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    // Lớp RelayCommand<T> cung cấp một ICommand để thực hiện hành động và kiểm tra điều kiện có thể thực thi với một kiểu dữ liệu cụ thể.
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        // Constructor mặc định của RelayCommand<T>.
        public RelayCommand(Action<T> execute) : this(execute, null) { }

        // Constructor của RelayCommand<T> với cả hành động thực hiện và kiểm tra điều kiện thực thi.
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            _execute = execute;
            _canExecute = canExecute;
        }

        [DebuggerStepThrough()]
        // Kiểm tra xem lệnh có thể thực thi hay không.
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        // Sự kiện được kích hoạt khi có sự thay đổi về trạng thái có thể thực thi của lệnh.
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Thực hiện lệnh.
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
