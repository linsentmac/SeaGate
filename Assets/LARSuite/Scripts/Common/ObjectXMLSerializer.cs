//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Xml.Serialization;

namespace LARSuite
{
    /// <summary>
    /// Serialize an object to/from a XML file.
    /// </summary>
    public class ObjectXMLSerializer<T> : IObjectSerializer<T> {
        public void SerializeToFile(string path, T theObject) {
            Stream writer = null;
            try {
                writer = FileManager.Instance.OpenFile(path, FileMode.OpenOrCreate);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, theObject);
            } catch(IOException ex) {
                throw new ObjectSerializerException(ex.Message);
            } finally {
                writer.Close();
            }
        }

        public T DeserializeFromFile(string path) {
            try {
                Stream reader = FileManager.Instance.OpenFile(path, FileMode.Open);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T newObject =  (T)serializer.Deserialize(reader);
                reader.Close();
                return newObject;
            } catch(IOException ex) {
                throw new ObjectSerializerException(ex.Message);
            }finally {
            }

        }
    }
}

