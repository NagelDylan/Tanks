// Author: Dylan Nagel
// File Name: FastBullet.cs
// Project Name: NagelD_PASS3
// Creation Date: May. 15, 2023
// Modified Date: June. 12, 2023
// Description: Creates an instance of a fast bullet, holding all important information for the gameplay

using Microsoft.Xna.Framework;
using System;

namespace NagelD_PASS3
{
    public class FastBullet:Bullet
    {
        //pre: a valid float angle, a valid vector2 spawn location within the board, a valid in ownership either 0 or 1. a valid max ricochet count
        //post: none
        //description: creates an instance of a fast bullet
        public FastBullet(float angle, Vector2 spawnLoc, int ownership, int maxRicCount) : base(angle, spawnLoc, ownership, maxRicCount)
        {
            //stores the bullet image
            bulletImg = Game1.fastBulletImg;

            //stores the bullet origin
            bulletOrigin = new Vector2(bulletImg.Width * 0.5f, bulletImg.Height * 0.5f);

            //stores the speed
            speed = new Vector2((float)(Game1.FAST_BULLET_SPEED * Math.Cos(angle)), (float)(Game1.FAST_BULLET_SPEED * Math.Sin(angle)));
        }
    }
}