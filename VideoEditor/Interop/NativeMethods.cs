using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Interop;

// C++で実装された動画処理エンジン (VideoEngine.dll) との連携を行うクラス
// P/Invoke (Platform Invoke) を使用して、ネイティブ関数を呼び出します。
internal static class NativeMethods
{
    // 連携するDLLの名前 (VideoEngine.dll)
    private const string DllName = "VideoEngine";

    // エンジンの初期化を行います
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Engine_Initialize();

    // エンジンのリソースを解放します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Engine_Cleanup();

    // 新しいタイムライン（編集プロジェクト）を作成し、ハンドルを返します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr Timeline_Create();

    // タイムラインを破棄します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Timeline_Destroy(IntPtr timelineHandle);

    // タイムラインに動画クリップを追加します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Timeline_AddVideoClip(IntPtr timelineHandle, string filePath, double startTime, double duration, double positionInTimeline);

    // タイムラインに音声トラックを追加します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Timeline_AddAudioTrack(IntPtr timelineHandle, string filePath, double startTime, double duration, double positionInTimeline);

    // タイムラインにテキストを追加します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Timeline_AddTextOverlay(IntPtr timelineHandle, string text, double positionInTimeline, double duration, int x, int y, int fontSize, string colorHex);

    // フィルター効果を適用します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Timeline_ApplyFilter(IntPtr timelineHandle, int filterType, double value);

    // 進捗状況を通知するためのコールバックデリゲート定義
    public delegate void ProgressCallback(int progress);

    // 動画の書き出し（エクスポート）を開始します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Timeline_Export(IntPtr timelineHandle, string outputPath, int width, int height, int fps, ProgressCallback progressCallback);

    // 書き出し処理をキャンセルします
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Timeline_CancelExport(IntPtr timelineHandle);

    // 直近のエラーメッセージを取得します
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Engine_GetLastError(StringBuilder buffer, int bufferSize);
}
