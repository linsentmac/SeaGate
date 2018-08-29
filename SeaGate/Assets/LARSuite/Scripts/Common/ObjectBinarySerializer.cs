//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LARSuite
{
    /// <summary>
    /// Serialize an object to/from a binary file.
    /// </summary>
    public class ObjectBinarySerializer<T> : IObjectSerializer<T> {
        public void SerializeToFile(string path, T theObject) {
            Stream writer = null;
            try {
                writer = FileManager.Instance.OpenFile(path, FileMode.OpenOrCreate);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(writer, theObject);
            } catch (IOException ex) {
                throw new ObjectSerializerException(ex.Message);
            } finally {
                writer.Close();
            }
        }

        public T DeserializeFromFile(string path) {
            try {
                Stream reader = FileManager.Instance.OpenFile(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                T newObject = (T)formatter.Deserialize(reader);
                reader.Close();
                return newObject;
            } catch (IOException ex) {
                throw new ObjectSerializerException(ex.Message);
            } finally {
            }
        }
    }
}

