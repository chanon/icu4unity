# icu4unity notes

This is the complete icu4c-63_1 source code with the icu4c-63_1-data zip file merged in the data folder.
License of ICU4C code can be found in the License file.

## Building ICU4C Data Files

The built data files are already included in icu4unity.
To rebuild the data files, you must build the whole ICU4C.

Recommended to do on Ubuntu.
Can also use "Windows Subsystem for Linux".

Run 
```
cd source
chmod +x runConfigureICU configure
./runConfigureICU Linux --with-data-packaging=archive
make
```