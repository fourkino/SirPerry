using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SirPerry_CCFRS
{
    class Player : Actor
    {
        float playerSpeed;
        int margin;
        public static int health;
        public bool dead; // used to determine reset/game over/draw
        public bool inPit, blocked;
        public static bool marginEnabled, godMode;
        float damageTimer, damageTimeMax;
        float dashCooldownTimer, dashCooldownMax, dashTimer, dashTimeMax;
        bool dashed, dashOnCooldown;
        public Texture2D dashBarTexture;
        int dashTextureRow;

        public Player(Vector2 pos, Texture2D tex) 
            : base(pos, tex)
        {
            playerSpeed = 250f;
            margin = 50;
            health = 3;
            dead = false;
            inPit = false;
            blocked = false;
            damageTimeMax = 1f; // 1  second of invulnerability
            damageTimer = 0;
            dashTimer = 0;
            dashTimeMax = .3f; // dash lasts half a sec
            dashCooldownTimer = 0;
            dashCooldownMax = 1f; // cooldown of 1 second
            dashed = false;
            dashOnCooldown = false;
            dashTextureRow = 0;
            rect = new Rectangle((int)(position.X - (texture.Width / 3 / 2)), (int)(position.Y - (texture.Height / 4 / 2)), (tex.Width / 3) - 12, (tex.Height / 4) - 8);

            marginEnabled = true;
            godMode = false;
        }

        public void Update(GameTime gameTime)
        {
            this.position.Y -= GameManager.worldScrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; // scroll with world

            rect.X = (int)(position.X - (texture.Width/3/2)) + 8;
            rect.Y = (int)(position.Y - (texture.Height/4/2)) + 5;

            if (!godMode)
            {
                //if off top, take damage/die
                if (position.Y < -20)
                {
                    position.Y += 50;
                    takeDamage();
                }
            }

            // update damage timer
            damageTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // check dash timer/bool
            if(dashed)
            {
                dashTextureRow = 1;
                dashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(dashTimer >= dashTimeMax)
                {
                    playerSpeed = 200f;
                    dashTimer = 0;
                    dashed = false;
                    dashOnCooldown = true;
                }
            }
            // when dash is done, start cooldown
            if(dashOnCooldown)
            {
                dashCooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(dashCooldownTimer >= dashCooldownMax)
                {
                    dashCooldownTimer = 0;
                    dashOnCooldown = false;
                    dashTextureRow = 0; // dash bar full
                }
            }
            checkForBlocked(); // TODO
        }

        // this currently only draws player-based UI
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(dashBarTexture, position: position + new Vector2(0, texture.Height/5.5f),
                sourceRectangle: new Rectangle(0, (dashBarTexture.Height/2) * dashTextureRow, dashBarTexture.Width, dashBarTexture.Height/2),
                origin: new Vector2(dashBarTexture.Width / 2, dashBarTexture.Height / 2),
                scale: new Vector2(1.5f, 1.5f),
                layerDepth: (.9f + (0f - .9f) * (position.Y - 0) / (GameManager.screenHeight - 0)));
        }

        public void resetHealth()
        {
            health = 3;
        }

        public void takeDamage()
        {
            if (damageTimer >= damageTimeMax)
            {
                health -= 1;
                if (health <= 0)
                {
                    die();
                    GameManager.soundManager.player_crushDeathInstance.Play(); // sound from chip damage
                }
                else
                {
                    GameManager.soundManager.player_hurtInstance.Play();
                }
                damageTimer = 0;
            }
        }

        public void die()
        {
            if (!godMode) // pitfall calls this function directly, so disable if godMode
            {
                health = 0;
                dead = true;
                // specific fall death sound called in pitfall
            }
        }

        public void checkForBlocked()
        {
            foreach (Obstacle o in ObstacleManager.obstacles)
            {
                if (o is Barricade)
                {
                    if (o.rect.Intersects(rect))
                    {
                        blocked = true;

                        if ((rect.Left) < o.rect.Left && (rect.Top) < o.rect.Bottom - 10f) // left & bottom
                        {
                            position.X -= 3.5f;
                            blocked = false;
                        }
                        if ((rect.Left) < o.rect.Left && (rect.Bottom) > o.rect.Top + 10f) // left & top
                        {
                            position.X -= 3.5f;
                            blocked = false;
                        }
                        if ((rect.Right) > o.rect.Right && (rect.Top) < o.rect.Bottom - 10f) // right & bottom
                        {
                            position.X += 3.5f;
                            blocked = false;
                        }
                        if ((rect.Right) > o.rect.Right && (rect.Bottom) > o.rect.Top + 10f) // right & top
                        {
                            position.X += 3.5f;
                            blocked = false;
                        }
                        if ((rect.Right) > o.rect.Left + 10f && (rect.Top) < o.rect.Top) // top & left
                        {
                            position.Y -= 3.5f;
                            blocked = false;
                        }
                        if ((rect.Left) < o.rect.Right - 10f && (rect.Top) < o.rect.Top) // top & right
                        {
                            position.Y -= 3.5f;
                            blocked = false;
                        }
                        if ((rect.Right) > o.rect.Left + 10f && (rect.Bottom) > o.rect.Bottom) // bottom & left
                        {
                            position.Y += 3.5f;
                            blocked = false;
                        }
                        if ((rect.Left) < o.rect.Right - 10f && (rect.Bottom) > o.rect.Bottom) // bottom & right
                        {
                            position.Y += 3.5f;
                            blocked = false;
                        }
                    }
                }
            }
        }

        // move over time, if within bounds and not blocked by obstacle
        public void moveDown(float delta)
        {
            // if margin is enabled move according to those rules, 
            // otherwise level is clear and move off bottom
            if (marginEnabled)
            {
                if (position.Y < GameManager.screenHeight - margin && !blocked)
                    position.Y += playerSpeed * delta;
            }
            else
                position.Y += (playerSpeed/2f) * delta;
        }
        public void moveUp(float delta)
        {
            if(!blocked)
                position.Y -= playerSpeed * delta;
        }
        public void moveLeft(float delta)
        {
            if (position.X > margin && !blocked)
                position.X -= playerSpeed * delta;
        }
        public void moveRight(float delta)
        {
            if(position.X < GameManager.screenWidth - margin && !blocked)
                position.X += playerSpeed * delta;
        }
        public void dash() // BOOST!
        {
            if (!dashOnCooldown)
            {
                playerSpeed = 600f; // 250f by default
                dashed = true;

                // called here because of local cooldown check
                GameManager.soundManager.player_dashInstance.Volume = .6f; 
                GameManager.soundManager.player_dashInstance.Play();
            }
        }
    }
}
