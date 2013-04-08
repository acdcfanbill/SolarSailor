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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace SolarSailor
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matricies
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }
        public Vector3 _pos {get; set; }
        public Vector3 _target { get; set; }
        public Vector3 _up { get; set; }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up, float fov)
            : base(game)
        {
            view = Matrix.CreateLookAt(pos, target, up);

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(fov),
                (float)Game.Window.ClientBounds.Width /
                (float)Game.Window.ClientBounds.Height,
                1, 30000);
            _pos = pos;
            _target = target;
            _up = up;
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //create a new view matrix each frame.  depends on any updated position,
            //target and up direciton.
            view = Matrix.CreateLookAt(_pos, _target, _up);
            base.Update(gameTime);
        }

        /// <summary>
        /// Called up update the camera for each frame.  pass in camera position and target
        /// position as well as the 'up' vector for the camera
        /// </summary>
        /// <param name="pos">Vector3 camera position</param>
        /// <param name="target">Vector3 target for camera to look at</param>
        /// <param name="up">Vector3 pointing up for camera</param>
        public void UpdateCamera(Vector3 pos, Vector3 target, Vector3 up)
        {
            _pos = pos; _target = target; _up = up;
        }
    }
}
