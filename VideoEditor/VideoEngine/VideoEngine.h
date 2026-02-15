#pragma once

#ifdef VIDEOENGINE_EXPORTS
#define VIDEOENGINE_API __declspec(dllexport)
#else
#define VIDEOENGINE_API __declspec(dllimport)
#endif

#include <cstdint>

extern "C" {

    // エンジンの初期化処理
    VIDEOENGINE_API int Engine_Initialize();

    // エンジンの終了処理（リソース解放）
    VIDEOENGINE_API void Engine_Cleanup();

    // 新しいタイムライン（プロジェクト）の作成
    // 戻り値: タイムラインオブジェクトへのポインタ
    VIDEOENGINE_API void* Timeline_Create();

    // タイムラインの破棄
    VIDEOENGINE_API void Timeline_Destroy(void* timelineHandle);

    // 動画クリップをタイムラインに追加
    VIDEOENGINE_API int Timeline_AddVideoClip(void* timelineHandle, const char* filePath, double startTime, double duration, double positionInTimeline);

    // BGM（音声トラック）をタイムラインに追加
    VIDEOENGINE_API int Timeline_AddAudioTrack(void* timelineHandle, const char* filePath, double startTime, double duration, double positionInTimeline);

    // テキストオーバーレイ（字幕など）を追加
    VIDEOENGINE_API int Timeline_AddTextOverlay(void* timelineHandle, const char* text, double positionInTimeline, double duration, int x, int y, int fontSize, const char* colorHex);

    // フィルター効果の適用 (0: なし, 1: グレースケール, 2: 明るさ)
    VIDEOENGINE_API int Timeline_ApplyFilter(void* timelineHandle, int filterType, double value);

    // 動画の書き出し（MP4形式）
    // progressCallback: 進捗状況(0-100)を通知するための関数ポインタ
    typedef void (*ProgressCallback)(int progress);
    VIDEOENGINE_API int Timeline_Export(void* timelineHandle, const char* outputPath, int width, int height, int fps, ProgressCallback progressCallback);
    
    // 書き出し処理の中断
    VIDEOENGINE_API void Timeline_CancelExport(void* timelineHandle);

    // 最後に発生したエラーメッセージの取得
    VIDEOENGINE_API void Engine_GetLastError(char* buffer, int bufferSize);
}
