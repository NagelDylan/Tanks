// Author: Dylan Nagel
// File Name: Tank.cs
// Project Name: NagelD_PASS3
// Description: Creates an instance of a tank, holding all important information for the gameplay

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameUtility;

namespace NagelD_PASS3
{
    public class Tank
    {
        //stores the tank rectangles and position
        protected Rectangle tankRec;
        protected Rectangle offsetRec;
        protected Rectangle shaftRec;
        protected Vector2 tankPos;

        //stores the angles
        protected float tankAng = 0;
        protected float shaftAng = 0;

        //stores the external angles
        protected float angleToPlayer;
        protected float modShaftAng;
        protected float modTankAng;

        //stores the tank images
        protected Texture2D tankImg;
        protected Texture2D shaftImg;

        //stores the id number
        private int id;

        //stores the start and end node
        protected Node start;
        protected Node end;

        //stores the booleans for movement
        protected bool isInNode = true;
        protected bool isRightAng = false;

        //stores the movement direction
        protected int moveDir;
        protected int tankAngleRotDir;

        //stores the shaft speed
        protected float shaftSpeed;

        //stores the minimum number of node that must be in path for the tank to move
        protected int minNode;

        //stores the shoot timer
        protected Timer shootTimer;

        //stores the total x and y movement
        protected Vector2 totalMovement = new Vector2(0, 0);

        //stores the path
        protected List<Node> path = new List<Node>();

        //stores the tile map
        protected Node[,] tileMap;

        //stores if the tank must see the player to move
        protected bool isMoveWithoutSight;

        //pre: a valid interger tank type, between 3 and 7, a valid vector2 spawn location withint the arena, a valid interger id number, a valid node 2d array tile map
        //post: none
        //description: creates an instance of a tank object
        public Tank(int tankType, Vector2 spawnLoc, int id, Node[,] tileMap)
        {
            //stores variable values
            this.tileMap = tileMap;
            this.id = id;

            //sets tank images
            SetTankImgs(tankType, spawnLoc);
        }

        //pre: a valid vector2 player position
        //post: none
        //description: moves the tank
        public virtual void Move(Vector2 playerPos)
        {
            //set the start and end node
            SetStartNode();
            end = FindEndNode(playerPos);

            //moves to path
            MoveToPath();
        }

        //pre: a valid GameTime gameTime, a valid vector2 player position
        //post: a Bullet object if tank is attacking, or null otherwise
        //description: runs the attack mechanism of the tank
        public virtual Bullet Attack(GameTime gameTime, Vector2 playerPos)
        {
            //updates the shoot timer
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

            //return null so no bullet is sent
            return null;
        }

        //pre: none
        //post: none
        //description: draws the tank
        public virtual void Draw()
        {
            //draws the tank
            Game1.spriteBatch.Draw(tankImg, tankRec, null, Color.White, tankAng, Game1.tankOrigin, SpriteEffects.None, 0);
            Game1.spriteBatch.Draw(shaftImg, shaftRec, null, Color.White, shaftAng, Game1.shaftOrigin, SpriteEffects.None, 0);
        }

        //pre: none
        //post: a float representing the tank angle
        //description: gets the angle of the tank
        public float GetTankAngle()
        {
            //returns the angle of the tank
            return tankAng;
        }

        //pre: none
        //post: a rectangle representing the offset rectangle
        //description: gets the offset rectangle of the tank
        public Rectangle GetOffsetRec()
        {
            //returns the offset rectangle of the tank
            return offsetRec;
        }

        //pre: none
        //post: a bullet that is specified to current shaft location
        //description: returns the designated bullet
        protected virtual Bullet ReturnBullet()
        {
            //return bullet
            return new SlowBullet(shaftAng, new Vector2((float)(shaftRec.X + Game1.SHAFT_LENGTH * Math.Cos(shaftAng)), (float)(shaftRec.Y + Game1.SHAFT_LENGTH * Math.Sin(shaftAng))), Game1.EN_BUL, Game1.NORM_BUL_MAX_RIC);
        }

