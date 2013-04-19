using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolarSailor
{
    class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;

        public BasicModel(Model m)
        {
            model = m;
        }

        public virtual void Update()
        {

        }

        public virtual void Draw(Camera camera)
        {
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
                    be.World =  mesh.ParentBone.Transform * GetWorld();
                }

                mesh.Draw();
            }
        }

        public virtual Matrix GetWorld()
        {
            return world;
        }
        //==========================================================
        //This likely needs to be changed to work
        //==========================================================
        public bool CollidesWith(Model otherModel, Matrix otherWorld)
        {
            //From book
            //Loop through each ModelMesh in both objects and compare
            //all bounding spheres for collisions.
            foreach (ModelMesh meshes in model.Meshes)
            {
                foreach (ModelMesh otherMeshes in model.Meshes)
                {
                    if (meshes.BoundingSphere.Transform(
                        GetWorld()).Intersects(
                        otherMeshes.BoundingSphere.Transform(otherWorld)))
                        return true;
                }
            }
            return false;
        }
    }
}
