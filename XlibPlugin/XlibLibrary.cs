namespace XlibPlugin;

using static DeafMan1983.ConvFunctions;
using System.Runtime.InteropServices;

using TerraFX.Interop.Xlib;
using static TerraFX.Interop.Xlib.Xlib;

public unsafe class XlibLibrary
{
    // public string Name { get => "hello"; }
    [UnmanagedCallersOnly(EntryPoint = "get_name_from_plugin")]
    public static sbyte *Name_from_Plugin()
    {
        return SBytePointerFromString("x11");
    }

    // public string Description { get => "Displays hello message."; }
    [UnmanagedCallersOnly(EntryPoint = "get_description_from_plugin")]
    public static sbyte *FuncDescription()
    {
        return SBytePointerFromString("Displays Xlib.");
    }

    [UnmanagedCallersOnly(EntryPoint = "execute_from_plugin")]
    public static int FuncExecute()
    {
        Console.WriteLine("Xlib loading...");
        Display *display = XOpenDisplay(null);
        Alloc(display);

        int screen_number = XDefaultScreen(display);

        nuint black = XBlackPixel(display, screen_number);
        nuint white = XWhitePixel(display, screen_number);

        Console.WriteLine("Black is {0}", black);
        Console.WriteLine("White is {0}", white);

        XCloseDisplay(display);
        Free(display);
        return 0;
    }
}