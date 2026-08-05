# CONTINUIDAD GPT — Unity Farm Simulator

Este archivo es el **punto de entrada permanente** para retomar el desarrollo sin depender de la memoria de un chat.

> Un chat nuevo debe leer este archivo desde `main` antes de buscar la bitácora técnica.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/player-prefab-depth-sorting`
- **Fase actual:** Fase 5 — Prefab reutilizable y profundidad visual
- **Estado:** implementación y validación local completas; revisión y versionado de recursos generados pendientes
- **Cabeza remota registrada de la rama activa:** `1f21bccb73015e4c7bfa7cde93f8deab65114540`
- **Bugs activos:** ninguno
- **Último bug verificado:** `BUG-0006` — faltaba una referencia 2D compatible con el sorting
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

### Validación confirmada

- Unity abre normalmente fuera de Safe Mode.
- Player prefab reutilizable generado.
- Sorting layers cenitales generadas.
- `Order in Layer` del jugador varía aproximadamente entre **9600 y 10400**.
- El rectángulo morado `Depth Sorting Reference` comparte el sistema de sorting por pies.
- Al caminar por encima de su línea de pies, el personaje queda detrás.
- Al regresar por debajo, vuelve a renderizarse delante.
- EditMode: **36/36**.
- PlayMode: **6/6**.
- Todas las pruebas pasaron sin errores.

### Estado local no protegido todavía

Unity generó o actualizó localmente:

- `ProjectSettings/TagManager.asset`;
- Player prefab y su `.meta`;
- posibles metadatos de carpetas asociados.

Estos archivos todavía deben revisarse en GitHub Desktop antes de hacer commit y push.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Cambiar conceptualmente a la rama activa indicada arriba.
3. Leer `BITÁCORA_GPT.MD` desde la rama activa.
4. Leer `BUGS.MD` desde la rama activa.
5. Leer `MIGRACION_DESDE_FARMING_GAME_A.MD`.
6. Comparar `main` con la rama activa y revisar commits o PR recientes.
7. Continuar desde **“Próximo paso exacto”** en la bitácora.

---

## Reglas críticas

- No asumir que los cambios locales pendientes ya fueron versionados.
- No modificar el repositorio antes de leer la documentación indicada.
- `main` debe mantenerse estable.
- Los cambios locales sin commit/push no están protegidos por este sistema y deben comunicarse antes de cambiar de chat.
- Después de cada implementación o corrección realizada por GPT, actualizar `BITÁCORA_GPT.MD` en el mismo bloque.
- Después de cada validación local del usuario, registrar el resultado antes de continuar.
- Cada vez que cambie la rama activa, la fase o el estado de validación, actualizar este archivo en `main`.

---

## Prompt mínimo para un chat nuevo

```text
Continúa trabajando en mi repositorio:
https://github.com/xArtXtrAx/Unity-Farm-Simulator

Lee primero CONTINUIDAD_GPT.md desde main. Después sigue exactamente el orden de lectura y el “Próximo paso exacto” que ahí se indican. No asumas que los cambios locales pendientes ya fueron versionados. Tras cada implementación o corrección, actualiza BITÁCORA_GPT.MD y mantén actualizado CONTINUIDAD_GPT.md cuando cambie la rama activa, la fase o el estado.
```

---

## Mantenimiento de este archivo

Mantener breve. Actualizar únicamente rama activa, fase, estado, commit remoto, pruebas, cambios locales y cualquier advertencia necesaria para reanudar sin ambigüedad.
