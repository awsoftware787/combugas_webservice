# Combugas WebService

Servicio web ASP.NET clásico (`.asmx`) desarrollado en C# y dirigido a **.NET Framework 4.8**. La solución contiene un único proyecto web y utiliza SQL Server mediante LINQ to SQL.

> No es un proyecto .NET moderno (SDK-style/.NET Core). Debe compilarse en Windows con las herramientas de Visual Studio para ASP.NET sobre .NET Framework.

## Requisitos

- Windows 10 u 11.
- Visual Studio 2019 o 2022 con la carga de trabajo **ASP.NET y desarrollo web**.
- **.NET Framework 4.8 SDK** y **Targeting Pack**. Pueden seleccionarse desde Visual Studio Installer, en **Componentes individuales**.
- NuGet (incluido en Visual Studio).
- Para ejecutar el servicio: IIS Express (incluido con la carga de trabajo web).
- Para usar las funciones que consultan datos: una instancia de SQL Server con la base de datos correspondiente y credenciales de acceso.
- Para enviar notificaciones SMS: una cuenta de Twilio con un Account SID y un Auth Token activos.

## Configuración para la primera compilación

### 1. Obtener el código

Clona el repositorio y entra en su directorio:

```powershell
git clone <URL_DEL_REPOSITORIO>
cd combugas_webservice
```

No muevas el proyecto fuera de esta estructura, ya que el archivo `.csproj` referencia los paquetes mediante rutas relativas a la carpeta `packages` de la raíz.

### 2. Instalar los componentes de Visual Studio

Abre **Visual Studio Installer**, selecciona **Modificar** sobre tu instalación y confirma que estén instalados:

1. La carga de trabajo **ASP.NET y desarrollo web**.
2. El SDK y Targeting Pack de **.NET Framework 4.8**.
3. IIS Express.

La carga de trabajo web instala también los targets de MSBuild que necesita este tipo de proyecto (`Microsoft.WebApplication.targets`).

### 3. Abrir la solución

Abre [ws_combugasclientes.sln](ws_combugasclientes.sln) con Visual Studio. Si Visual Studio pregunta si debe restaurar paquetes NuGet, acepta y espera a que termine.

También puedes forzar la restauración desde el **Explorador de soluciones** haciendo clic derecho sobre la solución y eligiendo **Restaurar paquetes NuGet**.

Como alternativa, desde una terminal donde esté disponible `nuget.exe`:

```powershell
nuget restore .\ws_combugasclientes.sln
```

La restauración debe dejar disponibles, entre otros, `Microsoft.Net.Compilers`, `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` y los paquetes de Application Insights dentro de `packages\`.

### 4. Compilar

En Visual Studio:

1. Selecciona **Debug** y **Any CPU**.
2. Ejecuta **Compilar > Compilar solución** (`Ctrl+Mayús+B`).
3. Comprueba que el resultado indique `0 errores`.

La salida se genera en `ws_combugasclientes\bin\`.

Desde una **Developer PowerShell for Visual Studio** también se puede compilar con:

```powershell
msbuild .\ws_combugasclientes.sln /restore /p:Configuration=Debug /p:Platform="Any CPU"
```

Si `/restore` no restaura los paquetes de `packages.config` en tu versión de MSBuild, ejecuta primero `nuget restore` como se indica en el paso anterior.

## Configuración para la primera ejecución

La base de datos no es necesaria para que MSBuild genere el ensamblado, pero sí para utilizar los métodos del servicio que acceden a datos.

### 1. Crear la configuración privada local

Los datos de cada entorno no se guardan directamente en [Web.config](ws_combugasclientes/Web.config). Ese archivo carga dos archivos locales mediante `configSource`:

- `AppSettings.local.config`: URL, puerto, claves de Maps y Twilio, y opciones del sitio.
- `ConnectionStrings.local.config`: conexiones de SQL Server.

Estos archivos están excluidos por `.gitignore` y deben entregarse al desarrollador por un medio seguro. Para crear una configuración nueva a partir de las plantillas:

```powershell
Copy-Item .\ws_combugasclientes\AppSettings.example.config .\ws_combugasclientes\AppSettings.local.config
Copy-Item .\ws_combugasclientes\ConnectionStrings.example.config .\ws_combugasclientes\ConnectionStrings.local.config
```

Después reemplaza todos los valores `REEMPLAZAR_*`. No agregues contraseñas, tokens, rutas ni URLs privadas a los archivos `*.example.config`.

### 2. Configurar Twilio

El servicio utiliza Twilio para enviar notificaciones SMS desde `Operadores.asmx.cs`. Obtén un **Account SID** y un **Auth Token** activos desde la consola de la cuenta de Twilio y agrégalos únicamente a `ws_combugasclientes\AppSettings.local.config`:

```xml
<add key="TWILIO_ACCOUNT_SID" value="TU_ACCOUNT_SID" />
<add key="TWILIO_AUTH_TOKEN" value="TU_AUTH_TOKEN" />
```

El archivo debe conservar un solo elemento raíz `<appSettings>` y ambas entradas deben quedar dentro de él. Si creaste el archivo copiando `AppSettings.example.config`, reemplaza los valores `REEMPLAZAR_ACCOUNT_SID` y `REEMPLAZAR_AUTH_TOKEN`.

Estas credenciales no deben escribirse directamente en archivos `.cs`, `Web.config`, archivos de ejemplo ni otros archivos versionados. `AppSettings.local.config` está excluido por `.gitignore`; compruébalo antes de hacer commit con:

```powershell
git check-ignore -v .\ws_combugasclientes\AppSettings.local.config
```

Si un token aparece accidentalmente en Git, rótalo inmediatamente desde Twilio y elimina el valor de todos los commits afectados. Permitir el secreto mediante el enlace de desbloqueo de GitHub no sustituye la rotación.

Después de cambiar las credenciales, reinicia IIS Express y prueba una operación que envíe un SMS. No reutilices el Auth Token que originó una alerta de GitHub; genera y configura uno nuevo.

### 3. Configurar SQL Server

Edita `ws_combugasclientes\ConnectionStrings.local.config`. Como mínimo, revisa las conexiones principales:

- `combugasCCConnectionString`
- `combugasWSConnectionString`

Ejemplo con autenticación integrada de Windows:

```xml
<add name="combugasCCConnectionString"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=combugasCC;Integrated Security=True;Connect Timeout=600"
     providerName="System.Data.SqlClient" />
