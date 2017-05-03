using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class Pitfall : Obstacle
    {
        public static Texture2D texture;

        public Pitfall(Vector2 pos, Texture2D tex)
            : base(pos, tex)
        {
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            layerDepth = .9f;
            texWidth = tex.Width;
        }

        public override void checkCollision(Player player, List<Soldier> soldiers)
        {
            if (rect.Contains(player.position + new Vector2(0, 30))) // corrected for height of player sprite (evaluates at feet, so player can walk along bottom edge)
            {
                player.die();
                GameManager.soundManager.player_fallDeathInstance.Play();
            }
            foreach (Soldier s in soldiers)
            {
                if (rect.Contains(s.position + new Vector2(0, 30)))
                {
                    s.dead = true;
                    GameManager.soundManager.getSoldierDeathInstance().Play();
                }
            }
        }
    }
}
