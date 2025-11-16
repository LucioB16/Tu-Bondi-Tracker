# TuBondi.ApiClient

Biblioteca tipo class library que encapsula los endpoints publicados por Tu Bondi usando .NET 10. Toda la superficie está completamente documentada con XML docs en español e incluye convertidores para formatos JSON inestables.

## Requisitos

- .NET SDK 10.0.100 o superior.

## Instalación

1. Clonar este repositorio.
2. Abrir la solución `TuBondi.sln` en Visual Studio / Rider / VS Code.
3. Referenciar el proyecto `TuBondi.ApiClient` desde otras capas de la arquitectura limpia o empaquetarlo con `dotnet pack`.

## Uso rápido

```csharp
var options = new TuBondiApiOptions();
using var client = new TuBondiClient(options);
await client.InitializeSessionAsync();
var lines = await client.GetLinesAndRoutesAsync();
var bounds = await client.GetViewBoundsAsync();
var traza = await client.SelectRouteTraceAsync(rutaId: 99, clienteId: 411);
var coches = await client.QueryVehiclesByRouteAsync(rutaId: 99, clienteId: 411, stopCode: "A911");
var arrivals = await client.GetArrivalsAsync(stopCode: "A911");
```

## Notas sobre payloads reales

- Las trazas y desvíos se modelan con arreglos posicionales: `TrazaPoint` espera `[lon, lat, course]` y cada `GeoPoint` de los desvíos usa `[lon, lat]`.
- El arreglo `a` en `Arribo` siempre contiene cuatro dobles (`lon1`, `lat1`, `lon2`, `lat2`) que se mantienen sin alterar para cálculos posteriores.
- `VehiclesByRouteResponse.Error` puede ser `null` o una lista de cadenas, y `demora_minutos` utiliza el centinela `99999` para indicar que la demora es desconocida.
- Varias magnitudes llegan como cadenas o números indistintamente (lat/lon, cursos, distancias y tiempos), por lo que se proveen `FlexibleDoubleConverter`/`FlexibleIntConverter` para asegurar la deserialización.
- Campos como `horaTeoricaAjustada`, `horaTeorica`, `demora` y `notificacion` son opcionales y pueden omitirse o llegar vacíos según la línea consultada.

## Endpoints cubiertos

- `GET /web/urbano/?conf=...` (sesión inicial y cookie `PHPSESSID`).
- `POST /usuario/urbano2_cmd.php?cmd=lineasyrutas`.
- `POST /usuario/urbano2_cmd.php?cmd=vista`.
- `POST /usuario/urbano2_cmd.php?cmd=seleccionatraza`.
- `POST /usuario/urbano2_cmd.php?cmd=consultacocheporruta`.
- `POST /usuario/urbano2_cmd.php` con `cmd=proximos_arribos`.

## Autenticación / Sesión

El método `InitializeSessionAsync` ejecuta la navegación inicial para capturar `PHPSESSID`. Todas las llamadas posteriores reutilizan el mismo `CookieContainer`. Use la propiedad `CurrentPhpSessionId` para diagnóstico.

## Opciones disponibles

- `BaseAddress`: host base de la API (por defecto `https://micronauta4.dnsalias.net/`).
- `Conf`: configuración de ciudad enviada en los formularios (`cbaciudad`).
- `Timeout`: tiempo máximo por request (10 segundos predeterminado).
- `UserAgent`: cabecera enviada en cada request para identificar la integración.

## Errores comunes

- Invocar cualquier comando sin haber llamado `InitializeSessionAsync` produce HTTP 302/403 porque falta `PHPSESSID`.
- Respuestas `4xx/5xx` incluyen un snippet del cuerpo para facilitar el diagnóstico.
- Timeouts o cancelaciones propagan `TaskCanceledException` debido a la configuración de `HttpClient`.

## Roadmap

- Implementar capas Domain/Application/Infrastructure con casos de uso reales.
- Agregar Web API en `TuBondi.Presentation.WebApi` exponiendo endpoints REST propios.
- Incorporar proyectos de pruebas automatizadas.

## Validación manual

Se ejecutó localmente un snippet equivalente al mostrado en "Uso rápido", verificando que `InitializeSessionAsync()` y `GetLinesAndRoutesAsync()` completaran sin excepciones y que `lines.Lineas` contuviera elementos (indicando sesión válida).
