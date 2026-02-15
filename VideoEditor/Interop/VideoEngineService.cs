using System;
using System.Text;
using System.Threading.Tasks;
using CoreModels;

namespace Interop;

// UI層から動画処理エンジンを操作するためのサービスクラス
// NativeMethods をラップし、より安全で使いやすいインターフェースを提供します。
public class VideoEngineService : IDisposable
{
    // C++側のタイムラインオブジェクトへのハンドル（ポインタ）
    private IntPtr _timelineHandle;
    private bool _disposed;
    // ガベージコレクションを防ぐためにコールバックを保持
    private NativeMethods.ProgressCallback? _progressCallback;

    // コンストラクタ：エンジンの初期化とタイムラインの作成を行います
    public VideoEngineService()
    {
        NativeMethods.Engine_Initialize();
        _timelineHandle = NativeMethods.Timeline_Create();
    }

    // 動画クリップを追加します
    public void AddVideoClip(VideoClip clip)
    {
        CheckDisposed();
        int result = NativeMethods.Timeline_AddVideoClip(_timelineHandle, clip.FilePath, clip.StartTime, clip.Duration, clip.PositionInTimeline);
        if (result != 0) ThrowLastError();
    }

    // 音声トラックを追加します
    public void AddAudioTrack(AudioTrack track)
    {
        CheckDisposed();
        int result = NativeMethods.Timeline_AddAudioTrack(_timelineHandle, track.FilePath, track.StartTime, track.Duration, track.PositionInTimeline);
        if (result != 0) ThrowLastError();
    }

    // テキストオーバーレイを追加します
    public void AddTextOverlay(TextOverlay overlay)
    {
        CheckDisposed();
        int result = NativeMethods.Timeline_AddTextOverlay(_timelineHandle, overlay.Text, overlay.PositionInTimeline, overlay.Duration, overlay.X, overlay.Y, overlay.FontSize, overlay.ColorHex);
        if (result != 0) ThrowLastError();
    }

    // フィルターを適用します
    public void ApplyFilter(FilterType filterType, double value)
    {
        CheckDisposed();
        int result = NativeMethods.Timeline_ApplyFilter(_timelineHandle, (int)filterType, value);
        if (result != 0) ThrowLastError();
    }

    // 動画の書き出しを非同期で実行します
    // IProgress<int> を使用して進捗状況をUIに報告します
    public async Task ExportAsync(string outputPath, int width, int height, int fps, IProgress<int> progress)
    {
        CheckDisposed();

        // アンマネージコードに渡すデリゲートを作成
        _progressCallback = (p) => progress?.Report(p);
        
        // 重い処理なのでバックグラウンドスレッドで実行
        await Task.Run(() =>
        {
            int result = NativeMethods.Timeline_Export(_timelineHandle, outputPath, width, height, fps, _progressCallback);
            if (result != 0 && result != 1) // 0: 成功, 1: キャンセル
            {
                // 本来はここでエラーハンドリングを行う
            }
        });
    }

    // 書き出しをキャンセルします
    public void CancelExport()
    {
        CheckDisposed();
        NativeMethods.Timeline_CancelExport(_timelineHandle);
    }

    // エンジンから最後のエラーを取得して例外をスローします
    private void ThrowLastError()
    {
        StringBuilder sb = new StringBuilder(1024);
        NativeMethods.Engine_GetLastError(sb, sb.Capacity);
        throw new Exception($"VideoEngine Error: {sb}");
    }

    private void CheckDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(VideoEngineService));
    }

    // リソースの解放（IDisposable実装）
    public void Dispose()
    {
        if (!_disposed)
        {
            if (_timelineHandle != IntPtr.Zero)
            {
                // ネイティブ側のタイムラインオブジェクトを破棄
                NativeMethods.Timeline_Destroy(_timelineHandle);
                _timelineHandle = IntPtr.Zero;
            }
            NativeMethods.Engine_Cleanup();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    ~VideoEngineService()
    {
        Dispose();
    }
}
