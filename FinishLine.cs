using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class FinishLine
    {
        public static Texture2D texture;
        public float layerDepth;
        public Rectangle rect;
        Vector2 position;
        bool playerCleared, soldiersCleared;
        Dictionary<Soldier, bool>soldierFinish;
        List<Soldier> previousList;

        public FinishLine(Vector2 pos, Texture2D tex)
        {
            position = pos;
            rect = new Rectangle((int)pos.X, (int)pos.Y, tex.Width, tex.Height);
            layerDepth = .99f;
            playerCleared = false;
            soldiersCleared = true;
            soldierFinish = new Dictionary<Soldier, bool>(Formation.soldiers.Count);
            foreach(Soldier s in Formation.soldiers)
            {
                soldierFinish.Add(s, false);
            }
            previousList = Formation.soldiers;
        }

        public bool cleared(Player player, List<Soldier> soldiers)
        {
            //if we lost someone, refresh dictionary
            if(soldiers.Count <= previousList.Count)
            {
                soldierFinish = new Dictionary<Soldier, bool>(soldiers.Count);
                foreach (Soldier s in Formation.soldiers)
                {
                    soldierFinish.Add(s, false);
                }
            }
            if (player.rect.Y > rect.Y) // corrected for height of player sprite (evaluates at feet, so player can walk along bottom edge)
            {
                playerCleared = true;
            }
            else
                playerCleared = false;

            foreach (Soldier s in soldiers)
            {
                if (s.rect.Y > rect.Y)
                {
                    soldierFinish[s] = true;
                }
            }
            if (soldiers.Count > 0) // only if soldiers exist, otherwise we get exception
                soldiersCleared = soldierFinish.ElementAt(0).Value; // true or false
            else soldiersCleared = false;
            if (soldiersCleared)
            {
                foreach (Soldier s in soldierFinish.Keys)
                {
                    // assuming we started true, check if any in Dict are false. if so, break;
                    if (soldierFinish[s] == false)
                    {
                        soldiersCleared = false;
                        break;
                    }
                }
            }
            // if all soldiers are accounted for...
            // and player is clear, return true
            if (playerCleared && soldiersCleared)
                return true;
            else
                return false;
            previousList = Formation.soldiers; // in case list is different next check
        }

        public virtual void Update(GameTime gameTime)
        {
            position.Y -= GameManager.worldScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //layerDepth = .9f + (0f - .9f) * ((position.Y - 0) / (GameManager.screenHeight - 0));
            // update collision rect without affecting width or height
            rect.X = (int)position.X;
            rect.Y = (int)position.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, layerDepth: layerDepth);
        }
    }
}
