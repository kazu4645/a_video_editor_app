using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UI;

// ViewModelの基底クラス
// プロパティ変更通知（INotifyPropertyChanged）の実装を提供します
public class ViewModelBase : INotifyPropertyChanged
{
    // プロパティ変更時に発生するイベント
    public event PropertyChangedEventHandler? PropertyChanged;

    // プロパティ変更を通知するヘルパーメソッド
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // 値を変更し、変更があった場合のみイベントを発火するヘルパーメソッド
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
