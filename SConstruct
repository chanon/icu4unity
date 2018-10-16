sources = ['src/icu4unity.cpp']

env = Environment()
output = env.SharedLibrary(target = 'icu4unity', source = sources)

Default(output)

# scons examples
# env.Append(CPPPATH = ['/usr/local/include/'])
# env.Append(CCFLAGS= ['-O3'])
# env.Append(CPPDEFINES=['BIG_ENDIAN'])
# env.Append(CPPDEFINES={'RELEASE_BUILD' : '1'})
# env.Append(LIBPATH = ['/usr/local/lib/'])
# env.Append(LIBS = ['SDL_image','GL'])
# env.Append(LINKFLAGS = ['-Wl,--rpath,/usr/local/lib/'])