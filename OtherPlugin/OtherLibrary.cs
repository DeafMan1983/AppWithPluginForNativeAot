namespace OtherPlugin;

using static DeafMan1983.ConvFunctions;
using System.Runtime.InteropServices;

public unsafe class OtherLibrary
{
    // public string Name { get => "hello"; }
    [UnmanagedCallersOnly(EntryPoint = "get_name_from_plugin")]
    public static sbyte *FuncName()
    {
        return SBytePointerFromString("other");
    }

    // public string Description { get => "Displays hello message."; }
    [UnmanagedCallersOnly(EntryPoint = "get_description_from_plugin")]
    public static sbyte *FuncDescription()
    {
        return SBytePointerFromString("Display other plugin.");
    }

    [UnmanagedCallersOnly(EntryPoint = "execute_from_plugin")]
    public static int FuncExecute()
    {
        Console.WriteLine("Other !!!");
        return 0;
    }

}