# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head remoto registrado:** `633a85c92f890027c79e418a9e45955074874adc`
- **Head funcional A3:** `0a05af5a50538e31acdd849d7eb603d4a6096c76`
- **Bloque actual:** A3 — escena artística de exhibición Cozy Farm
- **Estado:** generador corregido y cuatro pruebas EditMode implementados remotamente; generación local de la escena y validación pendientes
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
- Slicing aprobado: 3 objetos, 3 semillas, 18 etapas de cultivo y 4 tiles.
- `tools.png` permanece Single y sin cortes porque no contiene iconos apropiados de azada o regadera.
- Alias reversibles: `turnip` usa provisionalmente arte `radish`; `cabbage`, arte `lettuce`.
- Validación final A2 de Arturo: **130/130 EditMode**, **6/6 PlayMode**, sin errores.

## Cozy Farm A3 — pendiente de validación

Se añadió un generador Editor reproducible:

```text
Assets/_Project/Scripts/Editor/CozyFarmShowcaseScenePipeline.cs
```

Al importar la rama, genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

La escena es independiente de `Lab` y muestra:

- el prefab actual del héroe;
- fondo de césped;
- cuatro muestras de terreno;
- tres objetos cosechados y tres bolsas de semillas;
- tres filas con seis etapas de crecimiento por cultivo.

Correcciones preventivas aplicadas:

- la cámara reutiliza `SpatialModel.CameraOrthographicSize` (**4.21875**) en lugar de un valor duplicado;
- los fondos y muestras usan parches de sprites individuales, no `SpriteDrawMode.Tiled`, evitando dependencias de malla Full Rect y advertencias con sprites Tight.

También se añadieron cuatro pruebas en `CozyFarmShowcaseSceneTests.cs` para verificar generación/firma, grupos curados, prefab del héroe y cámara conforme al contrato central.

No se añadieron Tilemaps, paletas, UI, hotbar, lógica funcional ni cambios al dominio.

## Próxima acción

1. En GitHub Desktop, hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
2. Abrir Unity y esperar compilación/importación.
3. Confirmar que aparezca `Assets/_Project/Scenes/CozyFarmShowcase.unity`.
4. Abrir esa escena y pulsar Play; `Lab` no debe cambiar.
5. Evaluar escala y compatibilidad visual del héroe con terreno, objetos, semillas y cultivos.
6. Ejecutar EditMode completo; esperado **134/134**.
7. Ejecutar PlayMode completo; esperado **6/6**.
8. Si Unity genera `CozyFarmShowcase.unity` y su `.meta` como cambios locales, no hacer commit todavía: reportar primero la apariencia y los resultados.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `BITÁCORA_GPT.MD` desde `chore/cozy-farm-art-intake`.
3. Leer `COZY_FARM_INTAKE.md` desde esa rama.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas y commits recientes.
6. Continuar desde la actualización posterior de la bitácora y el próximo paso de `COZY_FARM_INTAKE.md`.

## Reglas críticas

- No añadir funcionalidad de juego a la rama artística.
- No reemplazar el héroe actual.
- No subir el ZIP completo ni GIF de referencia.
- No hacer slicing masivo; solo recursos con consumidor o prueba definida.
- No asignar imágenes falsas a azada o regadera.
- No afirmar que A3 compila, genera la escena o pasa pruebas hasta recibir la validación de Arturo.
- Después de cada implementación, corrección o integración, mantener la documentación sincronizada.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. La Fase 6 está integrada. Cozy Farm A1 y A2 están validados con 130/130 EditMode, 6/6 PlayMode y cero errores. A3 está implementado funcionalmente en 0a05af5a50538e31acdd849d7eb603d4a6096c76 y documentado hasta 633a85c92f890027c79e418a9e45955074874adc: un pipeline Editor genera Assets/_Project/Scenes/CozyFarmShowcase.unity y cuatro pruebas nuevas elevan el esperado a 134/134 EditMode. La cámara usa SpatialModel.CameraOrthographicSize y los parches no usan SpriteDrawMode.Tiled. La generación local, inspección visual y pruebas A3 siguen pendientes. Conserva el héroe y no avances a Tilemaps o UI antes del reporte de Arturo.
```
