using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaSyx.Models.AdminShell;
using BaSyx.Models.Extensions;
using FluentAssertions;

namespace BaSyx.Tests
{
    [TestClass]
    public class SubmodelConversionTests
    {
        private class Address
        {
            public string Street { get; set; }
            public int No { get; set; }
            public DateTime MovedIn { get; set; }
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Active { get; set; }
            public DateTime Birthday { get; set; }
            public Address Address { get; set; }
            public List<string> Tags { get; set; }
            public int[] Scores { get; set; }
            public List<Address> PreviousAddresses { get; set; }
            public Dictionary<string, string> Meta { get; set; }
        }

        [TestMethod]
        public void FullConversionRoundtrip()
        {
            // Arrange: Build a complex instance with primitives, datetime, arrays, lists, nested lists of complex types and a dictionary
            var now = DateTime.SpecifyKind(DateTime.UtcNow.AddMinutes(-12).AddSeconds(-3), DateTimeKind.Utc);
            var person = new Person
            {
                Name = "Alice",
                Age = 37,
                Active = true,
                Birthday = now,
                Address = new Address { Street = "Main St", No = 5, MovedIn = now.AddYears(-3) },
                Tags = new List<string> { "tag1", "tag2", "tag3" },
                Scores = new[] { 10, 20, 30, 40 },
                PreviousAddresses = new List<Address>
            {
                new Address { Street = "1st Ave", No = 11, MovedIn = now.AddYears(-6) },
                new Address { Street = "2nd Ave", No = 22, MovedIn = now.AddYears(-5) }
            },
                Meta = new Dictionary<string, string>
            {
                { "k1", "v1" },
                { "k2", "v2" }
            }
            };

            // Act: Convert instance -> SubmodelElementCollection
            ISubmodelElementCollection smc = person.CreateSubmodelElementCollectionFromObject("Person");

            // Assert structure
            Assert.IsNotNull(smc);
            Assert.IsTrue(smc.Value.Value.HasChild("Name"));
            Assert.IsTrue(smc.Value.Value.HasChild("Age"));
            Assert.IsTrue(smc.Value.Value.HasChild("Active"));
            Assert.IsTrue(smc.Value.Value.HasChild("Birthday"));
            Assert.IsTrue(smc.Value.Value.HasChild("Address"));
            Assert.IsTrue(smc.Value.Value.HasChild("Tags"));
            Assert.IsTrue(smc.Value.Value.HasChild("Scores"));
            Assert.IsTrue(smc.Value.Value.HasChild("PreviousAddresses"));
            Assert.IsTrue(smc.Value.Value.HasChild("Meta"));

            // Simple properties are Properties
            Assert.AreEqual(ModelType.Property, smc["Name"].ModelType);
            Assert.AreEqual(ModelType.Property, smc["Age"].ModelType);
            Assert.AreEqual(ModelType.Property, smc["Active"].ModelType);
            Assert.AreEqual(ModelType.Property, smc["Birthday"].ModelType);

            // Complex types are SubmodelElementCollections
            Assert.AreEqual(ModelType.SubmodelElementCollection, smc["Address"].ModelType);

            // Lists/Arrays are SubmodelElementLists
            Assert.AreEqual(ModelType.SubmodelElementList, smc["Tags"].ModelType);
            Assert.AreEqual(ModelType.SubmodelElementList, smc["Scores"].ModelType);
            Assert.AreEqual(ModelType.SubmodelElementList, smc["PreviousAddresses"].ModelType);

            // Dictionary<string, string> currently maps to a SubmodelElementCollection of Properties
            Assert.AreEqual(ModelType.SubmodelElementCollection, smc["Meta"].ModelType);

            // Validate list type metadata: simple list -> Property items, complex list -> SMC items
            var tagsSml = smc["Tags"] as ISubmodelElementList;
            var scoresSml = smc["Scores"] as ISubmodelElementList;
            var prevAddrSml = smc["PreviousAddresses"] as ISubmodelElementList;

            Assert.IsNotNull(tagsSml);
            Assert.AreEqual(ModelType.Property, tagsSml.TypeValueListElement);
            Assert.IsNotNull(scoresSml);
            Assert.AreEqual(ModelType.Property, scoresSml.TypeValueListElement);
            Assert.IsNotNull(prevAddrSml);
            Assert.AreEqual(ModelType.SubmodelElementCollection, prevAddrSml.TypeValueListElement);

            // Validate dictionary mapping by reading bound values
            var metaSmc = smc["Meta"] as ISubmodelElementCollection;
            Assert.IsNotNull(metaSmc);
            Assert.AreEqual("v1", metaSmc["k1"].As<IProperty>().GetValue<string>());
            Assert.AreEqual("v2", metaSmc["k2"].As<IProperty>().GetValue<string>());

            // Read primitive property values back through GetValue<T>
            Assert.AreEqual(person.Name, smc["Name"].As<IProperty>().GetValue<string>());
            Assert.AreEqual(person.Age, smc["Age"].As<IProperty>().GetValue<int>());
            Assert.AreEqual(person.Active, smc["Active"].As<IProperty>().GetValue<bool>());
            Assert.AreEqual(person.Birthday, smc["Birthday"].As<IProperty>().GetValue<DateTime>());

            // Read nested complex properties from collection
            var addressSmc = smc["Address"] as ISubmodelElementCollection;
            Assert.IsNotNull(addressSmc);
            Assert.AreEqual(person.Address.Street, addressSmc["Street"].As<IProperty>().GetValue<string>());
            Assert.AreEqual(person.Address.No, addressSmc["No"].As<IProperty>().GetValue<int>());
            Assert.AreEqual(person.Address.MovedIn, addressSmc["MovedIn"].As<IProperty>().GetValue<DateTime>());

            // Read array/list values (ToEnumerable<T> over container)
            var tagsRoundtrip = tagsSml.Value.Value.ToEnumerable<string>().ToList();
            var scoresRoundtrip = scoresSml.Value.Value.ToEnumerable<int>().ToArray();
            AssertSequenceEqual(person.Tags, tagsRoundtrip);
            AssertSequenceEqual(person.Scores, scoresRoundtrip);

            // Read complex list values to typed objects
            var prevAddrsRoundtrip = prevAddrSml.Value.Value.ToEnumerable<Address>().ToList();
            Assert.AreEqual(person.PreviousAddresses.Count, prevAddrsRoundtrip.Count);
            for (int i = 0; i < prevAddrsRoundtrip.Count; i++)
            {
                Assert.AreEqual(person.PreviousAddresses[i].Street, prevAddrsRoundtrip[i].Street);
                Assert.AreEqual(person.PreviousAddresses[i].No, prevAddrsRoundtrip[i].No);
                Assert.AreEqual(person.PreviousAddresses[i].MovedIn, prevAddrsRoundtrip[i].MovedIn);
            }

            // Act: Convert SubmodelElementCollection -> new Person instance
            var person2 = smc.ToObject<Person>();

            // Assert: dictionary back-conversion isn't supported in this pass — skip Meta
            Assert.IsNotNull(person2);
            Assert.AreEqual(person.Name, person2.Name);
            Assert.AreEqual(person.Age, person2.Age);
            Assert.AreEqual(person.Active, person2.Active);
            Assert.AreEqual(person.Birthday, person2.Birthday);
            Assert.IsNotNull(person2.Address);
            Assert.AreEqual(person.Address.Street, person2.Address.Street);
            Assert.AreEqual(person.Address.No, person2.Address.No);
            Assert.AreEqual(person.Address.MovedIn, person2.Address.MovedIn);
            Assert.IsNotNull(person2.Tags);
            Assert.IsNotNull(person2.Scores);
            Assert.IsNotNull(person2.PreviousAddresses);
            AssertSequenceEqual(person.Tags, person2.Tags);
            AssertSequenceEqual(person.Scores, person2.Scores);
            Assert.AreEqual(person.PreviousAddresses.Count, person2.PreviousAddresses.Count);
            for (int i = 0; i < person2.PreviousAddresses.Count; i++)
            {
                Assert.AreEqual(person.PreviousAddresses[i].Street, person2.PreviousAddresses[i].Street);
                Assert.AreEqual(person.PreviousAddresses[i].No, person2.PreviousAddresses[i].No);
                Assert.AreEqual(person.PreviousAddresses[i].MovedIn, person2.PreviousAddresses[i].MovedIn);
            }

            // Test: Update bound values via SME and verify they propagate to the original object instance
            smc["Age"].As<IProperty>().SetValue(42);
            Assert.AreEqual(42, person.Age);

            // Update bound list "Tags" via SetValueScope and check propagated changes
            var newTags = new List<string> { "x", "y", "z" };
            var tagsSm = smc["Tags"] as ISubmodelElementList;
            Assert.IsNotNull(tagsSm);
            var tagsValueScope = new SubmodelElementListValue<string>(tagsSm, newTags);
            tagsSm.SetValueScope(tagsValueScope).Wait();
            AssertSequenceEqual(newTags, person.Tags);

            // Test: Create SME from a primitive instance
            int a = 7;
            var aSme = a.CreateSubmodelElementFromInstance("A");
            Assert.IsNotNull(aSme);
            Assert.AreEqual(ModelType.Property, aSme.ModelType);
            Assert.AreEqual(7, aSme.As<IProperty>().GetValue<int>());

            // Test: Create SME from a list instance and convert back to array
            var nums = new[] { 1, 2, 3, 4 };
            var numsSme = nums.CreateSubmodelElementFromInstance("Nums") as ISubmodelElementList;
            Assert.IsNotNull(numsSme);
            Assert.AreEqual(ModelType.SubmodelElementList, numsSme.ModelType);
            var numsBack = (int[])numsSme.ToObject(typeof(int[]));
            AssertSequenceEqual(nums, numsBack);

            // Test: Create SME from a complex instance and convert back
            var addrSme = person.Address.CreateSubmodelElementFromInstance("Addr") as ISubmodelElementCollection;
            Assert.IsNotNull(addrSme);
            var addrBack = addrSme.ToObject<Address>();
            Assert.AreEqual(person.Address.Street, addrBack.Street);
            Assert.AreEqual(person.Address.No, addrBack.No);
            Assert.AreEqual(person.Address.MovedIn, addrBack.MovedIn);
        }

        private static void AssertSequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected == null && actual == null)
                return;

            Assert.IsNotNull(expected, "Expected sequence is null while actual is not");
            Assert.IsNotNull(actual, "Actual sequence is null while expected is not");

            var exp = expected.ToList();
            var act = actual.ToList();
            Assert.AreEqual(exp.Count, act.Count, "Sequences differ in length");

            for (int i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(exp[i], act[i], $"Sequences differ at index {i}");
            }
        }
    }
}