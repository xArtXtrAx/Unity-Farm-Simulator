# CONTINUIDAD GPT — Unity Farm Simulator

Este archivo es el **punto de entrada permanente** para retomar el desarrollo sin depender de la memoria de un chat.

> Un chat nuevo debe leer este archivo desde `main` antes de buscar la bitácora técnica.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/player-prefab-depth-sorting`
- **Fase actual:** Fase 5 — Prefab reutilizable y profundidad visual
- **Estado:** implementación compila y genera recursos; validación automatizada y manual pendiente
- **Cabeza remota registrada de la rama activa:** `4bcc9dc50ce97bf6444716751237c40204c5d7f7`
- **Bugs activos:** ninguno
- **Último bug verificado:** `BUG-0005` — colisión entre `FarmSimulator.Application` y `UnityEngine.Application`
- **Corrección funcional:** `a9a3fe23e214f5d98ba8f9ac8c5677d131b0f036`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

### Validación confirmada

- Unity abre normalmente fuera de Safe Mode.
- Consola: **0 errores y 0 advertencias**.
- Player prefab reutilizable generado.
- Sorting layers cenitales generadas.

### Validación pendiente

- EditMode esperado: **36/36**.
- PlayMode esperado: **6/6**.
- Movimiento y animación con teclado y DualSense.
- Límites visuales.
- Cambio de `sortingOrder` según la Y de los pies.
- Revisión de cambios locales generados antes de commit.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Cambiar conceptualmente a la **rama activa** indicada arriba.
3. Leer `BITÁCORA_GPT.MD` desde la rama activa.
4. Leer `BUGS.MD` desde la rama activa.
5. Leer `MIGRACION_DESDE_FARMING_GAME_A.MD`.
6. Comparar `main` con la rama activa y revisar sus commits recientes.
7. Continuar desde la sección **“Próximo paso exacto”** de la bitácora.

---

## Reglas críticas

- No asumir que una implementación o corrección pasó pruebas locales cuando la bitácora indique que está pendiente.
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

Lee primero CONTINUIDAD_GPT.md desde main. Después sigue exactamente el orden de lectura y el “Próximo paso exacto” que ahí se indican. No asumas que las pruebas locales pendientes ya pasaron. Tras cada implementación o corrección, actualiza BITÁCORA_GPT.MD y mantén actualizado CONTINUIDAD_GPT.md cuando cambie la rama activa, la fase o el estado.
```

---

## Mantenimiento de este archivo

Este documento debe ser breve. No sustituye a la bitácora técnica.

Actualizar únicamente:

- rama activa;
- fase actual;
- estado de implementación/validación;
- commit remoto de referencia;
- conteos de pruebas esperados o validados;
- cualquier advertencia necesaria para reanudar sin ambigüedad.
