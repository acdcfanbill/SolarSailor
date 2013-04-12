using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SolarSailor
{
    class UserShip : BasicModel
    {
        //=================================================================

        Vector3 position = Vector3.Zero;
        Matrix shipRotation = Matrix.Identity;

        float camHeading = 0; //zero is directly behind the ship, so it 360, hence it's deg
        float camInclination = 20; //zero is directly behind the ship, also in degrees, -90 to 90
        float camDistance = 50;  //no clue what min/max shoudl be, have to test

        //==================================================================

        //need a maximum turn speed for the ship
        float _maxXRad, _maxYRad, _maxZRad;

        //max speed for the ship
        float maxSpeed = 200;

        float secs;

        public UserShip(Model m, float maxXRad, float maxYRad, float maxZRad)
            :base(m)
        {
            secs = 0;
            this._maxXRad = maxXRad; this._maxYRad = maxYRad; this._maxZRad = maxZRad;
        }

        public void Update(GameTime gameTime, float inputXDeg, float inputYDeg, float inputZDeg, float throttlePercent)
        {
            secs = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //change to radians
            inputZDeg = MathHelper.ToRadians(inputZDeg); inputXDeg = MathHelper.ToRadians(inputXDeg); inputYDeg = MathHelper.ToRadians(inputYDeg);
            //clamp rotation speed depending on the ship
            ClampRotation(ref inputXDeg, ref inputYDeg, ref inputZDeg);

            float xDelta = secs * inputXDeg; float yDelta = secs * inputYDeg; float zDelta = secs * inputZDeg;

            //Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(-1,0,0) , xDelta) * Quaternion.CreateFromAxisAngle(new Vector3(0, -1, 0), yDelta) * Quaternion.CreateFromAxisAngle(new Vector3(0,0,-1), zDelta);
            //Matrix temp = Matrix.CreateFromQuaternion(shipRotation);
            //Vector3 templeft = temp.Left; templeft.Normalize(); Vector3 tempforward = temp.Forward; tempforward.Normalize(); Vector3 tempup = temp.Up; tempup.Normalize();
            //Quaternion additionalRot = Quaternion.CreateFromAxisAngle(templeft, xDelta) * Quaternion.CreateFromAxisAngle(tempforward, yDelta) * Quaternion.CreateFromAxisAngle(tempup, zDelta);
            Matrix additionalRot = Matrix.CreateRotationX(zDelta) * Matrix.CreateRotationY(-xDelta) * Matrix.CreateRotationZ(-yDelta);
            shipRotation = additionalRot * shipRotation;
            float moveSpeed = secs * throttlePercent * maxSpeed;
     
            MoveForward(ref position, shipRotation, moveSpeed);

            //Game1.camera.Rotate(xDelta, zDelta);
            //Game1.camera.LookAt(this.position);
            //Game1.camera.Update(gameTime);
            //UpdateCamera(moveSpeed, additionalRot);
            
        }

        private void UpdateCamera(float forwardSpeed, Matrix rotation)
        {
            Vector3 campos = Game1.camera._pos;// -position;
            campos = Vector3.Transform(campos, rotation);
            //campos += position;
            //MoveForward(ref campos, shipRotation, forwardSpeed);

            //Vector3 camup = new Vector3(0, 1, 0);
            //camup = Vector3.Transform(camup, shipRotation);
            Game1.camera.UpdateCamera(campos, this.position, shipRotation.Up);
        }
        //temp keyboard controls
        //private void ProcessKeyboard(GameTime gameTime)
        //{
        //    float leftRightRot = 0;

        //    float turningSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
        //    turningSpeed *= 1.6f;
        //    KeyboardState keys = Keyboard.GetState();
        //    if (keys.IsKeyDown(Keys.Right))
        //        leftRightRot += turningSpeed;
        //    if (keys.IsKeyDown(Keys.Left))
        //        leftRightRot -= turningSpeed;

        //    float upDownRot = 0;
        //    if (keys.IsKeyDown(Keys.Down))
        //        upDownRot -= turningSpeed;
        //    if (keys.IsKeyDown(Keys.Up))
        //        upDownRot += turningSpeed;

        //    Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRot);
        //    shipRotation *= additionalRot;
        //}

        /// <summary>
        /// Function to move the ship forward
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotationQuat"></param>
        /// <param name="forwardSpeed"></param>
        private void MoveForward(ref Vector3 position, Matrix rotationQuat, float forwardSpeed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(-1, 0, 0), rotationQuat);
            position += addVector * forwardSpeed;
        }

        //public override Matrix GetWorld()
        //{
        //    return translation;
        //}

        public override void Draw(Camera camera)
        {
            Matrix worldMatrix = Matrix.CreateRotationY(MathHelper.Pi) * shipRotation * Matrix.CreateTranslation(position);
 
             Matrix[] transforms = new Matrix[model.Bones.Count];
             model.CopyAbsoluteBoneTransformsTo(transforms);
             foreach (ModelMesh mesh in model.Meshes)
             {
                 foreach (BasicEffect be in mesh.Effects)
                 {
                     be.EnableDefaultLighting();
                     be.PreferPerPixelLighting = true;
                     be.Projection = camera.projection;
                     be.View = camera.view;
                     be.World = mesh.ParentBone.Transform * worldMatrix;
                 }
                 mesh.Draw();
             }
        }

        private void ClampRotation(ref float inputXDeg, ref float inputYDeg, ref float inputZDeg)
        {
            if (inputZDeg > _maxZRad)
                inputZDeg = _maxZRad;
            if (inputZDeg < -_maxZRad)
                inputZDeg = -_maxZRad;
            if (inputXDeg > _maxXRad)
                inputXDeg = _maxXRad;
            if (inputXDeg < -_maxXRad)
                inputXDeg = -_maxXRad;
            if (inputYDeg > _maxYRad)
                inputYDeg = _maxYRad;
            if (inputYDeg < -_maxYRad)
                inputYDeg = -_maxYRad;
        }
    }
}
