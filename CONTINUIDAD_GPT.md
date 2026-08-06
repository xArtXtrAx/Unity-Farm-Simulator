# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Head integrado más reciente:** `794aab44916f974bc885aec0a203823041c72f8f`
- **Integración más reciente:** PR #15 — `Consolidate seasonal tiles, scene sizing and collider authoring`
- **Rama activa:** `dev/farm-scene-authoring`
- **Primer commit documental de la rama activa:** `07e0d1b04e696200c4e4961787a036770982089a`
- **Estado de la rama activa:** abierta para continuar el desarrollo desde el `main` posterior al PR #15
- **Bugs activos documentados:** consultar `BUGS.MD`; no asumir que un bug está verificado sin confirmación local de Arturo

## Regla de continuidad

Antes de modificar el repositorio:

1. Leer este archivo desde `main`.
2. Leer `BITÁCORA_GPT.MD` desde la rama activa cuando exista; si no, desde `main`.
3. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
4. Leer la bitácora específica del sistema que se vaya a tocar, cuando exista.
5. Revisar ramas, commits y PR recientes antes de escribir.

Después de cada implementación, corrección o integración realizada por GPT, actualizar `BITÁCORA_GPT.MD` con:

- lo realizado;
- lo comprobado;
- lo pendiente;
- commits y PR asociados;
- siguiente paso exacto.

Ningún cambio se considera cerrado documentalmente hasta completar ese registro.

---

## Línea base integrada

El repositorio ya contiene, entre otros bloques:

- arquitectura por capas y `FarmSimulator.Domain` independiente de `UnityEngine`;
- cámara ortográfica, plano lógico XY y Physics 2D;
- movimiento por teclado y DualSense;
- héroe animado, prefab reutilizable y profundidad por Y;
- catálogo e inventario de dominio;
- hotbar de ocho slots;
- recepción y slicing curado del paquete Cozy Farm;
- escenas `Farm` y `HouseInterior`;
- agricultura inicial con suelo en Tilemap y cultivos como entidades visuales;
- Farm Development Kit para tiles, edificios, huellas, colliders y tamaño de escena.

## Decisiones técnicas y visuales vigentes

- Conservar el héroe actual; no reemplazarlo automáticamente.
- No subir el ZIP completo de Cozy Farm ni sus GIF.
- Mantener la raíz y el collider del jugador en escala 1.0.
- Mantener `FarmSimulator.Domain` sin dependencia de `UnityEngine`.
- Separar responsabilidades visuales y físicas:
  - terreno y caminos en Tilemaps;
  - cultivos runtime mediante `SpriteRenderer`;
  - footprint lógico separado del collider físico y del sprite del edificio.
- No utilizar el antiguo reset automático de Farm; quedó desactivado por ser destructivo.
- No marcar bugs ni incrementos como **VERIFICADOS** antes de una confirmación local explícita.

---

## Integración reciente — PR #15

PR #15 fue fusionado en `main` mediante:

```text
794aab44916f974bc885aec0a203823041c72f8f
```

Incluye los siguientes incrementos:

### Selector estacional de tiles

- Navegador categorizado de tiles Cozy Farm por estación.
- Miniaturas reales de los tiles.
- Arrastre directo desde el navegador hacia Scene.
- Correcciones de compilación y metadatos asociados.
- Limpieza del foco de Tile Palette antes de entrar en Play para evitar estados visuales incorrectos.

Commits relevantes:

```text
34cd57780d008bca65201a1e071b0e69ce80ce0e
bbdb64b6ecfd9fa5bfffc40343c844fd6843be62
186f6e66cab7f2079e7c918c9ae2d089160e431a
cc42802702def1cccda4ef9579bde9ffcdf34c84
3818ecf344c0e6cdca518d4073f193a96216ce16
6295b506eaff833f88a986447a30c83318d11c3a
```

### Protección frente al reset destructivo de Farm

- El antiguo `FarmSceneGridLayoutResetter` dejó de aplicarse automáticamente.
- La acción quedó relegada a un menú Legacy con confirmación explícita.
- No recomendar ni ejecutar el reset heredado para ajustar el tamaño de una escena.

Commit:

```text
7711e95ac1994c526133d6074683c70ee63a1a26
```

### Selector de tamaño de escena

Herramienta:

```text
Tools > Farm Simulator > Farm Development Kit > Scene Size
```

Permite configurar ancho, alto y centro para:

- `Farm`;
- `HouseInterior`.

En Farm puede rellenar celdas vacías de Ground con césped y limpiar tiles fuera de límites. También crea o actualiza `Scene Authoring Bounds`.

Commits iniciales:

