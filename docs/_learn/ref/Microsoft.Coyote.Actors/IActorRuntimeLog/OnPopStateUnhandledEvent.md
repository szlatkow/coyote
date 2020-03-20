---
layout: reference
section: learn
title: OnPopStateUnhandledEvent
permalink: /learn/ref/Microsoft.Coyote.Actors/IActorRuntimeLog/OnPopStateUnhandledEvent
---
# IActorRuntimeLog.OnPopStateUnhandledEvent method

Invoked when the specified event cannot be handled in the current state, its exit handler is executed and then the state is popped and any previous "current state" is reentered. This handler is called when that pop has been done.

```csharp
public void OnPopStateUnhandledEvent(ActorId id, string stateName, Event e)
```

| parameter | description |
| --- | --- |
| id | The id of the actor that the pop executed in. |
| stateName | The state name, if the actor is a state machine and a state exists, else null. |
| e | The event that cannot be handled. |

## See Also

* class [ActorId](../ActorIdType)
* class [Event](../../Microsoft.Coyote/EventType)
* interface [IActorRuntimeLog](../IActorRuntimeLogType)
* namespace [Microsoft.Coyote.Actors](../IActorRuntimeLogType)
* assembly [Microsoft.Coyote](../../MicrosoftCoyoteAssembly)

<!-- DO NOT EDIT: generated by xmldocmd for Microsoft.Coyote.dll -->