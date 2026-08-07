# Bitácora — Regla de verificación de herramientas y acceso

**Fecha:** 2026-08-06  
**Proyecto:** Unity Farm Simulator  
**Rama:** `dev/farm-scene-authoring`

## Incidente

Durante el trabajo sobre el generador de arte placeholder, GPT afirmó que no tenía acceso de escritura al repositorio y ofreció entregar un parche manual en lugar de continuar modificando la rama.

La afirmación era incorrecta. El conector de GitHub seguía disponible y, al comprobarlo posteriormente, reportó permisos efectivos de lectura y escritura sobre:

```text
xArtXtrAx/Unity-Farm-Simulator
```

Permisos confirmados:

```text
pull
push
maintain
admin
```

## Clasificación

El incidente se clasifica como una **alucinación operativa**.

No consistió en inventar contenido del repositorio. Consistió en inventar una limitación de capacidad o disponibilidad de herramienta sin realizar antes una comprobación real.

## Causa de proceso

GPT interpretó incorrectamente el contexto del turno y concluyó que la herramienta de GitHub no estaba disponible. Esa conclusión se expresó como un hecho sin intentar primero una operación de lectura sobre el repositorio.

El error fue especialmente relevante porque:

- GitHub había sido utilizado durante la misma sesión;
- no existía un fallo confirmado del conector;
- la afirmación interrumpió innecesariamente el flujo de trabajo;
- trasladó al usuario una tarea manual que GPT sí podía ejecutar.

## Regla operativa obligatoria

> **Nunca afirmar que se perdió acceso a GitHub, al repositorio o a otra herramienta conectada sin comprobarlo primero mediante una llamada real a la herramienta correspondiente.**

Procedimiento obligatorio:

1. Ejecutar una operación de lectura mínima y no destructiva, por ejemplo consultar el repositorio, la rama o un archivo conocido.
2. Si la llamada funciona, continuar el trabajo normalmente.
3. Si falla, revisar el error concreto y distinguir entre:
   - ruta o referencia incorrecta;
   - archivo inexistente;
   - permiso insuficiente;
   - conector desconectado;
   - fallo temporal del servicio;
   - herramienta realmente no disponible.
4. No interpretar un `404` sobre un archivo o ruta como pérdida general de acceso al repositorio.
5. No ofrecer trabajo manual al usuario mientras exista una herramienta conectada capaz de completar la tarea.
6. Solo declarar pérdida de acceso cuando una comprobación explícita la sustente.

## Regla de comunicación

Cuando exista incertidumbre sobre una herramienta, usar una formulación verificable:

```text
Voy a comprobar el acceso antes de concluir que se perdió.
```

No usar sin evidencia:

```text
No tengo acceso al repositorio.
No puedo escribir en GitHub desde este turno.
La conexión se perdió.
```

## Aplicación al proyecto

Esta regla forma parte del contrato de trabajo de `Unity Farm Simulator` y debe leerse junto con:

- `CONTINUIDAD_GPT.md`;
- `BITÁCORA_GPT.MD`;
- `BUGS.MD`;
- las bitácoras transaccionales de la rama activa.

En chats futuros, antes de detener una implementación por una supuesta pérdida de acceso, GPT debe ejecutar una comprobación real del repositorio `xArtXtrAx/Unity-Farm-Simulator`.

## Estado

- Incidente reconocido: **sí**.
- Acceso real verificado posteriormente: **sí**.
- Regla preventiva documentada: **sí**.
- Validación futura: comprobar que la regla se respete ante el siguiente error o ambigüedad de herramientas.
