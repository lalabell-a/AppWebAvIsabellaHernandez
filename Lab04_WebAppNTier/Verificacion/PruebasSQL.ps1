param([Parameter(Mandatory=$true)][string]$ConnectionString)
$ErrorActionPreference='Stop'
$bin=Join-Path (Split-Path $PSScriptRoot -Parent) 'WebAppPoliMusicV3\bin'
Add-Type -Path (Join-Path $bin 'EntityLayer.PoliMusicV3.dll')
Add-Type -Path (Join-Path $bin 'DAL.PoliMusicV3.dll')
Add-Type -Path (Join-Path $bin 'BLL.PoliMusicV3.dll')
$service=[BLL.PoliMusicV3.User]::new($ConnectionString)
$tag=[Guid]::NewGuid().ToString('N').Substring(0,10)
$user=[EntityLayer.PoliMusicV3.User]::new()
$user.UserName='qa04_'+$tag; $user.Email='qa04_'+$tag+'@example.test'
$user.Password=[EntityLayer.PoliMusicV3.Util.Hash]::GeneratePasswordHash('SoloPrueba_'+$tag)
$user.Birthday='2000-02-29'; $user.UserType=0; $user.Photo='~/UserImages/default.jpg'
$script:count=0
function Check([bool]$result,[string]$name){if(!$result){throw ('FALLO: '+$name)}; $script:count++; Write-Output ('APROBADA: '+$name)}
$id=0
try {
 Check ($service.Save($user) -eq 1) 'BLL y DAL insertan usuario con SQL'
 $id=$service.CheckExistUser($user.UserName); $user.ID=$id
 Check ($id -gt 0) 'Consulta de usuario por nombre'
 Check ($service.CheckExistEmail($user.Email) -eq $id) 'Consulta de usuario por correo'
 Check ($service.ValidationsDuplicated($user).Length -gt 0) 'BLL detecta duplicados'
 $rows=$service.AuthenticateUser($user.UserName,$user.Password)
 Check ($rows.Rows.Count -eq 1) 'Autenticación válida con SQL'
 Check ($service.AuthenticateUser($user.UserName,'incorrecta').Rows.Count -eq 0) 'Autenticación inválida con SQL'
 $user.Email='actualizado_'+$tag+'@example.test'; $savedPassword=$user.Password; $user.Password=''
 Check ($service.Update($user) -eq 1) 'Actualizar usuario conservando contraseña'
 Check ($service.AuthenticateUser($user.UserName,$savedPassword).Rows.Count -eq 1) 'Contraseña permanece válida tras actualizar'
 Check (!$service.Read().Columns.Contains('PASSWORD')) 'Lectura no expone contraseñas'
 $songs=[BLL.PoliMusicV3.Song]::new($ConnectionString)
 Check ($songs.Read('Adventure').Rows.Count -eq 1) 'Canción consultada mediante BLL y DAL'
 Check ($songs.Read("' OR 1=1 --").Rows.Count -eq 0) 'Filtro SQL parametrizado'
 Check ($songs.Read('%').Rows.Count -eq 0) 'LIKE trata porcentaje como literal'
 Check ($songs.Read('_').Rows.Count -eq 0) 'LIKE trata guion bajo como literal'
 Check ($null -eq $songs.ReadPath(0)) 'BLL rechaza identificador de audio inválido'
 Check ($songs.ReadPath(1) -eq '~/SongFiles/bensound-adventure.mp3') 'Ruta del audio desde SQL'
 Check ($service.Delete($id) -eq 1) 'Eliminar exclusivamente el usuario temporal de prueba'
 Check ($service.CheckExistUser($user.UserName) -eq 0) 'Confirmar eliminación del usuario temporal'
 $id=0
 Write-Output ('TOTAL: '+$script:count+' pruebas SQL aprobadas.')
} finally { if($id -gt 0){[void]$service.Delete($id)} }
