# Importación local del paquete completo Cozy Farm

## Motivo

El paquete completo comprado incluye recursos con licencia de uso para proyectos,
pero no permite redistribuir el asset pack. Por ello, las hojas fuente se importan
en cada copia local del proyecto y se mantienen fuera del control de versiones
mientras el repositorio sea accesible públicamente.

Página del autor y licencia publicada:

https://shubibubi.itch.io/cozy-farm

## Contenido detectado en `full version.zip`

- 63 hojas PNG utilizables por Unity.
- 344 GIF de previsualización de animaciones.
- 3 archivos de texto con información del paquete.

Los PNG contienen las hojas fuente. Los GIF se conservan como referencia visual,
pero no se colocan dentro de `Assets`, para evitar que Unity importe cientos de
previsualizaciones redundantes.

## Importación

1. Actualizar la rama del proyecto y abrir Unity.
2. Esperar a que termine de compilar.
3. Ejecutar:

```text
Tools > Farm Simulator > Import Full Cozy Farm Pack...
```

4. Seleccionar el archivo comprado `full version.zip`.
5. Esperar a que finalice la barra de progreso.

Las hojas PNG quedarán disponibles en:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Full
```

Las vistas GIF se conservarán fuera de Unity en:

```text
LocalAssets/CozyFarm/Previews
```

## Ajustes aplicados a los PNG

- Texture Type: Sprite.
- Sprite Mode: Single.
- Pixels Per Unit: 16.
- Filter Mode: Point.
- Mip Maps: desactivados.
- Wrap Mode: Clamp.
- Compression: Uncompressed.
- Max Size: 4096, para conservar `global.png` sin reducción.
- Generación automática de formas físicas: desactivada.

Las hojas se mantienen inicialmente como sprites únicos. Cada sistema futuro
curará y dividirá únicamente las hojas que consuma, con nombres, pivotes y rejillas
versionados mediante sus pipelines de Editor. Esto evita producir cientos de
recortes anónimos o incorrectos antes de conocer el uso real de cada recurso.

## Validación

Después de importar, ejecutar:

```text
Tools > Farm Simulator > Validate Full Cozy Farm Pack Import
```

El importador genera un manifiesto local con:

- nombre del ZIP;
- fecha UTC de importación;
- SHA-256 del archivo;
- cantidades de PNG, GIF y TXT.

Para el ZIP revisado durante esta fase, el SHA-256 es:

```text
3e9f1e1d26079b5224ac127a05eef3b0da06ecdfc40141acda573488e9ba6a7d
```

## Control de versiones

`.gitignore` excluye:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Full
LocalAssets
```

No se deben forzar estos archivos en un repositorio público. Si el proyecto se
comparte en privado con más colaboradores, cada persona debe tener derecho de
uso del paquete o importar su propia copia licenciada.
