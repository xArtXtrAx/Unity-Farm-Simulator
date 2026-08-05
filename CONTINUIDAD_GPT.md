# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head remoto registrado:** `57fadeeca15792e880d3c1ea5e2c594b43229b77`
- **Head funcional A2:** `d88eeacb38ff50963b31d1d8a0f2c91b4297bf49`
- **Bloque actual:** A2 — slicing curado de Cozy Farm
- **Estado:** implementado remotamente; reimportación y validación local pendientes
- **Commit de assets fuente publicado por Arturo:** `e4540b42d275b650f726bad41d4546787ae544e9`
- **Última fase funcional:** Fase 6, integrada mediante PR #6
- **Squash commit Fase 6:** `4abce7561215a28e7a37e082cbaacf3825021e92`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

## Línea base integrada

- Fases 1 a 6 integradas en `main`.
- EditMode funcional confirmado: **124/124**.
- PlayMode confirmado: **6/6**.
- Catálogo e inventario de dominio integrados.
- `FarmSimulator.Domain` permanece independiente de `UnityEngine`.

## Bloque artístico A1 — validado

- Cinco hojas fuente Cozy Farm versionadas en `Pilot/Source`.
- Configuración: Sprite, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.
- Arturo confirmó que la importación y todas las pruebas quedaron OK después del push.
- El héroe actual permanece intacto en `Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png`.

## Bloque artístico A2 — pendiente de validación

Se añadieron slices explícitos y nombrados, sin duplicar ni alterar los PNG:

- 3 objetos: nabo, zanahoria y col;
- 3 bolsas de semillas;
- 18 etapas de cultivo: 6 por cultivo;
- 4 tiles piloto: césped, tierra, agua y tierra labrada;
- 6 pruebas EditMode nuevas en `CozyFarmPilotArtTests.cs`.

Decisiones:

- `turnip` usa provisionalmente el gráfico denominado `radish` por el paquete.
- `cabbage` usa provisionalmente el gráfico denominado `lettuce`.
- Los IDs de dominio no cambian.
- `tools.png` contiene máquinas, no iconos apropiados de azada o regadera; queda sin slicing.
- La prueba de `tools.png` fue endurecida en `d88eeacb38ff50963b31d1d8a0f2c91b4297bf49` para comprobar que no existan aliases falsos, sin depender de cómo Unity represente internamente un Sprite Single.
- La bitácora transaccional fue cerrada en `57fadeeca15792e880d3c1ea5e2c594b43229b77`.
- No se crean todavía Tilemaps, paletas, prefabs, escenas, hotbar ni conexiones funcionales.

## Próxima acción

1. En GitHub Desktop, hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
2. Abrir Unity y esperar la reimportación de `items`, `seeds`, `crops` y `tiles`.
3. Confirmar visualmente 3 sprites de objetos, 3 de semillas, 18 de cultivos y 4 de terreno.
4. Ejecutar EditMode completo; esperado **130/130**.
5. Ejecutar PlayMode completo; esperado **6/6**.
6. Confirmar que los cultivos usan pivote inferior central y evaluar los aliases visuales provisionales.
7. Reportar resultados antes de crear Tilemaps, UI o integración con inventario.

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
- No afirmar que A2 compila o que las pruebas pasan hasta recibir la validación de Arturo.
- Después de cada implementación, corrección o integración, mantener la documentación sincronizada.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. La Fase 6 está integrada. A1 de Cozy Farm fue validado con la línea base 124/124 EditMode y 6/6 PlayMode. A2 está implementado funcionalmente en d88eeacb38ff50963b31d1d8a0f2c91b4297bf49 y documentado hasta 57fadeeca15792e880d3c1ea5e2c594b43229b77: slices de 3 objetos, 3 semillas, 18 etapas de cultivo y 4 tiles, más 6 pruebas EditMode. La validación esperada es 130/130 EditMode y 6/6 PlayMode. Conserva el héroe y no avances a Tilemaps o UI antes del reporte de Arturo.
```