```

Usa el nombre real de tu instancia y base de datos. La identidad con la que se ejecute IIS Express debe tener permisos en SQL Server. Si se utiliza autenticación SQL, las credenciales deben existir únicamente en el archivo local.

> El repositorio no incluye scripts para crear o poblar la base de datos. Debes solicitar un respaldo o los scripts de esquema/datos al responsable del proyecto.

### 4. Alinear la URL y el puerto local

El proyecto declara actualmente la URL de IIS Express `http://localhost:52665/` en el archivo `.csproj`. Configura los valores correspondientes en `AppSettings.local.config`:

```xml
<add key="URLSITIO" value="http://localhost:" />
<add key="PUERTOSITIO" value="51201" />
```

Antes de probar operaciones que construyan URLs hacia el propio servicio, haz que ambos puertos coincidan. Puedes cambiar `PUERTOSITIO` al puerto asignado por Visual Studio o modificar la URL del proyecto desde **Propiedades > Web**.

### 5. Iniciar y comprobar el servicio

1. Haz clic derecho en `ws_combugasclientes` y selecciona **Establecer como proyecto de inicio**.
2. Inicia con **IIS Express** (`F5` o `Ctrl+F5`).
3. Abre uno de los endpoints ASMX, por ejemplo:

```text
http://localhost:<PUERTO>/ws/clientes.asmx
```

Si aparece la página de descripción del servicio y su lista de operaciones, IIS Express y ASP.NET están funcionando. Las llamadas que consulten datos requerirán además una conexión válida a SQL Server.

## Problemas frecuentes

### Faltan paquetes NuGet

Errores que mencionan archivos `.props`, `.targets` o DLL dentro de `packages\` suelen indicar una restauración incompleta. Cierra Visual Studio, ejecuta:

```powershell
nuget restore .\ws_combugasclientes.sln
```

Después vuelve a abrir la solución y ejecuta **Limpiar solución** y **Recompilar solución**.

### No se encuentra `Microsoft.WebApplication.targets`

Instala o repara la carga de trabajo **ASP.NET y desarrollo web** desde Visual Studio Installer. Compilar únicamente con el SDK de `dotnet` no es suficiente para este proyecto clásico.

### No se encuentra .NET Framework 4.8

Instala el **Developer Pack/Targeting Pack de .NET Framework 4.8**. El runtime por sí solo puede no incluir las referencias necesarias para compilar.

### Error de conexión a SQL Server

Verifica que:

- el servicio de SQL Server esté iniciado;
- el nombre de servidor e instancia sea correcto;
- la base de datos exista;
- la autenticación indicada por la cadena esté habilitada; y
- el usuario tenga permisos sobre la base.

### Twilio no envía mensajes

Comprueba que:

- `TWILIO_ACCOUNT_SID` y `TWILIO_AUTH_TOKEN` existan en `AppSettings.local.config` y no conserven valores `REEMPLAZAR_*` ni valores vacíos;
- el Auth Token siga activo y corresponda al mismo Account SID;
- la cuenta y el número remitente de Twilio estén habilitados para el destino; y
- IIS Express se haya reiniciado después de actualizar el archivo de configuración.

Un error de autenticación requiere configurar credenciales válidas; nunca copies el token al código fuente para intentar resolverlo.

### El endpoint no abre o usa un puerto incorrecto

Confirma el puerto que muestra IIS Express y alinéalo con `PUERTOSITIO` en `AppSettings.local.config`. Revisa también que `ws_combugasclientes` sea el proyecto de inicio.

## Archivos principales

- `ws_combugasclientes.sln`: solución de Visual Studio.
- `ws_combugasclientes/ws_combugasclientes.csproj`: proyecto web y referencias de compilación.
- `ws_combugasclientes/packages.config`: dependencias NuGet.
- `ws_combugasclientes/Web.config`: configuración compartida de ASP.NET y referencias a la configuración privada.
- `ws_combugasclientes/*.example.config`: plantillas seguras que sí pueden guardarse en Git.
- `ws_combugasclientes/*.local.config`: valores privados de cada entorno; están ignorados por Git.
- `ws_combugasclientes/ws/`: endpoints `.asmx`.
