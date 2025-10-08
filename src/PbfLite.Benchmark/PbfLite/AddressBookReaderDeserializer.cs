using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PbfLite.Benchmark.Models;

namespace PbfLite.Benchmark.PbfLite;

static class AddressBookDeserializer
{
    public static AddressBook Deserialize(byte[] source)
    {
        var reader = PbfBlockReader.Create(source);

        var result = new AddressBook()
        {
            People = new List<Person>(10000)
        };

        var (fieldNumber, wireType) = reader.ReadFieldHeader();
        while (fieldNumber != 0)
        {
            switch (fieldNumber)
            {
                case 1: result.People.Add(DeserializePerson(PbfBlockReader.Create(reader.ReadLengthPrefixedBytes()))); break;
                default: reader.SkipField(wireType); break;
            }

            (fieldNumber, wireType) = reader.ReadFieldHeader();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Person DeserializePerson(PbfBlockReader reader)
    {
        var result = new Person()
        {
            Phones = new List<PhoneNumber>(2)
        };

        var (fieldNumber, wireType) = reader.ReadFieldHeader();
        while (fieldNumber != 0)
        {
            switch (fieldNumber)
            {
                case 1: result.Name = reader.ReadString(); break;
                case 2: result.ID = reader.ReadInt(); break;
                case 3: result.Email = reader.ReadString(); break;
                case 4: result.Phones.Add(DeserializePhone(PbfBlockReader.Create(reader.ReadLengthPrefixedBytes()))); break;
                default: reader.SkipField(wireType); break;
            }

            (fieldNumber, wireType) = reader.ReadFieldHeader();
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static PhoneNumber DeserializePhone(PbfBlockReader reader)
    {
        var result = new PhoneNumber();

        var (fieldNumber, wireType) = reader.ReadFieldHeader();
        while (fieldNumber != 0)
        {
            switch (fieldNumber)
            {
                case 1: result.Number = reader.ReadString(); break;
                case 2: result.Type = (PhoneType)reader.ReadInt(); break;
                default: reader.SkipField(wireType); break;
            }

            (fieldNumber, wireType) = reader.ReadFieldHeader();
        }

        return result;
    }
}