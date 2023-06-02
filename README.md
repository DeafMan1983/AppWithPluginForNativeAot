# AppWithPlugin For NativeAot
Successor of AppWithPlugin by Microsoft and I use for NativeAot Examples - Wonderful for Dotnet 6.0 and greater support
Imagine about loading current native libraries like you made app, tools or runtime in C#.
Nor assemblies support ( `System.Reflection:Assembly` )

Now we use Net 6.x or greater. Yay I am very happy because I know how do I get successful with AppWithPlugin for Assembly by Microsoft :)

Stop using `ICommand`
Using class `Command`
Because Command.cs need use with unmanaged function pointers and interop with current operating system
For Linux libc -> c
For Windows Kernel32
For macOS libSystem

If you add Windows and macOS interop sorry I am lazy for to add.

Happy testing with loading native plugin libraries 
PS: XlibPlugin supports only Linux/FreeBSD (But it is possible.) Thanks!

For AppWithPlugin `dotnet publish -c Release -r <rid> --self-contained`<br />
For (Hello, Other or Xlib)Plugin `dotnet publish -c Release -r <rid> -p:NativeLib=Shared --self-contained<br />
And remember all native libraries copy and paste to `AppWithPlugin/bin/Release/net7.0/<rid>/native`
  
Enjoy your knowledges of native libraries 
