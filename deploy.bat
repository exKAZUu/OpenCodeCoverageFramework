mkdir nuspec
copy *.nuspec nuspec /y
cd nuspec
mkdir lib
mkdir lib\Library
copy ..\Occf.Core\bin\Release\*.dll lib\
copy ..\Occf.Core\bin\Release\*.pdb lib\
del lib\Paraiba.*
FOR %%f IN (*.nuspec) DO (
	nuget pack %%f
)
FOR %%f IN (*.nupkg) DO (
	nuget push %%f
)
cd ..\
rd nuspec /s /q
