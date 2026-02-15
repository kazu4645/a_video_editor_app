using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using CoreModels;
using Interop;
using Microsoft.Win32;

namespace UI;

// アプリケーションのメイン画面のViewModel
// UIとデータ/ロジックの仲介を行います（MVVMパターン）
public class MainViewModel : ViewModelBase
{
    // 動画処理エンジンへのサービス
    private readonly VideoEngineService _videoEngine;
    
    // バッキングフィールド
    private string _statusMessage = "Ready";
    private int _exportProgress;
    private bool _isBusy;

    // UIにバインドするコレクション（リスト表示用）
    public ObservableCollection<VideoClip> VideoClips { get; } = new();
    public ObservableCollection<AudioTrack> AudioTracks { get; } = new();
    public ObservableCollection<TextOverlay> TextOverlays { get; } = new();

    // ステータスバーに表示するメッセージ
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    // 書き出しの進捗状況 (0-100)
    public int ExportProgress
    {
        get => _exportProgress;
        set => SetProperty(ref _exportProgress, value);
    }

    // 処理中フラグ（ボタンの有効/無効制御に使用）
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    // UIのボタンアクションに対応するコマンド
    public ICommand AddVideoCommand { get; }
    public ICommand AddAudioCommand { get; }
    public ICommand AddTextCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand ApplyFilterCommand { get; }

    public MainViewModel()
    {
        // エンジンサービスの初期化
        _videoEngine = new VideoEngineService();
        
        // コマンドの初期化
        AddVideoCommand = new RelayCommand(AddVideo);
        AddAudioCommand = new RelayCommand(AddAudio);
        AddTextCommand = new RelayCommand(AddText);
        // 書き出し中はボタンを押せないように制御
        ExportCommand = new RelayCommand(Export, _ => !IsBusy);
        ApplyFilterCommand = new RelayCommand(ApplyFilter);
    }

    // 動画追加処理
    private void AddVideo(object? _)
    {
        // ファイル選択ダイアログを表示
        var dlg = new OpenFileDialog { Filter = "Video Files|*.mp4;*.avi;*.mov" };
        if (dlg.ShowDialog() == true)
        {
            // モックデータとしてクリップを作成（実際の長さ取得はFFmpeg等が必要）
            var clip = new VideoClip 
            { 
                FilePath = dlg.FileName,
                StartTime = 0,
                Duration = 10, // 仮の長さ
                PositionInTimeline = VideoClips.Count * 10 
            };
            
            try
            {
                // エンジンに追加
                _videoEngine.AddVideoClip(clip);
                // UIコレクションに追加
                VideoClips.Add(clip);
                StatusMessage = $"Added video: {System.IO.Path.GetFileName(clip.FilePath)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }
    }

    private void AddAudio(object? _)
    {
        var dlg = new OpenFileDialog { Filter = "Audio Files|*.mp3;*.wav" };
        if (dlg.ShowDialog() == true)
        {
            var track = new AudioTrack
            {
                FilePath = dlg.FileName,
                StartTime = 0,
                Duration = 30, // Mock
                PositionInTimeline = 0
            };

            try
            {
                _videoEngine.AddAudioTrack(track);
                AudioTracks.Add(track);
                StatusMessage = $"Added audio: {System.IO.Path.GetFileName(track.FilePath)}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }
    }

    private void AddText(object? _)
    {
        // Simple mock input
        var overlay = new TextOverlay
        {
            Text = "Sample Text",
            PositionInTimeline = 0,
            Duration = 5,
            X = 100,
            Y = 100,
            FontSize = 24,
            ColorHex = "#FFFFFF"
        };

        try
        {
            _videoEngine.AddTextOverlay(overlay);
            TextOverlays.Add(overlay);
            StatusMessage = "Added text overlay";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
    }

    private void ApplyFilter(object? parameter)
    {
        if (parameter is string filterName && Enum.TryParse<FilterType>(filterName, out var type))
        {
            try
            {
                _videoEngine.ApplyFilter(type, 1.0); // Simple on/off for now
                StatusMessage = $"Applied filter: {type}";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }
    }

    // 動画書き出し処理（非同期）
    private async void Export(object? _)
    {
        var dlg = new SaveFileDialog { Filter = "MP4 Video|*.mp4", FileName = "output.mp4" };
        if (dlg.ShowDialog() == true)
        {
            IsBusy = true;
            StatusMessage = "Exporting...";
            ExportProgress = 0;

            // 進捗報告用のオブジェクト
            var progress = new Progress<int>(p => ExportProgress = p);

            try
            {
                // 非同期で書き出しを実行
                await _videoEngine.ExportAsync(dlg.FileName, 1920, 1080, 30, progress);
                StatusMessage = "Export Completed!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export Failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
