# Laboratorio 03 - Web Forms monolítico

**Estudiante:** Isabella Hernández · Aplicaciones Web Avanzadas

Este laboratorio es independiente de `PoliMusic`. Coloca la carpeta `Lab03_WebAppMonolitico` junto a `PoliMusic`, nunca dentro de ella.

## Contenido

- `SlnAppPoliMusic.slnx` y `.sln`: solución para Visual Studio.
- `WebAppPoliMusicV2`: aplicación ASP.NET Web Forms, C# 2.0, .NET Framework 2.0, un proyecto y un ensamblado.
- `Entities`, `DAL`, `BLL`, `Util`: organización interna del mismo proyecto, conforme a la guía.
- `Login.aspx`, `SignUp.aspx`, `Logout.aspx`, `Default.aspx`, `Song.aspx`, `DesktopAdmin.aspx`.
- `Database/01_CrearBaseLaboratorio.sql`: base exclusiva `BDD_PoliMusic_Lab03`, con las columnas originales de `TBL_USER` y `TBL_SONG`. Usa el mismo nombre lógico de conexión de la guía.
- `Media.ashx`: entrega el MP3 con autenticación y peticiones parciales para avanzar y retroceder.

## Preparación en tu PC

1. Habilita **.NET Framework 3.5 (incluye 2.0 y 3.0)**. Instala la carga de trabajo **ASP.NET y desarrollo web** de Visual Studio y SQL Server/LocalDB.
2. Abre `SlnAppPoliMusic.slnx` (o `.sln`). Mantén el destino **.NET Framework 2.0**; no conviertas a ASP.NET Core.
3. En SQL Server Management Studio, conéctate a tu instancia y ejecuta `Database/01_CrearBaseLaboratorio.sql`. Si la instancia admite autenticación integrada de Windows, también puedes ejecutar `ConfigurarBase.ps1` desde Windows PowerShell con el parámetro `-Servidor`. Para la instancia de Docker con autenticación SQL, usa SSMS y configura la conexión con tus propios datos como se indica más abajo.
4. Revisa `WebAppPoliMusicV2/Web.config`: `Data Source` debe coincidir con tu instancia y `Initial Catalog` debe ser `BDD_PoliMusic_Lab03`. En el ZIP, el valor predeterminado usa `(localdb)\MSSQLLocalDB` y autenticación integrada. En la copia de GitHub, crea `ConnectionStrings.local.config` a partir del archivo `.example`, como se indica más abajo.
5. Establece `WebAppPoliMusicV2` como proyecto de inicio y `Default.aspx` como página de inicio. En IIS Express utiliza **CLR v2.0**, sin HTTPS, como pide la guía.
6. Compila y ejecuta. También puedes ejecutar `Compilar.ps1 -Precompilar` y luego `Ejecutar.ps1`.
7. Regístrate desde `SignUp.aspx` y entra con esa cuenta. No hay contraseñas ni usuarios de prueba incorporados.

## Pruebas funcionales que deben ejecutarse con SQL Server

| Caso | Acción | Resultado esperado |
| --- | --- | --- |
| Acceso protegido | Abrir `Song.aspx` sin sesión | Redirección a Login |
| Login vacío | Enviar sin usuario/contraseña | Mensaje de campos requeridos |
| Login incorrecto | Credenciales no válidas | Mensaje sin crear sesión |
| Registro | Crear usuario válido | Confirmación e inserción en TBL_USER |
| Duplicados | Repetir usuario o correo | Rechazo sin nueva cuenta |
| Confirmación | Contraseñas diferentes | Rechazo |
| Fecha | Ingresar 31/02/2000 o una fecha futura | Rechazo |
| Foto | Subir un archivo que no sea JPG/PNG válido | Rechazo |
| Inicio correcto | Entrar con la cuenta creada | Default → Song y tabla desde SQL |
| Buscar | Buscar `Adventure` y luego un título inexistente | Coincidencia y estado vacío |
| Audio | Play, pausa, volumen y desplazamiento | Reproducción del MP3 local |
| Cierre de sesión | Salir y volver a Song | Login; no conservar acceso |

## Audio y datos

Se copió `bensound-adventure.mp3` de los recursos ya disponibles. No se inventaron archivos para las otras canciones del ejemplo. Para añadirlas, copia sus MP3 a `SongFiles` e inserta sus rutas en `TBL_SONG`. La tabla muestra `PLAYS` guardado en SQL; reproducir no incrementa ese campo, pues la guía solo exige consultar y reproducir.

La guía usa SHA1 para las contraseñas. Se conserva esa compatibilidad académica y se incluye la utilidad PBKDF2 sugerida, sin cambiar el formato de los datos de la práctica. Esta solución se destina al laboratorio local.

## Repositorio

Publicado en la rama `main`: [Lab03_WebAppMonolitico](https://github.com/lalabell-a/AppWebAvIsabellaHernandez/tree/main/Lab03_WebAppMonolitico).

Commit de publicación del código: [`122135e`](https://github.com/lalabell-a/AppWebAvIsabellaHernandez/commit/122135e2fb7dda0892c1516f512f11f2ea282a7e). Se verificaron 59 archivos añadidos exclusivamente dentro del laboratorio, sin cambios en `PoliMusic`. La evidencia real está en `Entregables/GitHub_Publicacion.png` y en el informe PDF. La documentación se actualiza después de esa publicación inicial.

## Ejecución verificada en esta PC

La instancia utilizada en la prueba fue `127.0.0.1,14333`, con la base independiente `BDD_PoliMusic_Lab03`. Se verificaron el registro, el inicio de sesión, las validaciones, la consulta de Adventure, la búsqueda, la reproducción y el cierre de sesión. La cuenta de prueba se creó desde el formulario como `isabella.hernandez`.

La copia instalada en tu PC usa `Web.config` con `configSource="ConnectionStrings.local.config"`. Ese archivo contiene la conexión cifrada con la protección de datos de Windows y está excluido de Git. No se incluye en el ZIP y no se puede reutilizar en otra PC. La aplicación continúa usando el nombre lógico de conexión exigido por la guía.

El ZIP incluye una configuración portable de LocalDB. Para usar otra instancia SQL Server, copia `ConnectionStrings.local.config.example` como `ConnectionStrings.local.config`, coloca tus propios datos y sustituye la sección de conexión de `Web.config` por `<connectionStrings configSource="ConnectionStrings.local.config" />`. El ejemplo se destina a una instancia local de desarrollo con certificado propio. No publiques el archivo real con credenciales.

## Estado de validación

Consultar `Entregables/Informe_Laboratorio03.pdf` y `Entregables/validacion.json`. Los casos pendientes de ejecución no se presentan como aprobados. La validación del compilador, de las páginas y de SQL se registra por separado.
