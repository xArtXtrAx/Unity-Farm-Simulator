# Cozy Farm — piloto de recepción artística

## Procedencia

- Paquete: **Cozy Farm**.
- Autor acreditado en el archivo adquirido: **shubibubi**.
- Copia comprada y proporcionada por Arturo García el 5 de agosto de 2026.
- Archivo fuente local: `full version.zip` (no versionado).
- Estos recursos son de terceros y no deben redistribuirse como paquete independiente.

## Alcance de este piloto

Este directorio contiene únicamente cinco hojas PNG originales elegidas para evaluar compatibilidad visual y técnica. No incluye el ZIP, GIF, animales, edificios, enemigos, máquinas, variantes estacionales completas ni personajes.

| Archivo versionado | Ruta original en el ZIP | Dimensiones | Bytes | SHA-256 |
|---|---|---:|---:|---|
| `items.png` | `full version/ui/items.png` | 160 × 192 | 27576 | `56fa2dc04d0d3fe5e4c43ee81d294b6922c684aed1ea0aa8a50e6a0af68b8fab` |
| `seeds.png` | `full version/farming/seeds.png` | 112 × 96 | 7606 | `c702d1596d76867ad6e92bd060500ec6e12ab3e010c4b3cf3e459932674fe8dc` |
| `tools.png` | `full version/farming/tools.png` | 592 × 64 | 6232 | `8bbe499b1373a7dd333a037ec5ee223e6cd03443d72b5b3643a4349ab612a0b1` |
| `crops.png` | `full version/farming/crops.png` | 96 × 592 | 24888 | `19797cad992769348ec3f22b4952447997d09383a259083c7a1392d6d1532a74` |
| `tiles.png` | `full version/tiles/tiles.png` | 864 × 800 | 175337 | `cd3c0a91c3466ea54061f5fb008bb77aa8dc257e0d027f3d0505dc4e35b8f49e` |

## Importación inicial

Las cinco hojas se publican sin edición destructiva y con una configuración conservadora:

- Texture Type: Sprite (2D and UI).
- Sprite Mode: Single durante la recepción inicial.
- Pixels Per Unit: 16.
- Filter Mode: Point.
- Mip Maps: desactivados.
- Compresión: desactivada en la plataforma por defecto.
- Wrap Mode: Clamp.
- Alpha Is Transparency: activado.

`Sprite Mode: Single` es deliberadamente temporal: permite validar importación, escala, nitidez y compatibilidad antes de definir reglas de slicing por hoja. No se deben crear nombres de sprites ni rectángulos definitivos hasta revisar cada cuadrícula en Unity.

## Decisiones vigentes

- El héroe actual permanece en `Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png`.
- No se modifican su prefab, Animator, clips, pivote, collider ni profundidad.
- No se crean escenas, prefabs, Tilemaps, paletas, ScriptableObjects ni UI en este bloque.
- No se usa `global.png`; las hojas especializadas serán la fuente preferida para evitar duplicados.
- No se incluirá `item_carry.png` hasta decidir cómo se representarán objetos sostenidos por el héroe existente.

## Validación local requerida

1. Sincronizar la rama `chore/cozy-farm-art-intake`.
2. Abrir el proyecto con Unity `6000.3.21f1` y esperar la importación.
3. Confirmar que no aparecen errores ni ciclos de reimportación.
4. Revisar los cinco importadores y confirmar Point, 16 PPU, sin mipmaps y sin compresión.
5. Comparar visualmente el héroe actual con objetos y tiles a escala mundial equivalente.
6. Ejecutar EditMode completo; línea base esperada, todavía no confirmada después de la importación: 124/124.
7. Ejecutar PlayMode completo como regresión; línea base esperada, todavía no confirmada: 6/6.
8. No comenzar slicing masivo hasta registrar el resultado de esta validación.
