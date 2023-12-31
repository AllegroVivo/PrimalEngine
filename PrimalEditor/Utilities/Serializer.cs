﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

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
            Debug.WriteLine(ex.Message);
            // TODO: log error
        }
    }

    internal static T FromFile<T>(String path)
    {
        try
        {
            using FileStream fs = new(path, FileMode.Open);
            DataContractSerializer serializer = new(typeof(T));
            T instance = (T)serializer.ReadObject(fs);
            return instance;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            // TODO: log error
            return default;
        }
    }
}