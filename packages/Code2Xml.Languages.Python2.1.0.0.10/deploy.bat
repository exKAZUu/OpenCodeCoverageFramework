copy ..\bin\Release\*.dll lib\
copy ..\bin\Release\*.pdb lib\
xcopy ..\bin\Release\ParserScripts content\ParserScripts /c /e /y
del *.nupkg
FOR %%f IN (*.nuspec) DO (
	nuget pack %%f
)
FOR %%f IN (*.nupkg) DO (
	nuget push %%f
)
