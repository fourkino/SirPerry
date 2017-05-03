using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class GameScreen : Screen
    {
        public GameScreen()
        {
            Type = "GameScreen";
        }

        public override void Update(GameTime gameTime)
        {
            Type = "PauseScreen";
        }
    }
}
