$ErrorActionPreference='Stop'
$vswhere=Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if(!(Test-Path -LiteralPath $vswhere)){throw 'Instala Visual Studio con ASP.NET y desarrollo web y el targeting pack .NET Framework 4.8.'}
$msbuild=& $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if(!$msbuild){throw 'No se encontró MSBuild.'}
& $msbuild (Join-Path $PSScriptRoot 'SlnAppPoliMusic.sln') /p:Configuration=Release /v:minimal /nologo
if($LASTEXITCODE -ne 0){throw 'Falló la compilación de la solución.'}
