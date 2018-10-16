@echo off

set proj=%cd%
rd build_windows /s /q

md build_windows\64 & pushd build_windows\64
cmake -DTARGET_PLATFORM=Windows -G "Visual Studio 15 2017 Win64" %proj%
popd

cmake --build build_windows\64 --config Release

pause