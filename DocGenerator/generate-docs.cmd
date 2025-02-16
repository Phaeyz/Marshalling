setlocal
set libName=Phaeyz.Marshalling
set repoUrl=https://github.com/Phaeyz/Marshalling
dotnet run ..\%libName%\bin\Debug\net9.0\%libName%.dll ..\docs --source %repoUrl%/blob/main/%libName% --namespace %libName% --clean
endlocal