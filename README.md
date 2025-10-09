
# PbfLITE

PbfLITE is a low-level .NET Protocol Buffers implementation. It is intended for scenarios where you need fine-grained control over serialization and deserialization without the overhead of reflection, but it requires developers to manually implement the serialization / deserialization logic.

## Features

- **No reflection**
- **Manual control over serialization** for maximum performance


## Limitations

- **Whole message must be loaded into memory:** PbfLITE requires the entire protobuf message to be available as a contiguous byte array or span. Streaming is not supported.

## Getting Started

Add a reference to the `PbfLite` project and start using `PbfBlockReader` and `PbfBlockWriter` for manual protobuf serialization and deserialization.

## Example: Deserializing an AddressBook

Below is an example of how to use PbfLITE to deserialize a protobuf-encoded `AddressBook` message:

```csharp
using PbfLite;
using System.Collections.Generic;

public class AddressBook {
	public List<Person> People { get; set; } = new();
}

public class Person {
	public int ID { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
}

public static class AddressBookDeserializer {
	public static AddressBook Deserialize(byte[] source) {
		var reader = PbfBlockReader.Create(source);
		var result = new AddressBook();

		var (fieldNumber, wireType) = reader.ReadFieldHeader();
		while (fieldNumber != 0) {
			switch (fieldNumber) {
				case 1:
					result.People.Add(DeserializePerson(PbfBlockReader.Create(reader.ReadLengthPrefixedBytes())));
					break;
				default:
					reader.SkipField(wireType);
					break;
			}
			(fieldNumber, wireType) = reader.ReadFieldHeader();
		}
		return result;
	}

	private static Person DeserializePerson(PbfBlockReader reader) {
		var result = new Person();
        
		var (fieldNumber, wireType) = reader.ReadFieldHeader();
		while (fieldNumber != 0) {
			switch (fieldNumber) {
				case 1: result.Name = reader.ReadString(); break;
				case 2: result.ID = reader.ReadInt(); break;
				case 3: result.Email = reader.ReadString(); break;
				default: reader.SkipField(wireType); break;
			}
			(fieldNumber, wireType) = reader.ReadFieldHeader();
		}
		return result;
	}
}
```