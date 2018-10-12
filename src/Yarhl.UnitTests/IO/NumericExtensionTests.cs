﻿//
// NumericExtensionTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Yarhl.UnitTests.IO
{
    using NUnit.Framework;
    using Yarhl.IO;

    [TestFixture]
    public class NumericExtensionTests
    {
        [Test]
        public void PadUInt64()
        {
            Assert.AreEqual(0x123456789C, 0x123456789Au.Pad(4));
            Assert.AreEqual(0x123456789C, 0x123456789Cu.Pad(4));
        }

        [Test]
        public void PadInt64()
        {
            Assert.AreEqual(0x123456789C, 0x123456789A.Pad(4));
            Assert.AreEqual(0x123456789C, 0x123456789C.Pad(4));
        }

        [Test]
        public void PadUInt32()
        {
            Assert.AreEqual(0x1234567C, 0x1234567Au.Pad(4));
            Assert.AreEqual(0x1234567C, 0x1234567Cu.Pad(4));
        }

        [Test]
        public void PadInt32()
        {
            Assert.AreEqual(0x1234567C, 0x1234567A.Pad(4));
            Assert.AreEqual(0x1234567C, 0x1234567C.Pad(4));
        }

        [Test]
        public void PadUInt16()
        {
            Assert.AreEqual(0x123C, ((ushort)0x123A).Pad(4));
            Assert.AreEqual(0x123C, ((ushort)0x123C).Pad(4));
        }

        [Test]
        public void PadInt16()
        {
            Assert.AreEqual(0x123C, ((short)0x123A).Pad(4));
            Assert.AreEqual(0x123C, ((short)0x123C).Pad(4));
        }
    }
}
