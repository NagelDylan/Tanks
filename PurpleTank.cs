// Author: Dylan Nagel
// File Name: PurpleTank.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Creates an instance of a purple tank, taking in and sorting the purple tank data

using System;
using Microsoft.Xna.Framework;
using GameUtility;

namespace NagelD_PASS3
{
    public class PurpleTank:Tank
    {
        //pre: a valid interger tank type, between 3 and 7, a valid vector2 spawn location withint the arena, a valid interger id number, a valid node 2d array tile map
        //post: none
        //description: creates an instance of a purple tank object
        public PurpleTank(int tankType, Vector2 spawnLoc, int id, Node[,] tileMap): base(tankType, spawnLoc, id, tileMap)
        {
            //sets up the shoot timer variable
            shootTimer = new Timer(Game1.PURP_TANK_TIME, true);

            //sets up the shaft speed
            shaftSpeed = Game1.PURP_SHAFT_TURN_SPEED;

            //sets up minimum number of nodes
            minNode = GameObject.PURP_TANK_MIN_NODE;

            //sets up if the tank can move without sight of player
            isMoveWithoutSight = true;
        }

        //pre: a valid vector2 player position
        //post: a node representing the end node
        //description: finds the end node for the purple tank
        protected override Node FindEndNode(Vector2 playerPos)
        {
            //returns the node that the player is on
            return tileMap[(int)Math.Floor((playerPos.X - Game1.LEFT_PLAY_X) / (double)GameObject.WALL_LENGTH), (int)Math.Floor((playerPos.Y - Game1.TOP_PLAY_Y) / (double)GameObject.WALL_LENGTH)];
        }
    }
}