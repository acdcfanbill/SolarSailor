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
using System.IO;

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
        GameTimer gameTimer;
        GoalModel goalRing;

        FileStream fs;
        StreamWriter sw;

        float x = 0f;
        float y = 0f;
        float z = 0f;
        float throttlePercent;
        bool rmb;
        bool goalExists;

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

            gameTimer = new GameTimer(Game1._gameLengthInSeconds);

            //this was in for debugging, it's been removed from all Model class drawing routines
            //BoundingSphereRenderer.InitializeGraphics(Game.GraphicsDevice, 30);

            base.Initialize();
        }

        /// <summary>
        /// load content, models, make asteroids, load sounds
        /// </summary>
        protected override void LoadContent()
        {
            models.Add(new UserShip(Game.Content.Load<Model>(@"models/SentinelSVForBlog"),
                Game.Content.Load<Model>(@"models/arrow"), 3f, 3f, 3f));

            //Still trying to figure out exactly what coordinates to pass to the constructor
            //staticModel.Add(new StaticModel(Game.Content.Load<Model>(@"models/SentinelSVForBlog"),new Vector3(-50, 20, -35)));
            
            //Randomize these so that each course is different
            //Also, add a skin to them. I tried to do it but Blender was a bit confusing.
            for (int i = 0; i <= 1500; i++)
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
            if (!goalExists)
            {
                GoalMaker();
            }

            //update the gameTimer
            gameTimer.Update(gameTime);
            if (gameTimer.CheckTimer())
            {
                //SaveScore();
                Game1.currentGameState = Game1.GameState.GameOver;
            }
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
                    Vector3 pushDir = sm.GetPosition() - us.GetPosition();
                    pushDir.Normalize();
                    pushDir *= -2;
                    us.PushShip(pushDir);

                   throttlePercent = 0; 
                   soundBank.PlayCue("Fusion shot");                    
                }
            }
            //do ship's update
            foreach (UserShip m in models)
            {
                m.Update(gameTime, x, y, z, throttlePercent);
                if(goalRing.CollidesWith(m.model, m.GetWorld()))
                    SuccessfulCapture();
            }

            oldKeyboardState = keyboardState;
            soundBank.PlayCue("Thrusters");
            audioEngine.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the world
        /// </summary>
        /// <param name="gameTime">gameTime object</param>
        public override void Draw(GameTime gameTime)
        {
            //draw the skybx
            Game1.skyBox.DrawSkyBox();

            //draw the goalring
            goalRing.Draw(Game1.camera);

            foreach (UserShip m in models)
            {
                m.Draw(Game1.camera, Game.GraphicsDevice);
            }

            foreach (StaticModel sm in staticModel)
            {
                sm.Draw(Game1.camera, Game.GraphicsDevice);
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// get the mouse input, puts movemetn into pass by ref variables
        /// </summary>
        /// <param name="x">change in x</param>
        /// <param name="y">change in y</param>
        /// <param name="z">change in mouse wheel</param>
        /// <param name="rmb">whether or not right mouse is down</param>
        private void getMouseInput(ref float x, ref float y, ref float z, ref bool rmb)
        {
            //Noticed that the ship steers from the nose, rather than from
            //the midpoint or cockpit or whatever.
            //This was fixed by moving the 'origin' in the ship model - bill
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
            //x *= speed; y *= speed; z *= speed;

            //may need to adjust camera/ship control
            if (Game1.invertYAxis)
                y = -y;

            //reset mouse position
            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);
            oldMouseState = Mouse.GetState();
        }

        private void GoalMaker()
        {
            if (goalExists)
                return;
            goalExists = true;
            bool collision = false;
            do
            {
                collision = false;
                goalRing = new GoalModel(Game.Content.Load<Model>(@"models/goalring"),
                     new Vector3((randpos.Next(-500, 500)), (randpos.Next(-500, 500)), (randpos.Next(-500, 500))),
                     new Vector3((randpos.Next(-10, 10)), (randpos.Next(-10, 10)), (randpos.Next(-10, 10))));
                foreach (StaticModel sm in staticModel)
                {
                    if(sm.CollidesWith(goalRing.model,goalRing.GetWorld()))
                        collision = true;
                }
            } while (collision);

        }

        private void AsteroidMaker()
        {
            //Good god look at all those end parentheses.
            //Uses one random generator because otherwise we get some ridiculous problems with seeding
            //and all the asteroids end up in one nice, tight line instead of scattered everywhere.
            //--bill I added on a new vector to randomize z,y,z rotations so they are all facing the same way
            staticModel.Add(new StaticModel(Game.Content.Load<Model>(@"models/spacerock"),
                new Vector3((randpos.Next(-500, 500)), (randpos.Next(-500, 500)), (randpos.Next(-500, 500))),
                new Vector3((randpos.Next(-10, 10)), (randpos.Next(-10, 10)), (randpos.Next(-10, 10)))));
        }

        /// <summary>
        /// Helper method, resets stuff due to us 'capturing' a goal
        /// </summary>
        private void SuccessfulCapture()
        {
            goalExists = false;
            gameTimer.AddTime(Game1._gameTimeAddedForSuccessfulCapture);
            Game1.hud.newDelivery();
        }

        /// <summary>
        /// returns the posision of hte goal
        /// </summary>
        /// <returns>Vector3 position of the goal</returns>
        public Vector3 GetGoalPosition()
        {
            return goalRing.GetPosition();

        }

        /// <summary>
        /// returns the time remaining in the game, hud needs this
        /// </summary>
        /// <returns>double, time left in seconds</returns>
        public double GetTimeRemaining()
        {
            return gameTimer.GetTimeLeft();
        }

        /// <summary>
        /// Helper function to return the ship's position. our controllable ship
        /// is the first one in the model list
        /// </summary>
        /// <returns>Vector3 of ship position</returns>
        public Vector3 GetShipPosition()
        {
            foreach (UserShip s in models)
            {
                return s.GetPosition();
            }
            throw new InvalidProgramException("No Usership to get Position from");
        }

        /// <summary>
        /// Helper function to return the ships rotation.  all these ship orientation
        /// methods are for the supid skybox 
        /// </summary>
        /// <returns>Matrix that describes the current ship rotation</returns>
        public Matrix GetShipRotation()
        {
            foreach (UserShip s in models)
            {
                return s.GetRotation();
            }
            throw new InvalidProgramException("No Usership to get rotation data from");
        }

        /// <summary>
        /// Returns the Usership, I don't know if we actually used this...
        /// </summary>
        /// <returns>UserShip class</returns>
        public UserShip getUserShip()
        {
            foreach (UserShip s in models)
            {
                return s;
            }
            throw new InvalidProgramException("No Usership to pass");
        }

        //unimplimented due to time constraints
        //public void SaveScore()
        //{
        //    fs = new FileStream(Game.Content.RootDirectory.ToString() + "/Scores.txt", FileMode.Open);
        //    sw = new StreamWriter(fs);

        //    sw.WriteLine(Game1.hud.score.ToString());

        //    sw.Close();
        //    fs.Close();
        //}
    }
}