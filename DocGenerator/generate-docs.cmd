setlocal
set lib=Phaeyz.Marshalling
set repoUrl=https://github.com/Phaeyz/Marshalling
dotnet run ..\%lib%\bin\Debug\net9.0\%lib%.dll ..\docs --source %repoUrl%/blob/main/%lib% --namespace %lib% --clean
endlocal