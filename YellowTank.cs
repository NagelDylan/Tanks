// Author: Dylan Nagel
// File Name: YellowTank.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Creates an instance of a yellow tank, holding all important information for the gameplay

using Microsoft.Xna.Framework;
using GameUtility;

namespace NagelD_PASS3
{
    public class YellowTank:Tank
    {
        //creates a variable to store the turn timer
        Timer turnTimer;

        //creates a variable to store the turning direction
        int turnDir = 1;

        //pre: a valid interger tank type, between 3 and 7, a valid vector2 spawn location withint the arena, a valid interger id number, a valid node 2d array tile map
        //post: none
        //description: creates an instance of a yellow tank object
        public YellowTank(int tankType, Vector2 spawnLoc, int id, Node[,] tileMap) : base (tankType, spawnLoc, id, tileMap)
        {
            //sets up shoot and turn timers
            shootTimer = new Timer(Game1.rng.Next(Game1.YEL_MIN_SHOOT_TIME, Game1.YEL_MAX_SHOOT_TIME), true);
            turnTimer = new Timer(Game1.rng.Next(Game1.YEL_TURN_MIN_TIME, Game1.YEL_TURN_MAX_TIME), true);
        }

        //pre: a valid GameTime gameTime, a valid vector2 player position
        public override Bullet Attack(GameTime gameTime, Vector2 playerPos)
        {
            //updates shoot timer and turn timer
            shootTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            turnTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //checks if turn timer is finished
            if (turnTimer.IsFinished())
            {
                //sets new value to turn timer
                turnTimer = new Timer(Game1.rng.Next(Game1.YEL_TURN_MIN_TIME, Game1.YEL_TURN_MAX_TIME), true);

                //changes turn direction
                turnDir *= -1;
            }
            else
            {
                //turns the shaft
                shaftAng += Game1.SHAFT_TURN_SPEED * turnDir;
            }

            //checks if the shoot timer is finsished
            if (shootTimer.IsFinished())
            {
                //sets new value to shoot timer
                shootTimer = new Timer(Game1.rng.Next(Game1.YEL_MIN_SHOOT_TIME, Game1.YEL_MAX_SHOOT_TIME), true);

                //returns a bullet
                return ReturnBullet();
            }

            //returns null since no bullet
            return null;
        }

        //pre: a valid vector2 playerposition
        //post: none
        //description: controls the movement of the yellow tank (no movement)
        public override void Move(Vector2 playerPos)
        {
            
        }
    }
}
