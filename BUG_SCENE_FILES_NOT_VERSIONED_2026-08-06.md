# BUG-0015 — Farm y HouseInterior no estaban versionadas

- **Estado:** CORREGIDO, pendiente de validación local
- **Severidad:** S1 — Crítica
- **Detectado:** 2026-08-06
- **Sistema:** escenas / control de versiones / autoría de Tilemaps
- **Rama:** `dev/farm-scene-authoring`
- **Reportado por:** Arturo

## Comportamiento observado

Después de cambiar a `dev/farm-scene-authoring`, Unity no mostraba `Farm.unity` ni `HouseInterior.unity`. GitHub tampoco contenía estas escenas en `main`, en la rama activa ni en las ramas históricas revisadas.

## Causa raíz

Las escenas se habían generado y editado únicamente en la copia local, pero nunca se añadieron a Git junto con sus archivos `.meta`. El proyecto conservaba un generador automático antiguo que podía recrearlas, pero ese generador usaba una composición anterior incompatible con el Tile Palette y la arquitectura moderna.

## Riesgo

- pérdida de trabajo de autoría al cambiar de copia o limpiar el workspace;
- clones limpios sin escenas principales;
- regeneración accidental con tiles y sprites obsoletos;
- Build Settings apuntando a escenas inexistentes.

## Solución aplicada

- `HouseAndSleepScenePipeline` dejó de generar automáticamente y quedó como stub Legacy.
- Se creó `ModernFarmSceneAuthoring`, con generación explícita, capas modernas y opción de backup antes de reemplazar.
- Se sustituyeron las pruebas que ejecutaban el reset antiguo por pruebas de la jerarquía moderna.
- Se documentó que las escenas generadas y sus `.meta` deben versionarse inmediatamente.

## Validación pendiente

1. Compilar el proyecto localmente.
2. Generar ambas escenas mediante Scene Recovery.
3. Confirmar que Farm contiene `Ground`, `Paths`, `Soil` y `Decoration`.
4. Confirmar portales, spawns, cama, cámara y límites.
5. Ejecutar EditMode y PlayMode completos.
6. Confirmar que los cuatro archivos de escena quedan rastreados por Git.

No pasar a **VERIFICADO** hasta recibir confirmación local de Arturo.
