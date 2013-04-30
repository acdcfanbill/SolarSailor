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
using System.Threading;



namespace SolarSailor
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HUD : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //time and score variables
        TimeSpan convertedTimeLeft;
        public double score;
        double scoreDecrement;

        public Boolean start;
        KeyboardState keyBoardState;
        SpriteBatch spriteBatch;
        SpriteFont font;
        SpriteFont hudFont;

        string timeLeftString = "Time Left: ";
        string scoreString = "Score: ";
        string targetString = "Take Cargo To ";
        
        public HUD(Game game)
            : base(game)
        {
           
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("StartFont");
            hudFont = Game.Content.Load<SpriteFont>("HudFont");
            scoreDecrement = 100; //100 pts a second
            
            base.Initialize();
            
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //waits for player to start
            if (!start)
            {
                keyBoardState = Keyboard.GetState();
                if (keyBoardState.IsKeyDown(Keys.Space))
                {
                    start = true;
                    //tcb = updateTime;
                    //timer = new Timer(tcb, null, 0, 100);
                }
            }
            else
                updateScore(gameTime); //only update score if we've started

            convertedTimeLeft = TimeSpan.FromSeconds(Game1.modelManager.GetTimeRemaining());

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            if (!start)
            {
                spriteBatch.DrawString(font, "Press The Space Bar To Start Timer",
                                        new Vector2(Game.Window.ClientBounds.Width / 2-300, Game.Window.ClientBounds.Height / 2-75), Color.Red);
                spriteBatch.DrawString(hudFont, timeLeftString + convertedTimeLeft.ToString("g"), new Vector2(10, 10), Color.WhiteSmoke);
                spriteBatch.DrawString(hudFont, scoreString + score.ToString("N0"), new Vector2(Game.Window.ClientBounds.Width - 195, 10), Color.WhiteSmoke);
                spriteBatch.DrawString(hudFont, targetString, new Vector2(Game.Window.ClientBounds.Width / 2 - 140, 10), Color.WhiteSmoke);
            }
            else
            {
                spriteBatch.DrawString(hudFont, timeLeftString + convertedTimeLeft.ToString("g"), new Vector2(10, 10), Color.WhiteSmoke);
                spriteBatch.DrawString(hudFont, scoreString + score.ToString("N0"), new Vector2(Game.Window.ClientBounds.Width - 195, 10), Color.WhiteSmoke);
                spriteBatch.DrawString(hudFont, targetString, new Vector2(Game.Window.ClientBounds.Width/2-140, 10), Color.WhiteSmoke);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //public void updateTime(Object state)
        //{
        //    //timer is up
        //    if (timeLeftInMilSec<=0)
        //    {
        //        timer.Dispose();
        //        Game1.currentGameState = Game1.GameState.GameOver;
        //    }

        //    timeLeftInMilSec -= timeDecrement;
        //    //convertedTimeLeft = TimeSpan.FromMilliseconds(timeLeftInMilSec);
        //    convertedTimeLeft = TimeSpan.FromSeconds(Game1.modelManager.GetTimeRemaining());
        //    updateScore();
            
        //}

        public void updateScore(GameTime gt)
        {
            float secs = (float)gt.ElapsedGameTime.TotalSeconds;
            score -= secs * scoreDecrement;
        }

        public void newDelivery() 
        {
            
            //timeLeftInMilSec = timeLimit;
            //start = false;
            score += 10000;
            //scoreDecrement = 108 / timeLimit;
            //scoreDecrement = scoreDecrement * 10000;
        }

    }
}
