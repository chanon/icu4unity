cmake_minimum_required(VERSION 3.12)

if(${TARGET_PLATFORM} STREQUAL iOS)
    set(IOS_PLATFORM OS)
    set(IOS_DEPLOYMENT_TARGET 9.0)
    set(CMAKE_OSX_DEPLOYMENT_TARGET ${IOS_DEPLOYMENT_TARGET})
    set(CMAKE_TOOLCHAIN_FILE cmake/ios.toolchain.cmake)
endif()

if(${TARGET_PLATFORM} STREQUAL Android)
    set(CMAKE_ANDROID_NDK $ENV{ANDROID_NDK_ROOT})
    set(CMAKE_TOOLCHAIN_FILE ${CMAKE_ANDROID_NDK}/build/cmake/android.toolchain.cmake)
endif()

set(PROJ icu4unity)
project(${PROJ})

include_directories (${CMAKE_CURRENT_SOURCE_DIR}/src/icu4c-63_1/source/common)

add_definitions(-DU_CHARSET_IS_UTF8=1)
add_definitions(-DU_COMMON_IMPLEMENTATION)
add_definitions(-DU_STATIC_IMPLEMENTATION)

add_definitions(-DUCONFIG_NO_LEGACY_CONVERSION=1)
add_definitions(-DUCONFIG_ONLY_HTML_CONVERSION=1)
add_definitions(-DUCONFIG_NO_COLLATION=1)
add_definitions(-DUCONFIG_NO_FORMATTING=1)
add_definitions(-DUCONFIG_NO_TRANSLITERATION=1)
add_definitions(-DUCONFIG_NO_REGULAR_EXPRESSIONS=1)

#add_definitions(-DU_HIDE_DEPRECATED_API)
#add_definitions(-DU_HIDE_DRAFT_API)
#add_definitions(-DU_HIDE_INTERNAL_API)
#add_definitions(-DU_HIDE_SYSTEM_API)  
#add_definitions(-DU_HIDE_OBSOLETE_API)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11 -fvisibility=hidden -fvisibility-inlines-hidden")
set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fvisibility=hidden -fvisibility-inlines-hidden -Wno-shorten-64-to-32")

if(${TARGET_PLATFORM} STREQUAL Android)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -s")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -s")
endif()

if(${TARGET_PLATFORM} STREQUAL iOS)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -s")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -s")
endif()

file(GLOB icu4c_sources ${CMAKE_CURRENT_SOURCE_DIR}/src/icu4c-63_1/source/common/*.cpp)
file(GLOB_RECURSE icu4unity_sources ${CMAKE_CURRENT_SOURCE_DIR}/src/icu4unity/*.cpp)

set(sources ${icu4c_sources} ${icu4unity_sources} ${CMAKE_CURRENT_SOURCE_DIR}/src/icu4c-63_1/source/stubdata/stubdata.cpp)

set(OUTPUT_BASE ${CMAKE_CURRENT_SOURCE_DIR}/../asset/Assets/Plugins/icu4unity)

if(${TARGET_PLATFORM} STREQUAL macOS)
    set(CMAKE_OSX_ARCHITECTURES x86_64)
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM})
    add_library(${PROJ} MODULE ${sources})
    set_target_properties(${PROJ} PROPERTIES BUNDLE TRUE)
endif()

if(${TARGET_PLATFORM} STREQUAL Windows)
    set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${OUTPUT_BASE}/x86_64)
    set(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE ${OUTPUT_BASE}/x86_64)
    add_library(${PROJ} SHARED ${sources})    
    # add_executable(${PROJ}_test ${sources})
endif()

if(${TARGET_PLATFORM} STREQUAL iOS)
    add_definitions(-DIOS_PLATFORM=OS)
    add_definitions(-DIOS_DEPLOYMENT_TARGET=${IOS_DEPLOYMENT_TARGET})
    set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM})
    add_library(${PROJ} STATIC ${sources})
    set_xcode_property(${PROJ} IPHONEOS_DEPLOYMENT_TARGET "${IOS_DEPLOYMENT_TARGET}" "All")
endif()

if(${TARGET_PLATFORM} STREQUAL Android)
    add_compile_definitions(UNITY_ANDROID=1)
    set(ANDROID_PLATFORM 16)
    set(CMAKE_SYSTEM_VERSION ${ANDROID_PLATFORM})
    set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${OUTPUT_BASE}/${TARGET_PLATFORM}/libs/${ANDROID_ABI})
    add_library(${PROJ} SHARED ${sources})    
endif()