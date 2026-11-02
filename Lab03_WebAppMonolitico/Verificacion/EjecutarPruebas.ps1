$ErrorActionPreference='Stop'
$lab=Split-Path $PSScriptRoot -Parent
& (Join-Path $lab 'Compilar.ps1')
$dll=Join-Path $lab 'WebAppPoliMusicV2\bin\WebAppPoliMusicV2.dll'
$runner=Join-Path $PSScriptRoot 'PruebasCodigo.exe'
Copy-Item -LiteralPath $dll -Destination $PSScriptRoot -Force
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:exe ("/out:"+$runner) ("/reference:"+$dll) /reference:System.Web.dll (Join-Path $PSScriptRoot 'PruebasCodigo.cs')
if($LASTEXITCODE -ne 0) { throw 'No se pudo compilar el ejecutor de pruebas.' }
& $runner
if($LASTEXITCODE -ne 0) { throw 'Una prueba ha fallado.' }
