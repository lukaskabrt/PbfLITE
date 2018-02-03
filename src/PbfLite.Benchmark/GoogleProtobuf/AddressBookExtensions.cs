using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M = PbfLite.Benchmark.Models;
namespace PbfLite.Benchmark.GoogleProtobuf {
    static class AddressBookExtensions {
        public static AddressBook Map(M.AddressBook source) {
            var result = new AddressBook();

            foreach (var sourcePerson in source.People) {
                var person = new Person() {
                    Email = sourcePerson.Email,
                    Id = sourcePerson.ID,
                    Name = sourcePerson.Name
                };

                if (sourcePerson.Phones != null) {
                    foreach (var sourcePhone in sourcePerson.Phones) {
                        person.Phones.Add(new Person.Types.PhoneNumber() {
                            Number = sourcePhone.Number,
                            Type = (Person.Types.PhoneType)sourcePhone.Type
                        });
                    }
                }

                result.People.Add(person);
            }

            return result;
        }
    }
}
