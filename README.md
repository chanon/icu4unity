# icu4unity
ICU4C library wrapper for the Unity game engine.
Focused on word/line breaking for Asian languages.

# What is it?
It allows to use ICU4C's line breaking facilities to do word/line breaking for Asian languages in Unity.

ICU4C is the same library used in many widely used software applications for line breaking such as Google Chrome.

You can now bring that same line breaking ability to your Unity game with __icu4unity__, simplifying localization and display of Asian language text.

# Languages Supported
* Chinese
* Japanese
* Thai
* All other languages except Kmer, Laos and Burmese

Kmer, Laos and Burmese can be added by rebuilding the ICU data files to include their dictionaries.

# Platforms Supported
Tested on:
* Windows 64 bit
* Android
* macOS
* iOS

# Size Requirements

## Data file size
To use ICU4C, its data file, icudt63l.dat, must be included in the final app in StreamingAssets. 

The size is around 4.4MB and compresses to 2.4MB

Additionally for Android the whole file must be loaded into memory, using 4MB of memory. 

A workaround for Android so it doesn't require 4MB of memory would be to copy the data file out of the jar file into SD card. Then the path could be given to ICU4C. However I haven't implemented this yet.

## Code size
* For Windows the dll file is about 1MB
* For macOS the bundle size is about 1.5MB
* For Android the .so files are about 1.7MB
* For iOS the .a file is about 8MB, actual contribution to final executable size is less than 1.5MB

# Todo
* Document how to use
* Create example project

# License
Author: Chanon Sajjamanochai
See LICENSE.md
