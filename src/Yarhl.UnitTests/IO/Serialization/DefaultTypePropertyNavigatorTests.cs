namespace Yarhl.UnitTests.IO.Serialization;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Yarhl.IO.Serialization;
using Yarhl.IO.Serialization.Attributes;

[TestFixture]
public class DefaultTypePropertyNavigatorTests
{
    [Test]
    public void PropertiesReturnedInOrderViaAttributes()
    {
        var navigator = new DefaultTypePropertyNavigator();

        FieldInfo[] fields = navigator.IterateFields(typeof(SimpleType)).ToArray();

        Assert.That(fields, Has.Length.EqualTo(2));
        Assert.Multiple(() => {
            Assert.That(fields[0].Name, Is.EqualTo(nameof(SimpleType.Prop2)));
            Assert.That(fields[1].Name, Is.EqualTo(nameof(SimpleType.Prop1)));
        });
    }

    [Test]
    public void IgnorePrivateProperties()
    {
        var navigator = new DefaultTypePropertyNavigator();

        FieldInfo[] fields = navigator.IterateFields(typeof(IgnorePrivatePropertiesType)).ToArray();

        Assert.That(fields, Has.Length.EqualTo(1));
        Assert.That(fields[0].Name, Is.EqualTo(nameof(IgnorePrivatePropertiesType.Prop0)));
    }

    [Test]
    public void IgnoreStaticProperties()
    {
        var navigator = new DefaultTypePropertyNavigator();

        FieldInfo[] fields = navigator.IterateFields(typeof(IgnoreStaticPropertiesType)).ToArray();

        Assert.That(fields, Has.Length.EqualTo(1));
        Assert.That(fields[0].Name, Is.EqualTo(nameof(IgnoreStaticPropertiesType.Prop1)));
    }

    [Test]
    public void IgnorePropertiesWithIgnoreAttribute()
    {
        var navigator = new DefaultTypePropertyNavigator();

        FieldInfo[] fields = navigator.IterateFields(typeof(IgnorePropertiesWithIgnoreAttributeType)).ToArray();

        Assert.That(fields, Has.Length.EqualTo(2));
        Assert.Multiple(() => {
            Assert.That(fields[0].Name, Is.EqualTo(nameof(IgnorePropertiesWithIgnoreAttributeType.Prop0)));
            Assert.That(fields[1].Name, Is.EqualTo(nameof(IgnorePropertiesWithIgnoreAttributeType.Prop2)));
        });
    }

    [Test]
    public void IgnorePropertiesWithoutPublicGetterOrSetter()
    {
        var navigator = new DefaultTypePropertyNavigator();

        FieldInfo[] fields = navigator.IterateFields(typeof(IgnorePropertiesWithoutPublicGetterOrSetterType)).ToArray();

        Assert.That(fields, Has.Length.EqualTo(1));
        Assert.That(fields[0].Name, Is.EqualTo(nameof(IgnorePropertiesWithoutPublicGetterOrSetterType.ValidProp)));
    }

    [Test]
    public void PropertyOrderWithInheritance()
    {
        var navigator = new DefaultTypePropertyNavigator();

        FieldInfo[] fields = navigator.IterateFields(typeof(InheritedType)).ToArray();

        Assert.That(fields, Has.Length.EqualTo(5));
        Assert.Multiple(() => {
            Assert.That(fields[0].Name, Is.EqualTo(nameof(InheritedType.Prop0)));
            Assert.That(fields[1].Name, Is.EqualTo(nameof(InheritedType.PropBase0)));
            Assert.That(fields[2].Name, Is.EqualTo(nameof(InheritedType.Prop1)));
            Assert.That(fields[3].Name, Is.EqualTo(nameof(InheritedType.PropBase1)));
            Assert.That(fields[4].Name, Is.EqualTo(nameof(InheritedType.Prop2)));
        });
    }

    [Test]
    public void TypeWithoutPropertyOrderAttributeThrowsInNet60()
    {
        if (!RuntimeInformation.FrameworkDescription.StartsWith(".NET 6")) {
            Assert.Ignore("Test for another platform");
            return;
        }

        var navigator = new DefaultTypePropertyNavigator();

        Assert.That(
            () => navigator.IterateFields(typeof(PropertiesWithoutOrderAttributeType)).ToArray(),
            Throws.Exception.InstanceOf<FormatException>());
    }

