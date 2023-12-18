// Author: Dylan Nagel
// File Name: BlueTank.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Creates an instance of a blue tank, taking in and sorting the blue tank data

using Microsoft.Xna.Framework;
using GameUtility;

namespace NagelD_PASS3
{
    public class BlueTank:Tank
    {
        //pre: a valid interger tank type, between 3 and 7, a valid vector2 spawn location withint the arena, a valid interger id number, a valid node 2d array tile map
        //post: none
        //description: creates an instance of a blue tank object
        public BlueTank(int tankType, Vector2 spawnLoc, int id, Node[,] tileMap):base(tankType, spawnLoc, id, tileMap)
        {
            //sets up the shoot timer
            shootTimer = new Timer(Game1.BLUE_TANK_TIME, true);

            //sets up the shaft speed
            shaftSpeed = Game1.BLUE_SHAFT_TURN_SPEED;

            //sets up minimum number of nodes
            minNode = GameObject.BLUE_TANK_MIN_NODE;

            //sets up if the tank can move without sight of player
            isMoveWithoutSight = true;
        }
    }
}
