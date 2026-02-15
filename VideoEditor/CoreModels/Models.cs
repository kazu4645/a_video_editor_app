using System.Collections.ObjectModel;

namespace CoreModels;

// タイムライン上の動画クリップを表すクラス
public class VideoClip
{
    // 動画ファイルのパス
    public string FilePath { get; set; } = string.Empty;
    // 動画の開始時間（クリップ内）
    public double StartTime { get; set; }
    // クリップの再生時間
    public double Duration { get; set; }
    // タイムライン上の配置位置（秒）
    public double PositionInTimeline { get; set; }
}

// タイムライン上の音声トラックを表すクラス
public class AudioTrack
{
    // 音声ファイルのパス
    public string FilePath { get; set; } = string.Empty;
    public double StartTime { get; set; }
    // トラックの再生時間
    public double Duration { get; set; }
    // タイムライン上の配置位置（秒）
    public double PositionInTimeline { get; set; }
}

// 動画上に表示するテキストオーバーレイを表すクラス
public class TextOverlay
{
    // 表示するテキスト内容
    public string Text { get; set; } = string.Empty;
    // 表示開始位置（秒）
    public double PositionInTimeline { get; set; }
    // 表示時間
    public double Duration { get; set; }
    // 画面上のX座標
    public int X { get; set; }
    // 画面上のY座標
    public int Y { get; set; }
    // フォントサイズ
    public int FontSize { get; set; }
    // 文字色（16進数カラーコード）
    public string ColorHex { get; set; } = "#FFFFFF";
}

// 適用可能なフィルターの種類
public enum FilterType
{
    None = 0,      // なし
    Grayscale = 1, // グレースケール（白黒）
    Brightness = 2 // 明るさ調整
}
