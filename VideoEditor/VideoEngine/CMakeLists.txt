cmake_minimum_required(VERSION 3.10)
project(VideoEngine)

set(CMAKE_CXX_STANDARD 17)

# Define output directory for DLL to be the same as the C# output or a common bin
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/../bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_SOURCE_DIR}/../bin)

# Include directories for FFmpeg (Assuming they are in ThirdParty/include)
include_directories(${CMAKE_SOURCE_DIR}/../ThirdParty/include)

# Link directories for FFmpeg (Assuming they are in ThirdParty/lib)
link_directories(${CMAKE_SOURCE_DIR}/../ThirdParty/lib)

add_library(VideoEngine SHARED VideoEngine.cpp)

# Link FFmpeg libraries (names may vary based on distribution)
# target_link_libraries(VideoEngine avcodec avformat avutil swscale)

# Definitions to export DLL symbols on Windows
if(WIN32)
    target_compile_definitions(VideoEngine PRIVATE VIDEOENGINE_EXPORTS)
endif()
