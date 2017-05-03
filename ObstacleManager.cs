using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class ObstacleManager
    {
        const int MAX_OBSTACLES =12;
        public static List<Obstacle> obstacles;
        int i;
        int obstacleTypeRNG;
        Random random;
        float generationTimer, generationTimeMax;
        ProjectileManager projectileManager; // .update called in update, .draw in draw, and .add called in generate
        int leftSide, rightSide, marginMax;
        float marginMinX, marginMaxX;

        public ObstacleManager()
        {
            obstacles = new List<Obstacle>();
            obstacles.Capacity = MAX_OBSTACLES;
            i = 0;
            obstacleTypeRNG = 0;
            random = new Random();
            generationTimer = 0;
            // ground obstacles are drawn from top left of texture, 
            // so following measurements are based off that
            leftSide = 50; // 0 + side margin
            rightSide = (GameManager.screenWidth - 50) - 192; // right side of screen w/ margin - width of obstacle textures
            marginMax = GameManager.screenWidth / 5;
            marginMinX = 0;
            marginMaxX = 0;

            switch (GameManager.difficulty) // set generation timer based off difficulty
            {
                case Difficulty.Easy:
                    generationTimeMax = 4f;
                    break;
                case Difficulty.Medium:
                    generationTimeMax = 3f;
                    break;
                case Difficulty.Hard:
                    generationTimeMax = 2f;
                    break;
            }
            projectileManager = new ProjectileManager();
        }

        public void Update(GameTime gameTime, Player p, bool finishLineSpawned)
        {
            foreach(Obstacle o in obstacles)
            {
                o.Update(gameTime);
                o.checkCollision(p, Formation.soldiers);
                //recycle(o);
            }
            if (!finishLineSpawned)
            {
                generate(gameTime, p); // generate Obstacles in this manager
            }
            projectileManager.Update(gameTime, p);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Obstacle o in obstacles)
            {
                o.Draw(spriteBatch);
            }
            projectileManager.Draw(spriteBatch);
        }
        public void add(Vector2 pos)
        {
            if (obstacles.Count < obstacles.Capacity)
            {
                obstacleTypeRNG = random.Next(0, 5); // 0 - 4
                // randomly add either barricade or pitfall
                if(obstacleTypeRNG <= 2)    // 60% barricades
                        obstacles.Add(new Barricade(pos, Barricade.texture));
                        
                if(obstacleTypeRNG > 2)     // 40% pitfalls
                        obstacles.Add(new Pitfall(pos, Pitfall.texture));
            }
            else
            {
                //update
                if (i >= obstacles.Capacity) i = 0;

                obstacles[i++].position = pos;
            }
        }

        public void generate(GameTime gameTime, Player p)
        {
            generationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(generationTimer >= generationTimeMax)
            {
                marginMinX = p.position.X - leftSide;
                marginMaxX = rightSide - p.position.X;

                // check max margin size
                if (marginMinX > marginMax)
                    marginMinX = marginMax;
                if (marginMaxX > marginMax)
                    marginMaxX = marginMax;

                // rand vect between margins based off player position
                Vector2 randVector = new Vector2(
                    random.Next((int)(p.position.X - marginMinX), (int)(p.position.X + marginMaxX)),
                    random.Next(GameManager.screenHeight, GameManager.screenHeight + 50));
                add(randVector); // create random obstacle at random pos
                projectileManager.add(); // create random projectile at random pos
                generationTimer = 0;
            }
        }

        // recycle for testing, delete for gameplay?
        /*
        public void recycle(Obstacle o)
        {
            if (o.rect.Y + o.rect.Height + 50 < 0)
            {
                o.position.Y += o.rect.Height + GameManager.screenHeight + 50;
            }
        }*/
    }
}
