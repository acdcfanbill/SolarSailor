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
    /// GameTimer class. Keeps track of time since you started playing, once you've hit your
    /// max time it sets a boolean that hte game is over.  the model manager checks this to see
    /// if it should set gameover. -bill
    /// </summary>
    public class GameTimer
    {
        private double timeSinceStart;
        private double endTime;
        private bool gameOver;

        /// <summary>
        /// GameTimer constructor
        /// </summary>
        /// <param name="endTime">Length of the entire game before ingame time bonuses</param>
        public GameTimer(float endTime)
        {
            this.timeSinceStart = 0f;
            this.endTime = (double)endTime;
            gameOver = false;
        }

        /// <summary>
        /// Update for the Timer.  This updates the realtime elapsed during gameplay and
        /// if we are over the timer, then it sets a game over boolean to true.
        /// </summary>
        /// <param name="gT">GameTime object, used to calculate time elapsed</param>
        public void Update(GameTime gT)
        {
            double secs = gT.ElapsedGameTime.TotalSeconds;
            timeSinceStart += secs;
            if (timeSinceStart > endTime)
                gameOver = true;
        }

        /// <summary>
        /// Allows adding time to the timer so you can fly longer
        /// </summary>
        /// <param name="secsToAdd">float of seconds to add to timer</param>
        public void AddTime(double secsToAdd)
        {
            endTime += secsToAdd;
        }

        /// <summary>
        /// Checks to see if we have passed the time limit
        /// </summary>
        /// <returns>true if time is over limit, false otherwise</returns>
        public bool CheckTimer()
        {
            return gameOver;
        }

        /// <summary>
        /// Returns the time left to play, useful for the HUD to display a timer
        /// </summary>
        /// <returns>double - time left to play</returns>
        public double GetTimeLeft()
        {
            return endTime - timeSinceStart;
        }
    }
}
