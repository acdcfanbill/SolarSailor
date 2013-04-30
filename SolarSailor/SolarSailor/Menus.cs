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
    public class Menus : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //Game States
        private Game1.GameState previousGameState;
        private Game1.GameState savedGameState;
        KeyboardState keyboardState;
        bool keyDown = false;
        bool moveDown = false;
        bool moveUp = false;

        //music stuff
        bool noMusic = true;

        //Main Menu Text
        SpriteBatch menuSpriteBatch;
        Texture2D deathScreen;
        Texture2D instructionScreen;
        Texture2D companyScreen;
        Texture2D presentsScreen;
        Texture2D gameScreen;
        Texture2D white;
        Texture2D credits;
        SpriteFont titleFont;
        SpriteFont menuFont;
        Color mColor;
        string title;
        List<String> menuOps;
        List<String> pauseOps;
        List<String> gameOverOps;
        List<String> instruction;
        List<String> creditsScreen;
        int sel;

        //startup timing
        int msSinceStart = 0;

        //need a scaled rectangle for the instruciton screen to display on
        Rectangle instRect;
        Rectangle startRect;

        //three options currently
        int selMax = 2;
        public Menus(Game game)
            : base(game)
        {

            //main menu
            mColor = Color.WhiteSmoke;
            title = "SOLAR SAILOR";
            menuOps = new List<string>();
            menuOps.Add("Start New Game");
            menuOps.Add("Instructions");
            menuOps.Add("Credits");
            menuOps.Add("Exit Game");

            //pause menu
            pauseOps = new List<string>();
            pauseOps.Add("Resume Game");
            pauseOps.Add("Instructions");
            pauseOps.Add("Exit Game");

            //game over menu
            gameOverOps = new List<string>();
            gameOverOps.Add("Start a New Game");
            gameOverOps.Add("Instructions");
            gameOverOps.Add("Credits");
            gameOverOps.Add("Exit Game");

            //Instruction screen
            instruction = new List<string>();
            instruction.Add("Steer your ship towards the goal by following the arrow!");
            instruction.Add("The mouse controls the movement of your ship, the mouse wheel allows you to roll.");
            instruction.Add("Press \"O\" to go faster, and \"L\" to slow down.");
            instruction.Add("Complete as many goals as you can before time runs out.");

            //Credits Screen
            creditsScreen = new List<string>();
            creditsScreen.Add("Solar Sailor Credits:");
            creditsScreen.Add("Marc Gubbels");
            creditsScreen.Add("Nate Lundin");
            creditsScreen.Add("Bill Conn");
            creditsScreen.Add("Gunnar Wiese");
            creditsScreen.Add("Joe Von Holtum");
            creditsScreen.Add("\n");
            creditsScreen.Add("Also thanks to:");
            creditsScreen.Add("Some people on the internet with decent models we could use.");

            //start with the top option
            sel = 0;

            //Text For menu 
            titleFont = Game.Content.Load<SpriteFont>("SpriteFont1");
            menuFont = Game.Content.Load<SpriteFont>("mFont");

            //pictures for startup
            companyScreen = Game.Content.Load<Texture2D>("textures/DTPlogo");
            presentsScreen = Game.Content.Load<Texture2D>("textures/presentsscreen");
            gameScreen = Game.Content.Load<Texture2D>("textures/introscreen");
            white = Game.Content.Load<Texture2D>("textures/introscreen");

            //credit scren
            credits = Game.Content.Load<Texture2D>("textures/credits");

            //picture for death screen
            deathScreen = Game.Content.Load<Texture2D>("textures/GameOverScreen");

            //picture for instructions (Don't need until we have instruction screen
            //instructionScreen = Game.Content.Load<Texture2D>("textures/instructions");
            /*
            //if the instruction screen is larger than the display size, we have to scale it
            //assumes we never run on a display that's taller than it is wide
            if (Game1.screenSize.Y < instructionScreen.Height)
            {
                float aR = instructionScreen.Width / instructionScreen.Height;
                int sideBuffer = Math.Abs((int)(Game1.screenSize.Y * aR) - instructionScreen.Width) / 2;
                instRect = new Rectangle(sideBuffer, 0, (int)Game1.screenSize.X - sideBuffer * 2, (int)Game1.screenSize.Y);
            }
            else
            {
                Vector2 center;
                center.Y = Game1.screenSize.Y / 2;
                center.X = Game1.screenSize.X / 2;
                instRect = new Rectangle((int)center.X - instructionScreen.Width / 2,
                    (int)center.Y - instructionScreen.Height / 2,
                    instructionScreen.Width,
                    instructionScreen.Height);
            }
             * */
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            menuSpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            keyboardState = Keyboard.GetState();
            //get the keystates
            bool sDown = keyboardState.IsKeyDown(Keys.S);
            bool downDown = keyboardState.IsKeyDown(Keys.Down);
            bool wDown = keyboardState.IsKeyDown(Keys.W);
            bool upDown = keyboardState.IsKeyDown(Keys.Up);
            bool escDown = keyboardState.IsKeyDown(Keys.Escape);
            bool enter = keyboardState.IsKeyDown(Keys.Enter);

            //need to save whatever the previous menu was so we can go back to that
            //when we do the esc key
            if (previousGameState == Game1.GameState.MainMenu)
                savedGameState = Game1.GameState.MainMenu;
            if (previousGameState == Game1.GameState.PauseMenu)
                savedGameState = Game1.GameState.PauseMenu;
            if (previousGameState == Game1.GameState.GameOver)
                savedGameState = Game1.GameState.GameOver;

            switch (Game1.currentGameState)
            {
                case Game1.GameState.StartUp:
                    msSinceStart += gameTime.ElapsedGameTime.Milliseconds;
                    if (!keyDown && enter)
                        msSinceStart = 6001;
                    if (msSinceStart > 6000)
                        Game1.currentGameState = Game1.GameState.MainMenu;
                    break;
                case Game1.GameState.MainMenu:
                    #region Main Menu
                    MediaPlayer.Stop();
                    noMusic = true;
                    selMax = menuOps.Count - 1;
                    if (!keyDown && (sDown || downDown))
                    {
                        keyDown = true;
                        moveDown = true;
                    }
                    if (!keyDown && (wDown || upDown))
                    {
                        keyDown = true;
                        moveUp = true;
                        moveDown = false; //ignore down
                    }
                    if (moveDown)
                    {
                        sel++;
                        if (sel > selMax)
                            sel = 0;
                        moveDown = false;
                    }
                    if (moveUp)
                    {
                        sel--;
                        if (sel < 0)
                            sel = selMax;
                        moveUp = false;
                    }

                    if (!keyDown && enter)
                    {
                        enter = false; //reset for next time
                        if (sel == 0) Game1.currentGameState = Game1.GameState.NewGame;
                        if (sel == 1) Game1.currentGameState = Game1.GameState.InstructionScreen;
                        if (sel == 2) Game1.currentGameState = Game1.GameState.Credits;
                        if (sel == 3) Game1.currentGameState = Game1.GameState.GameExit;
                    }
                    #endregion
                    break;
                case Game1.GameState.InstructionScreen:
                    #region Instruction Screen area
                    if (escDown)
                        Game1.currentGameState = savedGameState;
                    #endregion
                    break;
                case Game1.GameState.InGame:
                    #region In Game
                    if (noMusic)
                    {
                        noMusic = false;
                        //Game1.audioManager.playBackgroundMusic();
                    }
                    if (!keyDown && escDown)
                    {
                        Game1.currentGameState = Game1.GameState.PauseMenu;
                    }
                    #endregion
                    break;
                case Game1.GameState.PauseMenu:
                    #region Pause Menu
                    selMax = pauseOps.Count - 1;
                    noMusic = true;
                    MediaPlayer.Pause();
                    if (!keyDown && (sDown || downDown))
                    {
                        keyDown = true;
                        moveDown = true;
                    }
                    if (!keyDown && (wDown || upDown))
                    {
                        keyDown = true;
                        moveUp = true;
                        moveDown = false; //ignore down
                    }
                    if (moveDown)
                    {
                        sel++;
                        if (sel > selMax)
                            sel = 0;
                        moveDown = false;
                    }
                    if (moveUp)
                    {
                        sel--;
                        if (sel < 0)
                            sel = selMax;
                        moveUp = false;
                    }

                    if (enter)
                    {
                        enter = false; //reset for next time
                        if (sel == 0) Game1.currentGameState = Game1.GameState.InGame;
                        if (sel == 1) Game1.currentGameState = Game1.GameState.InstructionScreen;
                        if (sel == 2) Game1.currentGameState = Game1.GameState.GameExit;
                    }
                    #endregion
                    break;
                case Game1.GameState.Credits:
                    #region Credits
                    //Game1.audioManager.playCredits();
                    //won't play audio correctly, leaving out
                    noMusic = true;
                    if (!keyDown && escDown)
                        Game1.currentGameState = savedGameState;
                    #endregion
                    break;
                case Game1.GameState.GameOver:
                    #region GameOver
                    MediaPlayer.Stop();
                    noMusic = true;
                    selMax = gameOverOps.Count - 1;
                    if (!keyDown && (sDown || downDown))
                    {
                        keyDown = true;
                        moveDown = true;
                    }
                    if (!keyDown && (wDown || upDown))
                    {
                        keyDown = true;
                        moveUp = true;
                        moveDown = false; //ignore down
                    }
                    if (moveDown)
                    {
                        sel++;
                        if (sel > selMax)
                            sel = 0;
                        moveDown = false;
                    }
                    if (moveUp)
                    {
                        sel--;
                        if (sel < 0)
                            sel = selMax;
                        moveUp = false;
                    }

                    if (enter)
                    {
                        enter = false; //reset for next time
                        if (sel == 0) Game1.currentGameState = Game1.GameState.NewGame;
                        if (sel == 1) Game1.currentGameState = Game1.GameState.InstructionScreen;
                        if (sel == 2) Game1.currentGameState = Game1.GameState.Credits;
                        if (sel == 3) Game1.currentGameState = Game1.GameState.GameExit;
                    }
                    #endregion
                    break;
                    
            }
            //save the current gamestate for use next time
            previousGameState = Game1.currentGameState;

            //reset for next time through
            keyDown = sDown || downDown || wDown || upDown || escDown || enter;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            int c = 0;
            switch (Game1.currentGameState)
            {
                case Game1.GameState.StartUp:
                    #region Start Up
                    Game.GraphicsDevice.Clear(Color.Black);
                    menuSpriteBatch.Begin();
                    if (msSinceStart > 0 && msSinceStart < 2000)
                    {
                        startRect.X = ((int)Game1.screenSize.X - companyScreen.Width) / 2;
                        startRect.Y = ((int)Game1.screenSize.Y - companyScreen.Height) / 2;
                        startRect.Width = companyScreen.Width;
                        startRect.Height = companyScreen.Height;
                        menuSpriteBatch.Draw(companyScreen, startRect, Color.White);
                    }
                    if (msSinceStart >= 2000 && msSinceStart < 3000)
                    {
                        startRect.X = ((int)Game1.screenSize.X - presentsScreen.Width) / 2;
                        startRect.Y = ((int)Game1.screenSize.Y - presentsScreen.Height) / 2;
                        startRect.Width = presentsScreen.Width;
                        startRect.Height = presentsScreen.Height;
                        menuSpriteBatch.Draw(presentsScreen, startRect, Color.White);
                    }
                    if (msSinceStart >= 3000 && msSinceStart < 6000)
                    {
                        startRect.X = ((int)Game1.screenSize.X - gameScreen.Width) / 2;
                        startRect.Y = ((int)Game1.screenSize.Y - gameScreen.Height) / 2;
                        startRect.Width = gameScreen.Width;
                        startRect.Height = gameScreen.Height;
                        menuSpriteBatch.Draw(gameScreen, startRect, Color.White);
                    }
                    menuSpriteBatch.End();
                    #endregion
                    break;
                case Game1.GameState.MainMenu:
                    #region Main Menu
                    Game.GraphicsDevice.Clear(Color.Black);
                    menuSpriteBatch.Begin();
                    startRect.X = ((int)Game1.screenSize.X - gameScreen.Width) / 2;
                    startRect.Y = ((int)Game1.screenSize.Y - gameScreen.Height) / 2;
                    startRect.Width = gameScreen.Width;
                    startRect.Height = gameScreen.Height;
                    menuSpriteBatch.Draw(gameScreen, startRect, Color.White);
                    startRect.X = ((int)Game1.screenSize.X - white.Width) / 2;
                    startRect.Y = ((int)Game1.screenSize.Y - white.Height) / 2;
                    startRect.Width = white.Width;
                    startRect.Height = white.Height;
                    menuSpriteBatch.Draw(white, startRect, Color.White);
                    menuSpriteBatch.DrawString(titleFont, title,
                                            new Vector2((Game.Window.ClientBounds.Width / 2) - (titleFont.MeasureString(title).X / 2),
                                                        (Game.Window.ClientBounds.Height / 2) - (titleFont.MeasureString(title).Y / 2) - 50), Color.Gold);
                    for (int i = 0; i <= menuOps.Count - 1; i++)
                    {
                        if (sel == i) mColor = Color.Red;
                        menuSpriteBatch.DrawString(menuFont, menuOps[i],
                                            new Vector2((Game.Window.ClientBounds.Width / 2) - (titleFont.MeasureString(title).X / 2),
                                                        (Game.Window.ClientBounds.Height / 2) - (titleFont.MeasureString(title).Y / 2) + c), mColor);
                        mColor = Color.White;
                        c += 20;
                    }
                    menuSpriteBatch.End();
                    #endregion
                    break;
                case Game1.GameState.InstructionScreen:
                    #region Instruction Screen
                    Game.GraphicsDevice.Clear(Color.Black);
                    menuSpriteBatch.Begin();
                    menuSpriteBatch.Draw(gameScreen, instRect, Color.White);
                    for(int i = 0; i < instruction.Count; ++i)
                    {
                        menuSpriteBatch.DrawString(titleFont, instruction[i],
                                            new Vector2(0,
                                                         (Game.Window.ClientBounds.Height / 2) - (titleFont.MeasureString(title).Y / 2) + c), Color.WhiteSmoke);
                        c += 20;
                    }
                    menuSpriteBatch.End();
                    #endregion
                    break;
                case Game1.GameState.NewGame:
                    #region New Game
                    Game.GraphicsDevice.Clear(Color.Black);
                    #endregion
                    break;
                case Game1.GameState.PauseMenu:
                    #region Pause Menu
                  
                    Game.GraphicsDevice.Clear(Color.Black);
                    menuSpriteBatch.Begin();
                    menuSpriteBatch.DrawString(titleFont, title,
                                            new Vector2((Game.Window.ClientBounds.Width / 2) - (titleFont.MeasureString(title).X / 2),
                                                        (Game.Window.ClientBounds.Height / 2) - (titleFont.MeasureString(title).Y / 2) - 50), Color.Gold);
                    for (int i = 0; i <= pauseOps.Count - 1; i++)
                    {
                        if (sel == i) mColor = Color.Red;
                        menuSpriteBatch.DrawString(menuFont, pauseOps[i],
                                            new Vector2((Game.Window.ClientBounds.Width / 2) - (titleFont.MeasureString(title).X / 2),
                                                        (Game.Window.ClientBounds.Height / 2) - (titleFont.MeasureString(title).Y / 2) + c), mColor);
                        mColor = Color.WhiteSmoke;
                        c += 20;
                    }
                    menuSpriteBatch.End();
                    

                    #endregion
                    break;
                case Game1.GameState.GameOver:
                    #region Game Over
                    Game.GraphicsDevice.Clear(Color.Black);
                    menuSpriteBatch.Begin();
                    menuSpriteBatch.Draw(deathScreen, new Rectangle(0, 0, deathScreen.Width, deathScreen.Height),
                        Color.White);
                    menuSpriteBatch.DrawString(titleFont, title,
                                            new Vector2((Game.Window.ClientBounds.Width / 2) - (titleFont.MeasureString(title).X / 2),
                                                        (Game.Window.ClientBounds.Height / 2) - (titleFont.MeasureString(title).Y / 2) - 50), Color.Gold);
                    for (int i = 0; i <= gameOverOps.Count - 1; i++)
                    {
                        if (sel == i) mColor = Color.Red;
                        menuSpriteBatch.DrawString(menuFont, gameOverOps[i],
                                            new Vector2((Game.Window.ClientBounds.Width / 2) - (titleFont.MeasureString(title).X / 2),
                                                        (Game.Window.ClientBounds.Height / 2) - (titleFont.MeasureString(title).Y / 2) + c), mColor);
                        mColor = Color.WhiteSmoke;
                        c += 20;
                    }
                    menuSpriteBatch.End();
                    #endregion
                    break;
                case Game1.GameState.Credits:
                    #region Credits
                    Game.GraphicsDevice.Clear(Color.Black);
                    menuSpriteBatch.Begin();
                    menuSpriteBatch.Draw(credits, instRect, Color.White);
                    for (int i = 0; i < creditsScreen.Count; ++i)
                    {
                        menuSpriteBatch.DrawString(titleFont, creditsScreen[i],
                                            new Vector2(0, ((Game.Window.ClientBounds.Height / 2) - titleFont.MeasureString(title).Y / 2) + c),
                                                        Color.WhiteSmoke);
                        c += 20;
                    }
                    menuSpriteBatch.End();
                    #endregion
                    break;
                case Game1.GameState.GameExit:
                    #region Game Exit
                    Game.GraphicsDevice.Clear(Color.Black);
                    #endregion
                    break;
            }

            base.Draw(gameTime);
        }


    }
}