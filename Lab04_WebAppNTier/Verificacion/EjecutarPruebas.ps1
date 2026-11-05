$ErrorActionPreference='Stop'
$lab=Split-Path $PSScriptRoot -Parent
& (Join-Path $lab 'Compilar.ps1')
$bin=Join-Path $lab 'WebAppPoliMusicV3\bin'
Get-ChildItem -LiteralPath $bin -Filter '*.dll' | Copy-Item -Destination $PSScriptRoot -Force
$csc=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$runner=Join-Path $PSScriptRoot 'PruebasCapas.exe'
& $csc /nologo ("/out:"+$runner) ("/reference:"+(Join-Path $bin 'EntityLayer.PoliMusicV3.dll')) ("/reference:"+(Join-Path $bin 'BLL.PoliMusicV3.dll')) (Join-Path $PSScriptRoot 'PruebasCapas.cs')
if($LASTEXITCODE -ne 0){throw 'Falló la compilación del ejecutor de pruebas.'}
& $runner
if($LASTEXITCODE -ne 0){throw 'Una prueba falló.'}
