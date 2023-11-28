// Copyright (c) 2023 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Yarhl.UnitTests.Plugins;

using System;

#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable S2326 // Unused type parameters should be removed

public interface IExistsInterface
{
}

public interface IGenericInterface<T>
{
}

public interface IGenericInterface<T1, T2>
{
}

public class ExistsClass : IExistsInterface
{
}

public class Generic1Class : IGenericInterface<int>
{
}

public class Generic2Class : IGenericInterface<string, int>
{
}

public class GenericMultipleClass :
    IGenericInterface<string, int>,
    IGenericInterface<int, string>
{
}

public abstract class AbstractClass : IExistsInterface
{
}

public abstract class AbstractGenericClass : IGenericInterface<string, int>
{
}

public class ConstructorWithException
{
    public ConstructorWithException()
    {
        throw new Exception();
    }
}
