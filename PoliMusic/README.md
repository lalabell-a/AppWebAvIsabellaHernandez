# PoliMusic

React + TypeScript + Vite + Tailwind, Lucide, Motion, TanStack Query y React Router. API de referencia ASP.NET Core 8 + EF Core + SQL Server, separada en Domain, Application, Infrastructure y Api.

## Estado real de la entrega

La web publicada funciona en **modo demostración local**. Se basa en los registros y relaciones del SQL adjunto. Las playlists y el perfil se guardan en el navegador; no se comparten ni escriben en SQL Server. El acceso y el registro necesitan la API. Nunca se guardan contraseñas en localStorage.

El ZIP recibido contiene solamente `bensound-adventure.mp3`, imágenes de usuario y de playlists. Adventure se reproduce realmente; las otras cuatro canciones requieren sus MP3. El selector de audio permite asociar un archivo durante la sesión; no lo sube al servidor. Las imágenes de álbumes y artistas no venían en el ZIP: se crearon composiciones editoriales con paisajes de Unsplash, indicadas como ambientación, no retratos reales. No se inventa un artista principal de álbum.

API y SQL Server **no se ejecutaron ni compilaron** en este entorno: no existe dotnet ni una instancia de SQL Server. No se verificó visualmente en un navegador por no disponer de la habilidad de control de navegador requerida por Sites. TypeScript y Vite compilaron correctamente. Se renderizaron en servidor las 12 rutas principales sin errores (no sustituye una prueba interactiva). Se comprobaron las asociaciones de las semillas y la integridad/duración del MP3 (187.115 s).

## Ejecutar el frontend

Requisitos: Node 20.19+ o 22.12+.

```bash
npm ci
cp .env.example .env
npm run dev
```

`npm run build` genera `dist/`. El hosting debe resolver rutas de SPA hacia `index.html` (álbumes, playlists, perfil, etc.). `VITE_API_URL` vacío activa la demo. Para integración, configura el origen HTTPS de la API y vuelve a compilar.

## Preparar SQL Server

1. Conserva una copia de seguridad de cualquier BD existente. **No ejecutes el script original completo**: contiene `DROP DATABASE`, contraseña del login y privilegio `db_owner`.
2. Si la base ya existe, omite `000_new_database_only.sql`. Si es una instalación nueva, crea una base vacía `BDD_PoliMusic` y ejecuta ese archivo una sola vez. No crea logins ni elimina bases.
3. Revisa y ejecuta `001_security.sql`: amplía PASSWORD a varchar(255), detecta duplicados, crea índices únicos y restricciones de tipos. No cambia contraseñas existentes.
4. Revisa y ejecuta `002_play_events.sql`: recibos con ID único y fecha de inicio para evitar incrementos repetidos de PLAYS.
5. Usa una cuenta de servicio con permisos mínimos sobre tablas necesarias. No utilices db_owner. Configura el secreto de conexión fuera del repositorio.
6. Las cuentas de ejemplo originales (texto plano o SHA-1) son rechazadas intencionalmente. Registra una cuenta nueva. Un DBA autorizado puede asignarle `USER_TYPE=1` para administrar; no existe autoasignación de rol en el registro. El restablecimiento por correo no está implementado.

## Ejecutar la API

Requisitos: .NET SDK 8, SQL Server y un certificado HTTPS confiable. Restaura dependencias y compila antes de usar; esta parte necesita validación en tu máquina.

```bash
dotnet dev-certs https --trust
dotnet restore backend/PoliMusic.Api/PoliMusic.Api.csproj
dotnet build backend/PoliMusic.Api/PoliMusic.Api.csproj
dotnet run --project backend/PoliMusic.Api/PoliMusic.Api.csproj
```

Configura estas variables del proceso (ver backend/.env.example; .NET no carga automáticamente ese archivo):

- `ConnectionStrings__Music`: cadena de conexión privada de SQL Server; valida el certificado del servidor.
- `FrontendOrigin`: origen exacto HTTPS del frontend, sin barra final. Solo se aceptan mutaciones desde este origen.
- `MediaRoot`: ruta absoluta a `public/resources` en el servidor.
- `ASPNETCORE_URLS`: por ejemplo `https://localhost:7043`.
- `ASPNETCORE_ENVIRONMENT=Development` habilita `/swagger`.

