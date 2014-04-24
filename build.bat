@ECHO OFF

SET TARGET=build

IF (%1)==() SET TARGET=build
IF NOT (%1)==() SET TARGET=%1

nant %TARGET% -D:project.name=TimeTraveller -D:msbuild.home=c:\WINDOWS\Microsoft.NET\Framework\v3.5
