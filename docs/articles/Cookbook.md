# Cookbook

## IO Examples
### Padding
```csharp
```

## FileSystem Examples
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
