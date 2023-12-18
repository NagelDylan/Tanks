// Author: Dylan Nagel
// File Name: Node.cs
// Project Name: NagelD_PASS3
// Description: Creates an instance of a node, holding all important information about the node

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NagelD_PASS3
{
    public class Node
    {
        //stores heuristic values
        private float f = 0f;
        private float g = 0f;
        private float h = 0f;

        //stores the parent node
        private Node parent = null;

        //stores the row and column
        private int row;
        private int col;

        //stores the id number
        private int id;

        //stores the tile type
        private int tileType;

        //stores the adjacent nodes
        private List<Node> adjacent = new List<Node>();

        //stores the rectangle
        private Rectangle rec;

        //pre: a valid interger column within the arena limits, a valid interger row within the arena limits, a valid interger tiletype
        //post: none
        //description: creates an instance of the node object
        public Node(int col, int row, int tileType)
        {
            //store row and column
            this.row = row;
            this.col = col;

            //store the tile type
            this.tileType = tileType;

            //set the id number
            id = row * GameObject.NUM_COL + col;

            //set the rectangle
            rec = new Rectangle(Game1.LEFT_PLAY_X +  col * GameObject.WALL_LENGTH, Game1.TOP_PLAY_Y + row * GameObject.WALL_LENGTH, GameObject.WALL_LENGTH, GameObject.WALL_LENGTH);
        }

        //pre: a valid node 2d array map
        //post: none
        //description: sets the adjacencies
        public void SetAdjacencies(Node[,] map)
        {
            //checks if the node to the left is within arena
            if (row - 1 >= 0)
            {
                //adds the node to the left to adjacent list
                adjacent.Add(map[col, row-1]);
            }

            //checks if the column above is within arena
            if (col - 1 >= 0)
            {
                //adds the node above to adjacent list
                adjacent.Add(map[col - 1, row]);
            }

            //checks if the node to the right is within arena
            if (row + 1 < GameObject.NUM_ROWS)
            {
                //adds the node to the right to adjacent list
                adjacent.Add(map[col, row + 1]);
            }

            //checks if the node below is within arena
            if (col + 1 < GameObject.NUM_COL)
            {
                //adds the node below to adjacent list
                adjacent.Add(map[col + 1, row]);
            }
        }

        //pre: a valid interger tiletype
        //post: none
        //description: resets the nodes values
        public void ResetNode(int tileType)
        {
            //stores the new tiletype
            this.tileType = tileType;

            //resets heuristic values
            f = 0;
            g = 0;
            h = 0;

            //sets parent to null
            parent = null;
        }

        //pre: none
        //post: a flaot representing the f value
        //description: gets the f value
        public float GetFVal()
        {
            //returns the f value
            return f;
        }

        //pre: none
        //post: a flaot representing the g value
        //description: gets the g value
        public float GetGVal()
        {
            //returns the g value
            return g;
        }

        //pre: none
        //post: a flaot representing the h value
        //description: gets the h value
        public float GetHVal()
        {
            //returns the h value
            return h;
        }

        //pre: none
        //post: a node representing the parent
        //description: gets the parent node
        public Node GetParent()
        {
            //returns the parent node
            return parent;
        }

        //pre: none
        //post: an interger representing the id number
        //description: gets the id number
        public int GetId()
        {
            //returns the id number
            return id;
        }

        //pre: none
        //post: a float representing the tile type
        //description: gets the tile type
        public float GetTileType()
        {
            //returns the tile type
            return tileType;
        }

        //pre: none
        //post: a rectangle representing the node rectanlge
        //description: gets the node rectangle
        public Rectangle GetRec()
        {
            //returns the node rectangle
            return rec;
        }

        //pre: none
        //post: an interger representing the row number
        //description: gets the row number
        public int GetRow()
        {
            //returns the row number
            return row;
        }

        //pre: none
        //post: an interger representing the column number
        //description: gets the column number
        public int GetCol()
        {
            //returns the column number
            return col;
        }

        //pre: none
        //post: an list of nodes representing the adjacent nodes
        //description: gets the adjacent nodes
        public List<Node> GetAdjacent()
        {
            //returns the adjacent nodes
            return adjacent;
        }

        //pre: a valid interger representing the new id number
        //post: none
        //description: sets the id number
        public void SetId(int id)
        {
            //sets the id number
            this.id = id;
        }

        //pre: a valid node representing the new node
        //post: none
        //description: sets the node
        public void SetParent(Node parent)
        {
            //sets the parent node
            this.parent = parent;
        }

        //pre: a valid float representing the new h value
        //post: none
        //description: sets the h value
        public void SetHVal(float h)
        {
            //sets the h value
            this.h = h;
        }

        //pre: a valid float representing the new f value
        //post: none
        //description: sets the f value
        public void SetFVal(float f)
        {
            //sets the f value
            this.f = f;
        }

        //pre: a valid float representing the new g value
        //post: none
        //description: sets the g value
        public void SetGVal(float g)
        {
            //sets the g value
            this.g = g;
        }
    }
}
