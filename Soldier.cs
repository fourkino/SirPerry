using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SirPerry_CCFRS
{
    class Soldier : Actor
    {
        public Vector2 position;
        Vector2 velocity;
        Vector2 acceleration;
        Vector2 maxSpeed;
        Vector2 maxForce;
        Vector2 maxBoundaryForce;
        Vector2 worldScrollForce;
        public float rotation;
        float angle;
        public static Texture2D texture;
        Vector2 separationVector, alignmentVector, cohesionVector;
        public static float separationWeight, alignmentWeight, cohesionWeight, playerFearFactor; // instantiated in GM for testing purposes
        public static float desiredSeparation, neighborhoodRadius, desiredPlayerRadius;
        public static float margin, bottomMargin;
        public static int screenWidth, screenHeight;
        public bool dead, inPit; // handled in boundary function and set from obstacle collision functions
        int i = 0;
        //drawing info
        public int rows, columns;
        int currRow, currCol;
        int currentFrame, totalFrames;
        int width, height;
        Rectangle sourceRect, destRect;
        float totalElapsed, timePerFrame, maxTPF, minTPF;
        float delta;
        float layerDepth;
        public static bool godMode, bottomMarginEnabled;
        Random random;

        public Soldier(Vector2 pos)
            : base(pos)
        {
            screenWidth = GameManager.screenWidth;
            screenHeight = GameManager.screenHeight;
            position = pos;
            velocity = new Vector2(0, 2);
            acceleration = new Vector2(0, 0);
            maxSpeed = new Vector2(3f, 3f);
            maxForce = new Vector2(0.1f, 0.1f); // force at which applied behaviors affect velocity. originally .1, lower = softer and slower response (.05)
            maxBoundaryForce = new Vector2(1, 1);
            worldScrollForce = new Vector2(0, -GameManager.worldScrollSpeed);
            rotation = 0;
            desiredSeparation = 50.0f; // personal space
            neighborhoodRadius = 250.0f; // interpersonal detection area of group
            desiredPlayerRadius = 150f; // higher = player detection over greater distance
            margin = 50; // from edges of screen
            bottomMargin = screenHeight * 0.25f; // from bottom of screen, 1/4
            dead = false;
            inPit = false;
            rect = new Rectangle((int)(position.X - (texture.Width / 3 / 2)), (int)(position.Y - (texture.Height / 4 / 2)), (texture.Width / 3) - 12, (texture.Height / 4) - 8);
            godMode = false;
            bottomMarginEnabled = true;

            //drawing info
            rows = 4;
            columns = 3;
            currentFrame = 0;
            totalFrames = rows * columns;
            maxTPF = 1 / 6f; // time per frame in fps
            minTPF = 1 / 2f;
            layerDepth = 0f; // 0 - 1, 0 being frontmost, 1 being backmost 
        }

        public void Update(GameTime gameTime)
        {
            //worldScrollForce = Vector2.Multiply(worldScrollForce, (float)gameTime.ElapsedGameTime.TotalSeconds);
            velocity = Vector2.Add(velocity, acceleration);
            //velocity = Vector2.Add(velocity, worldScrollForce);
            velocity = Vector2.Clamp(velocity, -maxSpeed, maxSpeed);

            // trigger redirect sounds at random when around a certain speed
            /*
            if(velocity.X >= maxSpeed.X*.75f
                || velocity.Y >= maxSpeed.Y*.75f)
            {
                random = new Random();

                if(random.Next(10) <= 1)
                {
                    //GameManager.soundManager.getSoldierSpawnInstance().Play();
                }
            }
             * */

            position = Vector2.Add(position, velocity);
            rect.X = (int)(position.X - (texture.Width / 3 / 2)) + 8;
            rect.Y = (int)(position.Y - (texture.Height / 4 / 2)) + 5;
            // map animation fps to velocity magnitude
            // first grab velocity mag
            float d = magnitude(velocity);
            // map speed from 0 - maxspeed to min tpf - max tpf (time per frame in frames per second)
            // map(value, istart, istop, ostart, ostop)
            // { return ostart + (ostop - ostart) * ((value - istart) / (istop -istart)); }
            timePerFrame = minTPF + (maxTPF - minTPF) * ((d - 0) / (maxSpeed.X - 0));

            // map y value to layerdepth of sprite. higher the y, BLANK the layerdepth, making sprites lower on screen draw "after"/"on top of" higher sprites
            layerDepth = .9f + (0f - .9f) * ((position.Y - 0) / (screenHeight - 0));

            // angle of velocity direction (applied to sprite in GM Draw)
            // theta = atan( y / x )
            angle = (float)(Math.Atan2(velocity.Y, velocity.X) + Math.PI/2);
            rotation = angle;
            
            // reset acceleration each update
            acceleration = Vector2.Multiply(acceleration, Vector2.Zero);
        }

        // calc ai direction
        public void calculateDirection(GameTime gameTime)
        {
            delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // decide which spritesheet row (anim) to use based on direction
            if (((float)(7 * Math.PI / 4)) <= rotation && rotation <= ((float)(Math.PI / 4))) // pointing up
            {
                currRow = 3;
            }

            if (((float)Math.PI / 4) < rotation && rotation < ((float)(3 * Math.PI / 4))) // pointing left
            {
                currRow = 2;
            }

            if (((float)(3 * Math.PI / 4)) <= rotation && rotation <= ((float)(5 * Math.PI / 4))) // pointing down
            {
                currRow = 0;
            }

            if (((float)(5 * Math.PI / 4)) < rotation && rotation < ((float)(7 * Math.PI / 4))) //pointing right
            {
                currRow = 1;
            }

            totalElapsed += delta;
            if (totalElapsed > timePerFrame)
            {
                currentFrame++;
                if (currentFrame == 3)
                {
                    currentFrame = 0;
                }
                totalElapsed -= timePerFrame;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            width = texture.Width / columns;
            height = texture.Height / rows;
            currCol = currentFrame % columns;

            sourceRect = new Rectangle(width * currCol, height * currRow, width, height);
            destRect = new Rectangle((int)position.X, (int)position.Y, width, height);

            spriteBatch.Draw(texture, null, destRect, sourceRect,
                origin: new Vector2(texture.Width / columns / 2, texture.Height / rows / 2),
                scale: new Vector2(.5f, .5f),
                layerDepth: layerDepth);
        }

        public void run(List<Soldier> soldiers, GameTime gameTime)
        {
            groupUp(soldiers);
            Update(gameTime);
            calculateDirection(gameTime);
            boundaries();
            checkForBlocked();
        }

        void groupUp(List<Soldier> soldiers)
        {
            // grouping involves three rules
            separationVector = separate(soldiers);
            alignmentVector = align(soldiers);
            cohesionVector = cohesion(soldiers);
            // weigh importance of each rule
            separationVector = Vector2.Multiply(separationVector, separationWeight);
            alignmentVector = Vector2.Multiply(alignmentVector, alignmentWeight);
            cohesionVector = Vector2.Multiply(cohesionVector, cohesionWeight);
            // apply forces
            applyForce(separationVector);
            applyForce(alignmentVector);
            applyForce(cohesionVector);
        }

        // check for nearby soldiers and steer away
        Vector2 separate(List<Soldier> soldiers)
        {
            Vector2 steeringVector = Vector2.Zero;
            int count = 0;

            // for every soldier in group, check if it's too close
            foreach(Soldier s in soldiers)
            {
                float d = Vector2.Distance(position, s.position);
                // if distance is greater than 0 and less than desired distance
                if((d > 0) && (d < desiredSeparation))
                {
                    // calculate vector pointing away from neighbor
                    Vector2 diff = Vector2.Subtract(position, s.position);
                    diff.Normalize();
                    diff = Vector2.Divide(diff, d);
                    steeringVector = Vector2.Add(steeringVector, diff);
                    count++;
                }
            }
            // get average
            if(count > 0)
            {
                steeringVector = Vector2.Divide(steeringVector, (float)count);
            }
            if(magnitude(steeringVector) > 0)
            {
                steeringVector.Normalize();
                steeringVector = Vector2.Multiply(steeringVector, maxSpeed);
                steeringVector = Vector2.Subtract(steeringVector, velocity);
                steeringVector = Vector2.Clamp(steeringVector, -maxForce, maxForce);
            }

            return steeringVector;
        }

        // check for nearby soldiers and calc average velocity
        Vector2 align(List<Soldier> soldiers)
        {
            Vector2 vectorSum = Vector2.Zero;
            int count = 0;

            foreach(Soldier s in soldiers)
            {
                float d = Vector2.Distance(position, s.position);
                if ((d>0) && (d < neighborhoodRadius))
                {
                    vectorSum = Vector2.Add(vectorSum, s.velocity);
                    count++;
                }
            }
            if(count > 0)
            {
                vectorSum = Vector2.Divide(vectorSum, (float)count);
                vectorSum.Normalize();
                vectorSum = Vector2.Multiply(vectorSum, maxSpeed);
                Vector2 steeringVector = Vector2.Subtract(vectorSum, velocity);
                steeringVector = Vector2.Clamp(steeringVector, -maxForce, maxForce);
                return steeringVector;
            }
            else
            {
                return Vector2.Zero;
            }
        }

        Vector2 cohesion(List<Soldier> soldiers)
        {
            Vector2 vectorSum = Vector2.Zero;
            int count = 0;

            foreach(Soldier s in soldiers)
            {
                float d = Vector2.Distance(position, s.position);
                if((d>0) && (d< neighborhoodRadius))
                {
                    vectorSum = Vector2.Add(vectorSum, s.position);
                    count++;
                }
            }
            if(count > 0)
            {
                // get average position and steer towards
                vectorSum = Vector2.Divide(vectorSum, count);
                return seek(vectorSum); 
            }
            else
            {
                return Vector2.Zero;
            }
        }

        void boundaries()
        {
            Vector2 desiredVector = Vector2.Zero;

            if(position.X < margin) // left boundary
            {
                desiredVector = new Vector2(maxSpeed.X, velocity.Y);
            }
            else if(position.X > screenWidth - margin) // right boundary
            {
                desiredVector = new Vector2(-maxSpeed.X, velocity.Y);
            }

            if(position.Y < -margin) // top boundary -- death
            {
                //desiredVector = new Vector2(velocity.X, maxSpeed.Y);
                dead = true;
                GameManager.soundManager.getSoldierDeathInstance().Play();
            }
            else if (bottomMarginEnabled)
            {
                if (position.Y > screenHeight - bottomMargin) // bottom boundary, slow down a bit
                {
                    desiredVector = new Vector2(velocity.X * .5f, -maxSpeed.Y * .5f);
                }
            }

            if(desiredVector != Vector2.Zero)
            {
                desiredVector.Normalize();
                desiredVector = Vector2.Multiply(desiredVector, maxSpeed);
                Vector2 steeringVector = Vector2.Subtract(desiredVector, velocity);
                steeringVector = Vector2.Clamp(steeringVector, -maxBoundaryForce, maxBoundaryForce);
                applyForce(steeringVector);
            }
        }

        public void checkForBlocked()
        {
            Vector2 desiredVector = Vector2.Zero;
            foreach (Obstacle o in ObstacleManager.obstacles)
            {
                if (o is Barricade)
                {
                    if (o.rect.Intersects(rect))
                    {
                        if ((rect.Left) < o.rect.Left && (rect.Top) < o.rect.Bottom - 10f) // left & bottom
                        {
                            desiredVector = new Vector2(-velocity.X * .5f,0);
                        }
                        if ((rect.Left) < o.rect.Left && (rect.Bottom) > o.rect.Top + 10f) // left & top
                        {
                            desiredVector = new Vector2(-velocity.X * .5f,0);
                        }
                        if ((rect.Right) > o.rect.Right && (rect.Top) < o.rect.Bottom - 10f) // right & bottom
                        {
                            desiredVector = new Vector2(velocity.X * .5f,0);
                        }
                        if ((rect.Right) > o.rect.Right && (rect.Bottom) > o.rect.Top + 10f) // right & top
                        {
                            desiredVector = new Vector2(velocity.X * .5f,0);
                        }
                        if ((rect.Right) > o.rect.Left + 10f && (rect.Top) < o.rect.Top) // top & left
                        {
                            desiredVector = new Vector2(velocity.X * .5f, -maxSpeed.Y * .5f);
                        }
                        if ((rect.Left) < o.rect.Right - 10f && (rect.Top) < o.rect.Top) // top & right
                        {
                            desiredVector = new Vector2(velocity.X * .5f, -maxSpeed.Y * .5f);
                        }
                        if ((rect.Right) > o.rect.Left + 10f && (rect.Bottom) > o.rect.Bottom) // bottom & left
                        {
                            desiredVector = new Vector2(velocity.X * .5f, maxSpeed.Y * .5f);
                        }
                        if ((rect.Left) < o.rect.Right - 10f && (rect.Bottom) > o.rect.Bottom) // bottom & right
                        {
                            desiredVector = new Vector2(velocity.X * .5f, maxSpeed.Y * .5f);
                        }
                    }
                }
            }
            if (desiredVector != Vector2.Zero)
            {
                desiredVector.Normalize();
                desiredVector = Vector2.Multiply(desiredVector, maxSpeed);
                Vector2 steeringVector = Vector2.Subtract(desiredVector, velocity);
                steeringVector = Vector2.Clamp(steeringVector, -maxBoundaryForce, maxBoundaryForce);
                applyForce(steeringVector);
            }
        }

        Vector2 seek(Vector2 target)
        {
            Vector2 desiredVector = Vector2.Subtract(target, position); // seeking

            // magnitude of desiredVector
            float d = magnitude(desiredVector);
            // if within detectionRadius to player, slow down to stop
            if (d < neighborhoodRadius)
            {
                // map(value, istart, istop, ostart, ostop)
                // { return ostart + (ostop - ostart) * ((value - istart) / (istop -istart)); }
                float m = 0 + (maxSpeed.X - 0) * ((d - 0) / (neighborhoodRadius - 0));
                desiredVector.Normalize();
                desiredVector = Vector2.Multiply(desiredVector, m);
            }
            else
            {
                desiredVector.Normalize();
                desiredVector = Vector2.Multiply(desiredVector, maxSpeed);
            }
            Vector2 steeringVector = Vector2.Subtract(desiredVector, velocity);
            steeringVector = Vector2.Clamp(steeringVector, -maxForce, maxForce);

            return steeringVector;
        }
        
        public void fleeFromPlayer(Vector2 target)
        {
            Vector2 steeringVector = Vector2.Zero; // flee direction

            // magnitude of desiredVector
            float d = Vector2.Distance(position, target);
            // if within radius to player, steer away
            if ((d > 0) && (d < desiredPlayerRadius))
            {
                // map(value, istart, istop, ostart, ostop)
                // { return ostart + (ostop - ostart) * ((value - istart) / (istop -istart)); }
                Vector2 diff = Vector2.Subtract(position, target);
                diff.Normalize();
                diff = Vector2.Divide(diff, d);
                steeringVector = Vector2.Add(steeringVector, diff);
            }
            if (magnitude(steeringVector) > 0)
            {
                steeringVector.Normalize();
                steeringVector = Vector2.Multiply(steeringVector, maxSpeed);
                steeringVector = Vector2.Subtract(steeringVector, velocity);
                steeringVector = Vector2.Clamp(steeringVector, -maxForce, maxForce);
            }

            steeringVector = Vector2.Multiply(steeringVector, playerFearFactor);
            applyForce(steeringVector);
        }
        
        public void fleeFromPlayerAlone(Vector2 target)
        {
            Vector2 desiredVector = Vector2.Subtract(position, target); // flee direction

            // magnitude of desiredVector
            float d = magnitude(desiredVector);
            // if within radius to player, steer away
            if ((d > 0) && (d < desiredPlayerRadius))
            {
                // map(value, istart, istop, ostart, ostop)
                // { return ostart + (ostop - ostart) * ((value - istart) / (istop -istart)); }
                float m = maxSpeed.X + (0 - maxSpeed.X) * ((d - 0) / (desiredPlayerRadius - 0));
                desiredVector.Normalize();
                desiredVector = Vector2.Multiply(desiredVector, m * 2);
            }
            else
            {
                desiredVector.Normalize();
                desiredVector = Vector2.Multiply(desiredVector, maxSpeed/9f);
            }
            
            Vector2 steeringVector = Vector2.Subtract(desiredVector, velocity);
            steeringVector = Vector2.Clamp(steeringVector, -maxForce, maxForce);
            

            steeringVector = Vector2.Multiply(steeringVector, playerFearFactor);
            applyForce(steeringVector);
        }

        public void moveDown()
        {
            velocity = new Vector2(0, maxSpeed.Y / 3);
        }

        void applyForce(Vector2 force)
        {
            acceleration = Vector2.Add(acceleration, force);
        }

        float magnitude(Vector2 vector)
        {
            // magnitude = SQRT(x^2 + y^2)
            return (float)(Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2)));
        }
    }
}
