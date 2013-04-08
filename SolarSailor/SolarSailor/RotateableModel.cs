using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSailor
{
    class RotateableModel : BasicModel
    {
        Matrix _rotation = Matrix.Identity;
        //need a Vector for position and one for velocity
        //Vector3 _position;//position is global position
        //Vector3 _velocity;//velocity is relative to ships axes
        //going to need a maximum turn speed per ship
        float _maxXRad;
        float _maxYRad;
        float _maxZRad;

        public RotateableModel(Model m, float maxXRad, float maxYRad, float maxZRad)
            : base(m)
        {
            //set up max rotation values
            _maxXRad = maxXRad; _maxYRad = maxYRad; _maxZRad = maxZRad;
        }
        public RotateableModel(Model m)
            : this(m, 50f, 50f, 50f)
        {
        }

        /// <summary>
        /// Update RotateableModel
        /// </summary>
        /// <param name="x">rotation input in degrees</param>
        /// <param name="y">rotation input in degrees</param>
        /// <param name="z">rotation input in degrees</param>
        public virtual void Update(GameTime gameTime, float x, float y, float z)
        {
            float secs = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //change to radians
            z = MathHelper.ToRadians(z); x = MathHelper.ToRadians(x); y = MathHelper.ToRadians(y);
            if (z > _maxZRad)
                z = _maxZRad;
            if (x > _maxXRad)
                x = _maxXRad;
            if (y > _maxYRad)
                y = _maxYRad;

            float zDelta = secs * z * MathHelper.TwoPi;
            float xDelta = secs * x * MathHelper.TwoPi;
            float yDelta = secs * y * MathHelper.TwoPi;

            //_rotation *= Matrix.CreateFromYawPitchRoll(zDelta, yDelta, zDelta);

            _rotation = Matrix.CreateRotationX(xDelta) * Matrix.CreateRotationY(yDelta) * Matrix.CreateRotationZ(zDelta) * _rotation;

            base.Update();
        }

        public override Matrix GetWorld()
        {
            return world * _rotation;
        }
    }
}
