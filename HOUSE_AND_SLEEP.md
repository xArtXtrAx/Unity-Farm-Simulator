# Casa del héroe y ciclo de descanso

## Estado

Primera implementación en revisión de la fase `house-sleep-day-cycle`.

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
- Exterior provisional de la casa e interior construidos con los sprites
  curados `cozy_grass` y `cozy_dirt`.
- Prefab real del héroe, con raíz y collider conservados en escala 1.0.
- Visual del héroe a 1.5 únicamente en las instancias de estas escenas.
- Interacción frontal por alcance, dirección, tolerancia lateral y prioridad.
- Tecla `E` y botón sur del gamepad/DualSense.
- Entrada y salida mediante spawns identificados.
- Cama interactiva y avance de calendario.
- Calendario puro: 28 días por estación y cuatro estaciones por año.
- HUD con fecha y aviso de interacción.
- Pruebas EditMode para el calendario.
- Pruebas PlayMode para entrada, salida, descanso y spawns.

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
2. Confirmar que `Farm` y `HouseInterior` fueron generadas y añadidas a Build Settings.
3. Ejecutar todas las pruebas EditMode y PlayMode.
4. Abrir `Farm` y entrar en Play Mode.
5. Acercarse mirando hacia la puerta y pulsar `E` o `X`.
6. Confirmar que el héroe aparece dentro junto a la entrada.
7. Acercarse mirando hacia la cama y dormir.
8. Confirmar que la fecha avanza y el héroe despierta junto a la cama.
9. Salir y confirmar que reaparece frente a la casa.
10. Verificar teclado, DualSense, movimiento, collider y hotbar existente en `Lab`.

## Exclusiones

Esta fase todavía no incluye:

- fundido o animación de sueño;
- reloj intradía;
- energía;
- guardado;
- crecimiento de cultivos;
- arado, riego, siembra o cosecha;
- cofre;
- arte completo de edificio o mobiliario fuera del piloto curado;
- cambio de `Bootstrap` para iniciar en `Farm`.

Después de la validación, la fase puede incorporar las escenas generadas al
commit de cierre, actualizar la bitácora y cambiar el arranque de `Bootstrap`
en un paso separado y fácil de revertir.
