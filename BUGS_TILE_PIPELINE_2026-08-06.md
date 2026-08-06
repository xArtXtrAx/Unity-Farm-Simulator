# Reporte de bugs — Pipeline agrícola y Tilemaps

Fecha: 2026-08-06  
Rama: `agent/cozy-art-pipeline`

Este archivo complementa `BUGS.MD` durante el PR #14.

## BUG-0009 — Tierra y cultivo competían por la misma celda

- **Estado:** CORREGIDO; validación local pendiente.
- **Severidad:** S2 — Alta.
- **Causa final:** los cultivos no deben formar parte del Tilemap de autoría. Aunque una capa `Crops` separada evitaba el reemplazo directo, seguía mezclando entidades dinámicas con arte estático y complicaba transparencia, escala y animación.
- **Solución final:** eliminar el Tilemap y la paleta `Crops`; cada parcela posee un `Crop Entity Visual` con `SpriteRenderer`.
- **Prueba de regresión:** la escena debe tener cuatro Tilemaps y nueve `SpriteRenderer` de cultivo, sin ningún Tilemap llamado `Crops`.

## BUG-0010 — Fondo opaco, centrado y pivote inconsistentes en cultivos

- **Estado:** CORREGIDO; validación local pendiente.
- **Severidad:** S2 — Alta.
- **Causa final:** los recortes fuente contienen fondo de previsualización y no son tiles de mundo. Pintarlos manualmente hacía visible el rectángulo completo y forzaba una escala fija de celda.
- **Solución final:** generar 18 sprites transparentes para runtime y mostrarlos únicamente mediante `SpriteRenderer`. `FarmPlotBehaviour` controla escala, posición y etapa visual.
- **Prueba de regresión:** al sembrar, el suelo debe permanecer visible; al dormir, solo cambia el sprite del cultivo.

## BUG-0012 — La región inicial de la casa apuntaba a utilería vecina

- **Estado:** CORREGIDO; validación visual local pendiente.
- **Severidad:** S2 — Alta.
- **Detectado:** 2026-08-06.
- **Sistema:** catálogo Full-Pack de edificios.
- **Comportamiento observado:** el sprite generado mostraba vegetación, una cerca y una banca en lugar de una casa completa.
- **Causa:** `StarterHouseSource` apuntaba a la franja situada por encima de la fila de casas verdes del atlas.
- **Solución:** se movió el rectángulo exclusivamente en `CozyFarmBuildingCatalog` a `RectInt(681, 548, 68, 86)` y se añadió una prueba que fija esas coordenadas.
- **Validación pendiente:** regenerar el sprite, aplicar la casa a `Farm`, confirmar que el edificio está completo y aislado, y ejecutar EditMode y PlayMode. No pasar a **VERIFICADO** hasta recibir confirmación de Arturo.

## Incremento — Catálogo reutilizable y selector de casas

- **Estado:** IMPLEMENTADO; compilación y validación visual local pendientes.
- `CozyFarmBuildingCatalog` contiene cinco variantes con identificador estable, nombre, rectángulo de atlas y metadatos de puerta, portal, aparición, collider, escala, sombra y sorting.
- `Generate Cozy Full-Pack Building Sprites` genera todas las variantes registradas.
- `CozyFarmHouseStyleWindow` añade `Tools → Farm Simulator → House Style Selector`.
- El selector recuerda el estilo elegido mediante `EditorPrefs` y permite generar o aplicar la variante desde una sola ventana.
- `CozyFarmHouseExteriorUpgrader` reconstruye la composición visual `Cozy Full-Pack House v5` y consume los metadatos de la variante.
- Se añadieron pruebas de identificadores, rutas, regiones y metadatos positivos.
- Las cuatro variantes nuevas no se consideran aprobadas visualmente hasta revisarlas individualmente en Unity.

## Incidencia de compatibilidad Unity 6

- `TextureImporter.spriteAlignment` no existe como propiedad directa en Unity 6.3.
- Se sustituyó por `TextureImporterSettings.spriteAlignment` y `spritePivot`.
- Estado: **VERIFICADO por compilación posterior reportada por Arturo**.

## Validación pendiente

1. actualizar la rama y confirmar que Unity compila;
2. abrir `Tools → Farm Simulator → House Style Selector`;
3. generar los cinco sprites;
4. aplicar cada variante y comprobar que contiene una sola casa completa;
5. validar escala, puerta, portal, collider, punto de aparición, sombra y sorting;
6. probar entrada y salida de la casa con cada estilo;
7. ejecutar EditMode y PlayMode completos;
8. no marcar variantes ni `BUG-0012` como **VERIFICADO** hasta recibir confirmación de Arturo.
