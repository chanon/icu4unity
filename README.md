# icu4unity
ICU4C library wrapper for the Unity game engine.
Focused on word/line breaking for Asian languages.

# Currently in Development

# What is it?
It allows to use ICU4C's line breaking facilities to do word/line breaking for Asian languages in Unity.

ICU4C is the same library used in many widely used software applications for line breaking such as Google Chrome.

You can now bring that same line breaking ability to your Unity game with __icu4unity__, simplifying localization and display of Asian language text.

# Languages Supported
* Chinese
* Japanese
* Thai

Kmer, Laos and Burmese can be added by rebuilding the ICU data files to include their dictionaries.
For other languages it should also work correctly.

# Size Requirements
* To use ICU4C, its data file must be included in the final app. The size is around 4MB.
* The shared library files are about 1MB
* Additionally for Android the whole file must be loaded into memory, using 4MB of memory. 

A workaround for Android so it doesn't require 4MB of memory would be to copy the data file out of the jar file into SD card. Then the path could be given to ICU4C. However I haven't implemented this yet.

# Platforms Supported
Tested on:
* Windows 64 bit
* Android

To Implement:
* macOS
* iOS

# License
See LICENSE.md
