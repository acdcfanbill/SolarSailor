using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace SolarSailor
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class ObjectiveArrow : StaticModel
    {
        Matrix arrowRotation;

        public ObjectiveArrow(Model m, Vector3 initialPosition)
            : base(m, initialPosition)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, Vector3 shipPosition, Vector3 goalPosition)
        {
            //Get distance between points 
            Vector3 dist = shipPosition - goalPosition;
            dist.Normalize();

            //Get angle to arbitrary vector and compute the rotation axis 
            float theta = (float)Math.Acos(Vector3.Dot(dist, Vector3.Up));
            Vector3 cross = Vector3.Cross(Vector3.Up, dist);
            cross.Normalize();

            arrowRotation = Matrix.CreateFromAxisAngle(cross, theta);

            base.Update(gameTime);
        }

        public void SetPosition(Vector3 newPos)
        {
            this.position = newPos;
        }

        public override void Draw(Camera camera)
        {
            Matrix worldMatrix = Matrix.CreateRotationZ(MathHelper.Pi) * arrowRotation * Matrix.CreateTranslation(position);

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
            //this causes us to get two arrows, one of which isn't orienting toward the goal
            //base.Draw(camera);
        }
    }
}