```text
b8cef00de5850eeb4fbde862061ecb119685107c
c4cc4c367df3c62cdf249919de275b0daed35e57
```

### Cámara de seguimiento

- Farm usa cámara ortográfica siguiendo al jugador.
- El zoom se configura por altura visible en celdas.
- Puede suavizar el seguimiento.
- Puede limitar la cámara a los bounds de escena.

Commits:

```text
b80a74ad4a5911e09301f58f518e92e04b1a9219
e571301ea5a8bc8f49af637129ee97c8fc51a079
e739e780f1356e37e7fd1479ce67ddc2025b55bc
68ee45992fa28769a6868be5941776ee889fa96f
```

### Expansión real del área jugable

El selector de tamaño ahora sincroniza los límites físicos de movimiento mediante cuatro colliders:

```text
Movement Boundary
├── Boundary Left
├── Boundary Right
├── Boundary Bottom
└── Boundary Top
```

Esto corrige la diferencia entre ampliar el mapa visible y ampliar el espacio por el que el jugador puede desplazarse.

Commits:

```text
7f580db74e2e447642f83389f970f2e33125b556
37e74aad1236d167ad9cb631689fec19034c1277
```

**Validación local confirmada por Arturo:** el área física recorrible se expande correctamente al cambiar el tamaño de Farm.

### Autoría de collider desde Footprint Editor

El Footprint Editor permite modificar de forma persistente:

- centro del `BoxCollider2D`;
- tamaño;
- desplazamiento fino;
- ancho y alto;
- alineación de la base;
- restauración al valor predeterminado del catálogo.

El collider editado:

- se previsualiza en naranja;
- se aplica al regenerar el prefab;
- se conserva al ejecutar `Rebuild definitions`;
- solo vuelve al catálogo mediante `Reset collider to catalog default`.

Commits:

```text
19bb965b15541819062340eb5a93e8f3e0427b72
8d33e728bd8e12f89c0fb03413b3effcf3c90cb7
170c341727e3272df67753c07e31921ea83b4fc9
```

Arturo confirmó que la herramienta y su flujo visual funcionan como esperaba. La compilación completa, suites y colisión física de todos los prefabs siguen sujetas a validación específica cuando corresponda.

---

## Estado documental

`BITÁCORA_GPT.MD` fue actualizado en la rama activa mediante:

```text
07e0d1b04e696200c4e4961787a036770982089a
```

Ese archivo es el documento maestro transaccional y contiene el detalle de continuidad de los incrementos recientes.

`BUGS.MD` y los reportes específicos deben actualizarse cuando se cierre la siguiente transacción documental. No inferir estados nuevos únicamente desde los commits.

---

## Rama activa y forma de trabajo

Rama actual:

```text
dev/farm-scene-authoring
```

Fue creada desde el `main` posterior al PR #15. Todos los siguientes cambios deben publicarse ahí hasta abrir un nuevo PR de integración.

Al escribir archivos mediante GitHub:

- usar siempre el parámetro `branch` correcto;
- verificar el SHA resultante;
- no escribir accidentalmente en `main`;
- actualizar `BITÁCORA_GPT.MD` antes de considerar terminado el incremento.

---

## Próximo paso exacto

1. Continuar el desarrollo únicamente en `dev/farm-scene-authoring`.
2. Antes de la siguiente implementación, leer `BITÁCORA_GPT.MD` actualizado en esa rama.
3. Definir con Arturo el siguiente bloque acotado de autoría de escenas o contenido de Farm.
4. Implementar y publicar commits en la rama activa.
5. Solicitar validación local cuando el cambio dependa de Unity, presentación o física.
6. Actualizar `BITÁCORA_GPT.MD` con realizado, comprobado, pendiente y siguiente paso.
7. Actualizar `BUGS.MD` cuando el incremento corrija o revele un defecto.
8. Abrir PR contra `main` solo cuando el bloque esté consolidado documentalmente.

---

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee primero CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD desde dev/farm-scene-authoring, seguido de BUGS.MD y MIGRACION_DESDE_FARMING_GAME_A.MD. El PR #15 fue fusionado en main mediante 794aab44916f974bc885aec0a203823041c72f8f e integra el navegador estacional de tiles, arrastre a Scene, protección contra el reset destructivo, tamaño configurable de Farm/HouseInterior, cámara de seguimiento, expansión real del área jugable y edición persistente de colliders de edificios. Arturo confirmó que el área jugable ampliada funciona y aprobó el flujo del Footprint Editor. La rama activa es dev/farm-scene-authoring y BITÁCORA_GPT.MD fue actualizada en 07e0d1b04e696200c4e4961787a036770982089a. No marques nada como verificado sin validación local explícita y actualiza la bitácora maestra al cerrar cada incremento.
```
