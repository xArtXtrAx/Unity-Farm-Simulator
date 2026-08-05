# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head remoto registrado:** `53103a792839fe03c873a08e296af35994129f42`
- **Head funcional A2:** `d88eeacb38ff50963b31d1d8a0f2c91b4297bf49`
- **Bloque actual:** A2 — slicing curado de Cozy Farm
- **Estado:** validación visual aprobada; pruebas automáticas locales pendientes
- **Commit de assets fuente publicado por Arturo:** `e4540b42d275b650f726bad41d4546787ae544e9`
- **Última fase funcional:** Fase 6, integrada mediante PR #6
- **Squash commit Fase 6:** `4abce7561215a28e7a37e082cbaacf3825021e92`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

## Línea base integrada

- Fases 1 a 6 integradas en `main`.
- EditMode funcional confirmado antes de A2: **124/124**.
- PlayMode confirmado: **6/6**.
- Catálogo e inventario de dominio integrados.
- `FarmSimulator.Domain` permanece independiente de `UnityEngine`.

## Bloque artístico A1 — validado

- Cinco hojas fuente Cozy Farm versionadas en `Pilot/Source`.
- Configuración: Sprite, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.
- Arturo confirmó que la importación y todas las pruebas quedaron OK después del push.
- El héroe actual permanece intacto en `Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png`.

## Bloque artístico A2 — validación visual aprobada

Se añadieron slices explícitos y nombrados, sin duplicar ni alterar los PNG:

- 3 objetos: nabo, zanahoria y col;
- 3 bolsas de semillas;
- 18 etapas de cultivo: 6 por cultivo;
- 4 tiles piloto: césped, tierra, agua y tierra labrada;
- 6 pruebas EditMode nuevas en `CozyFarmPilotArtTests.cs`.

Arturo confirmó visualmente en Unity:

- `items.png`: **3** sprites;
- `seeds.png`: **3** sprites;
- `crops.png`: **18** sprites;
- `tiles.png`: **4** sprites;
- `tools.png`: una planilla sin cortes.

Decisiones:

- `turnip` usa provisionalmente el gráfico denominado `radish` por el paquete.
- `cabbage` usa provisionalmente el gráfico denominado `lettuce`.
- Los IDs de dominio no cambian.
- `tools.png` contiene máquinas, no iconos apropiados de azada o regadera; queda sin slicing.
- No se crean todavía Tilemaps, paletas, prefabs, escenas, hotbar ni conexiones funcionales.

## Próxima acción

1. Ejecutar EditMode completo; esperado **130/130**.
2. Ejecutar PlayMode completo; esperado **6/6**.
3. Confirmar los conteos exactos y cualquier error o advertencia.
4. Después de aprobar ambas suites, crear una escena artística de exhibición separada para comparar héroe, terreno, objetos, semillas y etapas de cultivo.
5. No crear todavía el Tilemap funcional ni conectar la hotbar.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `BITÁCORA_GPT.MD` desde `chore/cozy-farm-art-intake`.
3. Leer `COZY_FARM_INTAKE.md` desde esa rama.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas y commits recientes.
6. Continuar desde el próximo paso indicado en `COZY_FARM_INTAKE.md`.

## Reglas críticas

- No añadir funcionalidad de juego a la rama artística.
- No reemplazar el héroe actual.
- No subir el ZIP completo ni GIF de referencia.
- No hacer slicing masivo; solo slices con consumidor o prueba definida.
- No asignar imágenes falsas a azada o regadera.
- No afirmar que A2 pasa las pruebas hasta recibir los conteos exactos de Arturo.
- Después de cada implementación, corrección o integración, mantener la documentación sincronizada.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. La Fase 6 está integrada. A1 de Cozy Farm fue validado con la línea base 124/124 EditMode y 6/6 PlayMode. A2 está implementado y su validación visual fue aprobada: 3 objetos, 3 semillas, 18 etapas de cultivo, 4 tiles y tools.png sin cortes. Las pruebas esperadas son 130/130 EditMode y 6/6 PlayMode; no avances a la escena de exhibición hasta recibir esos conteos exactos. Conserva el héroe y no crees todavía Tilemaps o UI funcional.
```
