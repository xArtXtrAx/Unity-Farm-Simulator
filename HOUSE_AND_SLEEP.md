# Casa del héroe y ciclo de descanso

## Estado

Implementación en revisión de la fase `house-sleep-day-cycle`.

La fase añade un recorrido funcional independiente:

```text
Farm → puerta de la casa → HouseInterior → cama → nuevo día → Farm
```

La línea estable de `main` permanece sin modificaciones. `Bootstrap` continúa
cargando `Lab` mientras esta fase se valida visualmente y con pruebas locales.

## Alcance

- Escenas generadas `Farm` y `HouseInterior`.
- Inclusión automática de ambas escenas en Build Settings después de
  `Bootstrap` y `Lab`.
- Cabaña modular construida con sprites reales del atlas Cozy Farm disponible
  en el repositorio: paneles de madera, porche, flores, banco, lámpara,
  arbustos, árbol, rocas y cerca.
- Interior construido con paneles de madera y mobiliario compuesto del mismo
  atlas, sin rectángulos de terreno recoloreados.
- Cama reconocible construida con panel, banco y cerca del atlas disponible.
- Importador reproducible para los recortes de terreno y arte de la cabaña.
- Prefab real del héroe, con raíz y collider conservados en escala 1.0.
- Visual del héroe a 1.5 únicamente en las instancias de estas escenas.
- Interacción frontal por alcance, dirección, tolerancia lateral y prioridad.
- Tecla `E` y botón sur del gamepad/DualSense.
- Entrada y salida mediante spawns identificados.
- Cama interactiva y avance de calendario.
- Calendario puro: 28 días por estación y cuatro estaciones por año.
- HUD con fecha y aviso de interacción.
- Pruebas EditMode para calendario, importación del atlas y escenas generadas.
- Pruebas PlayMode para entrada, salida, descanso y spawns.

## Arte disponible

El piloto versionado contiene `tiles.png`, además de hojas de cultivos, semillas,
ítems y herramientas. No contiene la hoja separada de edificios ni Cozy
Interior. Por esa razón esta revisión usa exclusivamente piezas reales ya
presentes en `tiles.png` y compone una cabaña modular, en lugar de copiar o
redistribuir archivos externos que no forman parte del repositorio.

Los recortes se administran desde:

```text
Assets/_Project/Scripts/Editor/CozyFarmHouseArtPipeline.cs
```

Puede reconstruirse su importación manualmente desde:

```text
Tools > Farm Simulator > Rebuild Cozy Farm House Art
```

## Generación de escenas

Al abrir la rama en Unity, el pipeline genera o actualiza:

```text
Assets/_Project/Scenes/Farm.unity
Assets/_Project/Scenes/HouseInterior.unity
```

También pueden reconstruirse manualmente:

```text
Tools > Farm Simulator > Rebuild House and Sleep Scenes
```

Las escenas usan firmas de importación para que la generación sea idempotente.

## Controles

- Movimiento: controles existentes.
- Interacción: `E`.
- DualSense/gamepad: botón sur (`X` en DualSense).

## Spawns

```text
FarmStart
FarmHouseDoor
HouseEntrance
HouseBedWake
```

## Validación local requerida

1. Abrir Unity `6000.3.21f1` y esperar a que termine la importación.
2. Cerrar `Farm` y `HouseInterior` si estaban abiertas durante la actualización.
3. Confirmar que ambas escenas fueron regeneradas con firmas `v2`.
4. Ejecutar todas las pruebas EditMode y PlayMode.
5. Abrir `Farm` y entrar en Play Mode.
6. Revisar el exterior modular y entrar con `E` o `X`.
7. Confirmar que el héroe aparece dentro junto a la entrada.
8. Acercarse mirando hacia la cama y dormir.
9. Confirmar que la fecha avanza y el héroe despierta junto a la cama.
10. Salir y confirmar que reaparece frente a la casa.
11. Verificar teclado, DualSense, movimiento, collider y hotbar existente en `Lab`.

## Exclusiones

Esta fase todavía no incluye:

- fundido o animación de sueño;
- reloj intradía;
- energía;
- guardado;
- crecimiento de cultivos;
- arado, riego, siembra o cosecha;
- cofre funcional;
- hojas externas de edificios o Cozy Interior que no están versionadas;
- cambio de `Bootstrap` para iniciar en `Farm`.

Después de la validación visual, la fase puede incorporar las escenas generadas
al commit de cierre, actualizar la bitácora y cambiar el arranque de `Bootstrap`
en un paso separado y fácil de revertir.