        //pre: a valid interger value tank type between 3 and 7, a valid vector2 spawn location within the arena
        //post: none
        //description: sets the basic tank information
        protected void SetTankImgs(int tankType, Vector2 spawnLoc)
        {
            //sets the tank images
            tankImg = Game1.tankImgs[tankType - Game1.ARRAY_LOC_SUB];
            shaftImg = Game1.shaftImgs[tankType - Game1.ARRAY_LOC_SUB];

            //set the tank rectangles
            tankRec = new Rectangle(Game1.LEFT_PLAY_X + (int)spawnLoc.X, Game1.TOP_PLAY_Y + (int)spawnLoc.Y, Game1.TANK_LENGTH, Game1.TANK_LENGTH);
            shaftRec = new Rectangle(tankRec.X, tankRec.Y, (int)(shaftImg.Width * 0.4), (int)(shaftImg.Height * 0.4));
            offsetRec = new Rectangle((int)(tankRec.X - tankRec.Width * 0.5), (int)(tankRec.Y - tankRec.Height * 0.5), tankRec.Width, tankRec.Height);

            //sets the tank position
            tankPos.X = tankRec.X;
            tankPos.Y = tankRec.Y;
        }

        //pre: none
        //post: none
        //description: sets all tank features
        public void SetAllFeatures()
        {
            //set tank rectangles
            tankRec.X = (int)tankPos.X;
            tankRec.Y = (int)tankPos.Y;
            shaftRec.X = tankRec.X;
            shaftRec.Y = tankRec.Y;
            offsetRec.X = (int)(tankRec.X - tankRec.Width * 0.5);
            offsetRec.Y = (int)(tankRec.Y - tankRec.Height * 0.5);
        }

        //pre: a valid vector pushback
        //post: none
        //description: adds the pushback amount to the tank location
        public void PushTank(Vector2 pushBack)
        {
            //adds pushback to tank position
            tankPos += pushBack;

            //sets all features
            SetAllFeatures();
        }

        //pre: none
        //post: none
        //description: checks for GetCol()lision with the side of the screen
        public void SideScreenCol()
        {
            //checks if the tank rec is behind the left side of teh arena
            if (tankRec.X - tankRec.Width * 0.5 < Game1.LEFT_PLAY_X)
            {
                //sets tank positions
                tankRec.X = (int)(Game1.LEFT_PLAY_X + tankRec.Width * 0.5) + Game1.EXTRA_SPACE;
                tankPos.X = tankRec.X;
            }
            else if (tankRec.X + tankRec.Width * 0.5 > Game1.RIGHT_PLAY_X)
            {
                //sets tank positions
                tankRec.X = (int)(Game1.RIGHT_PLAY_X - tankRec.Width * 0.5) - Game1.EXTRA_SPACE;
                tankPos.X = tankRec.X;
            }

            //checks if the tank rec is above the top of the screen
            if (tankRec.Y - tankRec.Width * 0.5 < Game1.TOP_PLAY_Y)
            {
                //sets tank positions
                tankRec.Y = (int)(Game1.TOP_PLAY_Y + tankRec.Width * 0.5) + Game1.EXTRA_SPACE;
                tankPos.Y = tankRec.Y;
            }
            else if (tankRec.Y + tankRec.Width * 0.5 > Game1.BOT_PLAY_Y)
            {
                //sets tank positions
                tankRec.Y = (int)(Game1.BOT_PLAY_Y - tankRec.Width * 0.5) - Game1.EXTRA_SPACE;
                tankPos.Y = tankRec.Y;
            }

            //sets the rest of the tank rectangles
            shaftRec.X = tankRec.X;
            shaftRec.Y = tankRec.Y;
            offsetRec.X = (int)(tankRec.X - tankRec.Width * 0.5);
            offsetRec.Y = (int)(tankRec.Y - tankRec.Height * 0.5);
        }

        //pre: a valid node 2d array for the map, a valid interger for the target GetRow(), a valid interger for the target GetCol()umn
        //post: none
        //description: sets the h and f heuristic for each node
        public void SetHCost(Node[,] map, int targetRow, int targetCol)
        {
            //loops through the x length of the map
            for (int x = 0; x < map.GetLength(0); x++)
            {
                //loops through the y length of the map
                for (int i = 0; i < map.GetLength(1); i++)
                {
                    //sets the h and f cost for node at idnex x and i
                    map[x, i].SetHVal(GetHCost(x, i, targetRow, targetCol));
                    map[x, i].SetFVal(map[x, i].GetGVal() + map[x, i].GetHVal());
                }
            }
        }

