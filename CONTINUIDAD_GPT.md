# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama artística cerrada para funcionalidad nueva:** `chore/cozy-farm-art-intake`
- **Última integración:** PR #7 — `Add Cozy Farm pilot art and visual calibration`
- **Squash commit PR #7:** `7860095d0d165c83585f21579e9794ea57ec0a35`
- **Registro posterior de Cozy Farm:** `4b352496328d7d18643022544834fa71a1eddff2`
- **Estado:** Fases 1–6 y piloto artístico Cozy Farm integrados en `main`
- **Validación final:** **138/138 EditMode**, **6/6 PlayMode**, **0 errores**
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

## Línea base integrada

- Proyecto Unity/URP y arquitectura por capas.
- Calibración cenital XY, cámara ortográfica y resolución lógica 960×540.
- Movimiento por teclado y DualSense.
- Héroe animado y prefab reutilizable con profundidad por Y.
- Catálogo de objetos e inventario puro en Domain.
- Piloto Cozy Farm con fuentes curadas, slicing nombrado, pruebas y pipeline de exhibición reproducible.

`FarmSimulator.Domain` permanece independiente de `UnityEngine`.

## Piloto Cozy Farm integrado

Fuentes:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/
├── items.png
├── seeds.png
├── tools.png
├── crops.png
└── tiles.png
```

Slicing aprobado:

- 3 objetos cosechados;
- 3 bolsas de semillas;
- 18 etapas de cultivo;
- 4 muestras de terreno;
- `tools.png` permanece Single.

Configuración común: Sprite 2D/UI, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.

Alias provisionales:

- `radish → turnip`;
- `lettuce → cabbage`.

## Decisiones visuales aprobadas

- conservar el héroe actual;
- raíz y collider del jugador en **1.0**;
- visual del héroe en **1.5** frente al mundo Cozy Farm;
- tiles y cultivos en **1.0**;
- iconos del catálogo en **0.75** dentro de la referencia actual;
- `stage_0` de los tres cultivos con offset vertical **−0.3**;
- `cozy_tilled_soil` es provisionalmente un hoyo/montículo, no una parcela rectangular completa.

La escala 1.5 todavía no se ha trasladado al prefab real. Debe hacerse únicamente en una rama funcional con pruebas específicas.

## Escena generada

El pipeline Editor genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

La escena y su `.meta` están ignorados porque son salidas reproducibles, no fuentes canónicas.

## Alcance que sigue intacto

No se modificaron:

- `Lab`;
- prefab, spritesheet o animaciones reales del héroe;
- collider, movimiento o Input System;
- Domain, catálogo o inventario durante el bloque artístico;
- Tilemaps, agricultura funcional, hotbar o UI conectada.

El ZIP completo, los GIF y el resto del paquete Cozy Farm permanecen fuera del repositorio.

## Próxima fase recomendada

Abrir una rama funcional separada, propuesta:

```text
feature/inventory-hotbar-presentation
```

Objetivo inicial:

- presentar los ocho slots del inventario de dominio;
- mostrar selección y cantidades;
- conectar únicamente los iconos disponibles del piloto;
- mantener placeholders explícitos para azada y regadera hasta localizar arte correcto;
- no implementar todavía agricultura, economía o guardado;
- aplicar la escala visual 1.5 al prefab real solo si forma parte del mismo contrato y queda cubierta por pruebas de regresión.

## Próxima acción local

1. En GitHub Desktop, cambiar a `main`.
2. Ejecutar **Fetch origin / Pull origin**.
3. Confirmar que `CozyFarmShowcase.unity` y su `.meta` no aparezcan como cambios pendientes.
4. Abrir Unity y permitir la importación de `main`.
5. Ejecutar una regresión rápida si Unity reimporta recursos de forma significativa.
6. No continuar desarrollando sobre `chore/cozy-farm-art-intake`.
7. Crear la siguiente rama funcional solo después de definir su alcance exacto.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `BITÁCORA_GPT.MD` desde `main`.
3. Leer `COZY_FARM_INTAKE.md` desde `main`.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas y commits recientes.

## Reglas críticas

- No reemplazar automáticamente el héroe.
- No subir el ZIP completo ni GIF.
- No asignar imágenes falsas a azada o regadera.
- No escalar la raíz o el collider del jugador.
- No continuar funcionalidad nueva en la rama artística.
- No crear Tilemaps o agricultura funcional dentro del bloque de hotbar.
- Mantener documentación sincronizada después de cada transacción.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md, BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde main. Fases 1–6 y el piloto Cozy Farm están integrados. PR #7 fue fusionado mediante squash en 7860095d0d165c83585f21579e9794ea57ec0a35. Validación final: 138/138 EditMode, 6/6 PlayMode y cero errores. Decisiones visuales: héroe visual 1.5, raíz/collider 1.0, tiles/cultivos 1.0, iconos 0.75 y stage_0 con offset Y −0.3. El prefab real sigue intacto. La siguiente rama propuesta es feature/inventory-hotbar-presentation; define el alcance antes de crearla.
```
