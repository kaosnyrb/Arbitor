using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MastersProject
{
    class Camera
    {
        public static Vector2 CameraPosition = new Vector2(0, 0);

        public static Rectangle FieldOfView = new Rectangle();

        public static void Update()
        {
            CameraPosition.X = (int)CameraPosition.X;
            CameraPosition.Y = (int)CameraPosition.Y;

            FieldOfView.X = (int)CameraPosition.X - 100;
            FieldOfView.Y = (int)CameraPosition.Y - 100;
            FieldOfView.Width = 1000;
            FieldOfView.Height = 800;
        }
    }
}