        //pre: a valid interger for the tile GetRow(), a valid interger for the tile GetCol()umn, a valid interger for the targer GetRow(), a valid interger for the target GetCol()umn
        //post: a float representing the h cost
        //description: 
        private float GetHCost(int tileRow, int tileCol, int targetRow, int targetCol)
        {
            //returns the h cost
            return Math.Abs(targetRow - tileRow) * GameObject.HV_COST + Math.Abs(targetCol - tileCol) * GameObject.HV_COST;
        }

        //pre: a valid vector2 player position, a valid vector2 tank position
        //post: a boolean represnting if the tank can see the player
        //description: checks if the tank can see player
        protected bool CanSeePlayer(Vector2 playerPos, Vector2 tankPos)
        {
            //set max distances
            float minDist = 1000;
            float tankToWall = 1000;

            //loops through the x length of the tile map
            for (int i = 0; i < tileMap.GetLength(0); i++)
            {
                //loops through the y length of the tile map
                for (int x = 0; x < tileMap.GetLength(1); x++)
                {
                    //checks if the tile type is wall
                    if (tileMap[i, x].GetTileType() == Game1.WALL)
                    {
                        //checks if the line between the player and the tank intersects with the line on each side of the grid squares
                        if (Util.Intersects(playerPos, tankPos, new Vector2(tileMap[i, x].GetRec().X, tileMap[i, x].GetRec().Y), new Vector2(tileMap[i, x].GetRec().X, tileMap[i, x].GetRec().Y + tileMap[i, x].GetRec().Height)))
                        {
                            //sets tank to wall based on distance
                            tankToWall = (float)Math.Sqrt(Math.Pow(tankPos.X - tileMap[i, x].GetRec().X, 2) + Math.Pow(tankPos.Y - tileMap[i, x].GetRec().Y + 0.5 * tileMap[i, x].GetRec().Height, 2));
                        }
                        else if (Util.Intersects(playerPos, tankPos, new Vector2(tileMap[i, x].GetRec().X, tileMap[i, x].GetRec().Y), new Vector2(tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width, tileMap[i, x].GetRec().Y)))
                        {
                            //sets tank to wall based on distance
                            tankToWall = (float)Math.Sqrt(Math.Pow(tankPos.X - tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width * 0.5, 2) + Math.Pow(tankPos.Y - tileMap[i, x].GetRec().Y, 2));
                        }
                        else if (Util.Intersects(playerPos, tankPos, new Vector2(tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width, tileMap[i, x].GetRec().Y), new Vector2(tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width, tileMap[i, x].GetRec().Y + tileMap[i, x].GetRec().Height)))
                        {
                            //sets tank to wall based on distance
                            tankToWall = (float)Math.Sqrt(Math.Pow(tankPos.X - tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width * 0.5, 2) + Math.Pow(tankPos.Y - tileMap[i, x].GetRec().Y + tileMap[i, x].GetRec().Height * 0.5, 2));
                        }
                        else if (Util.Intersects(playerPos, tankPos, new Vector2(tileMap[i, x].GetRec().X, tileMap[i, x].GetRec().Y + tileMap[i, x].GetRec().Height), new Vector2(tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width, tileMap[i, x].GetRec().Y + tileMap[i, x].GetRec().Height)))
                        {
                            //sets tank to wall based on distance
                            tankToWall = (float)Math.Sqrt(Math.Pow(tankPos.X - tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width * 0.5, 2) + Math.Pow(tankPos.Y - tileMap[i, x].GetRec().Y + tileMap[i, x].GetRec().Height, 2));
                        }

                        //checks if the tank to wall is less than the minimum distance
                        if (tankToWall < minDist)
                        {
                            //sets the minimum distance to tank to wall
                            minDist = tankToWall;
                        }
                    }
                }
            }

            //checks if the min distance is less than the distance from tank to player
            if (minDist < Math.Sqrt(Math.Pow(playerPos.X - tankPos.X, 2) + Math.Pow(playerPos.Y - tankPos.Y, 2)))
            {
                //returns false to represent cant see
                return false;
            }

            //returns true to represent can see
            return true;
        }

