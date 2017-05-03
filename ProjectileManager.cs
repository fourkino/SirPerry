using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class ProjectileManager
    {
        List<Obstacle> projectiles;
        const int MAX_PROJECTILES = 5;
        int i = 0;
        Random random;
        // list of projectiles
        // add function to create projectile
        // update function to update projectiles
        // draw function to draw projectiles and related effects
        /* -- determine y of projectile when created, then draw shadow on ground "below" prjectile that
         * updates over time until boulder impacts, effect plays, collision check and end
         * 
         * When creating Boulders, instantiate at a higher Y value, and let its update descend its Y
         * over time.
         */
        public ProjectileManager()
        {
            projectiles = new List<Obstacle>();
            projectiles.Capacity = MAX_PROJECTILES;
            random = new Random();
        }
        public void add()
        {
            // rand vect between side margins and top/bottom of screen
            Vector2 randVector = new Vector2(random.Next(50, GameManager.screenWidth - 128),
                random.Next(250, GameManager.screenHeight));
            if (projectiles.Count < projectiles.Capacity)
                projectiles.Add(new Boulder(randVector, Boulder.texture));
            else
            {
                if (i >= projectiles.Capacity) i = 0;
                projectiles[i++].position = randVector;
            }
        }
        public void Update(GameTime gameTime, Player p)
        {
            foreach(Obstacle o in projectiles)
            {
                o.Update(gameTime);
                o.checkCollision(p, Formation.soldiers);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (projectiles.Count > 0)
            {
                foreach (Boulder o in projectiles)
                {
                    o.Draw(spriteBatch);
                    if (o.doneExploding)
                    {
                        projectiles.Remove(o);
                        break;
                    }
                }
            }
        }
    }
}