    [Test]
    public void TypeWithoutPropertyOrderAttributeWorksInNet80()
    {
        if (RuntimeInformation.FrameworkDescription.StartsWith(".NET 6")) {
            Assert.Ignore("Test for another platform");
            return;
        }

        var navigator = new DefaultTypePropertyNavigator();

        FieldInfo[] fields = navigator.IterateFields(typeof(PropertiesWithoutOrderAttributeType)).ToArray();

        Assert.That(fields, Has.Length.EqualTo(2));
        Assert.Multiple(() => {
            Assert.That(fields[0].Name, Is.EqualTo(nameof(PropertiesWithoutOrderAttributeType.Prop0)));
            Assert.That(fields[1].Name, Is.EqualTo(nameof(PropertiesWithoutOrderAttributeType.Prop1)));
        });
    }

    [Test]
    public void TypeWithSomePropertyMissingOrderAttributeThrows()
    {
        var navigator = new DefaultTypePropertyNavigator();

        Assert.That(
            () => navigator.IterateFields(typeof(SomePropertiesWithoutOrderAttributeType)).ToArray(),
            Throws.Exception.InstanceOf<FormatException>());
    }

#pragma warning disable S3459 // unused properties

    private sealed class SimpleType
    {
        [BinaryOrder(10)]
        public int Prop1 { get; set; }

        [BinaryOrder(-5)]
        public int Prop2 { get; set; }
    }

    private sealed class IgnorePrivatePropertiesType
    {
        [BinaryOrder(0)]
        public int Prop0 { get; set; }

        [BinaryOrder(1)]
        private int Prop1 { get; set; }
    }

    private sealed class IgnoreStaticPropertiesType
    {
        [BinaryOrder(-1)]
        public static int Prop0 { get; set; }

        [BinaryOrder(0)]
        public int Prop1 { get; set; }
    }

    private sealed class IgnorePropertiesWithIgnoreAttributeType
    {
        [BinaryOrder(-1)]
        public int Prop0 { get; set; }

        [BinaryOrder(0)]
        [BinaryIgnore]
        public int Prop1 { get; set; }

        [BinaryOrder(1)]
        public int Prop2 { get; set; }
    }

    private class IgnorePropertiesWithoutPublicGetterOrSetterType
    {
        private int val;

        [BinaryOrder(-1)]
        public int Prop0 { private get; set; }

        [BinaryOrder(0)]
        public int ValidProp { get; set; }

        [BinaryOrder(1)]
        public int Prop2 { get; private set; }

        [BinaryOrder(2)]
        public int Prop3 { internal get; set; }

        [BinaryOrder(3)]
        public int Prop4 { get; protected set; }

        [BinaryOrder(4)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("", "S2376", Justification = "Test")]
        public int Prop5 {
            set => val = value;
        }

        [BinaryOrder(5)]
        public int Prop6 {
            get => val;
        }

        [BinaryOrder(6)]
        internal int Prop7 { get; set; }

        [BinaryOrder(7)]
        protected int Prop8 { get; set; }

        [BinaryOrder(8)]
        private int Prop9 { get; set; }
    }

    private class BaseType
    {
        [BinaryOrder(5)]
        public int PropBase0 { get; set; }

        [BinaryOrder(10)]
        public int PropBase1 { get; set; }
    }

    private sealed class InheritedType : BaseType
    {
        [BinaryOrder(4)]
        public int Prop0 { get; set; }

        [BinaryOrder(8)]
        public int Prop1 { get; set; }

        [BinaryOrder(15)]
        public int Prop2 { get; set; }
    }

    private sealed class PropertiesWithoutOrderAttributeType
    {
        public int Prop0 { get; set; }

        public int Prop1 { get; set; }
    }

    private sealed class SomePropertiesWithoutOrderAttributeType
    {
        [BinaryOrder(0)]
        public int Prop0 { get; set; }

        public int Prop1 { get; set; }
    }
}
