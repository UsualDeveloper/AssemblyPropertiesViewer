# AssemblyPropertiesViewer
This is a simple .NET project that allows user to show various properties of a selected assembly, both directly written in dll files and calculated.

The project uses separate application domain to execute code with additional restrictions.

The application uses set of analysing classes to show different assembly properties:
 - is assembly compiled in "debug" or "release" mode
 - full assembly name
 - target framework
 - MD5 checksum