Para desarrollo integrado usa HTTPS en el frontend mediante un proxy local con certificado confiable (por ejemplo el servidor HTTPS de tu IDE); configura su origen exacto en CORS. Las cookies son Secure, HttpOnly y SameSite=None. El navegador podría bloquear cookies de terceros si frontend y API están en sitios diferentes: en producción usa un mismo sitio o proxy `/api`. Nunca desactives estas protecciones como solución.

La API se aloja por separado: Sites publica la web React, no ejecuta ASP.NET ni conecta por TCP a SQL Server. Una conexión local de tu laptop no es accesible desde otros dispositivos; para integrar en producción despliega la API HTTPS y configura `VITE_API_URL`.

## Mapeo funcional

| Función | Datos |
|---|---|
| Canciones y clasificación | TBL_SONG, PLAYS |
| Álbumes y canciones | TBL_ALBUM + TBL_TRACKLIST |
| Artistas y canciones | TBL_ARTIST + TBL_PLAYER |
| Playlists propias | TBL_PLAYLIST, ID_USER |
| Privacidad | TYPE: 0 pública, 1 privada, según TBL_CATALOG |
| Contenido de playlist | TBL_PLAYLIST_SONG |
| Perfil y rol | TBL_USER; USER_TYPE 0/1 |

No hay historial individual, favoritos ni duración en el esquema original. La duración se lee del archivo de audio. Se omitió “Continuar escuchando” al no haber historial. El catálogo de tipos no tiene FK desde los campos enteros, por eso la migración usa CHECK para los valores definidos.

## Seguridad implementada en el código de API

Hash de contraseña mediante PasswordHasher de ASP.NET Identity, validación de DTOs, cookies seguras, CORS de origen único y verificación de Origin para mutaciones, límite de solicitudes, consultas EF parametrizadas, restricciones de propietario/administrador en playlists y rutas administrativas con Authorize. Los DTOs no exponen contraseñas ni hashes. Las rutas de audio se resuelven debajo de MediaRoot y soportan range requests.

PLAYS usa un recibo autenticado emitido por el servidor, espera de 30 segundos y consumo único con transacción. Esto evita doble incremento del mismo recibo; no es una solución antifraude contra automatización deliberada. El frontend demo muestra incrementos de sesión sin afirmar que modifican SQL Server.

## Pendientes y límites

- Compilar, ejecutar y probar la API con SQL Server; validar login, restricciones por rol, CORS/cookies y concurrencia con dos usuarios.
- Revisar diseño en escritorio y móvil, navegación de teclado y contraste con una auditoría de accesibilidad. No se certifica WCAG.
- Cargar Dreams, Energy, Scifi y Tomorrow en `resources/songfiles/` con los nombres del SQL. Añadir imágenes reales de álbumes/artistas si se desean.
- La administración implementa lectura/creación/edición/eliminación de canciones, álbumes y artistas, y asociaciones. Usuarios: lectura y cambio de rol; el registro crea usuarios. No incluye eliminación de usuarios ni gestión global de playlists desde el panel.
- El panel usa IDs para relaciones; sería conveniente sustituirlos por selectores consultables para un catálogo grande. Búsqueda y paginación son del lado del cliente.
- El registro de escucha cuenta una vez por carga del audio; la repetición del mismo archivo no cuenta nuevamente hasta una nueva selección.
- Añadir carga de archivos al servidor, restablecimiento de contraseña por correo, rotación/revocación de sesiones después de cambios de rol, limpieza de recibos antiguos y pruebas de integración antes de producción.
- Rutas heredadas de imágenes de perfil/playlists requieren normalización si se devuelven desde una BD original. La demo ya utiliza las rutas normalizadas de los recursos adjuntos.

## Imágenes

Paisajes descargados de images.unsplash.com: photo-1470252649378-9c29740c9fa8, photo-1464822759023-fed622ff2c3b, photo-1519681393784-d120267933ba, photo-1500530855697-b586d89ba3ee y photo-1470770841072-f978cf4d019e. Las imágenes y el MP3 proporcionados por el usuario se mantienen como recursos del proyecto; verifica tus permisos de uso antes de difusión pública.