        //pre: none
        //post: none
        //description: finds a path
        protected List<Node> FindPath()
        {
            //creates open and closed node lists
            List<Node> open = new List<Node>();
            List<Node> closed = new List<Node>();

            //loops through the tile map x length
            for (int i = 0; i < tileMap.GetLength(0); i++)
            {
                //loops through the tile map y length
                for (int x = 0; x < tileMap.GetLength(1); x++)
                {
                    //sets tile map parent at index i, x to null
                    tileMap[i, x].SetParent(null);
                }
            }

            //resets the g and h costs
            start.SetGVal(0);
            SetHCost(tileMap, end.GetRow(), end.GetCol());

            //creates a list of nodes to store the results
            List<Node> result = new List<Node>();

            //creates variabels to store minimum values
            float minF;
            int minIndex = 0;

            //creates a variable to store the current node
            Node curNode;

            //adds start to the open list
            open.Add(start);

            //attempts to find path until break
            while (true)
            {
                //sets min f value to 10000
                minF = 10000f;

                //loops through all nodes in open
                for (int i = 0; i < open.Count; i++)
                {
                    //checks if the f cost is less than teh min cose
                    if (open[i].GetFVal() < minF)
                    {
                        //sets the min valeues
                        minF = open[i].GetFVal();
                        minIndex = i;
                    }
                }

                //sets the curnode value
                curNode = open[minIndex];

                //removes the minimum node from open list and adds to closed list
                open.RemoveAt(minIndex);
                closed.Add(curNode);

                //checks if the curnode is equal to the end node
                if (curNode.GetId() == end.GetId())
                {
                    //break out of while loop
                    break;
                }

                //create a node to store the comp nodes
                Node compNode;

                //creates a list list of nodes for adjacencies
                List<Node> adjacent = curNode.GetAdjacent();

                //loop through all of the adjacencies of the current node
                for (int i = 0; i < adjacent.Count; i++)
                {
                    //set the comp node to the adjacent value at index i
                    compNode = adjacent[i];

                    //check if the tile type is not wall or water, and is not in the closed list
                    if (compNode.GetTileType() != Game1.WALL && compNode.GetTileType() != Game1.WATER && ContainsNode(closed, compNode) == GameObject.NOT_FOUND)
                    {
                        //creates variable for the new g cost
                        float newG = GetGCost(curNode);

                        //checks if the comp node is not in the open list
                        if (ContainsNode(open, compNode) == GameObject.NOT_FOUND)
                        {
                            //set the comp node parent to current node
                            compNode.SetParent(curNode);

                            //resets the g and f costs
                            compNode.SetGVal(newG);
                            compNode.SetFVal(compNode.GetGVal() + compNode.GetHVal());

                            //adds the comp node to the open list
                            open.Add(compNode);
                        }
                        else
                        {
                            //checks if the new g value is less than the comp node g value
                            if (newG < compNode.GetGVal())
                            {
                                //sets the comp node parent to cur node
                                compNode.SetParent(curNode);

                                //sets the comp node g and f values
                                compNode.SetGVal(newG);
                                compNode.SetFVal(compNode.GetGVal() + compNode.GetHVal());
                            }
                        }
                    }
                }

                //check if there is nothing left in the open list
                if (open.Count == 0)
                {
                    //break out of the while loop
                    break;
                }
            }

            //checks i the end node is not in the closed list
            if (ContainsNode(closed, end) != GameObject.NOT_FOUND)
            {
                //creates a node to store the end node
                Node pathNode = end;

                //loops through the nodes parents until the parent is null
                while (pathNode != null)
                {
                    //inserst the pathnode at the first index of result list
                    result.Insert(0, pathNode);

                    //sets the path node to its parent
                    pathNode = pathNode.GetParent();
                }
            }

            //returns the result
            return result;
        }

        //pre: a valid node for the parent nod
        //post: a float representing the g cost
        //description: 
        private float GetGCost(Node parentNode)
        {
            //returns the g cost
            return parentNode.GetGVal() + GameObject.HV_COST;
        }

        //pre: a valid list of nodes to store the nodes, a valid node represneting the chec k node
        //post: an interger representing the location of where the node is contained
        //description: checks if a node is in a specified list
        private int ContainsNode(List<Node> nodeList, Node checkNode)
        {
            //loop through the node list
            for (int i = 0; i < nodeList.Count; i++)
            {
                //check if the id of node in the list at index i is teh same as the id of the check node
                if (nodeList[i].GetId() == checkNode.GetId())
                {
                    //returns index i
                    return i;
                }
            }

            //returns not found interger
            return GameObject.NOT_FOUND;
        }

