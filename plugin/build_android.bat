@echo off

set proj=%cd%

md build_android\arm
pushd build_android\arm
cmake -DTARGET_PLATFORM=Android  -G "Unix Makefiles" -DCMAKE_MAKE_PROGRAM="%ANDROID_NDK_ROOT%/prebuilt/windows-x86_64/bin/make.exe" -DCMAKE_BUILD_TYPE=Release -DCMAKE_ANDROID_ABI="armeabi-v7a" %proj%
popd

md build_android\x86
pushd build_android\x86
cmake -DTARGET_PLATFORM=Android  -G "Unix Makefiles" -DCMAKE_MAKE_PROGRAM="%ANDROID_NDK_ROOT%/prebuilt/windows-x86_64/bin/make.exe" -DCMAKE_BUILD_TYPE=Release -DCMAKE_ANDROID_ABI="x86" %proj%
popd

cmake --build build_android\arm --config Release
cmake --build build_android\x86 --config Release

pause