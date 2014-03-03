using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cascade
{
    public class Camera
    {
        Vector3 pos = Vector3.Zero;
        public Vector3 Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }
        Vector3 lookAtPos = Vector3.Zero;
        public Vector3 LookAtPos
        {
            get
            {
                return lookAtPos;
            }
            set
            {
                lookAtPos = value;
            }
        }
        public Camera()
        {

        }
    }
}
