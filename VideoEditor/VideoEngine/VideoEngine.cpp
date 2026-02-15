#define VIDEOENGINE_EXPORTS
#include "VideoEngine.h"
#include <iostream>
#include <string>
#include <vector>
#include <thread>
#include <chrono>
#include <cstring>
#include <mutex>

// 内部状態を管理するためのモック（疑似）構造体
// 本来はここにFFmpegのコンテキストやグラフ情報が含まれます
struct Timeline {
    // 動画クリップ情報
    struct VideoClip {
        std::string filePath;
        double startTime;
        double duration;
        double position;
    };
    // 音声トラック情報
    struct AudioTrack {
        std::string filePath;
        double startTime;
        double duration;
        double position;
    };
    // テキストオーバーレイ情報
    struct TextOverlay {
        std::string text;
        double position;
        double duration;
        int x, y, fontSize;
        std::string colorHex;
    };

    std::vector<VideoClip> videoClips;
    std::vector<AudioTrack> audioTracks;
    std::vector<TextOverlay> textOverlays;
    
    int filterType = 0;
    double filterValue = 0.0;
    
    // 書き出し状態管理フラグ
    bool isExporting = false;
    bool cancelExport = false;
};

std::string g_lastError;
std::mutex g_mutex;

void SetError(const std::string& msg) {
    std::lock_guard<std::mutex> lock(g_mutex);
    g_lastError = msg;
}

// エンジン初期化
// FFmpegライブラリのロードや初期設定をここで行います
VIDEOENGINE_API int Engine_Initialize() {
    // av_register_all(); // 新しいFFmpegでは非推奨ですが、概念的にはここで登録します
    std::cout << "[VideoEngine] Initialized." << std::endl;
    return 0;
}

VIDEOENGINE_API void Engine_Cleanup() {
    std::cout << "[VideoEngine] Cleanup." << std::endl;
}

VIDEOENGINE_API void* Timeline_Create() {
    return new Timeline();
}

VIDEOENGINE_API void Timeline_Destroy(void* timelineHandle) {
    if (timelineHandle) {
        delete static_cast<Timeline*>(timelineHandle);
    }
}

VIDEOENGINE_API int Timeline_AddVideoClip(void* timelineHandle, const char* filePath, double startTime, double duration, double positionInTimeline) {
    if (!timelineHandle) return -1;
    Timeline* timeline = static_cast<Timeline*>(timelineHandle);
    
    Timeline::VideoClip clip;
    clip.filePath = filePath;
    clip.startTime = startTime;
    clip.duration = duration;
    clip.position = positionInTimeline;
    
    timeline->videoClips.push_back(clip);
    std::cout << "[VideoEngine] Added video clip: " << filePath << std::endl;
    return 0;
}

VIDEOENGINE_API int Timeline_AddAudioTrack(void* timelineHandle, const char* filePath, double startTime, double duration, double positionInTimeline) {
    if (!timelineHandle) return -1;
    Timeline* timeline = static_cast<Timeline*>(timelineHandle);
    
    Timeline::AudioTrack track;
    track.filePath = filePath;
    track.startTime = startTime;
    track.duration = duration;
    track.position = positionInTimeline;
    
    timeline->audioTracks.push_back(track);
    std::cout << "[VideoEngine] Added audio track: " << filePath << std::endl;
    return 0;
}

VIDEOENGINE_API int Timeline_AddTextOverlay(void* timelineHandle, const char* text, double positionInTimeline, double duration, int x, int y, int fontSize, const char* colorHex) {
    if (!timelineHandle) return -1;
    Timeline* timeline = static_cast<Timeline*>(timelineHandle);
    
    Timeline::TextOverlay overlay;
    overlay.text = text;
    overlay.position = positionInTimeline;
    overlay.duration = duration;
    overlay.x = x;
    overlay.y = y;
    overlay.fontSize = fontSize;
    overlay.colorHex = colorHex;
    
    timeline->textOverlays.push_back(overlay);
    std::cout << "[VideoEngine] Added text overlay: " << text << std::endl;
    return 0;
}

VIDEOENGINE_API int Timeline_ApplyFilter(void* timelineHandle, int filterType, double value) {
    if (!timelineHandle) return -1;
    Timeline* timeline = static_cast<Timeline*>(timelineHandle);
    timeline->filterType = filterType;
    timeline->filterValue = value;
    std::cout << "[VideoEngine] Applied filter: " << filterType << " value: " << value << std::endl;
    return 0;
}

// 動画の書き出し処理（モック実装）
// 実際にはFFmpegを使用してエンコード処理を行います
VIDEOENGINE_API int Timeline_Export(void* timelineHandle, const char* outputPath, int width, int height, int fps, ProgressCallback progressCallback) {
    if (!timelineHandle) return -1;
    Timeline* timeline = static_cast<Timeline*>(timelineHandle);
    
    timeline->isExporting = true;
    timeline->cancelExport = false;
    
    std::cout << "[VideoEngine] Starting export to " << outputPath << " (" << width << "x" << height << " @" << fps << "fps)" << std::endl;
    
    // 書き出しプロセスのシミュレーション
    // 0%から100%までループで進捗を進めます
    for (int i = 0; i <= 100; ++i) {
        // キャンセルリクエストの確認
        if (timeline->cancelExport) {
            std::cout << "[VideoEngine] Export cancelled." << std::endl;
            timeline->isExporting = false;
            return 1; // キャンセルされた
        }
        
        // UI側に進捗を通知
        if (progressCallback) {
            progressCallback(i);
        }
        
        // 処理時間をシミュレート（実際は重いエンコード処理が入る）
        std::this_thread::sleep_for(std::chrono::milliseconds(50));
    }
    
    timeline->isExporting = false;
    std::cout << "[VideoEngine] Export completed." << std::endl;
    return 0;
}

VIDEOENGINE_API void Timeline_CancelExport(void* timelineHandle) {
    if (!timelineHandle) return;
    Timeline* timeline = static_cast<Timeline*>(timelineHandle);
    if (timeline->isExporting) {
        timeline->cancelExport = true;
    }
}

VIDEOENGINE_API void Engine_GetLastError(char* buffer, int bufferSize) {
    std::lock_guard<std::mutex> lock(g_mutex);
    if (buffer && bufferSize > 0) {
        strncpy(buffer, g_lastError.c_str(), bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
    }
}
