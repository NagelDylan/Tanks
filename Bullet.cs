// Author: Dylan Nagel
// File Name: Bullet.cs
// Project Name: NagelD_PASS3
// Description: Creates an instance of a bullet, holding all important information for the gameplay

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NagelD_PASS3
{
    public class Bullet
    {
        //stores the bullet angle
        protected float angle;

        //stores the bullet location and rectangle
        protected Rectangle bulletRec;
        protected Vector2 bulletLoc;
        protected Rectangle offsetBulletRec;

        //stores the bullet speed
        protected Vector2 speed;

        //stores the maximum ricochet count
        protected int maxRicCount;

        //stores the origin location
        protected Vector2 bulletOrigin;

        //stores whether the play or enemy owns the bullet
        protected int ownership;

        //stores the visuals for the bullet
        protected Texture2D bulletImg;
        protected Color bulCol;

        //pre: a valid float angle, a valid vector2 spawn location within the board, a valid in ownership either 0 or 1. a valid max ricochet count
        //post:none
        //description: creates an instance of the bullet object
        public Bullet(float angle, Vector2 spawnLoc, int ownership, int maxRicCount)
        {
            //stores ownership
            this.ownership = ownership;

            //stores angle
            this.angle = angle;

            //stores the maximum ricochet cound
            this.maxRicCount = maxRicCount;

            //check if the player owns the bullet
            if (ownership == Game1.PLAY_BUL)
            {
                //set bullet colour to cornflower blue
                bulCol = Color.CornflowerBlue;
            }
            else
            {
                //set bullet colour to white
                bulCol = Color.White;
            }

            //sets the bullet rectangle
            bulletRec = new Rectangle((int)spawnLoc.X, (int)spawnLoc.Y, Game1.BULLET_SIZE, Game1.BULLET_SIZE);
            offsetBulletRec = new Rectangle((int)(spawnLoc.X - bulletRec.Width * 0.5), (int)(spawnLoc.Y - bulletRec.Height * 0.5), bulletRec.Width, bulletRec.Height);

            //sets the bullet location
            bulletLoc = new Vector2(bulletRec.X, bulletRec.Y);
        }

        //pre: none
        //post: none
        //dsecription: draws the bullet
        public virtual void Draw()
        {
            //draw the bullet
            Game1.spriteBatch.Draw(bulletImg, bulletRec, null, bulCol, angle, bulletOrigin, SpriteEffects.None, 0);
        }

        //pre: none
        //post: a boolean representing if the bullet is still alive
        //description: updates the bullet
        public bool UpdateBullet()
        {
            //moves the bullet
            MoveBullet();

            //gets the alive status of bullet
            return GetAliveStatus();
        }

        //pre: none
        //post: none
        //description: moves the bullet
        protected virtual void MoveBullet()
        {
            //moves the bullet location
            bulletLoc.X += speed.X;
            bulletLoc.Y += speed.Y;

            //moves the bullet rectangles
            bulletRec.X = (int)bulletLoc.X;
            bulletRec.Y = (int)bulletLoc.Y;
            offsetBulletRec.X = (int)(bulletRec.X - offsetBulletRec.Width * 0.5);
            offsetBulletRec.Y = (int)(bulletRec.Y - offsetBulletRec.Height * 0.5);

            //checks if the bullet is outside of the arena to the right or left
            if (bulletRec.X + bulletRec.Width * 0.5 > Game1.RIGHT_PLAY_X || bulletRec.X - bulletRec.Width * 0.5 < Game1.LEFT_PLAY_X)
            {
                //invert the x speed
                speed.X *= -1;

                //subtract 1 from max ric count
                maxRicCount--;
            }

            //checks if the bullet is outside of the arena to the top or bottom
            if (bulletRec.Y - bulletRec.Height * 0.5 < Game1.TOP_PLAY_Y || bulletRec.Y + bulletRec.Height * 0.5 > Game1.BOT_PLAY_Y)
            {
                //invert the y speed
                speed.Y *= -1;

                //subtract one from the max richochet count
                maxRicCount--;
            }
        }

        //pre: none
        //post: a boolean representing if the bullet is still alive
        //description: gets the alive status of bullet
        public bool GetAliveStatus()
        {
            //checks if the max ricochet count is negative
            if(maxRicCount < 0)
            {
                //returns false since bullet is dead
                return false;
            }

            //returns true since bullet is alive
            return true;
        }

        //pre: none
        //post: a rectangle representing the offset bullet rectangle
        //description: gets the bullet's offset rectangle
        public Rectangle GetOffsetRec()
        {
            //return offset bullet rectangle
            return offsetBulletRec;
        }

        //pre: none
        //post: a vector2 representing the bullet location
        //description: gets the bullet location
        public Vector2 GetLoc()
        {
            //returns the bullet location
            return bulletLoc;
        }

        //pre: a valid x and y speed multiplier (1 or -1)
        //post: none
        //description: changes the direction of the bullet
        public void ChangeDirection(int xSpeedMult, int ySpeedMult)
        {
            //multiply speed by the speed multiplier
            speed.X *= xSpeedMult;
            speed.Y *= ySpeedMult;

            //reduce max ric count by one
            maxRicCount--;
        }

        //pre: none
        //post: a interger representing the ownership
        //description: gets who owns the bullet
        public int GetBulType()
        {
            //returns the ownership
            return ownership;
        }

        //pre: none
        //post: a float representing the bullet angle
        //description: gets the bullet angle
        public float GetAngle()
        {
            //returns the bullet angle
            return angle;
        }

        //pre: none
        //post: a vector representing the bullet speed
        //description: gets the speed of the bullet
        public Vector2 GetSpeed()
        {
            //returns the speed of the bullet
            return speed;
        }

        //pre: none
        //post: a vector2 representing the explosion location for the bullet
        //description: gets the explosion location
        public Vector2 GetExploadLoc()
        {
            //returns the position for a explosion
            return new Vector2(bulletLoc.X - Game1.BULLET_EXPLOAD_SIZE * 0.5f, bulletLoc.Y - Game1.BULLET_EXPLOAD_SIZE * 0.5f);
        }
    }
}