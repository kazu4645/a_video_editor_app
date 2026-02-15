# VideoEditor

This is a simple video editor application built with C# (WPF) and C++ (Native Engine).

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 (with C++ and .NET Desktop Development workloads)
- FFmpeg (Shared libraries)

## Structure

- **UI**: WPF Application (MVVM)
- **CoreModels**: Shared C# data models
- **Interop**: P/Invoke layer to communicate with the engine
- **VideoEngine**: C++ DLL that handles video processing (wraps FFmpeg)
- **ThirdParty**: Place FFmpeg headers (`include`) and libraries (`lib`) here.

## Build Instructions

1.  **Setup FFmpeg**:
    - Download FFmpeg shared libraries (Dev version).
    - Copy `include` folder to `ThirdParty/include`.
    - Copy `lib` folder to `ThirdParty/lib`.
    - Copy `bin/*.dll` to the output directory (e.g., `VideoEditor/UI/bin/Debug/net8.0/`).

2.  **Build VideoEngine (C++)**:
    - Open `VideoEditor` folder in Visual Studio or use CMake.
    - Build `VideoEngine` project.
    - Ensure `VideoEngine.dll` is copied to the UI output directory.

3.  **Build UI (C#)**:
    - Open `VideoEditor.sln` (or create one adding all projects).
    - Build `UI` project.
    - Run `UI.exe`.

## Notes

- The current implementation of `VideoEngine` contains a stub/mock implementation for demonstration purposes. To use real FFmpeg, uncomment the FFmpeg code in `VideoEngine.cpp` and ensure libraries are linked correctly in `CMakeLists.txt`.
