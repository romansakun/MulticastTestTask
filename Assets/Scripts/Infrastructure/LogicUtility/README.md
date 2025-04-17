## Table Of Contents

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
<details>
<summary>Details</summary>

  - [Introduction](#introduction)
  - [Examples](#documentation)

</details>
<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Introduction

Logic utility - advanced hierarchical scoring-based logic framework

## Examples

First you need to build a logic agent. 
For this you will need a context class:

```csharp
public class SomeContext : IContext
{
    public string SomeValue { get; set; }

    public void Dispose()
    {

    }
}
```

Some action with this context:
```csharp
public class SomeAction : IAction<SomeContext>
{
    public INode<SomeContext> Next { get; set; }
    public string GetLog() => "SomeAction";

    public Task ExecuteAsync(SomeContext context)
    {
        context.SomeValue = "Hello";

        return Task.CompletedTask;
    }
}
```

Now let's create a logic agent:
```csharp
public class SomeLogicAgentController
{
    private readonly LogicAgent<SomeContext> _logicAgent;

    public SomeLogicAgentController()
    {
        var builder = new LogicBuilder<SomeContext>();

        var someAction = builder.AddAction<SomeAction>();
        someAction.SetAsRoot();

        _logicAgent = builder.Build();
    }

    public async Task Execute()
    {
        await _logicAgent.ExecuteAsync();

        Debug.Log(_logicAgent.Context.SomeValue); // Output "Hello"
    }

}
```

//todo: add more examples