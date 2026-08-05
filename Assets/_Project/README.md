# Assets del proyecto

Esta carpeta contiene exclusivamente los recursos propios de **Unity Farm Simulator**. Los assets y ajustes generados por paquetes de Unity permanecen fuera cuando el motor así lo requiera.

## Capas de código

- `Scripts/Domain`: reglas puras del juego, sin dependencia de `UnityEngine`.
- `Scripts/Application`: casos de uso y coordinación de reglas, sin dependencia de `UnityEngine`.
- `Scripts/Infrastructure`: persistencia, adaptadores y servicios vinculados al motor o la plataforma.
- `Scripts/Presentation`: `MonoBehaviour`, escenas, UI, input, animación y representación visual.
- `Scripts/Editor`: herramientas especializadas del Editor; nunca código de runtime.

## Pruebas

- `Tests/EditMode`: reglas y servicios que no necesitan ejecutar una escena.
- `Tests/PlayMode`: integración con componentes, escenas y ciclo de vida de Unity.

No colocar código nuevo en la assembly global `Assembly-CSharp`. Cada sistema debe pertenecer a una assembly definida y respetar la dirección de dependencias documentada.