        //pre: none
        //post: returns a vector2 to get the explosion location
        //description: gets the explosion location
        public Vector2 GetExploadLoc()
        {
            //returns the explosion location
            return new Vector2(tankPos.X - Game1.EXPLOAD_SIZE * 0.5f, tankPos.Y - Game1.EXPLOAD_SIZE * 0.5f);
        }

        //pre: a valid vector2 represnting the player position
        //post: a boolean representing if the tank is going to shoot
        //description: runs the actions of a tank if they can see the player
        protected bool SeePlayerAttack(Vector2 playerPos)
        {
            //set angles
            angleToPlayer = (float)(Math.PI + Math.Atan2(tankRec.Y - playerPos.Y, tankRec.X - playerPos.X));
            modShaftAng = (float)(shaftAng % (Math.PI * 2));

            //check if the mod shaft angle is negative
            if (Math.Sign(modShaftAng) == -1)
            {
                //add a full cycle to the mod shaft angle
                modShaftAng += (float)(Math.PI * 2);
            }

            //check if the the angel is within the bounds
            if (Math.Abs(angleToPlayer - modShaftAng) < Game1.ANGLE_BOUNDS)
            {
                //checks if the shoot timer is finsihed
                if (shootTimer.IsFinished())
                {
                    //resets the shoot timer
                    shootTimer.ResetTimer(true);

                    //return true to shoot a bullet
                    return true;
                }
            }
            else if (angleToPlayer >= Math.PI)
            {
                //check which direction the shaft should turn
                if (modShaftAng > angleToPlayer || modShaftAng < angleToPlayer - Math.PI)
                {
                    //turn the shaft in the counter clockwise direction
                    shaftAng -= shaftSpeed;
                }
                else
                {
                    //turn the shaft in the clockwise direction
                    shaftAng += shaftSpeed;
                }
            }
            else
            {
                //check which direction the shaft should turn
                if (modShaftAng > Math.PI + angleToPlayer || modShaftAng < angleToPlayer)
                {
                    //turn the shaft in the clockwise direction
                    shaftAng += shaftSpeed;
                }
                else
                {
                    //turn the shaft in the counter clockwise direction
                    shaftAng -= shaftSpeed;
                }
            }

            //return false to not shoot a bullet
            return false;
        }

        //pre: a valid float representing the turn speed
        //post: none
        //description: aims the shaft towards the front of the tank
        protected void AimToFront()
        {
            //sets the mod angles
            modShaftAng = (float)(shaftAng % (Math.PI * 2));
            modTankAng = (float)(tankAng % (Math.PI * 2));

            //checks if the mod shaft angle is negative
            if (Math.Sign(modShaftAng) == -1)
            {
                //add one cycle to the mod shaft angle
                modShaftAng += (float)(Math.PI * 2);
            }

            //checks if the mod tank angle is negative
            if (Math.Sign(modTankAng) == -1)
            {
                //add one cycle to the mod tank angle
                modTankAng += (float)(Math.PI * 2);
            }

            //check if the shaft is within the range of the tank angle
            if (Math.Abs(modShaftAng - modTankAng) > Game1.ANGLE_BOUNDS)
            {
                //check if the mod tank angle is greater than or equal to pi
                if (modTankAng >= Math.PI)
                {
                    //check which direction to move the shaft
                    if (modShaftAng > modTankAng || modShaftAng < modTankAng - Math.PI)
                    {
                        //move the shaft in the counter clockwise direction
                        shaftAng -= shaftSpeed;
                    }
                    else
                    {
                        //move the shaft in the clockwise direction
                        shaftAng += shaftSpeed;
                    }
                }
                else
                {
                    //check which direction to move the shaft
                    if (modShaftAng > Math.PI + modTankAng || modShaftAng < modTankAng)
                    {
                        //move the shaft in the clockwise direction
                        shaftAng += shaftSpeed;
                    }
                    else
                    {
                        //move the shaft in the counter clockwise direction
                        shaftAng -= shaftSpeed;
                    }
                }
            }
        }

