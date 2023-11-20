using System.Globalization;
using System.Text;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
    public double Height { get; set; }
    public bool IsStudent { get; set; }
}


public class JsonSerializer
{
    public static string Serialize(object obj)
    {
        var type = obj.GetType();
        var properties = type.GetProperties();

        var json = new StringBuilder("{");

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            var formattedValue = FormatValue(value);

            json.Append($"\"{property.Name}\": {formattedValue},");
        }

        // Remove the trailing comma if there are properties
        if (json.Length > 1)
            json.Length--;

        json.Append("}");
        return json.ToString();
    }

    private static string FormatValue(object value)
    {
        if (value == null)
        {
            return "null";
        }
        else if (value is string)
        {
            return $"\"{value}\"";
        }
        else if (value is bool)
        {
            return value.ToString().ToLower(); // Convert to lowercase to match JSON format
        }
        else
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}



public class JsonDeserializer
{
    public static T Deserialize<T>(string json)
    {
        var obj  = Activator.CreateInstance(typeof(T));
        var type = typeof (T);
        var properties = type.GetProperties();

        var keyValuePairs = json.Trim('{', '}').Split(',').Select(pair => pair.Split(":")).ToDictionary(
                pair => pair[0].Trim('\"').Trim(),
                pair => pair[1].Trim('\"').Trim());

        foreach(var prop in properties)
        {
            if(keyValuePairs.TryGetValue(prop.Name, out var value))
            {
                var convertedVal = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(obj, convertedVal);
            }
        }

        return (T)obj;
    }
}


class Program
{
    static void Main()
    {
        var person = new Person
        {
            Name = "John Doe",
            Age = 25,
            Height = 6.1,
            IsStudent = false
        };

        // Serialize to JSON
        var json = JsonSerializer.Serialize(person);
        Console.WriteLine($"Serialized JSON: {json}");

        // Deserialize from JSON
        var deserializedPerson = JsonDeserializer.Deserialize<Person>(json);
        Console.WriteLine("\nDeserialized Person:");
        Console.WriteLine($"Name: {deserializedPerson.Name}");
        Console.WriteLine($"Age: {deserializedPerson.Age}");
        Console.WriteLine($"Height: {deserializedPerson.Height}");
        Console.WriteLine($"IsStudent: {deserializedPerson.IsStudent}");
    }
}
