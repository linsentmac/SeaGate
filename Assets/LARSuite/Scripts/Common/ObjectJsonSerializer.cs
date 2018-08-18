//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace LARSuite
{
    /// <summary>
    /// Serialize an object to/from a Json file.
    /// </summary>
    public class ObjectJsonSerializer<T> : IObjectSerializer<T> {
        public void SerializeToFile(string path, T theObject) {
            throw new NotImplementedException();
        }

        public T DeserializeFromFile(string path) {
            throw new NotImplementedException();
        }
    }
}
