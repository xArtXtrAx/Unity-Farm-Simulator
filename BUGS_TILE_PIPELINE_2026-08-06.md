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

## BUG-0013 — R1 puede avanzar dos posiciones del hotbar con DualSense

- **Estado:** VERIFICADO.
- **Severidad:** S3 — Media.
- **Detectado:** 2026-08-06.
- **Sistema:** entrada de mando / inventario hotbar.
- **Comportamiento observado:** una pulsación de R1 podía seleccionar el objeto situado dos casillas a la derecha.
- **Solución:** `InventoryHotbarView` comparte una compuerta estática por frame y aplica un debounce de 120 ms a L1/R1 y rueda del ratón. Se cualificó `global::UnityEngine.Time` para evitar colisión con `FarmSimulator.Presentation.Time`.
- **Correcciones:** `8544ff1e654a59c0fff167b51b6c07de7247c912`, `7ce611c03ae95734338436d7a4506590a046c917`.
- **Verificación:** Arturo confirmó en Play Mode que L1 y R1 avanzan exactamente una casilla por pulsación.

## Incremento — Catálogo reutilizable y selector de casas

- **Estado:** IMPLEMENTADO; compilación y validación visual local pendientes.
- `CozyFarmBuildingCatalog` contiene cinco variantes con identificador estable, nombre, rectángulo de atlas y metadatos de puerta, portal, aparición, collider, escala, sombra y sorting.
- `Generate Cozy Full-Pack Building Sprites` genera todas las variantes registradas.
- `CozyFarmHouseStyleWindow` añade `Tools → Farm Simulator → House Style Selector`.
- El selector recuerda el estilo elegido mediante `EditorPrefs` y permite generar o aplicar la variante desde una sola ventana.
- `CozyFarmHouseExteriorUpgrader` reconstruye la composición visual `Cozy Full-Pack House v5` y consume los metadatos de la variante.
- Se añadieron pruebas de identificadores, rutas, regiones y metadatos positivos.
- Las cuatro variantes nuevas no se consideran aprobadas visualmente hasta revisarlas individualmente en Unity.

## Incremento — Reinicio de granja ajustado a la retícula

- **Estado:** IMPLEMENTADO; validación visual y funcional local pendiente.
- Nueva orden: `Tools → Farm Simulator → Reset Farm To Grid Starter Layout`.
- La granja conserva únicamente el terreno base, la casa seleccionada, nueve parcelas, una banca a la derecha y una lámpara a la izquierda.
- Se eliminan árboles, arbustos posteriores, rocas y cerca.
- Se limpian las capas `Paths`, `Soil` y `Decoration` para comenzar desde una base de autoría vacía.
- Casa, banca, lámpara, parcelas y spawn inicial se colocan mediante `Grid.GetCellCenterWorld`.
- El portal y el spawn de regreso se realinean con los metadatos de la variante de casa.
- Se añadió el marcador `Farm Grid Layout v1` para no reconstruir una escena ya aplicada durante cada recarga de dominio.
- No se considera validado hasta confirmar visualmente la composición, entrada/salida y suites completas.

## Incremento — Farm Development Kit: prefabs reutilizables

- **Estado:** VERIFICADO para la entrega rectangular anterior; reemplazado por huellas lógicas editables.
- Arturo confirmó que EditMode pasó sin errores y que la colocación/snap operaba en la retícula.
- `CozyFarmBuildingPrefabGenerator` crea un prefab por cada `CozyBuildingDefinition`.
- Cada prefab contiene visual, `BoxCollider2D`, anclas y metadatos de ocupación.

## BUG-0014 — La huella lógica no coincidía con la base visual de la casa

- **Estado:** CORREGIDO; validación visual y pruebas locales pendientes.
- **Severidad:** S3 — Media.
- **Detectado:** 2026-08-06.
- **Sistema:** Farm Development Kit / colocación de edificios.
- **Comportamiento observado:** primero la casa utilizó una huella automática `6 × 5`; después de migrarla a una máscara `4 × 3`, la huella seguía apareciendo demasiado abajo y el panel de celdas no mostraba el mismo patrón que la previsualización y Scene.
- **Causas:**
  1. `GridSize` se derivaba inicialmente de las dimensiones visuales completas.
  2. El generador reutilizable aplicaba `-Baseline`, moviendo el sprite en la dirección opuesta a la composición exterior.
  3. `Place on scene grid` podía instanciar un prefab previamente generado y desactualizado respecto de la definición editada.
  4. Los lienzos de ancho par utilizaban dos fórmulas diferentes: la máscara runtime `4 × 3` ocupaba columnas `-2..1`, mientras el panel editor dibujaba `-1..2`. Dos celdas reales quedaban invisibles en el panel aunque sí aparecían en la previsualización y en Scene.
- **Solución:** separar límites visuales y huella lógica; normalizar el prefab alrededor del pivote inferior central; regenerar antes de colocar; y centralizar el origen horizontal del lienzo en `GridBuildingFootprint.GetCanvasMinimumX()`, reutilizado por runtime y editor.
- **Autoría:** el Footprint Editor superpone sobre el sprite la misma máscara exacta que usan el prefab, el gizmo de Scene, el snap y las colisiones. Para un ancho de cuatro, las columnas canónicas son `-2, -1, 0, 1` y el ancla `(0,0)` aparece en la tercera columna.
- **Valor inicial para casas:** lienzo `4 × 3` con diez celdas ocupadas; la fila posterior solo ocupa las dos celdas centrales.
- **Commits de corrección más recientes:** `dc44fc52663c0303ae6af6657f723c8d21c3091d`, `225157451d83db5ff65161d40c3696328c29b14d`, `90dc5b8010853ed7d71b950caed63cce19b73e50`.
- **Validación pendiente:** abrir nuevamente una definición `4 × 3`; confirmar que el panel muestra las diez celdas indicadas por el contador y el mismo patrón que la superposición; regenerar, colocar, comprobar rojo/verde y ejecutar EditMode completo.

## Incidencia de compatibilidad Unity 6

- `TextureImporter.spriteAlignment` no existe como propiedad directa en Unity 6.3.
- Se sustituyó por `TextureImporterSettings.spriteAlignment` y `spritePivot`.
- Estado: **VERIFICADO por compilación posterior reportada por Arturo**.

## Estado de pruebas

- Arturo confirmó EditMode completo sin errores antes del último ajuste del origen horizontal de lienzos pares.
- `BUG-0013` quedó verificado manualmente con DualSense.
- `BUG-0014` requiere una nueva ejecución de EditMode y validación visual.

## Validación pendiente

1. actualizar la rama y confirmar que Unity compila;
2. abrir `Farm Development Kit → Footprint Editor` con una casa `4 × 3`;
3. confirmar que el panel, el contador y la superposición muestran exactamente las mismas diez celdas;
4. guardar con `Save + regenerate prefab` o usar `Regenerate + place on scene grid`;
5. eliminar instancias antiguas y colocar una nueva;
6. confirmar que el patrón coincide en Scene y cambia a rojo solo al intersectar otras celdas ocupadas;
7. ejecutar EditMode completo;
8. no marcar `BUG-0014` como VERIFICADO hasta recibir confirmación de Arturo.
