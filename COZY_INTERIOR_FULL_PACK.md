# Importación local del paquete completo Cozy Interior

## Contenido revisado

El archivo `Interior full.zip` contiene:

- 39 hojas PNG utilizables por Unity;
- 154 GIF de previsualización y referencia de animaciones;
- 2 archivos de texto;
- rejilla principal de 16 x 16 píxeles.

El archivo `info.txt` documenta además tamaños y tiempos para puertas, mascotas,
televisores, acuarios, chimeneas y velas. Los GIF se conservan como referencia,
pero las hojas PNG son las fuentes que consumirán los pipelines del juego.

SHA-256 del ZIP revisado:

```text
f1a3fa6ec260d1fffcc0842bf58c94097e0510488c856e1368915ae84d90084b
```

## Importación

1. Actualizar la rama y abrir Unity.
2. Esperar a que termine de compilar.
3. Ejecutar:

```text
Tools > Farm Simulator > Import Full Cozy Interior Pack...
```

4. Seleccionar `Interior full.zip`.
5. Esperar a que concluya la importación.

Las hojas PNG y los TXT quedarán en:

```text
Assets/_Project/Art/ThirdParty/CozyInterior/Full
```

Los GIF quedarán fuera de `Assets` en:

```text
LocalAssets/CozyInterior/Previews
```

## Configuración aplicada

- Texture Type: Sprite.
- Sprite Mode: Single.
- Pixels Per Unit: 16.
- Filter Mode: Point.
- Mip Maps: desactivados.
- Wrap Mode: Clamp.
- Compression: Uncompressed.
- Max Size: 8192, necesario para `global.png` de 4320 x 3440.
- Formas físicas automáticas: desactivadas.

Las hojas se conservan primero como sprites únicos. Los pipelines de cada sistema
seleccionarán y dividirán solo los recursos que consuman, con nombres, pivotes,
rejillas y tiempos de animación versionados.

## Catálogo estable

`CozyInteriorAssetCatalog` expone rutas para:

- wallpapers, puertas, alfombras y chimeneas;
- camas, decoración, mesas, sillas y almacenamiento;
- cocinas ensambladas;
- mascotas y hojas de animación de gato y yorkie.

También conserva los tamaños publicados por el autor: rejilla 16 x 16, puertas
48 x 32, mascotas 18 x 18 y televisores 16 x 16 o 32 x 32.

## Validación

Ejecutar:

```text
Tools > Farm Simulator > Validate Full Cozy Interior Pack Import
```

Resultado esperado:

```text
PNG: 39
GIF: 154
TXT: 2
```

Para comprobar exterior e interior juntos:

```text
Tools > Farm Simulator > Validate Complete Cozy Asset Library
```

## Control de versiones

Mientras el repositorio sea accesible públicamente, `.gitignore` mantiene fuera:

```text
Assets/_Project/Art/ThirdParty/CozyInterior/Full
LocalAssets/CozyInterior
```

El código del importador, el catálogo, la documentación y las pruebas sí se
versionan. Las hojas compradas permanecen en cada copia local autorizada.
