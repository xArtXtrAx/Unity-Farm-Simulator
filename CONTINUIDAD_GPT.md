# CONTINUIDAD GPT — Unity Farm Simulator

Este archivo es el **punto de entrada permanente** para retomar el desarrollo sin depender de la memoria de un chat.

> Un chat nuevo debe leer este archivo desde `main` antes de buscar la bitácora técnica.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/player-prefab-depth-sorting`
- **Fase actual:** Fase 5 — Prefab reutilizable y profundidad visual
- **Estado:** implementación y corrección visual terminadas remotamente; compilación, pruebas y validación local pendientes
- **Cabeza remota registrada de la rama activa:** `4e89b1313a7c5d064827911164ff1b859cf46537`
- **Bug activo:** `BUG-0006` — faltaba una referencia 2D compatible con el sorting; estado CORREGIDO, pendiente de validación
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

### Validación ya confirmada

- Unity abre normalmente fuera de Safe Mode.
- Consola previa: **0 errores y 0 advertencias**.
- Player prefab reutilizable generado.
- Sorting layers cenitales generadas.
- `Order in Layer` del jugador varía aproximadamente entre **9600 y 10400**.
- El cálculo numérico por Y funciona en la dirección esperada.

### Corrección pendiente de validar

- Se añadió `Depth Sorting Reference`, un rectángulo morado 2D sin collider.
- Usa la misma sorting layer y el mismo cálculo por pies que el jugador.
- Al acercarse desde abajo, el jugador debe verse delante.
- Tras cruzar hacia arriba la línea de pies de la referencia, debe verse detrás.
- EditMode esperado: **36/36**.
- PlayMode esperado: **6/6**.
- Consola esperada: **0 errores y 0 advertencias**.

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

Mantener breve. Actualizar únicamente rama activa, fase, estado, commit remoto, pruebas, cambios locales y cualquier advertencia necesaria para reanudar sin ambigüedad.
