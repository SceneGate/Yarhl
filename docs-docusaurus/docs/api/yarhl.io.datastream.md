# DataStream

Namespace: Yarhl.IO

Virtual [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)
with substream capabilities and read/write abstraction layer.

```csharp
public class DataStream : System.IO.Stream, System.IDisposable, System.IAsyncDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)
→
[MarshalByRefObject](https://docs.microsoft.com/en-us/dotnet/api/system.marshalbyrefobject)
→ [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream) →
[DataStream](./yarhl.io.datastream.md)<br/> Implements
[IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable),
[IAsyncDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

**Remarks:**

The type is thread-safe at the level of the substream. For instance, it is safe
to use several DataStream over the same base stream in parallel. The type is not
thread-safe for its method. For instance, it is NOT safe to use the same
DataStream in different threads at the same time.

## Properties

### **ActiveStreams**

Gets the number of streams in use.

```csharp
public static int ActiveStreams { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **Disposed**

Gets a value indicating whether this [DataStream](./yarhl.io.datastream.md) is
disposed.

```csharp
public bool Disposed { get; private set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **Offset**

Gets the offset from the BaseStream.

```csharp
public long Offset { get; private set; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Position**

Gets or sets the position from the start of this stream.

```csharp
public long Position { get; set; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **Length**

Gets the length of this stream.

```csharp
public long Length { get; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **ParentDataStream**

Gets the parent DataStream only if this stream was initialized from a
DataStream.

```csharp
public DataStream ParentDataStream { get; private set; }
```

#### Property Value

[DataStream](./yarhl.io.datastream.md)<br/>

### **BaseStream**

Gets the base stream.

```csharp
public Stream BaseStream { get; }
```

#### Property Value

[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>

### **EndOfStream**

Gets a value indicating whether the position is at end of the stream.

```csharp
public bool EndOfStream { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **AbsolutePosition**

Gets the position from the base stream.

```csharp
public long AbsolutePosition { get; }
```

#### Property Value

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>

### **CanRead**

Gets a value indicating whether the current stream supports reading.

```csharp
public bool CanRead { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CanWrite**

Gets a value indicating whether the current stream supports writing.

```csharp
public bool CanWrite { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CanSeek**

Gets a value indicating whether the current stream supports seeking.

```csharp
public bool CanSeek { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **CanTimeout**

Gets a value indicating whether the current stream support timeouts.

```csharp
public bool CanTimeout { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/>

### **ReadTimeout**

Gets or sets an invalid value as read time is not supported.

```csharp
public int ReadTimeout { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/>

### **WriteTimeout**

Gets or sets an invalid value as write time is not supported.

```csharp
public int WriteTimeout { get; set; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/>

## Constructors

### **DataStream()**

Initializes a new instance of the [DataStream](./yarhl.io.datastream.md) class.
A new stream is created in memory.

```csharp
public DataStream()
```

### **DataStream(Stream)**

Initializes a new instance of the [DataStream](./yarhl.io.datastream.md) class.

```csharp
public DataStream(Stream stream)
```

#### Parameters

`stream`
[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/> Base
stream.

**Remarks:**

The dispose ownership is transferred to this stream.

### **DataStream(Stream, Int64, Int64)**

Initializes a new instance of the [DataStream](./yarhl.io.datastream.md) class
from a substream transferring the ownership of the life-cycle. In the case the
stream is another [DataStream](./yarhl.io.datastream.md) the ownership is
inherited.

```csharp
public DataStream(Stream stream, long offset, long length)
```

#### Parameters

`stream`
[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/> Base
stream.

`offset` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Offset from the base stream.

`length` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Length of this substream.

### **DataStream(Stream, Int64, Int64, Boolean)**

Initializes a new instance of the [DataStream](./yarhl.io.datastream.md) class.

```csharp
public DataStream(Stream stream, long offset, long length, bool transferOwnership)
```

#### Parameters

`stream`
[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/> Base
stream.

`offset` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Offset from the base stream.

`length` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Length of this substream.

`transferOwnership`
[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/>
Transfer the ownsership of the stream argument to this class so it can dispose
it.

## Methods

### **SetLength(Int64)**

Sets the length of the current stream.

```csharp
public void SetLength(long value)
```

#### Parameters

`value` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
The new length value.

### **Flush()**

Clears all buffers for this stream and causes any buffered data to be written to
the underlying device.

```csharp
public void Flush()
```

### **Seek(Int64, SeekMode)**

#### Caution

Use the overload with SeekOrigin.

---

Move the position of the Stream.

```csharp
public void Seek(long shift, SeekMode mode)
```

#### Parameters

`shift` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Distance to move position.

`mode` [SeekMode](./yarhl.io.seekmode.md)<br/> Mode to move position.

### **Seek(Int64, SeekOrigin)**

Move the position of the stream.

```csharp
public long Seek(long offset, SeekOrigin origin)
```

#### Parameters

`offset` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Distance to move position.

`origin`
[SeekOrigin](https://docs.microsoft.com/en-us/dotnet/api/system.io.seekorigin)<br/>
Mode to move position.

#### Returns

[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/> The new
position of the stream.

### **PushToPosition(Int64, SeekMode)**

#### Caution

Use the overload with SeekOrigin.

---

Push the current position into a stack and move to a new one.

```csharp
public void PushToPosition(long shift, SeekMode mode)
```

#### Parameters

`shift` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Distance to move position.

`mode` [SeekMode](./yarhl.io.seekmode.md)<br/> Mode to move position.

### **PushToPosition(Int64, SeekOrigin)**

Push the current position into a stack and move to a new one.

```csharp
public void PushToPosition(long shift, SeekOrigin mode)
```

#### Parameters

`shift` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Distance to move position.

`mode`
[SeekOrigin](https://docs.microsoft.com/en-us/dotnet/api/system.io.seekorigin)<br/>
Mode to move position.

### **PushCurrentPosition()**

Push the current position into a stack.

```csharp
public void PushCurrentPosition()
```

### **PopPosition()**

Pop the last position from the stack and move to it.

```csharp
public void PopPosition()
```

### **RunInPosition(Action, Int64, SeekMode)**

#### Caution

Use the overload with SeekOrigin.

---

Run a method in a specific position. This command will move into the position,
run the method and return to the current position.

```csharp
public void RunInPosition(Action action, long position, SeekMode mode)
```

#### Parameters

`action`
[Action](https://docs.microsoft.com/en-us/dotnet/api/system.action)<br/> Action
to run.

`position`
[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/> Position
to move.

`mode` [SeekMode](./yarhl.io.seekmode.md)<br/> Mode to move position.

### **RunInPosition(Action, Int64, SeekOrigin)**

Run a method in a specific position. This command will move into the position,
run the method and return to the current position.

```csharp
public void RunInPosition(Action action, long position, SeekOrigin mode)
```

#### Parameters

`action`
[Action](https://docs.microsoft.com/en-us/dotnet/api/system.action)<br/> Action
to run.

`position`
[Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/> Position
to move.

`mode`
[SeekOrigin](https://docs.microsoft.com/en-us/dotnet/api/system.io.seekorigin)<br/>
Mode to move position.

### **ReadByte()**

Reads a byte from the stream and advances the position within the stream by one
byte, or returns -1 if at the end of the stream.

```csharp
public int ReadByte()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/> The
unsigned byte cast to an Int32, or -1 if at the end of the stream.

### **Read(Byte[], Int32, Int32)**

Reads a sequence of bytes from the current stream and advances the position
within the stream by the number of bytes read.

```csharp
public int Read(Byte[] buffer, int offset, int count)
```

#### Parameters

`buffer` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br/>
Buffer to copy data.

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/>
Index to start copying in buffer.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/>
Maximum number of bytes to read.

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/> The total
number of bytes read into the buffer. This can be less than the number of bytes
requested if that many bytes are not currently available, or zero (0) if the end
of the stream has been reached.

### **ReadFormat&lt;T&gt;()**

Reads a format from this stream.

```csharp
public T ReadFormat<T>()
```

#### Type Parameters

`T`<br/> The type of the format to read.

#### Returns

T<br/> The format read.

### **WriteByte(Byte)**

Writes a byte.

```csharp
public void WriteByte(byte value)
```

#### Parameters

`value` [Byte](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br/>
Byte value.

### **Write(Byte[], Int32, Int32)**

Writes the a portion of the buffer to the stream.

```csharp
public void Write(Byte[] buffer, int offset, int count)
```

#### Parameters

`buffer` [Byte[]](https://docs.microsoft.com/en-us/dotnet/api/system.byte)<br/>
Buffer to write.

`offset` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/>
Index in the buffer.

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br/>
Bytes to write.

### **WriteTo(String)**

Writes the complete stream into a file.

```csharp
public void WriteTo(string fileOut)
```

#### Parameters

`fileOut`
[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br/> Output
file path.

**Remarks:**

It preserves the current position and creates any required directory.

### **WriteTo(Stream)**

Writes the complete stream into another stream preserving the current position.

```csharp
public void WriteTo(Stream stream)
```

#### Parameters

`stream`
[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/> The
stream to write.

**Remarks:**

This method is similar to
[Stream.CopyTo(Stream)](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream.copyto).
The difference is that it copies always from the position 0 of the current
stream, and it preserves the current position afterwards. It writes into the
current position of the destination stream.

### **WriteSegmentTo(Int64, String)**

Writes a segment of the stream into a file from a defined position to the end.

```csharp
public void WriteSegmentTo(long start, string fileOut)
```

#### Parameters

`start` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Starting position to read from the current stream.

`fileOut`
[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br/> Output
file path.

**Remarks:**

It preserves the current position and creates any required directory.

### **WriteSegmentTo(Int64, Stream)**

Writes a segment of the stream into another stream from a defined position to
the end.

```csharp
public void WriteSegmentTo(long start, Stream stream)
```

#### Parameters

`start` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Starting position to read from the current stream.

`stream`
[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
Output stream.

**Remarks:**

It preserves the current position and writes to the current position of the
destination stream.

### **WriteSegmentTo(Int64, Int64, String)**

Writes a segment of the stream into a file.

```csharp
public void WriteSegmentTo(long start, long length, string fileOut)
```

#### Parameters

`start` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Starting position to read from the current stream.

`length` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Length of the segment to read.

`fileOut`
[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br/> Output
file path.

**Remarks:**

It preserves the current position and creates any required directory.

### **WriteSegmentTo(Int64, Int64, Stream)**

Writes a segment of the stream into another stream.

```csharp
public void WriteSegmentTo(long start, long length, Stream stream)
```

#### Parameters

`start` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Starting position to read from the current stream.

`length` [Int64](https://docs.microsoft.com/en-us/dotnet/api/system.int64)<br/>
Length of the segment to read.

`stream`
[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
Output stream.

**Remarks:**

It preserves the current position and writes to the current position of the
destination stream.

### **Compare(Stream)**

Compare the content of the stream with another one.

```csharp
public bool Compare(Stream otherStream)
```

#### Parameters

`otherStream`
[Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br/>
Stream to compare with.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/> The
result of the comparison.

### **Dispose(Boolean)**

Releases all resource used by the [DataStream](./yarhl.io.datastream.md) object.

```csharp
protected void Dispose(bool disposing)
```

#### Parameters

`disposing`
[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br/> If
set to free managed resources also.
