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
    /// Customize Exception for object serialization.
    /// </summary>
    public class ObjectSerializerException : Exception  {
        public ObjectSerializerException() {
        }

        public ObjectSerializerException(string message)
        : base(message){
        }

        public ObjectSerializerException(string message, Exception inner)
        : base(message, inner){
        }
    }
}

