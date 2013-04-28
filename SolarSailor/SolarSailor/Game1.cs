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
    /// This is the main type for your game
    /// </summary>
    /// 
    
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;
        Cue trackCue;

        public static Menus menu;
        //graphics properties
        public static Vector2 screenSize;
        private int preferredWidth;
        private int preferredHeight;
        private bool fullscreen = false;

        ModelManager modelManager;
        public static Camera camera;

        Texture2D sampleOverlay;

        //some static variables
        public static float _fov = 60;

        //some people may want to invert the control of the camera's y axis
        //we could probably make this controlable in the menu
        public static bool invertYAxis = true;

        //GameState info
        public enum GameState { StartUp, MainMenu, Credits, PauseMenu, NewGame, InGame, GameOver, YouWin, InstructionScreen, GameExit }
        public static GameState currentGameState = GameState.MainMenu;





        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            //fullscreen = true;

            if (fullscreen)
            {
                preferredWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                preferredHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                preferredWidth = 1024;
                preferredHeight = 768;
                graphics.IsFullScreen = false;
            }
            //don't want fulscreen deving, want a windowed game.
            graphics.PreferredBackBufferWidth = preferredWidth;
            graphics.PreferredBackBufferHeight = preferredHeight;
            graphics.PreferMultiSampling = false;

            screenSize = new Vector2(preferredWidth, preferredHeight);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camera = new Camera(this, new Vector3(5, -5, 15), Vector3.Zero, Vector3.Up, _fov);
            Components.Add(camera);

            menu = new Menus(this);
            Components.Add(menu);
            //camera = new ThirdPersonCamera();
            //camera.Perspective(90, (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
            //    1.0f, 3000.0f);
            //camera.LookAt(new Vector3(-15f, 10f, 0f),
            //    Vector3.Zero, Vector3.Up);

            currentGameState = GameState.StartUp;
            modelManager = new ModelManager(this);
            Components.Add(modelManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            sampleOverlay = Content.Load<Texture2D>(@"models\sampleoverlay");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            audioEngine = new AudioEngine(@"Content\Audio\GameAudio.xgs");
            waveBank = new WaveBank(audioEngine, @"Content\Audio\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, @"Content\Audio\Sound Bank.xsb");
            trackCue = soundBank.GetCue("DST-1990");
            trackCue.Play();
            base.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            // TODO: Add your update logic here
            switch (currentGameState)
            {
                case GameState.StartUp:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    break;
                case GameState.MainMenu:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    break;
                case GameState.NewGame:
                    modelManager = new ModelManager(this);
                    Components.Add(modelManager);
                    currentGameState = GameState.InGame;
                    break;
                case GameState.InGame:
                    modelManager.Enabled = true;
                    modelManager.Visible = true;
                    break;
                case GameState.PauseMenu:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    break;
                case GameState.Credits:
                    //spriteManager.Enabled = false;
                    //spriteManager.Visible = false;
                    break;
                case GameState.GameOver:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    Components.Remove(modelManager);
                    break;
                case GameState.YouWin:
                    modelManager.Enabled = false;
                    modelManager.Visible = false;
                    Components.Remove(modelManager);
                    break;
                case GameState.GameExit:
                    this.Exit();
                    break;
            }
            audioEngine.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.MainMenu:
                    break;
                case GameState.NewGame:
                    break;
                case GameState.InGame:
                    set3DDrawing();
                    break;
                case GameState.PauseMenu:
                    break;
                case GameState.GameOver:
                    break;
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
            //Here we add 2d stuff, I suppose
            spriteBatch.Begin();
            //Tried to center this as best as I could. The static values in between
            //(the 5 and the 100) are the modifiers for getting it centered. I think we'll
            //need a better crosshair than this, mine's kinda bad but I made it in about 45 seconds.
            spriteBatch.Draw(sampleOverlay,
                new Vector2((Window.ClientBounds.Width / 2)
                        - (sampleOverlay.Width / 2) + 5,
                        (Window.ClientBounds.Height / 2) - 100
                        - (sampleOverlay.Height / 2)),
                        Color.White);
            spriteBatch.End();
        }

        //need to setup drawing stuff for the 3d parts
        private void set3DDrawing()
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        }
    }
}
