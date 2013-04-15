﻿using System;
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
        float camDistance = 25;  //no clue what min/max shoudl be, have to test

        //need a max and a min pitch
        float maxPitch = 70; //in degrees
        float minPitch = -70; //in degrees

        //need a max and min distance
        float maxZoom = 100; //in... dimensionless units?
        float minZoom = 10;

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

            Matrix additionalRot = Matrix.CreateRotationX(zDelta) * Matrix.CreateRotationY(-xDelta) * Matrix.CreateRotationZ(-yDelta);
            shipRotation = additionalRot * shipRotation;
            float moveSpeed = secs * throttlePercent * maxSpeed;
     
            MoveForward(ref position, shipRotation, moveSpeed);

            UpdateCamera(moveSpeed, additionalRot);
            
        }

        private void UpdateCamera(float forwardSpeed, Matrix rotation)
        {
            Vector3 camAngle = new Vector3(1,0,0);
            camAngle = Vector3.Transform(camAngle, shipRotation);
            camAngle = Vector3.Transform(camAngle, Matrix.CreateRotationY(MathHelper.ToRadians(camHeading))); //rotate camera position for x
            camAngle = Vector3.Transform(camAngle, Matrix.CreateRotationZ(MathHelper.ToRadians(camInclination))); //rotate cameras inclination
            camAngle.Normalize();
            camAngle *= camDistance;
            Vector3 campos = this.position;
            campos = campos + camAngle;

            Game1.camera.UpdateCamera(campos, this.position, shipRotation.Up);
        }

        public void UpdateCameraVariables(float x, float y, float z)
        {
            camHeading += secs * x;
            if (camHeading < 0)
                camHeading = 360;
            if (camHeading > 360)
                camHeading = 0;
            camInclination += secs * y;
            if (camInclination > maxPitch)
                camInclination = maxPitch;
            if (camInclination < minPitch)
                camInclination = minPitch;
            camDistance += secs * -z;
            if (camDistance > maxZoom)
                camDistance = maxZoom;
            if (camDistance < minZoom)
                camDistance = minZoom;

            //I'm not convinced Clamp actually works...
            MathHelper.Clamp(camInclination, minPitch, maxPitch);
            MathHelper.Clamp(camDistance, minZoom, maxZoom);
        }

        /// <summary>
        /// Function to move the ship forward
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="forwardSpeed"></param>
        private void MoveForward(ref Vector3 position, Matrix rotation, float forwardSpeed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(-1, 0, 0), rotation);
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
