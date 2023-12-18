// Author: Dylan Nagel
// File Name: SlowBullet.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Creates an instance of a slow bullet bullet, adjusting specified variables for gameplay

using Microsoft.Xna.Framework;
using System;

namespace NagelD_PASS3
{
    public class SlowBullet:Bullet
    {
        //pre: a valid float angle, a valid vector2 spawn location within the board, a valid in ownership either 0 or 1. a valid max ricochet count
        //post: none
        //description: creates an instance of a slow bullet
        public SlowBullet(float angle, Vector2 spawnLoc, int ownership, int maxRicCount) :base(angle, spawnLoc, ownership, maxRicCount)
        {
            //sets bullet image
            bulletImg = Game1.bulletImg;

            //sets bullet origin
            bulletOrigin = new Vector2(bulletImg.Width * 0.5f, bulletImg.Height * 0.5f);

            //sets bullet speed
            speed = new Vector2((float)(Game1.BULLET_SPEED * Math.Cos(angle)), (float)(Game1.BULLET_SPEED * Math.Sin(angle)));        
        }
    }
}