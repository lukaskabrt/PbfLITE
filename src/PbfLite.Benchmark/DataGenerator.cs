using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PbfLite.Benchmark.Models;
using ProtoBuf;

namespace PbfLite.Benchmark;

public class DataGenerator
{
    private readonly Random _random;

    public DataGenerator()
    {
        _random = new Random();
    }

    public byte[] GenerateAddressBook()
    {
        var book = new AddressBook()
        {
            People = GeneratePeople()
        };

        using (var stream = new MemoryStream())
        {
            Serializer.Serialize(stream, book);
            return stream.ToArray();
        }
    }

    public void GenerateAddressBook(string filename)
    {
        File.WriteAllBytes(filename, GenerateAddressBook());
    }

    private Person[] GeneratePeople()
    {
        var people = new Person[10000];

        for (int i = 0; i < people.Length; i++)
        {
            people[i] = new Person()
            {
                ID = _random.Next(),
                Name = $"{GenerateRandomString(8)} {GenerateRandomString(10)}",
                Email = $"{GenerateRandomString(20)}@{GenerateRandomString(8)}.{GenerateRandomString(4)}",
                Phones = GeneratePhones()
            };
        }

        return people;
    }

    private PhoneNumber[] GeneratePhones()
    {
        var phones = new PhoneNumber[_random.Next(3)];

        for (int i = 0; i < phones.Length; i++)
        {
            phones[i] = new PhoneNumber()
            {
                Type = (PhoneType)_random.Next(2),
                Number = GenerateRandomNumber(9)
            };
        }

        return phones;
    }

    private string GenerateRandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        var stringChars = new char[length];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[_random.Next(chars.Length)];
        }

        return new String(stringChars);
    }

    private string GenerateRandomNumber(int length)
    {
        var chars = "0123456789";
        var stringChars = new char[length];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[_random.Next(chars.Length)];
        }

        return new String(stringChars);
    }
}
