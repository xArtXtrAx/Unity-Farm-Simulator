# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head remoto registrado:** `c65e2fea77502e2bd6cba98af089add0d439c5c9`
- **Head funcional A3.1:** `39abe438bb6068b21438fb836b5eea01295f0db3`
- **Bloque actual:** A3.1 — reequilibrio de escala y composición de la exhibición Cozy Farm
- **Estado:** implementado y documentado remotamente; regeneración, inspección visual y pruebas locales pendientes
- **A3 original:** validado técnicamente con **134/134 EditMode**, **6/6 PlayMode** y cero errores; composición rechazada visualmente por desproporción
- **Bloque A2:** validado localmente con **130/130 EditMode**, **6/6 PlayMode** y cero errores
- **Commit de assets fuente publicado por Arturo:** `e4540b42d275b650f726bad41d4546787ae544e9`
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
- El héroe actual, su prefab, animaciones, pivote, collider y sorting permanecen intactos.

## Cozy Farm A1 y A2 — validados

- Cinco hojas fuente versionadas en `Pilot/Source`.
- Configuración pixel-art: Sprite, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.
- Slicing aprobado: 3 objetos, 3 semillas, 18 etapas de cultivo y 4 muestras de terreno.
- `tools.png` permanece Single y sin cortes porque no contiene iconos apropiados de azada o regadera.
- Alias reversibles: `turnip` usa provisionalmente arte `radish`; `cabbage`, arte `lettuce`.
- Validación final A2 de Arturo: **130/130 EditMode**, **6/6 PlayMode**, sin errores.

## Cozy Farm A3 — validado técnicamente

El generador Editor crea localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

La primera composición conservó correctamente la cámara, el contenido curado y el prefab actual del héroe. Arturo confirmó:

- EditMode: **134/134**;
- PlayMode: **6/6**;
- errores: **0**.

La captura visual mostró:

- iconos de inventario tratados como objetos físicos de mundo;
- bases circulares repetidas bajo las 18 etapas;
- muestras 2×2 sobredimensionadas;
- exceso de espacio vacío.

Por ello A3 quedó aprobada técnicamente, pero no visualmente.

## Cozy Farm A3.1 — pendiente de validación

El generador y sus pruebas fueron reequilibrados:

- firma `cozy-farm-showcase-scene-v2` para regenerar la escena;
- objetos y semillas a escala **0.55** sobre una referencia de 3×2 slots;
- cultivos y héroe a escala de mundo **1.0**;
- cama compartida de tierra de **6×3 tiles**;
- eliminación de los 18 objetos `soil_for_*`;
- cuatro muestras de terreno de **un tile** cada una;
- héroe intacto sobre referencia de **2×2 tiles**;
- distribución compactada dentro del encuadre 960×540;
- protección contra sobrescritura si la escena anterior está abierta.

`cozy_tilled_soil` no fue recortado de nuevo: queda como muestra aislada provisional, pero ya no se repite bajo cada cultivo.

`CozyFarmShowcaseSceneTests.cs` pasa de cuatro a **seis** casos. Los dos nuevos verifican roles de escala y composición compartida.

No se modificaron PNG, `.meta` artísticos, `Lab`, prefab/spritesheet del héroe, Domain, inventario, Input System, Tilemaps, paletas o hotbar.

## Próxima acción

1. Cerrar `CozyFarmShowcase` en Unity antes de actualizar la rama.
2. En GitHub Desktop, hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
3. Abrir Unity y esperar compilación/importación.
4. La firma `v2` debe regenerar `Assets/_Project/Scenes/CozyFarmShowcase.unity`.
5. Si Unity avisa que la escena antigua estaba abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Abrir la escena, pulsar Play y tomar una captura.
7. Confirmar iconos menores, filas compactas sin círculos repetidos, cuatro muestras individuales y héroe intacto sobre 2×2 tiles.
8. Ejecutar EditMode completo; esperado **136/136**.
9. Ejecutar PlayMode completo; esperado **6/6**.
10. No hacer commit todavía de la escena generada: reportar primero apariencia, conteos y cualquier error o advertencia.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `BITÁCORA_GPT.MD` desde `chore/cozy-farm-art-intake`.
3. Leer `COZY_FARM_INTAKE.md` desde esa rama.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas y commits recientes.
6. Continuar desde la actualización posterior más reciente de la bitácora y el próximo paso de `COZY_FARM_INTAKE.md`.

## Reglas críticas

- No añadir funcionalidad de juego a la rama artística.
- No reemplazar el héroe actual.
- No subir el ZIP completo ni GIF de referencia.
- No hacer slicing masivo; solo recursos con consumidor o prueba definida.
- No asignar imágenes falsas a azada o regadera.
- No afirmar que A3.1 pasa pruebas o está aprobada visualmente hasta recibir la validación de Arturo.
- No hacer commit de la escena generada antes de la revisión visual.
- Después de cada implementación, corrección o integración, mantener la documentación sincronizada.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. La Fase 6 está integrada. Cozy Farm A1 y A2 están validados. A3 pasó 134/134 EditMode y 6/6 PlayMode sin errores, pero su composición fue rechazada por desproporción. A3.1 está implementado funcionalmente en 39abe438bb6068b21438fb836b5eea01295f0db3 y documentado hasta c65e2fea77502e2bd6cba98af089add0d439c5c9: iconos a 0.55, cultivos y héroe a 1.0, cama compartida 6×3, muestras individuales y dos pruebas nuevas. El esperado es 136/136 EditMode y 6/6 PlayMode. Conserva el héroe y no avances a Tilemaps o hotbar antes del reporte visual de Arturo.
```
