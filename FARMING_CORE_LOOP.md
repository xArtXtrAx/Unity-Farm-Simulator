# Primera vertical agrícola

## Recorrido

```text
seleccionar azada → arar parcela
seleccionar semillas → sembrar
seleccionar regadera → regar
entrar en casa → dormir
volver a la parcela → observar crecimiento
repetir riego y sueño → cosechar
```

## Controles

- `1–8`: seleccionar espacio de la hotbar.
- Rueda del ratón o `L1/R1`: cambiar selección.
- `E` o botón sur del gamepad: usar el objeto seleccionado sobre una parcela.

## Reglas

- Una parcela debe ararse antes de regarse o sembrarse.
- Cada bolsa sembrada consume una semilla del inventario.
- Un cultivo solo crece al avanzar el día si la parcela estaba regada.
- La humedad se consume al comenzar el nuevo día.
- La cosecha se añade al inventario y la parcela permanece arada.
- La granja y el inventario viven en `GameSessionRuntime`, por lo que sobreviven al cambio entre `Farm` y `HouseInterior`.

## Arte

La primera parcela utiliza los sprites versionados del piloto Cozy Farm:

- `cozy_grass`
- `cozy_tilled_soil`
- etapas 0–5 de nabo, zanahoria y col

## Generación

El campo se aplica automáticamente a `Farm.unity`. También puede forzarse desde:

```text
Tools > Farm Simulator > Apply Farming Field To Farm Scene
```

El generador añade una cuadrícula de 3 × 3 parcelas bajo:

```text
Farming Core Loop v1
└── Farm Plot Field
```

## Validación

1. Actualizar la rama `agent/farming-core-loop`.
2. Esperar la compilación y la actualización de `Farm`.
3. Ejecutar todas las pruebas EditMode.
4. Abrir `Farm` directamente y validar el recorrido completo.
5. Ejecutar las pruebas PlayMode para confirmar que la casa y el sueño no sufrieron regresiones.
