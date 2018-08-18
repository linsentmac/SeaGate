//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace LARSuite
{
    /// <summary>
    ///  Provide the head <see cref="Position"/> data from <see cref="PostitionEmulator"/> 
    ///  or real glass device(WIP). 
    /// </summary>
    public class PositionProvider : Singleton<PositionProvider> {

        private Position _latest;
        private Position _previous;

        private PositionProvider() {
        }

        public Position GetPosAtTime(double time) {

            return _latest;
        }

        public void Update(Position pos) {

            if (pos == null) return;

            if(_previous == null) {
                _previous = new Position();
            }

            if (_latest == null) {
                _previous.CopyFrom(pos);
            } else {
                _previous.CopyFrom(_latest);
            }

            if (_latest == null) {
                _latest = new Position();
            }

            _latest.CopyFrom(pos);
        }
    }

}
