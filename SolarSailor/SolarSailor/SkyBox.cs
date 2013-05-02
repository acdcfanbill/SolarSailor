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
    /// partial class, there are a bunch of other things in the skyboxbackrefrence class to do
    /// the box creation etc.
    /// </summary>
    public partial class SkyBox : Microsoft.Xna.Framework.GameComponent
    {
        TextureCube skyTex;

        Effect skyEffect;

        Matrix SkyWorld, View, Projection;

        Vector3 originalView = new Vector3(0, 0, 10);
        Vector3 position = Vector3.Zero;

        public SkyBox(Game game)
            : base(game)
        {
            this.LoadContent();
        }

        private void LoadContent()
        {
            skyEffect = Game.Content.Load<Effect>("SkyEffect");
            skyTex = Game.Content.Load<TextureCube>("SkyBoxTex");
            skyEffect.Parameters["tex"].SetValue(skyTex);

            SkyWorld = Matrix.Identity;
            View = Matrix.CreateLookAt(position, originalView, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(Game1._fov), Game.GraphicsDevice.Viewport.AspectRatio, 1, 20);

            //Backreference calls
            CreateCubeVertexBuffer();
            CreateCubeIndexBuffer();
        }

        public override void Update(GameTime gameTime)
        {
            //update posistion of the skybox so it follows the ship around
            position = Game1.modelManager.GetShipPosition();
            //update the world matrix for the skybox
            SkyWorld = Matrix.CreateTranslation(position);
            //update the view, need ship position, camera position, and the rotation of the ship for up, not camera up...
            View = Matrix.CreateLookAt(Game1.camera._pos, Game1.modelManager.GetShipPosition(), Game1.modelManager.GetShipRotation().Up);//Game1.camera._up);


            base.Update(gameTime);
        }

        /// <summary>
        /// drawing the skybox, uses a custom shader/effect
        /// </summary>
        public void DrawSkyBox()
        {
            Game.GraphicsDevice.Clear(Color.CornflowerBlue);

            Game.GraphicsDevice.SetVertexBuffer(vertices);
            Game.GraphicsDevice.Indices = indices;

            skyEffect.Parameters["WVP"].SetValue(SkyWorld * View * Projection);
            skyEffect.CurrentTechnique.Passes[0].Apply();

            Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, number_of_vertices, 0, number_of_indices / 3);
        }
    }
}
