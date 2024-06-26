# SkipRewritingAttribute class

Attribute for declaring source code targets that must not be rewritten.

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class SkipRewritingAttribute : Attribute
```

## Public Members

| name | description |
| --- | --- |
| [SkipRewritingAttribute](SkipRewritingAttribute/SkipRewritingAttribute.md)(…) | Initializes a new instance of the [`SkipRewritingAttribute`](./SkipRewritingAttribute.md) class. |
| readonly [Reason](SkipRewritingAttribute/Reason.md) | The reason for skipping rewriting. |

## See Also

* namespace [Microsoft.Coyote.Rewriting](../Microsoft.Coyote.RewritingNamespace.md)
* assembly [Microsoft.Coyote.Test](../Microsoft.Coyote.Test.md)

<!-- DO NOT EDIT: generated by xmldocmd for Microsoft.Coyote.Test.dll -->
