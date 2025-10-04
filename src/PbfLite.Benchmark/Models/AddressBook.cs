using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace PbfLite.Benchmark.Models;

[ProtoContract]
public class Person
{
    [ProtoMember(1, Name = "name")]
    public string Name { get; set; }

    [ProtoMember(2, Name = "id")]
    public int ID { get; set; }

    [ProtoMember(3, Name = "email")]
    public string Email { get; set; }

    [ProtoMember(4, Name = "phones")]
    public IList<PhoneNumber> Phones { get; set; }
}

public enum PhoneType
{
    MOBILE = 0,
    HOME = 1,
    WORK = 2
}

[ProtoContract]
public class PhoneNumber
{
    [ProtoMember(1, Name = "number")]
    public string Number { get; set; }

    [ProtoMember(2, Name = "type")]
    public PhoneType Type { get; set; }
}

[ProtoContract]
public class AddressBook
{
    [ProtoMember(1, Name = "people")]
    public IList<Person> People { get; set; }
}