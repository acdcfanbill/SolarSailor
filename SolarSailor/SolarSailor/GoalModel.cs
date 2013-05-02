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
    /// I guess I didn't really need a new class for this in its current form, but if
    /// we ended up changing it, it might be handy to have its own class already. -bill
    /// </summary>
    class GoalModel : StaticModel
    {
        public GoalModel(Model m, Vector3 position, Vector3 rotation)
            : base(m, position, rotation)
        {
        }
    }
}
