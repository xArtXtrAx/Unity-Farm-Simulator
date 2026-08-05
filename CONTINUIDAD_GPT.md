# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head funcional A3.3:** `879a05894f7989c51df650810a9c1b6c199838af`
- **Último registro de rama:** `0f711adcf7dd0ed0c268b57d053ab36f044ea7cc`
- **Bloque actual:** cierre del piloto artístico Cozy Farm
- **Estado:** A1–A3.3 validados; rama preparada para revisión y PR, todavía sin fusionar
- **Validación final:** **138/138 EditMode**, **6/6 PlayMode**, cero errores
- **A3.2:** héroe visual 1.5× aprobado frente a tiles y cultivos
- **A3.3:** iconos 0.75× y semillas sembradas con offset Y −0.3 aprobados
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
- Spritesheet, prefab, animaciones, collider, movimiento y sorting reales del héroe permanecen intactos.

## Piloto Cozy Farm aprobado

Fuentes versionadas:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/
├── items.png
├── seeds.png
├── tools.png
├── crops.png
└── tiles.png
```

Configuración:

- Sprite 2D/UI;
- 16 PPU;
- Point;
- sin mipmaps;
- Clamp;
- sin compresión.

Slicing aprobado:

- 3 objetos cosechados;
- 3 bolsas de semillas;
- 18 etapas de cultivo;
- 4 muestras de terreno;
- `tools.png` permanece Single y sin cortes.

Alias provisionales:

- `radish → turnip`;
- `lettuce → cabbage`.

## Decisiones visuales aprobadas

- conservar el héroe actual;
- raíz/collider del jugador en **1.0**;
- visual del héroe en **1.5** frente al mundo Cozy Farm;
- tiles y cultivos en **1.0**;
- iconos del catálogo en **0.75** dentro de la referencia actual;
- `stage_0` de los tres cultivos con offset vertical **−0.3**;
- `cozy_tilled_soil` es provisionalmente un hoyo/montículo, no una parcela rectangular completa.

La escala 1.5 todavía se aplica solo a la instancia generada de exhibición. El prefab real no se modificó en la rama artística.

## Validación final A3.3 — 2026-08-05

Arturo aportó capturas finales y confirmó:

- EditMode: **138/138**;
- PlayMode: **6/6**;
- errores: **0**;
- héroe, cultivos maduros y tiles proporcionales;
- iconos con presencia suficiente y todavía menores que el héroe;
- semillas plantadas mejor centradas;
- resultado aprobado como excelente punto de partida.

## Escena generada

El pipeline Editor genera:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

Es una salida reproducible y no una fuente canónica. La rama contiene exclusiones específicas:

```text
/Assets/_Project/Scenes/CozyFarmShowcase.unity
/Assets/_Project/Scenes/CozyFarmShowcase.unity.meta
```

GitHub Desktop no debe ofrecer esos archivos para commit después del próximo Pull.

## Próxima acción

1. Hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
2. Confirmar que `CozyFarmShowcase.unity` y su `.meta` desaparezcan de los cambios pendientes.
3. Revisar el diff completo de la rama contra `main`.
4. Crear un PR de cierre del piloto artístico.
5. No fusionar sin autorización explícita de Arturo.
6. Después de integrar, abrir una rama funcional separada para trasladar las decisiones aprobadas al prefab, hotbar, mundo o agricultura según el orden de migración.

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
- No trasladar todavía 1.5× al prefab real dentro de la rama artística.
- No crear Tilemap, agricultura funcional o hotbar conectada en esta rama.
- No fusionar el PR sin autorización explícita.
- Mantener documentación sincronizada después de cada transacción.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. Fase 6 está integrada. El piloto Cozy Farm A1–A3.3 está validado con 138/138 EditMode, 6/6 PlayMode y cero errores. Decisiones aprobadas: héroe visual 1.5, tiles/cultivos 1.0, iconos 0.75 y stage_0 con offset Y −0.3. El prefab real sigue intacto. La escena Showcase es generada y quedó ignorada. Revisa el diff, prepara el PR de cierre y no lo fusiones sin autorización de Arturo.
```
