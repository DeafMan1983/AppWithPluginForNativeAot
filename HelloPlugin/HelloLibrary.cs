namespace HelloPlugin;

using static DeafMan1983.ConvFunctions;
using System.Runtime.InteropServices;

public unsafe class HelloLibrary
{
    // public string Name { get => "hello"; }
    [UnmanagedCallersOnly(EntryPoint = "get_name_from_plugin")]
    public static sbyte *Name_from_Plugin()
    {
        return SBytePointerFromString("hello");
    }

    // public string Description { get => "Displays hello message."; }
    [UnmanagedCallersOnly(EntryPoint = "get_description_from_plugin")]
    public static sbyte *FuncDescription()
    {
        return SBytePointerFromString("Displays hello message.");
    }

    [UnmanagedCallersOnly(EntryPoint = "execute_from_plugin")]
    public static int FuncExecute()
    {
        Console.WriteLine("Hello !!!");
        return 0;
    }

}