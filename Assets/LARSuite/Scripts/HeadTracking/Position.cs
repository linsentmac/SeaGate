//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;

namespace LARSuite
{
    /// <summary>
    ///  Glass "Head" movement data. 
    /// </summary>
    public class Position {

        public Vector3 Pos { get; set; }
        public Quaternion Rot { get; set; }
        public double TimeStamp { get; set; }

        public void CopyFrom(Position pos) {
            if(pos != null) {
                Pos = pos.Pos;
                Rot = pos.Rot;
                TimeStamp = pos.TimeStamp;
            }
        }
    }
}
