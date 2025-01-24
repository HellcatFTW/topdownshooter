﻿using Microsoft.Xna.Framework.Input;
using System;

namespace TopDownShooter.Entity
{
    public sealed class Player : Entity
    {
        private readonly Texture2D TankHullTexture;
        private readonly Texture2D TankTurretTexture;

        private Vector2 directionToMouse;
        private float turretRotation = 0f;
        private float hullRotation = 0f;

        private const float movementSpeed = 3f;

        private const float shootCooldown = 1.2f;
        private float shootTimer = 0;
        

        public Player()
        {
            TankHullTexture = Globals.Content.Load<Texture2D>("TankHull");
            TankTurretTexture = Globals.Content.Load<Texture2D>("TankTurret");

            position = Vector2.Zero;
        }
        public override void Update()
        {
            Vector2 tempVector = Vector2.Zero;
            tempVector += new Vector2(Input.GetAxis.X, -Input.GetAxis.Y);
            position += tempVector.SafeNormalize(Vector2.Zero) * movementSpeed;

            directionToMouse = (World.mouseWorld - position).SafeNormalize(Vector2.Zero);

            World.cameraPos = position - new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2); ;

            if (shootTimer >= 0f)
            { 
                shootTimer -= (float)Globals.gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Input.MouseLeft && shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootCooldown;
            }
        }
        public override void Draw()
        {
            MouseState mouse = Mouse.GetState();

            Vector2 screenPos = position - World.cameraPos;

            if (Input.GetAxis != Vector2.Zero)
            {
                hullRotation = (float)Math.Atan2(Input.GetAxis.Y, -Input.GetAxis.X) - MathHelper.PiOver2;
            }

            Vector2 turretOrigin = new Vector2(TankTurretTexture.Width / 2, TankTurretTexture.Height / 2 + 9f); // 9 pixel offset to accommodate for the barrel
            Vector2 hullOrigin = new Vector2(TankHullTexture.Width / 2, TankTurretTexture.Height / 2);

            turretRotation = (float)Math.Atan2(directionToMouse.Y, directionToMouse.X);
            turretRotation += MathHelper.PiOver2;


            Globals.SpriteBatch.Draw(TankHullTexture, screenPos, null, Color.White, hullRotation, hullOrigin, 1, SpriteEffects.None, LayerDepths.Entities);

            Globals.SpriteBatch.Draw(TankTurretTexture, screenPos, null, Color.White, turretRotation, turretOrigin, 1, SpriteEffects.None, LayerDepths.Entities);
        }

        public void Shoot()
        {
            Projectile.NewProjectile<PlayerShell>(position + directionToMouse * 50f, directionToMouse);
        }
    }
}
