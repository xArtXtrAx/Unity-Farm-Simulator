# Cozy Farm — plan de recepción piloto

## Estado

- Rama: `chore/cozy-farm-art-intake`.
- El héroe actual se conserva sin cambios.
- El paquete completo `full version.zip` permanece fuera del repositorio.
- Se preparó un bundle local con cinco hojas PNG, sus `.meta` y documentación.
- Los binarios deben copiarse en el checkout local y publicarse mediante GitHub Desktop antes de abrir Unity.

## Archivos piloto

```text
Assets/_Project/Art/ThirdParty/CozyFarm/
├── README.md
└── Pilot/
    └── Source/
        ├── items.png
        ├── seeds.png
        ├── tools.png
        ├── crops.png
        └── tiles.png
```

Configuración inicial: Sprite, modo Single temporal, 16 PPU, Point, sin mipmaps, Clamp, transparencia habilitada y sin compresión por defecto.

## Exclusiones

No incluir ZIP, GIF, `global.png`, `item_carry.png`, personajes, animales, edificios, enemigos, máquinas ni variantes estacionales completas. No crear todavía slicing masivo, Tilemaps, paletas, prefabs, escenas o UI.

## Flujo local

1. Cambiar a esta rama y hacer Fetch/Pull.
2. Extraer el bundle en la raíz del repositorio.
3. Comprobar el alcance en GitHub Desktop, hacer un único commit y Push origin.
4. Abrir Unity y validar importación y escala.
5. Ejecutar EditMode y PlayMode completos.
6. Reportar resultados antes de continuar.
