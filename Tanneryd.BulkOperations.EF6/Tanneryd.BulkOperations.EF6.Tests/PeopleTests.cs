﻿/*
* Copyright ©  2017-2018 Tånneryd IT AB
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
*   http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tanneryd.BulkOperations.EF6.Model;
using Tanneryd.BulkOperations.EF6.Tests.DM;
using Tanneryd.BulkOperations.EF6.Tests.DM.People;
using Tanneryd.BulkOperations.EF6.Tests.EF;

namespace Tanneryd.BulkOperations.EF6.Tests
{ 
    [TestClass]
    public class PeopleTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<PeopleContext>());
        }

        [TestCleanup]
        public void Cleanup()
        {
            var db = new PeopleContext();
            db.People.RemoveRange(db.People.ToArray());
            db.SaveChanges();
        }

        [TestMethod]
        public void AddingChildWithMother()
        {
            var mother = new Person
            {
                FirstName = "Anna",
                LastName = "Andersson",
                BirthDate = new DateTime(1980, 1, 1),
            };
            var child = new Person
            {
                FirstName = "Arvid",
                LastName = "Andersson",
                BirthDate = new DateTime(2018, 1, 1),
                Mother = mother,
            };
            using (var db = new PeopleContext())
            {
                var request = new BulkInsertRequest<Person>
                {
                    Entities = new List<Person> {child},
                    Recursive = true,
                };
                db.BulkInsertAll(request);

                var people = db.People.ToArray();
                Assert.AreEqual(2, people.Length);
            }
        }

        [TestMethod]
        public void AddingMotherWithChild()
        {
            var child = new Person
            {
                FirstName = "Arvid",
                LastName = "Andersson",
                BirthDate = new DateTime(2018, 1, 1),
            };
            var mother = new Person
            {
                FirstName = "Anna",
                LastName = "Andersson",
                BirthDate = new DateTime(1980, 1, 1),
            };
            mother.Children.Add(child);

            using (var db = new PeopleContext())
            {
                var request = new BulkInsertRequest<Person>
                {
                    Entities = new List<Person> { mother },
                    Recursive = true,
                };
                db.BulkInsertAll(request);

                var people = db.People.ToArray();
                Assert.AreEqual(2, people.Length);
            }
        }
    }
}