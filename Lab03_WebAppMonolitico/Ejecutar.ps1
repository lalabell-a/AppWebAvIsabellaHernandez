param([int]$Puerto=8083)
$ErrorActionPreference='Stop'
& (Join-Path $PSScriptRoot 'Compilar.ps1')
$iis=Join-Path ${env:ProgramFiles} 'IIS Express\iisexpress.exe'
if(!(Test-Path -LiteralPath $iis)) { throw 'Instala IIS Express con la carga de trabajo ASP.NET de Visual Studio.' }
$web=Join-Path $PSScriptRoot 'WebAppPoliMusicV2'
Write-Host ('Abrir http://localhost:'+$Puerto+'/Login.aspx') -ForegroundColor Green
& $iis ("/path:"+$web) ("/port:"+$Puerto) /clr:v2.0
