using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

namespace PrimalEditor.Utilities;

public static class Serializer
{
    public static void ToFile<T>(T instance, String path)
    {
        try
        {
            using FileStream fs = new(path, FileMode.Create);
            DataContractSerializer serializer = new(typeof(T));
            serializer.WriteObject(fs, instance);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public static T FromFile<T>(String path)
    {
        try
        {
            using FileStream fs = new(path, FileMode.Open);
            DataContractSerializer serializer = new(typeof(T));
            return (T)serializer.ReadObject(fs);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return default;
        }
    }
}