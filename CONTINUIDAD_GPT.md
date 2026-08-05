# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head remoto registrado:** `b3056947ce708b9bc53e092e8204a5bcd3765020`
- **Bloque actual:** A1 — recepción piloto de Cozy Farm
- **Estado:** rama y documentación creadas; bundle local preparado; copia de assets, commit local y validación Unity pendientes
- **Commit inicial del bloque:** `d2845f676ea33260292d0f27f10a6bb578dcd3d5`
- **Corrección documental:** `b3056947ce708b9bc53e092e8204a5bcd3765020`, conserva íntegro el historial de Fases 1–6
- **Última fase funcional:** Fase 6, integrada mediante PR #6
- **Squash commit Fase 6:** `4abce7561215a28e7a37e082cbaacf3825021e92`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

## Línea base integrada

- Fases 1 a 6 integradas en `main`.
- EditMode confirmado: **124/124**.
- Casos nuevos de la Fase 6: **88/88**.
- Fallos reportados: **0**.
- PlayMode conserva la última regresión confirmada de **6/6**.
- Catálogo e inventario de dominio integrados.
- `FarmSimulator.Domain` permanece independiente de `UnityEngine`.

## Decisión artística vigente

- El héroe actual se conserva en `Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png`.
- No cambiar su prefab, Animator, clips, pivote, collider, movimiento o sorting salvo decisión explícita posterior.
- El ZIP completo de Cozy Farm no se versiona.
- El piloto se limita a `items.png`, `seeds.png`, `tools.png`, `crops.png` y `tiles.png`.
- No se incluyen GIF, `global.png`, `item_carry.png`, personajes, animales, edificios, enemigos o variantes completas.
- El bundle preparado contiene los cinco PNG, sus `.meta`, metas de carpetas y documentación.
- La configuración inicial es Sprite Single temporal, 16 PPU, Point, sin mipmaps, Clamp y sin compresión por defecto.

## Próxima acción

1. En GitHub Desktop, cambiar a `chore/cozy-farm-art-intake` y hacer Fetch/Pull.
2. Extraer `cozy-farm-pilot-intake.zip` en la raíz del repositorio.
3. Confirmar que solo aparezca `Assets/_Project/Art/ThirdParty/CozyFarm/...` y hacer un único commit y Push origin.
4. Abrir Unity y validar importación, escala y nitidez.
5. Ejecutar EditMode y PlayMode completos.
6. Reportar resultados antes de crear slicing, Tilemaps, paletas, prefabs o UI.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `BITÁCORA_GPT.MD` desde `chore/cozy-farm-art-intake`.
3. Leer `COZY_FARM_INTAKE.md` desde esa rama.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas y commits recientes.
6. Continuar desde “Próximo paso exacto” en la bitácora.

## Reglas críticas

- No añadir funcionalidad a la rama artística.
- No reemplazar el héroe actual.
- No subir el ZIP completo ni GIF de referencia.
- No iniciar slicing masivo antes de la validación local.
- No afirmar que Unity importa o que las pruebas pasan hasta recibir los resultados de Arturo.
- Después de cada implementación, corrección o integración, actualizar `BITÁCORA_GPT.MD` y mantener este archivo sincronizado.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. La Fase 6 está integrada y validada con 124/124 EditMode. La rama artística está en b3056947ce708b9bc53e092e8204a5bcd3765020; existe un bundle local con cinco PNG Cozy Farm, metas y documentación, pero su copia, commit y validación Unity siguen pendientes. Conserva el héroe actual y no avances a slicing masivo hasta la validación de Arturo.
```
