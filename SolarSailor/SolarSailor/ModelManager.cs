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

        float x = 0f;
        float y = 0f;
        float z = 0f;

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
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            models.Add(new RotateableModel(Game.Content.Load<Model>(@"models/cube")));

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (!keyboardState.IsKeyDown(Keys.Z) && oldKeyboardState.IsKeyDown(Keys.Z))
            {
                if (z == 0f)
                {
                    x = 0f;
                    y = 0f;
                    z = .5f;
                }
                else
                    z = 0f;
            }
            if (!keyboardState.IsKeyDown(Keys.X) && oldKeyboardState.IsKeyDown(Keys.X))
            {
                if (x == 0f)
                {
                    x = .5f;
                    y = 0f;
                    z = 0f;
                }
                else
                    x = 0f;
            }
            if (!keyboardState.IsKeyDown(Keys.Y) && oldKeyboardState.IsKeyDown(Keys.Y))
            {
                if (y == 0f)
                {
                    x = 0f;
                    y = .5f;
                    z = 0f;
                }
                else
                    y = 0f;
            }

            if (!keyboardState.IsKeyDown(Keys.S) && oldKeyboardState.IsKeyDown(Keys.S))
            {
                x = 0f; y = 0f; z = 0f;
            }

            foreach (RotateableModel m in models)
            {
                m.Update(gameTime, x, y, z);
                //m.Update();
            }

            oldKeyboardState = keyboardState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (BasicModel m in models)
            {
                m.Draw(((Game1)Game).camera);
            }

            base.Draw(gameTime);
        }
    }
}