        //pre: none
        //post: none
        //description: move based on location and pathway
        protected void MoveToPath()
        {
            //check if the tank is in the node
            if (isInNode)
            {
                //create a path
                path = FindPath();

                //check if the number of nodes in the path is more than the minimum number of nodes
                if (path.Count > minNode)
                {
                    //set the moded tank angle
                    modTankAng = (float)(tankAng % (Math.PI * 2));

                    //check if the tank angle is negative
                    if (Math.Sign(modTankAng) == -1)
                    {
                        //add one cycle to the tank angle
                        modTankAng += (float)(Math.PI * 2);
                    }

                    //set is in node to false
                    isInNode = false;

                    //check which direction the next location on the path is in regards to the tank
                    if (path[1].GetRow() < start.GetRow())
                    {
                        //set move direction to up
                        moveDir = Game1.UP;

                        //check which direction the tank should rotate
                        if (modTankAng > Math.PI * 1.5 || modTankAng < Math.PI * 0.5)
                        {
                            //set the rotation direction to counter clockwise
                            tankAngleRotDir = -1;
                        }
                        else
                        {
                            //set the rotation direction to clockwise
                            tankAngleRotDir = 1;
                        }
                    }
                    else if (path[1].GetRow() > start.GetRow())
                    {
                        //set move direction to down
                        moveDir = Game1.DOWN;

                        //check which direction the tank should rotate
                        if (modTankAng < Math.PI * 0.5 || modTankAng > Math.PI * 1.5)
                        {
                            //set the rotation direction to clockwise
                            tankAngleRotDir = 1;
                        }
                        else
                        {
                            //set the rotation direction to counter clockwise
                            tankAngleRotDir = -1;
                        }
                    }
                    else if (path[1].GetCol() < start.GetCol())
                    {
                        //set move direction to left
                        moveDir = Game1.LEFT;

                        //check which direction the tank should rotate
                        if (modTankAng > Math.PI && modTankAng < Math.PI * 2)
                        {
                            //set the rotation direction to counter clockwise
                            tankAngleRotDir = -1;
                        }
                        else
                        {
                            //set the rotation direction to clockwise
                            tankAngleRotDir = 1;
                        }
                    }
                    else
                    {
                        //set move direction to right
                        moveDir = Game1.RIGHT;

                        //check which direction the tank should rotate
                        if (modTankAng > Math.PI)
                        {
                            //set the rotation direction to clockwise
                            tankAngleRotDir = 1;
                        }
                        else
                        {
                            //set the rotation direction to counter clockwise
                            tankAngleRotDir = -1;
                        }
                    }

                    //check if the tank angle is within a small range of the angle it should be in
                    if (Math.Abs(modTankAng - moveDir * (float)Math.PI * 0.5f) < Game1.ANGLE_BOUNDS)
                    {
                        //set the is right angle variable to true
                        isRightAng = true;
                    }
                }
            }
            else
            {
                //check if the angle is a right angle
                if (!isRightAng)
                {
                    //change the tank angles
                    tankAng += Game1.TANK_ROT_SPEED * tankAngleRotDir;
                    modTankAng = (float)(tankAng % (Math.PI * 2));

                    //check if the mod tank angle is negative
                    if (Math.Sign(modTankAng) == -1)
                    {
                        //add one cycle to the mod tank angle
                        modTankAng += (float)(Math.PI * 2);
                    }

                    //check if the mod tank angle is within a small range of the correct angle
                    if (Math.Abs(moveDir * Math.PI * 0.5 - modTankAng) < Game1.ANGLE_BOUNDS)
                    {
                        //set angle to specified angle
                        modTankAng = moveDir * (float)Math.PI * 0.5f;

                        //set is right angle to true
                        isRightAng = true;
                    }
                }
                else
                {
                    //change the tank position
                    tankPos += Game1.MOVEMENT_SPEED[moveDir];
                    SetAllFeatures();

                    //add to the total movement
                    totalMovement += Game1.MOVEMENT_SPEED[moveDir];

                    //check if the total mvoement in the x or y direction is more than one grid length
                    if (Math.Abs(totalMovement.X) >= Game1.GRID_SIDE_LEN || Math.Abs(totalMovement.Y) >= Game1.GRID_SIDE_LEN)
                    {
                        //set is right angle variable to false
                        isRightAng = false;

                        //set is in node variable to true
                        isInNode = true;

                        //reset the total movement
                        totalMovement.X = 0;
                        totalMovement.Y = 0;
                    }
                }
            }
        }

