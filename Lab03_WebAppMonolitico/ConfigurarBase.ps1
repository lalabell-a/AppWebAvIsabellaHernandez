param([string]$Servidor='(localdb)\MSSQLLocalDB')
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Data
$builder=New-Object System.Data.SqlClient.SqlConnectionStringBuilder
$builder['Data Source']=$Servidor; $builder['Initial Catalog']='master'; $builder['Integrated Security']=$true; $builder['Connect Timeout']=10
$connection=New-Object System.Data.SqlClient.SqlConnection($builder.ConnectionString)
try {
    $connection.Open()
    $sql=[IO.File]::ReadAllText((Join-Path $PSScriptRoot 'Database\01_CrearBaseLaboratorio.sql'))
    foreach($batch in [regex]::Split($sql,'(?im)^\s*GO\s*\r?$')) {
        if($batch.Trim().Length -eq 0) { continue }
        $cmd=$connection.CreateCommand(); $cmd.CommandText=$batch; $cmd.CommandTimeout=60
        [void]$cmd.ExecuteNonQuery(); $cmd.Dispose()
    }
} finally { $connection.Dispose() }
$builder['Initial Catalog']='BDD_PoliMusic_Lab03'
$configPath=Join-Path $PSScriptRoot 'WebAppPoliMusicV2\Web.config'
[xml]$config=[IO.File]::ReadAllText($configPath)
$section=$config.SelectSingleNode('/configuration/connectionStrings')
$section.RemoveAll()
$add=$config.CreateElement('add')
$add.SetAttribute('name','BDD_PoliMusicConnectionString')
$add.SetAttribute('connectionString',$builder.ConnectionString)
$add.SetAttribute('providerName','System.Data.SqlClient')
[void]$section.AppendChild($add)
$config.Save($configPath)
Write-Host 'BDD_PoliMusic_Lab03 creada y conexion configurada.' -ForegroundColor Green
