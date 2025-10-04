using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using PbfLite.Benchmark.Models;

namespace PbfLite.Benchmark.PbfLite;

static class AddressBookDeserializer
{
    public static AddressBook Deserialize(byte[] source)
    {
        var data = PbfBlock.Create(source);

        var result = new AddressBook()
        {
            People = new List<Person>(10000)
        };

        var (fieldNumber, wireType) = data.ReadFieldHeader();
        while (fieldNumber != 0)
        {
            switch (fieldNumber)
            {
                case 1: result.People.Add(DeserializePerson(PbfBlock.Create(data.ReadLengthPrefixedBytes()))); break;
                default: data.SkipField(wireType); break;
            }

            (fieldNumber, wireType) = data.ReadFieldHeader();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Person DeserializePerson(PbfBlock data)
    {
        var result = new Person()
        {
            Phones = new List<PhoneNumber>(2)
        };

        var (fieldNumber, wireType) = data.ReadFieldHeader();
        while (fieldNumber != 0)
        {
            switch (fieldNumber)
            {
                case 1: result.Name = data.ReadString(); break;
                case 2: result.ID = data.ReadInt(); break;
                case 3: result.Email = data.ReadString(); break;
                case 4: result.Phones.Add(DeserializePhone(PbfBlock.Create(data.ReadLengthPrefixedBytes()))); break;
                default: data.SkipField(wireType); break;
            }

            (fieldNumber, wireType) = data.ReadFieldHeader();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PhoneNumber DeserializePhone(PbfBlock data)
    {
        var result = new PhoneNumber();

        var (fieldNumber, wireType) = data.ReadFieldHeader();
        while (fieldNumber != 0)
        {
            switch (fieldNumber)
            {
                case 1: result.Number = data.ReadString(); break;
                case 2: result.Type = (PhoneType)data.ReadInt(); break;
                default: data.SkipField(wireType); break;
            }

            (fieldNumber, wireType) = data.ReadFieldHeader();
        }

        return result;
    }

}
