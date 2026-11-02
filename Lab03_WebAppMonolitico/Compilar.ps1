param([switch]$Precompilar)
$ErrorActionPreference='Stop'
$web=Join-Path $PSScriptRoot 'WebAppPoliMusicV2'
$framework=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v2.0.50727'
if(!(Test-Path -LiteralPath $framework)) { $framework=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v2.0.50727' }
$compiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if(!(Test-Path -LiteralPath $compiler)) { $compiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if(!(Test-Path -LiteralPath $compiler)) { throw 'Habilita .NET Framework 3.5 (incluye 2.0 y 3.0) en Caracteristicas de Windows.' }
New-Item -ItemType Directory -Path (Join-Path $web 'bin') -Force | Out-Null
$sources=Get-ChildItem -LiteralPath $web -Recurse -Filter '*.cs' | Where-Object {$_.FullName -notmatch '\\(bin|obj)\\'} | ForEach-Object {'"'+$_.FullName+'"'}
$refs=@('System.dll','System.Configuration.dll','System.Data.dll','System.Drawing.dll','System.Web.dll','System.Xml.dll') | ForEach-Object {'/reference:"'+(Join-Path $framework $_)+'"'}
$response=Join-Path $PSScriptRoot '.compile.rsp'
$argsList=@('/nologo','/target:library','/warn:4','/nostdlib+','/langversion:ISO-2',('/reference:"'+(Join-Path $framework 'mscorlib.dll')+'"'),('/out:"'+(Join-Path $web 'bin\WebAppPoliMusicV2.dll')+'"'))+$refs+$sources
[IO.File]::WriteAllLines($response,$argsList,(New-Object Text.UTF8Encoding($false)))
& $compiler /noconfig ('@'+$response)
if($LASTEXITCODE -ne 0) { throw 'La compilacion C# 2.0 fallo.' }
if($Precompilar) {
    & (Join-Path $framework 'aspnet_compiler.exe') -v / -p $web
    if($LASTEXITCODE -ne 0) { throw 'La validacion de las paginas ASP.NET fallo.' }
}
Write-Host 'Compilacion .NET Framework 2.0 correcta.' -ForegroundColor Green
