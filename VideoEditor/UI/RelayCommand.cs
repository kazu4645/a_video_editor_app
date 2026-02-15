using System;
using System.Windows.Input;

namespace UI;

// MVVMパターンで使用するコマンドクラス
// ViewModelのメソッドをUIのコマンドとしてバインドできるようにします
public class RelayCommand : ICommand
{
    // コマンド実行時に呼ばれるアクション
    private readonly Action<object?> _execute;
    // コマンドが実行可能かどうかを判定する関数
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    // コマンドが現在実行可能かどうかを返します
    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    // コマンドを実行します
    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    // 実行可能性が変更されたことを通知するイベント
    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }
}
