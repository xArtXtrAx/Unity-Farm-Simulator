# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head funcional A3.2:** `d4d3757640aa5b4f232bbd35f28d9e924bb328b7`
- **Último registro de rama:** `b2a5d9254b1b757ea4b867396c7e4e4d97dca72d`
- **Bloque actual:** A3.2 — calibración del visual del héroe a 1.5×
- **Estado:** implementado y documentado remotamente; regeneración, inspección y pruebas locales pendientes
- **A3.1:** validada con **136/136 EditMode**, **6/6 PlayMode** y cero errores; proporción héroe/mundo rechazada visualmente
- **A3 original:** validada con **134/134 EditMode**, **6/6 PlayMode** y cero errores
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
- El spritesheet, prefab, animaciones, pivote, collider y sorting del héroe permanecen intactos.

## Cozy Farm A1 y A2

- Cinco hojas fuente versionadas en `Pilot/Source`.
- Configuración: Sprite, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.
- Slicing: 3 objetos, 3 semillas, 18 etapas y 4 muestras de terreno.
- `tools.png` permanece Single y sin cortes.
- Alias provisionales: `radish → turnip`, `lettuce → cabbage`.

## A3 y A3.1 — conclusiones corregidas

El pipeline genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

A3.1 mejoró la distribución:

- iconos a 0.55;
- cultivos/héroe a 1.0;
- cama 6×3;
- eliminación de `soil_for_*`;
- muestras individuales.

Validación A3.1 de Arturo:

- EditMode: **136/136**;
- PlayMode: **6/6**;
- errores: **0**.

Cinco capturas con el héroe movido junto a cada región demostraron que la conclusión anterior de que la geometría era suficiente era incorrecta. La evidencia directa muestra al héroe demasiado pequeño frente a tiles y cultivos maduros.

Diagnóstico:

- héroe 64×72 a 64 PPU = **1×1.125 unidades**;
- tile Cozy Farm 16×16 a 16 PPU = **1×1 unidad**;
- el héroe queda prácticamente de un tile de alto;
- reducir tiles rompería continuidad del grid;
- escalar la raíz del jugador alteraría collider y referencias técnicas.

A3.1 queda aprobada técnicamente, pero rechazada como proporción final.

## A3.2 — pendiente de validación

El generador aplica una calibración exclusivamente a la instancia de exhibición:

- firma `cozy-farm-showcase-scene-v3`;
- `HeroVisualScale = 1.5f`;
- raíz `Current Hero` en **1.0**;
- hijo `Playable Player Sprite` en **1.5**;
- collider, Rigidbody2D, motor, feet sorting y pivote lógico intactos;
- tiles y cultivos permanecen en **1.0**;
- tamaño visual esperado del héroe: **1.5×1.6875 unidades**.

La escala 1.5 conserva nitidez de los bloques lógicos del héroe: los bloques 4×4 pasan a 6×6 en la resolución de referencia.

`CozyFarmShowcaseSceneTests.cs` pasa de seis a **siete** casos. El nuevo caso protege que solo cambie el hijo visual y que el collider conserve sus medidas originales.

El prefab real no fue modificado. Solo se considerará trasladar la escala después de la aprobación visual de Arturo.

## Próxima acción

1. Cerrar `CozyFarmShowcase` en Unity.
2. Hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
3. Abrir Unity y esperar compilación/importación.
4. La firma `v3` debe regenerar la escena.
5. Si aparece el aviso de escena abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Colocar al héroe junto a un tile, un cultivo joven y uno maduro; compartir una captura.
7. Ejecutar EditMode; esperado **137/137**.
8. Ejecutar PlayMode; esperado **6/6**.
9. No hacer commit de `CozyFarmShowcase.unity` ni modificar el prefab real antes de aprobar la comparación.

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
- No escalar la raíz/collider del jugador para resolver una diferencia visual.
- No afirmar que 1.5× es definitivo antes del reporte local de Arturo.
- No modificar el prefab real ni avanzar a Tilemap/hotbar funcional antes de la decisión visual.
- Mantener documentación sincronizada después de cada transacción.

## Prompt mínimo para chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. Fase 6 está integrada. Cozy Farm A1/A2 están validados. A3.1 pasó 136/136 EditMode y 6/6 PlayMode sin errores, pero cinco comparaciones directas demostraron que el héroe a escala 1.0 sigue demasiado pequeño frente a tiles y cultivos. A3.2 está implementado hasta d4d3757640aa5b4f232bbd35f28d9e924bb328b7 y documentado hasta b2a5d9254b1b757ea4b867396c7e4e4d97dca72d: firma v3, raíz/collider 1.0 y solo Playable Player Sprite a 1.5. El esperado es 137/137 EditMode y 6/6 PlayMode. No cambies el prefab real ni avances a Tilemaps/hotbar antes del reporte visual de Arturo.
```
