# Cookbook

## IO

### Padding

```csharp
// Add 0s until position is divisible by 0x4
filesWriter.WritePadding(0x00, 0x4);

// Add 0s until position is divisible by 0x10
myDataWriter.WritePadding(0x00, 0x10);
```

## FileSystem

### Using converters with parameters

```csharp
public class ConverterWithParameter :
        IInitializer<int>,
        IConverter<BinaryFormat, Po>,
        IConverter<Po, BinaryFormat>
    {

        public int Parameter { get; set; }

        public void Initialize(int param)
        {
            Parameter = param;
        }

        public Po Convert(BinaryFormat source)
        {
            // Converter
        }

        public BinaryFormat Convert(Po source)
        {
            // Converter
        }
    }
```

Then you can use `TransformWith<ConverterWithParameter, int>(3)`.

You can use a custom class too:

```csharp
public class ConverterWithParameter :
    IInitializer<MyClass>,
    IConverter<BinaryFormat, Po>,
    IConverter<Po, BinaryFormat>

TransformWith<ConverterWithParameter, MyClass>(myClassInstance)
```

### Creating directory structure

```csharp
Node root = new Node("root");

string path = "/parent1/parent2/";
Node child = new Node("child");

// This will create /root/parent1/parent2/child
NodeFactory.CreateContainersForChild(root, path, child);
```

### Iterating children nodes

```csharp
foreach (Node node in Navigator.IterateNodes(source.Root)) {
    if (!node.IsContainer) {
        //This is your child, take care of him
    }
}
```

### Find a node

```csharp
// You can use full paths (starting with '/') or relative paths.
Navigator.SearchNode(nodeParent, "Child/SubChild");
```

### Deleting nodes

```csharp
var parent = new Node("Parent");
var child1 = new Node("Child1");
var child2 = new Node("Child2");
node.Add(child1);
node.Add(child2);

// If you remove passing a reference, the node is removed but it is NOT disposed
node.Remove(child1);

// If you remove passing a name (you don't have the reference),
// the node is removed AND disposed.
node.Remove("Child2");
```
