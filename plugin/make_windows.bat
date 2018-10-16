@echo off

set proj=%cd%
rd build_windows /s /q

rem don't build win32, uncomment if needed
rem md build_windows\32 & pushd build_windows\32
rem cmake -DTARGET_PLATFORM=Windows -G "Visual Studio 15 2017" %proj%
rem popd

md build_windows\64 & pushd build_windows\64
cmake -DTARGET_PLATFORM=Windows -G "Visual Studio 15 2017 Win64" %proj%
popd

rem cmake --build build_windows\32 --config Release
cmake --build build_windows\64 --config Release