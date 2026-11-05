# Laboratorio 04 Aplicación web N tier

Isabella Hernández · Aplicaciones Web Avanzadas

Solución independiente de .NET Framework 4.8. Colocar `Lab04_WebAppNTier` junto a `Lab03_WebAppMonolitico` y `PoliMusic`.

## Abrir y ejecutar

1. Iniciar Docker Desktop y el contenedor existente `sql-server2025` (puerto de la PC `14333`), o utilizar una instancia propia de SQL Server.
2. Ejecutar `Database/01_CrearBaseLaboratorio.sql` desde SSMS. Crea únicamente `BDD_PoliMusic_Lab04`; el script puede ejecutarse de nuevo sin duplicar Adventure.
3. Abrir `SlnAppPoliMusic.slnx` o `SlnAppPoliMusic.sln` en Visual Studio 2026. Se requiere la carga de ASP.NET y desarrollo web y el targeting pack de .NET Framework 4.8.
4. Configurar `WebAppPoliMusicV3` como proyecto de inicio y `Default.aspx` como página de inicio. Usar IIS Express, HTTP, CLR v4.0, puerto `8084`.
5. Revisar la conexión denominada `BDD_PoliMusicConnectionString`. El paquete portable usa LocalDB con autenticación integrada. Para Docker, copiar `ConnectionStrings.local.config.example` a `ConnectionStrings.local.config`, completar los datos propios y usar `<connectionStrings configSource="ConnectionStrings.local.config" />` en Web.config. El archivo real está excluido de Git. La copia preparada en la PC usa ese archivo local cifrado con la protección de datos de Windows.
6. Compilar y ejecutar con Ctrl+F5. Sin sesión se presenta Login. Crear una cuenta desde SignUp, iniciar sesión y abrir Canciones desde Inicio o el menú.

También se puede compilar desde Windows PowerShell con `./Compilar.ps1` y ejecutar las pruebas aisladas con `./Verificacion/EjecutarPruebas.ps1`.

## Proyectos y referencias

| Proyecto | Responsabilidad | Referencias de proyecto |
| --- | --- | --- |
| EntityLayer.PoliMusicV3 | User, Constants y Hash | Ninguna |
| DAL.PoliMusicV3 | SQL parametrizado, CRUD de User y consulta de Song | EntityLayer |
| BLL.PoliMusicV3 | Validaciones de registro, duplicados y servicios | EntityLayer y DAL |
| WebAppPoliMusicV3 | Web Forms, Site.Master, autenticación y audio | EntityLayer y BLL |

Las cuatro DLL tienen destino v4.8. La presentación no referencia DAL ni abre conexiones SQL. `System.Data.Common.DbException` se usa únicamente para presentar errores de disponibilidad. Los datos recorren Web → BLL → DAL → SQL Server; las entidades se comparten entre las capas. La solución tiene la carpeta virtual `WebAppPoliMusicV3_ASPX_F4.8_NLAYER`.

`Site.Master` se comparte entre Inicio, Canciones, administración y Logout. Incluye la marca `PoliMusic - isabella.hernandez`, el usuario de sesión y Cerrar sesión. Las rutas `/Default`, `/Song`, `/Login`, `/SignUp` y `/Logout` usan el enrutamiento de Web Forms. `ScriptManager` y `UpdatePanel` permiten buscar sin recargar toda la página. El diseño utiliza CSS y JavaScript propios sin descargas externas durante la ejecución.

## Canciones y archivos

`Song.aspx` consulta TBL_SONG mediante BLL.Song y DAL.Song. El GridView permite buscar, limpiar el filtro, paginar y reproducir MP3. `Media.ashx` exige autenticación y sesión, comprueba la ruta dentro de SongFiles y admite rangos HTTP. La carpeta SongFiles se bloquea para solicitudes directas. Se conserva el MP3 Adventure disponible en los recursos previos. PLAYS refleja el valor de SQL; el laboratorio no incorpora un contador automático de reproducciones.

La foto de perfil es opcional; se usa `default.jpg` si no se adjunta una. Las fotos nuevas aceptan JPG/PNG reales de hasta 10 MB. Las fechas se validan en BLL y se normalizan para SQL. La compatibilidad SHA1 solicitada por la guía se conserva para este laboratorio; la utilidad PBKDF2 también se incluye.

## Entrega y evidencia

El informe, las capturas y los resultados verificados se guardan en `Entregables`. No se deben publicar archivos locales con contraseñas, `.user`, `.vs`, `packages`, `bin`, `obj` ni MDF/LDF. Los resultados ejecutados se distinguen de los casos pendientes.

La guía menciona GitHub al inicio y Azure DevOps al final. Se continúa con el repositorio del curso ya utilizado: `https://github.com/lalabell-a/AppWebAvIsabellaHernandez`. La evidencia de publicación se añadirá después de comprobar el push del laboratorio 04.
