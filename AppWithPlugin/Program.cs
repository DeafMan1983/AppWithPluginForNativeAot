namespace AppWithPlugin;

using static DeafMan1983.ConvFunctions;

using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;

unsafe class Program
{
    // First get dlopen, dlsym and dlclose for Linux
    const string libc = "c";

    [DllImport(libc, EntryPoint="dlopen")]
    private static extern void *linux_dlopen(sbyte *so_file, int so_flags);
    
    [DllImport(libc, EntryPoint="dlsym")]
    private static extern void *linux_dlsym(void *so_handle, sbyte *func_name);

    [DllImport(libc, EntryPoint="dlclose")]
    private static extern void linux_dlclose(void *so_handle);

    [DllImport(libc, EntryPoint = "dlerror")]
    private static extern sbyte *linux_dlerror();

    // You would like to add for Windows and macOS


    // pass unmanaged function pointers for ICommand
    static delegate *unmanaged[Cdecl]<sbyte *> getNameFunc;
    static delegate *unmanaged[Cdecl]<sbyte *> getDescriptionFunc;
    static delegate *unmanaged[Cdecl]<int> executeFunc;
    
    // For LoadPluginLibrary for accessing native library
    static void *LoadPluginLibrary(string pluginPath)
    {
        void *handle = null;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // For Linux
            handle = linux_dlopen(SBytePointerFromString(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pluginPath)), 0x0001 | 0x0002);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // For Windows
            // You should put here
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // For macOS
            // You should put here
        }
        else
        {
            throw new PlatformNotSupportedException("Error: no support other os!");
        }

        return handle;
    }

    // GetProc for func_name like loaded native library with public methods
    static void *GetProc(void *handle, string func_name)
    {
        void *funcPtr = null;
        // For Linux
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            funcPtr = linux_dlsym(handle, SBytePointerFromString(func_name));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // You should put here GetProcAddress()
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // You should put here 
        }
        else
        {
            throw new PlatformNotSupportedException("Error: no support other os!");
        }

        return funcPtr;
    }

    // For Command uses with unmanaged function pointer accessibility 
    class Command
    {
        private void *_handle;

        public static IEnumerable<Command> CreateCommandFromLibraries(void *handle)
        {
            Command cmd = new();
            cmd._handle = handle;
            if (cmd._handle == null)
            {
                Console.WriteLine("Error: No loaded native library");
            }
            IList<Command> cmds = new List<Command>();
            cmds.Add(cmd);
            return cmds.ToArray();
        }

        // public string Name {get;}
        public string Name
        {
            get
            {
                getNameFunc = (delegate*unmanaged[Cdecl]<sbyte *>)GetProc(this._handle, "get_name_from_plugin");
                if (getNameFunc == null)
                {
                    Console.WriteLine(StringFromSBytePointer(linux_dlerror()));
                }
                return StringFromSBytePointer(getNameFunc());
            }
        }

        // public string Description {get;}
        public string Description
        {
            get
            {
                getDescriptionFunc = (delegate*unmanaged[Cdecl]<sbyte *>)GetProc(this._handle, "get_description_from_plugin");
                if (getDescriptionFunc == null)
                {
                    Console.WriteLine(StringFromSBytePointer(linux_dlerror()));
                }
                return StringFromSBytePointer(getDescriptionFunc());
            }
        }

        public int Execute()
        {
            executeFunc = (delegate *unmanaged[Cdecl]<int>)GetProc(this._handle, "execute_from_plugin");
            if (executeFunc == null)
            {
                Console.WriteLine(StringFromSBytePointer(linux_dlerror()));
            }
            return executeFunc();
        }
    }

    // There are magic of all native libraries loaded.
    static string[] pluginPaths
    {
        get
        {
            string rootDir = AppDomain.CurrentDomain.BaseDirectory;
            IList<string> nlpaths = new List<string>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                foreach (string so_path in Directory.EnumerateFiles(rootDir, "*.so", SearchOption.TopDirectoryOnly))
                {
                    if (File.Exists(so_path))
                    {
                        nlpaths.Add(so_path);
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                foreach (string dll_path in Directory.EnumerateFiles(rootDir, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    if (File.Exists(dll_path))
                    {
                        nlpaths.Add(dll_path);
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                foreach (string dylib_path in Directory.EnumerateFiles(rootDir, "*.dylib", SearchOption.TopDirectoryOnly))
                {
                    if (File.Exists(dylib_path))
                    {
                        nlpaths.Add(dylib_path);
                    }
                }
            }
            return nlpaths.ToArray();
        }
    }

    // Old AppWithPlugin but I replace ICommand with Command with unmanaged function pointers and it works fine.
    static void Main(string[] args)
    {
        try
        {
            if (args.Length == 1 && args[0] == "/d")
            {
                Console.WriteLine("Waiting for any key...");
                Console.ReadLine();
            }
            else
            {
                IEnumerable<Command> commands = pluginPaths.SelectMany(pluginPath =>
                {
                    void *handle = LoadPluginLibrary(pluginPath);
                    return Command.CreateCommandFromLibraries(handle).ToArray();
                }).ToList();

                if (args.Length == 0)
                {
                    Console.WriteLine("Commands from native libraries: ");
                    foreach (Command command in commands)
                    {
                        Console.WriteLine($"{command.Name}\t - {command.Description}");
                    }
                }
                else
                {
                    foreach (string commandName in args)
                    {
                        Console.WriteLine($"-- {commandName} --");
                        Command command = commands.FirstOrDefault(c => c.Name == commandName);
                        if (command == null)
                        {
                            Console.WriteLine("No such command is known.");
                            return;
                        }

                        command.Execute();
                        Console.WriteLine();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
