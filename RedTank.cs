// Author: Dylan Nagel
// File Name: RedTank.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Creates an instance of a red tank, taking in and sorting the red tank data

using System;
using Microsoft.Xna.Framework;
using GameUtility;

namespace NagelD_PASS3
{
    public class RedTank:Tank
    {
        //pre: a valid interger tank type, between 3 and 7, a valid vector2 spawn location withint the arena, a valid interger id number, a valid node 2d array tile map
        //post: none
        //description: creates an instance of a red tank object
        public RedTank(int tankType, Vector2 spawnLoc, int id, Node[,] tileMap) : base(tankType, spawnLoc, id, tileMap)
        {
            //sets up the shoot timer
            shootTimer = new Timer(Game1.RED_TANK_TIME, true);

            //sets up the shaft speed
            shaftSpeed = Game1.RED_SHAFT_TURN_SPEED;

            //sets up minimum number of nodes
            minNode = GameObject.RED_TANK_MIN_NODE;

            //sets up if the tank can move without sight of player
            isMoveWithoutSight = false;
        }

        //pre: a valid GameTime gameTime, a valid vector2 player position
        //post: none
        //description: updates the attacking portion of the red tank tank
        public override Bullet Attack(GameTime gameTime, Vector2 playerPos)
        {
            shootTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //check if the tank can see the player
            if (CanSeePlayer(playerPos, tankPos))
            {
                //check if the tank is going to shoot a bullet
                if (SeePlayerAttack(playerPos))
                {
                    //return a new bullet
                    return ReturnBullet();
                }
            }
            else
            {
                //aim to the front of the tank
                AimToFront();
            }

            //returns null since there is no bullet
            return null;
        }

        //pre: none
        //post: a bullet for a red tank
        //description: returns a bullet for a red tank
        protected override Bullet ReturnBullet()
        {
            //retrurns a fast bullet
            return new FastBullet(shaftAng, new Vector2((float)(shaftRec.X + Game1.SHAFT_LENGTH * Math.Cos(shaftAng)), (float)(shaftRec.Y + Game1.SHAFT_LENGTH * Math.Sin(shaftAng))), Game1.EN_BUL, Game1.RED_BUL_MAX_RIC);
        }
    }
}
