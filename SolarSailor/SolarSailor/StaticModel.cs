using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;



/*This class is for building static world objects like our various goals, or something.
 Might need better functionality later but this is a decent start, at least.
 
 I made corresponding changes in ModelManager.
 We might have to change this into a few different classes that have goal "boxes" drawn around them
 to register our ship entering or exiting so that we are able to update gamestate and score and whatnot.
 */
namespace SolarSailor
{
    class StaticModel : BasicModel
    {
        //Trying to get the position to be set appropriately via constructor but I think
        //setting it to a Zero vector overrides it and it gets messed up when we go to draw with
        //the createTranslation(initialPosition) bit.

        //If I mess with it by assigning it actual values here, the initial position of the ship
        //changes accordingly.
        public Vector3 initialPosition {get; protected set;}
        BoundingSphere boundingSphere;
        float xRotation;
        float yRotation;
        float zRotation;

        public StaticModel(Model m, Vector3 position, Vector3 rotation)
            : base(m)
        {
            this.initialPosition = position;
            xRotation = rotation.X; yRotation = rotation.Y; zRotation = rotation.Z;
            boundingSphere.Center = position;
            boundingSphere.Radius = 10;
        }
        //helper constructor to allow construction with only position specified
        public StaticModel(Model m, Vector3 position)
            : this(m,position,Vector3.Zero)
        {
        }

        public void Initialize()
        {

        }

        public void Update(GameTime gameTime)
        {

        }

        public Vector3 GetPosition()
        {
            return initialPosition;
        }

        public override Matrix GetWorld()
        {
            return Matrix.CreateRotationX(xRotation) * Matrix.CreateRotationY(yRotation) *
                Matrix.CreateRotationZ(zRotation) * Matrix.CreateTranslation(initialPosition);
        }
        public void Draw(Camera camera, GraphicsDevice gd)
        {
            Matrix worldMatrix = Matrix.CreateRotationX(xRotation) * Matrix.CreateRotationY(yRotation) * 
                Matrix.CreateRotationZ(zRotation) * Matrix.CreateTranslation(initialPosition);

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
                BoundingSphereRenderer.Render(boundingSphere, gd, Game1.camera.view, Game1.camera.projection, mesh.ParentBone.Transform * worldMatrix, Color.Red);
            }
        }

        public override bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            //From book
            //Loop through each ModelMesh in both objects and compare
            //all bounding spheres for collisions.
            //foreach (ModelMesh meshes in model.Meshes)
            //{
            //    foreach (ModelMesh otherMeshes in otherModel.Meshes)
            //    {
            //        if (meshes.BoundingSphere.Transform(
            //            GetWorld()).Intersects(
            //            otherMeshes.BoundingSphere.Transform(otherWorld)))
            //        {
            //            return true;
            //        }
            //    }
            //}
            //return false;

            //writing my own since this seems to be fucked
            foreach (ModelMesh otherMeshes in otherModel.Meshes)
            {
                if (this.boundingSphere.Intersects(otherMeshes.BoundingSphere.Transform(otherWorld)))
                    return true;
            }
            return false;
        }
    }
}
