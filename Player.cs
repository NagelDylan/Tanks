// Author: Dylan Nagel
// File Name: Player.cs
// Project Name: NagelD_PASS3
// Description: Creates an instance of a player, controlling the player movement and data

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NagelD_PASS3
{
    public class Player
    {
        //stores the rectangles
        private Rectangle tankRec;
        private Rectangle shaftRec;
        private Rectangle offsetRec;

        //stores the tank position
        private Vector2 tankPos;

        //stores the tank images
        private Texture2D tankImg;
        private Texture2D shaftImg;

        //stores 
        private float tankAng;
        private float shaftAng;

        //pre: none
        //post: none
        //description: creates an instance of the player class
        public Player()
        {
            //set tank images
            tankImg = Game1.tankImgs[Game1.PLAYER_TANK - Game1.ARRAY_LOC_SUB];
            shaftImg = Game1.shaftImgs[Game1.PLAYER_TANK - Game1.ARRAY_LOC_SUB];

            //set tank rectangles
            tankRec = new Rectangle(0, 0, Game1.TANK_LENGTH, Game1.TANK_LENGTH);
            shaftRec = new Rectangle(tankRec.X, tankRec.Y, (int)(shaftImg.Width * 0.4), (int)(shaftImg.Height * 0.4));
            offsetRec = new Rectangle((int)(tankRec.X - tankRec.Width * 0.5), (int)(tankRec.Y - tankRec.Height * 0.5), tankRec.Width, tankRec.Height);

            //set tank position
            tankPos.X = tankRec.X;
            tankPos.Y = tankRec.Y;
        }

        //pre: none
        //post: none
        //description: moves the player
        public void Move()
        {
            //checks if the player is clicking w
            if (Game1.kb.IsKeyDown(Keys.W))
            {
                //move the player
                tankPos.X += (float)(Game1.PLAY_TANK_SPEED * Math.Cos(tankAng));
                tankPos.Y += (float)(Game1.PLAY_TANK_SPEED * Math.Sin(tankAng));
            }

            //checks if the player is clicking s
            if (Game1.kb.IsKeyDown(Keys.S))
            {
                //move the player
                tankPos.X -= (float)(Game1.PLAY_TANK_SPEED * Math.Cos(tankAng) * 0.5);
                tankPos.Y -= (float)(Game1.PLAY_TANK_SPEED * Math.Sin(tankAng) * 0.5);
            }

            //checks if the player is outside of the arena in x direction
            if (tankRec.X - tankRec.Width * 0.5 < Game1.LEFT_PLAY_X)
            {
                //sets location to left side of screen
                tankPos.X = (int)(Game1.LEFT_PLAY_X + tankRec.Width * 0.5) + Game1.EXTRA_SPACE;
            }
            else if (tankRec.X + tankRec.Width * 0.5 > Game1.RIGHT_PLAY_X)
            {
                //sets location to right side of screen
                tankPos.X = (int)(Game1.RIGHT_PLAY_X - tankRec.Width * 0.5) - Game1.EXTRA_SPACE;
            }

            //checks if the player is outside of the arena in y direction
            if (tankRec.Y - tankRec.Width * 0.5 < Game1.TOP_PLAY_Y)
            {
                //sets location to top of screen
                tankPos.Y = (int)(Game1.TOP_PLAY_Y + tankRec.Width * 0.5) + Game1.EXTRA_SPACE;
            }
            else if (tankRec.Y + tankRec.Width * 0.5 > Game1.BOT_PLAY_Y)
            {
                //sets location to bottom of screen
                tankPos.Y = (int)(Game1.BOT_PLAY_Y - tankRec.Width * 0.5) - Game1.EXTRA_SPACE;
            }

            //checks if the player is clicking A
            if (Game1.kb.IsKeyDown(Keys.A))
            {
                //turn the tank counter clockwise
                tankAng -= Game1.TANK_ROT_SPEED;
            }

            //checks if the player is clicking D
            if (Game1.kb.IsKeyDown(Keys.D))
            {
                //turns the tank clockwise
                tankAng += Game1.TANK_ROT_SPEED;
            }

            //sets the shaft angle
            shaftAng = (float)(Math.Atan2(tankRec.X - Game1.mouse.X, Game1.mouse.Y - tankRec.Y) + Math.PI * 0.5);

            //sets all features based on movement
            SetAllFeatures();
        }

        //pre: none
        //post: a bullet representing the player bullet that is shot
        //description: updates the attack aspect of player
        public Bullet Attack()
        {
            //checks if the user is clicking the left button
            if(Game1.mouse.LeftButton == ButtonState.Pressed && Game1.prevMouse.LeftButton != ButtonState.Pressed)
            {
                //checks if the bullet is within the arena
                if(shaftRec.X + Game1.SHAFT_LENGTH * Math.Cos(shaftAng) - Game1.BULLET_SIZE * 0.5 > Game1.LEFT_PLAY_X && shaftRec.X + Game1.SHAFT_LENGTH * Math.Cos(shaftAng) + Game1.BULLET_SIZE * 0.5 < Game1.RIGHT_PLAY_X && shaftRec.Y + Game1.SHAFT_LENGTH * Math.Sin(shaftAng) - Game1.BULLET_SIZE * 0.5 > Game1.TOP_PLAY_Y && shaftRec.Y + Game1.SHAFT_LENGTH * Math.Sin(shaftAng) + Game1.BULLET_SIZE * 0.5 < Game1.BOT_PLAY_Y)
                {
                    //returns a slow bullet
                    return new SlowBullet(shaftAng, new Vector2((float)(shaftRec.X + Game1.SHAFT_LENGTH * Math.Cos(shaftAng)), (float)(shaftRec.Y + Game1.SHAFT_LENGTH * Math.Sin(shaftAng))), Game1.PLAY_BUL, Game1.NORM_BUL_MAX_RIC);
                }
            }

            //returns null since no bullet is shot
            return null;
        }

        //pre: a valid vector2 pushback
        //post: none
        //description: pushes the player
        public void PushPlayer(Vector2 pushBack)
        {
            //adds the pushback to the tank position
            tankPos += pushBack;

            //sets all features
            SetAllFeatures();
        }

        //pre: none
        //post: none
        //description: draws the tank
        public void Draw()
        {
            //draws tank
            Game1.spriteBatch.Draw(tankImg, tankRec, null, Color.White, tankAng, Game1.tankOrigin, SpriteEffects.None, 0);
            Game1.spriteBatch.Draw(shaftImg, shaftRec, null, Color.White, shaftAng, Game1.shaftOrigin, SpriteEffects.None, 0);
        }

        //pre: none
        //post: a float representing the tanks angle
        //desctiption: gets the tanks angle
        public float GetTankAngle()
        {
            //returns tank angle
            return tankAng;
        }

        //pre: none
        //post: a rectangle representing the tanks offset rectangle
        //desctiption: gets the tanks offset rectangle
        public Rectangle GetOffsetRec()
        {
            //returns the offset rectangle
            return offsetRec;
        }

        //pre: none
        //post: a vector2 representing the tanks position
        //desctiption: gets the tanks vector2 rectangle
        public Vector2 GetPos()
        {
            //returns the tank position
            return tankPos;
        }

        //pre: a valid vector2 player position within the arena
        //post: none
        //desctiption: sets the player position
        public void SetPos(Vector2 playerPos)
        {
            //set tank pos to player position
            tankPos = playerPos;

            //sets all the features
            SetAllFeatures();

            //adjusts the shaft angle
            shaftAng = (float)(Math.Atan2(tankRec.X - Game1.mouse.X, Game1.mouse.Y - tankRec.Y) + Math.PI * 0.5);
        }

        //pre: none
        //post: none
        //description: sets all tank position and rectangles features
        private void SetAllFeatures()
        {
            //sets tank rectangles
            tankRec.X = (int)tankPos.X;
            tankRec.Y = (int)tankPos.Y;
            offsetRec.X = (int)(tankRec.X - tankRec.Width * 0.5);
            offsetRec.Y = (int)(tankRec.Y - tankRec.Height * 0.5);

            //sets shaft rectangle
            shaftRec.X = tankRec.X;
            shaftRec.Y = tankRec.Y;
        }

        //pre: none
        //post: none
        //description: resets the tank angle
        public void Reset()
        {
            //sets the tank angle to zero
            tankAng = 0;
        }
    }
}
