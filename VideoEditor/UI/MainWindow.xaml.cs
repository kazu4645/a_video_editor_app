using System.Windows;

namespace UI
{
    // メイン画面のコードビハインド
    // MVVMパターンを採用しているため、ここにはビジネスロジックを記述しません。
    // UIの初期化のみを行います。
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
