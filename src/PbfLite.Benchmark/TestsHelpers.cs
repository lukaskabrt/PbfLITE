using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PbfLite.Benchmark;

static class TestsHelpers
{
    public static MemoryStream LoadAddressBookData()
    {
        var assembly = typeof(ReadTests).GetTypeInfo().Assembly;

        var stream = new MemoryStream();
        using (var source = assembly.GetManifestResourceStream("PbfLite.Benchmark.Data.addressbook.bin"))
        {
            source.CopyTo(stream);
        }

        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}
