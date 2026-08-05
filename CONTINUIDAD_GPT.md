# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head funcional A3.3:** `879a05894f7989c51df650810a9c1b6c199838af`
- **Último registro de rama:** `aef8e66ec6bbfdcbab8f3ffa043f60abda206c88`
- **Bloque actual:** A3.3 — escala de iconos y alineación de semillas plantadas
- **Estado:** implementado y documentado remotamente; regeneración, inspección y pruebas locales pendientes
- **A3.2:** aprobada visualmente; todas las pruebas solicitadas pasaron sin errores
- **A3.1:** validada con **136/136 EditMode**, **6/6 PlayMode** y cero errores; héroe a 1.0 rechazado visualmente
- **A2:** validada con **130/130 EditMode**, **6/6 PlayMode** y cero errores
- **Commit de assets fuente:** `e4540b42d275b650f726bad41d4546787ae544e9`
- **Última fase funcional:** Fase 6, integrada mediante PR #6
- **Squash commit Fase 6:** `4abce7561215a28e7a37e082cbaacf3825021e92`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

## Línea base integrada

- Fases 1 a 6 integradas en `main`.
- Catálogo e inventario de dominio integrados.
- `FarmSimulator.Domain` permanece independiente de `UnityEngine`.
- Spritesheet, prefab, animaciones, collider, movimiento y sorting del héroe permanecen intactos.

## Cozy Farm A1 y A2

- Cinco hojas fuente versionadas en `Pilot/Source`.
- Configuración: Sprite, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.
- Slicing: 3 objetos, 3 semillas, 18 etapas y 4 muestras de terreno.
- `tools.png` permanece Single y sin cortes.
- Alias provisionales: `radish → turnip`, `lettuce → cabbage`.

## A3.2 — proporción del héroe aprobada

El pipeline genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

A3.2 utiliza:

- raíz `Current Hero`: **1.0**;
- hijo `Playable Player Sprite`: **1.5**;
- tiles y cultivos: **1.0**;
- collider, Rigidbody2D, pivote lógico y sorting: sin cambios.

Arturo confirmó mediante seis comparaciones directas que el héroe ya se percibe proporcional frente a hortalizas y tiles. Todas las pruebas solicitadas pasaron sin errores.

El prefab real todavía no se modifica; la escala 1.5 sigue aplicada solo en la exhibición hasta cerrar el piloto artístico.

## A3.3 — pendiente de validación

Implementación:

- firma `cozy-farm-showcase-scene-v4`;
- `CatalogIconScale`: **0.55 → 0.75**;
- los seis iconos del catálogo conservan escala uniforme;
- 0.75 produce tres píxeles de pantalla por píxel fuente en la resolución lógica;
- `PlantedSeedStageYOffset = -0.3`;
- solo `stage_0` de nabo, zanahoria y col se desplaza verticalmente;
- `stage_1` a `stage_5` permanecen en sus posiciones anteriores;
- PNG, pivotes y `.meta` artísticos no cambian.

Pruebas:

- `CozyFarmShowcaseSceneTests.cs` pasa de siete a **ocho** casos;
- se fija la escala 0.75 de los seis iconos;
- se verifica el offset −0.3 de las tres semillas plantadas y que los primeros brotes no se muevan.

Esperado:

- EditMode: **138/138**;
- PlayMode: **6/6**.

## Próxima acción

1. Cerrar `CozyFarmShowcase` en Unity.
2. Hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
3. Abrir Unity y esperar compilación/importación.
4. La firma `v4` debe regenerar la escena.
5. Si aparece el aviso de escena abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Confirmar que los seis iconos sean mayores, nítidos y todavía menores que el héroe.
7. Confirmar que las tres semillas `stage_0` queden centradas en sus tiles y que los brotes no cambien.
8. Ejecutar EditMode; esperado **138/138**.
9. Ejecutar PlayMode; esperado **6/6**.
10. Compartir captura y resultados antes de modificar el prefab real o avanzar a Tilemaps/hotbar.
11. No hacer commit de `CozyFarmShowcase.unity` ni de su `.meta`.

---

## Orden obligatorio de lectura

1. Este archivo desde `main`.
2. `BITÁCORA_GPT.MD` desde `chore/cozy-farm-art-intake`.
3. `COZY_FARM_INTAKE.md` desde esa rama.
4. `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Ramas y commits recientes.

## Reglas críticas

- No reemplazar el héroe automáticamente.
- No subir el ZIP completo ni GIF.
- No asignar imágenes falsas a azada o regadera.
- No escalar la raíz o el collider del jugador.
- No modificar todavía el prefab real.
- No afirmar que A3.3 está validada antes del reporte local de Arturo.
- No avanzar a Tilemaps, agricultura funcional o hotbar conectada antes de cerrar el piloto visual.
- Mantener documentación sincronizada después de cada transacción.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. Fase 6 está integrada. Cozy Farm A1/A2 están validados. A3.2 fue aprobada visualmente: raíz/collider 1.0 y solo Playable Player Sprite a 1.5; todas las pruebas pasaron sin errores. A3.3 está implementada funcionalmente en 879a05894f7989c51df650810a9c1b6c199838af y documentada hasta aef8e66ec6bbfdcbab8f3ffa043f60abda206c88: firma v4, iconos 0.75 y stage_0 con offset Y −0.3. El esperado es 138/138 EditMode y 6/6 PlayMode. No modifiques el prefab real ni avances a Tilemaps/hotbar antes del reporte visual de Arturo.
```
