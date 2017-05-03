using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SirPerry_CCFRS
{
    class Scoreboard
    {
        //public bool clicked = false;
        public string totalSoldiers_string { get; set; }
        public static SpriteFont font;
        Vector2 textMiddlePoint;
        int soldierScore;
        //string rating_string;
        public static Texture2D starsTexture;
        int currRow;
        Vector2 starsMidPoint;
        /*
        enum Rating
        {
            Terrible,
            Bad,
            Good,
            Excellent
        }
        Rating rating;
         */

        public Color TextColor { get; set; }
        public Color BackgroundColor { get; set; }
        public static Texture2D texture;

        public Rectangle rect;
        public Vector2 Position { get; set; }

        public Scoreboard(Vector2 pos, int numSoldiersSurvived, int numSoldiersTotal, Color tcolor, Color bgcolor)
        {
            totalSoldiers_string = "/"+numSoldiersTotal; // set screen specific text of score
            soldierScore = numSoldiersSurvived;
            Position = pos;
            rect = new Rectangle((int)Position.X, (int)Position.Y, 800, 200);
            TextColor = tcolor;
            BackgroundColor = bgcolor;
            starsMidPoint = new Vector2(starsTexture.Width / 2, starsTexture.Height/8);
            currRow = 0;

            /* rating based off percents
             * below 30 = terrible
             * between 30 and 60 = bad
             * between 60 and 90 = good
             * above 90 = excellent
             */
            /*
            if (soldierScore < (numSoldiersTotal*.3f))
                rating = Rating.Terrible;
            else if (soldierScore >= (numSoldiersTotal * .3f) && soldierScore < (numSoldiersTotal * .6f))
                rating = Rating.Bad;
            else if (soldierScore >= (numSoldiersTotal * .6f) && soldierScore < (numSoldiersTotal * .9f))
                rating = Rating.Good;
            else if (soldierScore >= (numSoldiersTotal * .9f))
                rating = Rating.Excellent;
            */
            if (soldierScore < (numSoldiersTotal * .3f)) 
                currRow = 0; // 1 star rating
            else if (soldierScore >= (numSoldiersTotal * .3f) && soldierScore < (numSoldiersTotal * .6f))
                currRow = 1; // 2 stars
            else if (soldierScore >= (numSoldiersTotal * .6f) && soldierScore < (numSoldiersTotal * .9f))
                currRow = 2; // 3 stars
            else if (soldierScore >= (numSoldiersTotal * .9f))
                currRow = 3; // 4 stars
            /*
            switch(rating)
            {
                case Rating.Terrible:
                    // terrible sprite
                    rating_string = "Terrible!";
                    break;
                case Rating.Bad:
                    rating_string = "Bad!";
                    break;
                case Rating.Good:
                    rating_string = "Good!";
                    break;
                case Rating.Excellent:
                    rating_string = "Excellent!";
                    break;
            }*/
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            textMiddlePoint = font.MeasureString(totalSoldiers_string) / 2;

            spriteBatch.DrawString(spriteFont: font, text: "Soldiers: " + soldierScore + totalSoldiers_string,
                position: new Vector2((rect.Center.X - rect.Width/3), rect.Center.Y),
                scale: new Vector2(2, 2), color: TextColor, rotation: 0f,
                origin: textMiddlePoint, effects: SpriteEffects.None, layerDepth: 0f);

            //textMiddlePoint = font.MeasureString(rating_string) / 2;
            /*
            spriteBatch.DrawString(spriteFont: font, text: rating_string,
                position: new Vector2(rect.Center.X + rect.Width/4, rect.Center.Y),
                scale: new Vector2(2, 2), color: TextColor, rotation: 0f,
                origin: textMiddlePoint, effects: SpriteEffects.None, layerDepth: 0f);
            */
            spriteBatch.Draw(starsTexture, null,
                sourceRectangle: new Rectangle(0, (starsTexture.Height/4 * currRow), starsTexture.Width, starsTexture.Height/4),
                destinationRectangle: new Rectangle(rect.Center.X + rect.Width/5, rect.Center.Y, starsTexture.Width, starsTexture.Height/4),
                origin: starsMidPoint,
                scale: new Vector2(1f, 1f),
                layerDepth: 0f);

            spriteBatch.Draw(texture, destinationRectangle: rect, scale: new Vector2(1, 1),
                color: BackgroundColor, layerDepth: .1f);
        }
    }
}
