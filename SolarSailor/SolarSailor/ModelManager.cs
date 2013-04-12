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
    public class ModelManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        List<BasicModel> models = new List<BasicModel>();
        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;
        MouseState mouseState;
        MouseState oldMouseState;

        float x = 0f;
        float y = 0f;
        float z = 0f;
        float throttlePercent;

        public ModelManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            throttlePercent = 0.1f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //models.Add(new RotateableModel(Game.Content.Load<Model>(@"models/cube")));
            models.Add(new UserShip(Game.Content.Load<Model>(@"models/SentinelSVForBlog"), 1.5f, 1.5f, 1f));

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            //this is to figure out how much to move the ship/camera
            getMouseInput(ref x, ref y, ref z);

            if (keyboardState.IsKeyUp(Keys.Add) && oldKeyboardState.IsKeyDown(Keys.Add))
                throttlePercent = Math.Min(throttlePercent + .1f, 1.0f);
            if (keyboardState.IsKeyUp(Keys.Subtract) && oldKeyboardState.IsKeyDown(Keys.Subtract))
                throttlePercent = Math.Max(throttlePercent - .1f, 0.0f);

            foreach (UserShip m in models)
            {
                m.Update(gameTime, x, y, z, throttlePercent);
                //m.Update();
            }

            oldKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (UserShip m in models)
            {
                m.Draw(Game1.camera);
            }

            base.Draw(gameTime);
        }

        private void getMouseInput(ref float x, ref float y, ref float z)
        {
            //Noticed that the ship steers from the nose, rather than from
            //the midpoint or cockpit or whatever.
            mouseState = Mouse.GetState();

            x = mouseState.X - oldMouseState.X;
            y = mouseState.Y - oldMouseState.Y;
            z = mouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue;

            //speed adjust
            float speed = 5;
            //Y has -speed to fix inversion issue. Personal preference I suppose?
            x *= speed; y *= -speed; z *= speed;

            //reset mouse position
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            oldMouseState = Mouse.GetState();
        }
    }
}