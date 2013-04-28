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
        //Had to make a new list to hold static models.
        List<BasicModel> staticModel = new List<BasicModel>();

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;
        MouseState mouseState;
        MouseState oldMouseState;
        Random randpos;
        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue trackCue;

        float x = 0f;
        float y = 0f;
        float z = 0f;
        float throttlePercent;
        bool rmb;

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
            randpos = new Random();

            BoundingSphereRenderer.InitializeGraphics(Game.GraphicsDevice, 30);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //models.Add(new UserShip(Game.Content.Load<Model>(@"models/cube"), 1.5f, 1.5f, 1.5f));
            models.Add(new UserShip(Game.Content.Load<Model>(@"models/SentinelSVForBlog"), 1.5f, 1.5f, 1.5f));

            //Still trying to figure out exactly what coordinates to pass to the constructor
            staticModel.Add(new StaticModel(Game.Content.Load<Model>(@"models/SentinelSVForBlog"),new Vector3(-50, 20, -35)));
            
            //Randomize these so that each course is different
            //Also, add a skin to them. I tried to do it but Blender was a bit confusing.
            for (int i = 0; i <= 100; i++)
            {
                AsteroidMaker();
            }
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            trackCue = soundBank.GetCue("Fusion shot");
            trackCue = soundBank.GetCue("Thrusters");
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
            getMouseInput(ref x, ref y, ref z, ref rmb);

            if (keyboardState.IsKeyUp(Keys.O) && oldKeyboardState.IsKeyDown(Keys.O))
                throttlePercent = Math.Min(throttlePercent + .05f, 1.0f);
            if (keyboardState.IsKeyUp(Keys.L) && oldKeyboardState.IsKeyDown(Keys.L))
                throttlePercent = Math.Max(throttlePercent - .05f, 0.0f);
            //have to check and see if we are moving the camera first
            if(rmb)
            {
                foreach (UserShip s in models)
                    s.UpdateCameraVariables(x, y, z);

                x = 0; y = 0; z = 0;//reset all these to zero so the ships direction doesnt change
            }

            //Doesn't update anything currently.
            foreach (StaticModel sm in staticModel)
            {
                sm.Update(gameTime);
                if (sm.CollidesWith(models[0].model, models[0].GetWorld()))
                {
                    UserShip us = (UserShip)models[0];
                    Vector3 pushDir = sm.initialPosition + us.GetPosition();
                    pushDir.Normalize();
                    pushDir *= 2;
                    us.PushShip(pushDir);
                    //throttlePercent = 0;
                    soundBank.PlayCue("Fusion shot");
                   throttlePercent = 0;
                   //soundBank.PlayCue("Fusion shot");
                }
            }
            //do ship's update
            foreach (UserShip m in models)
            {
                m.Update(gameTime, x, y, z, throttlePercent);
                //m.Update();
            }

            oldKeyboardState = keyboardState;
            soundBank.PlayCue("Thrusters");
            audioEngine.Update();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (UserShip m in models)
            {
                m.Draw(Game1.camera);
                foreach (ModelMesh meshes in m.model.Meshes)
                {
                    //BoundingSphereRenderer(meshes.BoundingSphere);
                    //BoundingSphereRenderer.Render(meshes.BoundingSphere, Game.GraphicsDevice, Game1.camera.view, Game1.camera.projection, Color.Red);
                }
            }

            foreach (StaticModel sm in staticModel)
            {
                //foreach (ModelMesh meshes in sm.model.Meshes)
                //{
                //    //BoundingSphereRenderer(meshes.BoundingSphere);
                //    BoundingSphereRenderer.Render(meshes.BoundingSphere, Game.GraphicsDevice, Game1.camera.view, Game1.camera.projection, Color.Red);
                //}
                sm.Draw(Game1.camera, Game.GraphicsDevice);
            }
            base.Draw(gameTime);
        }

        private void getMouseInput(ref float x, ref float y, ref float z, ref bool rmb)
        {
            //Noticed that the ship steers from the nose, rather than from
            //the midpoint or cockpit or whatever.
            mouseState = Mouse.GetState();
            if (mouseState.RightButton == ButtonState.Pressed)
                rmb = true;
            else
                rmb = false;

            x = mouseState.X - oldMouseState.X;
            y = mouseState.Y - oldMouseState.Y;
            z = mouseState.ScrollWheelValue - oldMouseState.ScrollWheelValue;

            //speed adjust
            //float speed = 5;
            //Y has -speed to fix inversion issue. Personal preference I suppose?
            //x *= speed; y *= speed; z *= speed;

            //may need to adjust camera/ship control
            if (Game1.invertYAxis)
                y = -y;

            //reset mouse position
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            oldMouseState = Mouse.GetState();
        }

        private void AsteroidMaker()
        {
            //Good god look at all those end parentheses.
            //Uses one random generator because otherwise we get some ridiculous problems with seeding
            //and all the asteroids end up in one nice, tight line instead of scattered everywhere.
            //--bill I added on a new vector to randomize z,y,z rotations so they are all facing the same way
            //staticModel.Add(new StaticModel(Game.Content.Load<Model>(@"models/spacerock"),
            //    new Vector3((randpos.Next(-500, 500)), (randpos.Next(-500, 500)), (randpos.Next(-500, 500))),
            //    new Vector3((randpos.Next(-10,10)),(randpos.Next(-10,10)),(randpos.Next(-10,10)))));
            staticModel.Add(new StaticModel(Game.Content.Load<Model>(@"models/spacerock"),
                new Vector3((randpos.Next(-500, 500)), (randpos.Next(-500, 500)), (randpos.Next(-500, 500)))));

            //skybox
            staticModel.Add(new StaticModel(Game.Content.Load<Model>(@"models/spacerock"),
                new Vector3((randpos.Next(-25000, 25000)), (randpos.Next(-25000, 25000)), (randpos.Next(-25000, 25000))),
                new Vector3((randpos.Next(-400, 400)), (randpos.Next(-400, 400)), (randpos.Next(-700, 700)))));
        }
    }
}