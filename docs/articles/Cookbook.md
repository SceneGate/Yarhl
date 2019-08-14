# Cookbook

## IO Examples
### Padding
```csharp
```


## FileSystem Examples
### Creating directory structure
```csharp
```

### Iterating children nodes
```csharp
```

### Find a node
```csharp
```

### Deleting nodes
```csharp
var parent = new Node("Parent");
var child1 = new Node("Child1");
var child2 = new Node("Child2");
var child3 = new Node("Child3");
node.Add(child1);
node.Add(child2);

node.Remove(child1);  // returns true and not disposed
node.Remove("Child2"); // returns true and disposed
node.Remove("Fake"); // returns false
node.Remove(child3); // returns false
```
