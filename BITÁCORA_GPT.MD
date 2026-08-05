# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Ninguna implementación, corrección o integración realizada por GPT se considera terminada hasta registrar aquí lo realizado, lo comprobado, lo pendiente y el siguiente paso exacto.

---

## 1. Identidad y contrato vigente

- Repositorio: `xArtXtrAx/Unity-Farm-Simulator`.
- Unity: `6000.3.21f1`, URP, Windows PC.
- Ruta local habitual: `D:\\Git\\Unity-Farm-Simulator`.
- Rama estable: `main`.
- Rama activa: `chore/cozy-farm-art-intake`.
- Fuente funcional congelada: `xArtXtrAx/farming-game-A` en `dd32056c9f8142a2322bc2c1d41f0b05b002598f`.
- Vista cenital XY, Physics 2D, cámara ortográfica, 960 × 540, 64 PPU para el héroe.
- Héroe: frames 64 × 72, pivote X 0.5 / Y 0.12, teclado y DualSense.
- `FarmSimulator.Domain` no depende de `UnityEngine`.

Arquitectura: Domain → Application → Infrastructure / Presentation; Editor y Tests separados.

---

## 2. Línea base integrada

### Fases 1–5

Cimentación Unity/URP, calibración cenital, movimiento, sprite/animación, prefab reutilizable y profundidad por Y. PR #5 integrado en `341051f097d9d70796e69b9cddc9277ba3902ed0`. Bugs `BUG-0001` a `BUG-0006` verificados.

### Fase 6 — Catálogo e inventario de dominio

Estado: **IMPLEMENTADA, VALIDADA E INTEGRADA**.

- PR #6: `Add domain item catalog and inventory foundation`.
- Squash commit: `4abce7561215a28e7a37e082cbaacf3825021e92`.
- Catálogo puro con ocho IDs, categorías, stacks y precios.
- Inventario puro de ocho slots, selección circular, stacking determinista, adición atómica, consumo, snapshots y restauración validada.
- Estado inicial: azada ×1, regadera ×1, semillas de nabo ×20.
- Validación local: EditMode **124/124**, 88/88 casos nuevos, cero fallos.
- PlayMode conserva la regresión confirmada de **6/6**.

---

## 3. Bloque artístico A1 — Recepción piloto de Cozy Farm

Estado: **RAMA CREADA Y PAQUETE PILOTO PREPARADO; COPIA LOCAL, COMMIT DE ASSETS Y VALIDACIÓN UNITY PENDIENTES**.

Rama:

```text
chore/cozy-farm-art-intake
```

### Decisión de personaje

El héroe actual se conserva en `Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png`. Este bloque no modifica el PNG, prefab, Animator, clips, pivote, collider, movimiento ni sorting. Cualquier sustitución será una decisión visual posterior y explícita.

### Fuente estudiada

Archivo local no versionado: `full version.zip`, Cozy Farm de shubibubi, copia comprada proporcionada por Arturo el 5 de agosto de 2026. Inventario observado: 445 entradas, 63 PNG, 344 GIF y 3 TXT. Los GIF se consideran referencia temporal y no entran en Unity.

### Subconjunto preparado para versionar

Se prepararon cinco hojas PNG originales en un paquete local reproducible; todavía deben copiarse y confirmarse mediante GitHub Desktop:

- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/items.png` — 160 × 192.
- `.../seeds.png` — 112 × 96.
- `.../tools.png` — 592 × 64.
- `.../crops.png` — 96 × 592.
- `.../tiles.png` — 864 × 800.

El paquete también contiene `Assets/_Project/Art/ThirdParty/CozyFarm/README.md`, metas deterministas, procedencia, hashes, alcance y lista de validación.

No se incorporaron ZIP, GIF, `global.png`, `item_carry.png`, personajes, animales, edificios, enemigos, máquinas ni variantes estacionales completas.

### Configuración inicial de importación

- Sprite (2D and UI), modo Single temporal.
- 16 PPU, Point, sin mipmaps, Clamp, transparencia habilitada y sin compresión por defecto.
- Los PNG se preservan sin edición destructiva.
- El slicing queda bloqueado hasta validación local.

### Comprobaciones remotas

- Firmas PNG válidas y dimensiones verificadas.
- Archivos extraídos directamente de las rutas especializadas del ZIP.
- No se modificaron escenas, prefabs, scripts funcionales, asmdefs, Input System o Domain.
- Los binarios PNG aún no están publicados en GitHub; el conector no admite adjuntar archivos locales binarios directamente.
- No se afirma todavía que Unity importe sin advertencias ni que las pruebas sigan pasando.

---

## 4. Próximo paso exacto

1. En GitHub Desktop, cambiar a `chore/cozy-farm-art-intake` y ejecutar Fetch/Pull.
2. Extraer el paquete preparado en la raíz `D:\\Git\\Unity-Farm-Simulator`, conservando su estructura `Assets/_Project/Art/...`.
3. Revisar en GitHub Desktop que solo aparezcan el piloto Cozy Farm y su documentación; crear un commit local y hacer Push origin.
4. Abrir el proyecto con Unity `6000.3.21f1` y esperar la importación completa.
5. Confirmar que no hay errores ni ciclos de reimportación.
6. Revisar que las cinco texturas usan Point, 16 PPU, sin mipmaps y sin compresión.
7. Comparar héroe, objetos y terreno a escala mundial equivalente.
8. Ejecutar EditMode completo; resultado esperado no confirmado: 124/124.
9. Ejecutar PlayMode completo; resultado esperado no confirmado: 6/6.
10. Reportar consola, pruebas y percepción visual antes de crear slicing, Tilemaps, paletas o UI.
11. No agregar más recursos a esta rama hasta cerrar la validación del piloto.

---

## 5. Pendiente funcional

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 6. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `CONTINUIDAD_GPT.md` desde `main`; después esta bitácora desde `chore/cozy-farm-art-intake`, `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`. La Fase 6 está integrada y validada con 124/124 EditMode. La rama de recepción Cozy Farm está creada y existe un paquete local con cinco PNG fuente, metas y documentación; su commit y validación Unity siguen pendientes. Conserva el héroe actual y no añadas más recursos ni slicing hasta recibir la validación local de Arturo.
