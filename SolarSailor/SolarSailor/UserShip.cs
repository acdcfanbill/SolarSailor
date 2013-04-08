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
        //going to store velocity as Forward/Back,Left/Right,Up/Down
        //the Update should also reduce the Left/Right and Up/Down velocity over time so the ship
        //tries to fly true.
        //Matrix rotation = Matrix.Identity;
        //Matrix translation = Matrix.Identity;
        //Vector3 velocity = Vector3.Zero;
        //Vector3 position = Vector3.Zero;

        //Matrix localWorld;


        //=================================================================

        Vector3 position = Vector3.Zero;
        Quaternion shipRotation = Quaternion.Identity;

        //==================================================================

        //need a maximum turn speed for the ship
        float _maxXRad, _maxYRad, _maxZRad;

        float secs;

        public UserShip(Model m, float maxXRad, float maxYRad, float maxZRad)
            :base(m)
        {
            secs = 0;
            this._maxXRad = maxXRad; this._maxYRad = maxYRad; this._maxZRad = maxZRad;
        }

        public void Update(GameTime gameTime, float inputXDeg, float inputYDeg, float inputZDeg, float fwdInput)
        {
            secs = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //change to radians
            inputZDeg = MathHelper.ToRadians(inputZDeg); inputXDeg = MathHelper.ToRadians(inputXDeg); inputYDeg = MathHelper.ToRadians(inputYDeg);
            if (inputZDeg > _maxZRad)
                inputZDeg  = _maxZRad;
            if (inputXDeg > _maxXRad)
                inputXDeg = _maxXRad;
            if (inputYDeg > _maxYRad)
                inputYDeg = _maxYRad;

            float xDelta = secs * inputXDeg; float yDelta = secs * inputYDeg; float zDelta = secs * inputZDeg;

            Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), xDelta) * Quaternion.CreateFromAxisAngle(new Vector3(-1, 0, 0), yDelta) * Quaternion.CreateFromAxisAngle(new Vector3(0,-1,0), zDelta);
            shipRotation *= additionalRot;
            float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 500.0f * fwdInput;
            MoveForward(ref position, shipRotation, moveSpeed);

            //UpdateCamera();
            
        }

        private void UpdateCamera()
        {
            Vector3 campos = new Vector3(0, 25, 0);// Game1.camera._pos - position;
            campos = Vector3.Transform(campos, Matrix.CreateFromQuaternion(shipRotation));
            campos += position;

            Vector3 camup = new Vector3(0, 1, 0);
            camup = Vector3.Transform(camup, Matrix.CreateFromQuaternion(shipRotation));
            Game1.camera.UpdateCamera(campos, this.position, camup);
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
        /// <param name="speed"></param>
        private void MoveForward(ref Vector3 position, Quaternion rotationQuat, float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, -1, 0), rotationQuat);
            position += addVector * speed;
        }

        //public override Matrix GetWorld()
        //{
        //    return translation;
        //}

        public override void Draw(Camera camera)
        {
            Matrix worldMatrix = Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateFromQuaternion(shipRotation) * Matrix.CreateTranslation(position);
 
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
                     be.World = worldMatrix * mesh.ParentBone.Transform;
                 }
                 mesh.Draw();
             }
        }
    }
}
