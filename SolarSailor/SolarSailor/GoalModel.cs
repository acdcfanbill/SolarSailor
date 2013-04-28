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
    class GoalModel : StaticModel
    {
        private Vector3 vector3;

        public GoalModel(Model m, Vector3 position, Vector3 rotation)
            : base(m, position, rotation)
        {
        }
    }
}