        //pre: none
        //post: none
        //description: sets the start node for pathfinding
        protected void SetStartNode()
        {
            //creates and sets x and y loc variables
            int xLoc = (int)Math.Floor((tankRec.X - Game1.LEFT_PLAY_X) / (double)GameObject.WALL_LENGTH);
            int yLoc = (int)Math.Floor((tankRec.Y - Game1.TOP_PLAY_Y) / (double)GameObject.WALL_LENGTH);

            //checks if node is within confinements of x length of grid
            if (xLoc < 0)
            {
                //sets x location to 0
                xLoc = 0;
            }
            else if (xLoc >= GameObject.NUM_COL)
            {
                //sets x location to the last GetCol()umn x location
                xLoc = GameObject.NUM_COL - 1;
            }

            //checks if node is within confinements of x length of grid
            if (yLoc < 0)
            {
                //sets y location to 0
                yLoc = 0;
            }
            else if (yLoc >= GameObject.NUM_ROWS)
            {
                //sets y location to the last GetRow() y location
                yLoc = GameObject.NUM_ROWS - 1;
            }

            //set the start node location
            start = tileMap[xLoc, yLoc];
        }

        //pre: none
        //post: none
        //description: returns the end node for path finding
        protected virtual Node FindEndNode(Vector2 playerPos)
        {
            //checks if the red tank cant see player
            if (!isMoveWithoutSight && !CanSeePlayer(playerPos, tankPos))
            {
                //checks if end is not null
                if (end != null)
                {
                    //returns the current end node
                    return end;
                }
            }

            //stores values for information when red tank can see player
            Node openBestNode = null;
            float openMaxDist = 0;
            float openCurDist;

            //stores whetehr a hidden node was found or not
            bool isHidNodeFound = false;

            //stores values for information when red tank can't see player
            Node hidBestNode = null;
            float hidMaxDist = 0;
            float hidCurDist;

            //loop through the x length of the tile map
            for (int i = 0; i < tileMap.GetLength(0); i++)
            {
                //loop through the y length of the tile map
                for (int x = 0; x < tileMap.GetLength(1); x++)
                {
                    //checks if the tile map is not a wall
                    if (tileMap[i, x].GetTileType() != Game1.WALL)
                    {
                        //checks if the tank can not see the player at the centre of node at index i, x
                        if (!CanSeePlayer(playerPos, new Vector2(tileMap[i, x].GetRec().X + tileMap[i, x].GetRec().Width * 0.5f, tileMap[i, x].GetRec().Y + tileMap[i, x].GetRec().Height * 0.5f)))
                        {
                            //stores the distance to block at index i, x
                            hidCurDist = (float)Math.Sqrt(Math.Pow(playerPos.X - tileMap[i, x].GetRec().X, 2) + Math.Pow(playerPos.Y - tileMap[i, x].GetRec().Y, 2));

                            //checks if the current hidden distance is greater than the hidden maximum distance
                            if (hidCurDist > hidMaxDist)
                            {
                                //sets the hidden maximum distance to the current distance
                                hidMaxDist = hidCurDist;

                                //sets the node for hidden best node
                                hidBestNode = tileMap[i, x];

                                //sets is hidden found to true
                                isHidNodeFound = true;
                            }
                        }

                        //checks if a hidden node was not found
                        if (!isHidNodeFound)
                        {
                            //stores the distance to block at index i, x
                            openCurDist = (float)Math.Sqrt(Math.Pow(playerPos.X - tileMap[i, x].GetRec().X, 2) + Math.Pow(playerPos.Y - tileMap[i, x].GetRec().Y, 2));

                            //checks if the current open distance is greater than the open maximum distance
                            if (openCurDist > openMaxDist)
                            {
                                //sets the open maximum distance to the current distance
                                openMaxDist = openCurDist;

                                //sets the node for open best node
                                openBestNode = tileMap[i, x];
                            }
                        }
                    }
                }
            }

            //checks if the hidden node was found
            if (isHidNodeFound)
            {
                //returns the best hidden node
                return hidBestNode;
            }

            //returns the best open node
            return openBestNode;
        }

        //pre: none
        //post: an interger representing the ID number
        //description: gets the id number
        public int GetId()
        {
            //returns the id number
            return id;
        }
    }
}