//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

namespace LARSuite
{
    /// <summary>
    /// Interface for object serializer.
    /// </summary>
    public interface IObjectSerializer<T> {
        /// <summary>
        /// Serialize an object to a file.
        /// </summary>
        void SerializeToFile(string path, T theObject);

        /// <summary>
        /// Deserialize an object from a file.
        /// </summary>
        T DeserializeFromFile(string path);
    }
}

