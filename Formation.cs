using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SirPerry_CCFRS
{
    class Formation
    {
        const int MAX_AI = 10;
        public static List<Soldier> soldiers;
        int i;

        public Formation()
        {
            soldiers = new List<Soldier>();
            soldiers.Capacity = MAX_AI;
            i = 0;
        }

        public void run(GameTime gameTime, Actor player)
        {
            if (soldiers.Count > 1)
            {
                // use for instead of foreach in order to modify list on death
                for (int i = soldiers.Count - 1; i > -1; i--)
                {
                    soldiers[i].run(soldiers, gameTime);
                    soldiers[i].fleeFromPlayer(player.position);
                    if (!Soldier.godMode)
                    {
                        checkDeath((Soldier)soldiers[i], i);
                    }
                }
            }
            else // if alone, adopt stopping behavior in relation to player
            {
                // use for instead of foreach in order to modify list on death
                for (int i = soldiers.Count - 1; i > -1; i--)
                {
                    soldiers[i].run(soldiers, gameTime);
                    soldiers[i].fleeFromPlayerAlone(player.position);
                    if (!Soldier.godMode)
                    {
                        checkDeath((Soldier)soldiers[i], i);
                    }
                }
            }
        }

        public void add(float posX, float posY)
        {
            if (soldiers.Count < soldiers.Capacity)
            {
                soldiers.Add(new Soldier(new Vector2(posX, posY)));
            }
            else
            {
                if (i >= soldiers.Capacity) i = 0; // check i

                soldiers[i++].position = new Vector2(posX, posY);
            }
        }

        // if dead, remove from list
        public void checkDeath(Soldier s , int i)
        {
            if(s.dead == true)
            {
                soldiers.RemoveAt(i);
            }
        }

        public void moveDown()
        {
            foreach(Soldier s in soldiers)
            {
                s.moveDown();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Soldier s in soldiers)
            {
                s.Draw(spriteBatch); // tell each soldier to draw itself
            }
        }
    }
}
