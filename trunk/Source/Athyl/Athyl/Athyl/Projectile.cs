using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using NLog;

namespace Athyl
{
    class Projectile
    {
        #region Properties

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private int projectileVelocity;
        private Vector2 projectileDirection;
        private bool friendly;
        private float bulletLifeTime;
        private float meleeBulletLifeTime;
        private float bulletWasFired;
        private float damage;

        //Lists of things to remove
        private List<DrawableGameObject> bullets = new List<DrawableGameObject>();
        private List<DrawableGameObject> meleeBullets = new List<DrawableGameObject>();
        private List<DrawableGameObject> meleeremoveList = new List<DrawableGameObject>();
        private List<DrawableGameObject> removeList = new List<DrawableGameObject>();
        private List<Body> meleeremoveListbody = new List<Body>();
        private List<Body> removeListbody = new List<Body>();

        private Game1 game;
        private Player player;
        private Random random = new Random();

        #endregion
        #region Constructor
        public Projectile(Game1 game)
        {
            this.game = game;
            bulletLifeTime = 10.0f;
            meleeBulletLifeTime = 0.5f;
        }

        public Projectile(Game1 game, Player player)
        {
            this.game = game;
            this.player = player;
            bulletLifeTime = 10.0f;
        }
        #endregion
        #region AddBullet
        /// <summary>
        /// Adds a new bullet to the list
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="world"></param>
        public void NewBullet(Vector2 position, Player.Direction direction, World world, float speed, Body wheel, Body torso, float damage, bool sniper)
        {
            this.damage = damage;
            float spread = 0;
            float spreadDiagonal = 1;

            if (!sniper)
            {
                //Allows the linear bullets to have some spread!
                spread = random.Next(-2, 2);

                //Allows diagonal bullets to have some spread!
                spreadDiagonal = random.Next(1, 5);
                spread /= 133;
            }

            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(11, 8), 10, "shot");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bulletWasFired = Game1.runTime;
            bullet.body.IsSensor = true;
            bullet.body.IgnoreCollisionWith(wheel);
            bullet.body.IgnoreCollisionWith(torso);
            switch (direction)
            {
                case Player.Direction.Right:
                    bullet.body.Rotation = MathHelper.ToRadians(180);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, spread * 0.2f));
                    break;

                case Player.Direction.Left:
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, spread * 0.2f));
                    break;

                case Player.Direction.Down:
                    bullet.body.Rotation = MathHelper.ToRadians(270);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed));
                    break;

                case Player.Direction.Up:
                    bullet.body.Rotation = MathHelper.ToRadians(90);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed * -1.0f));
                    break;

                case Player.Direction.Upright:
                    bullet.body.Rotation = MathHelper.ToRadians(135);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2, speed / 2 * -1.0f) * spreadDiagonal);
                    break;

                case Player.Direction.Upleft:
                    bullet.body.Rotation = MathHelper.ToRadians(45);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2 * -1.0f) * spreadDiagonal);
                    break;

                case Player.Direction.Downright:
                    bullet.body.Rotation = MathHelper.ToRadians(-135);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2, speed / 2) * spreadDiagonal);
                    break;

                case Player.Direction.Downleft:
                    bullet.body.Rotation = MathHelper.ToRadians(-45);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2) * spreadDiagonal);
                    break;

                default:
                    break;

            }
            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        /// <summary>
        /// Create a sniper bullet in the direction of the crosshair
        /// </summary>
        /// <param name="position">Position to shoot from</param>
        /// <param name="direction">Direction of bullet</param>
        /// <param name="world"></param>
        /// <param name="wheel"></param>
        /// <param name="damage">Bullet damage</param>
        public void NewBullet(Vector2 position, Vector2 direction, World world, Body wheel, Body torso, float damage)
        {
            this.damage = damage;
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(11, 8), 10, "shot");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bulletWasFired = Game1.runTime;
            bullet.body.IsSensor = true;
            bullet.body.IgnoreCollisionWith(wheel);
            bullet.body.IgnoreCollisionWith(torso);

            //calculate direction and rotation
            float dotProd = Vector2.Dot(Vector2.UnitY, direction);
            float rotation = (direction.X > 0) ? -(float)Math.Acos(dotProd) + MathHelper.ToRadians(-90) : (float)Math.Acos(dotProd) + MathHelper.ToRadians(-90);
            bullet.body.Rotation = rotation;
            bullet.body.FixedRotation = true;

            bullets.Add(bullet);
            bullets[bullets.Count - 1].body.ApplyLinearImpulse(direction / 20);
            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        /// <summary>
        /// Adds a melee bullet
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="world"></param>
        /// <param name="speed"></param>
        /// <param name="wheel"></param>
        /// <param name="damage"></param>
        public void NewMeleeBullet(Vector2 position, Player.Direction direction, World world, float speed, Body wheel, float damage)
        {
            this.damage = damage;
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Fist"), new Vector2(22, 14), 10, "melee");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bulletWasFired = Game1.runTime;
            bullet.body.IsSensor = true;
            bullet.body.IgnoreCollisionWith(wheel);

            switch (direction)
            {
                case Player.Direction.Right:
                    //bullet.body.Rotation = MathHelper.ToRadians(180);
                    bullet.texture = game.Content.Load<Texture2D>("Projectiles/FistFlip");
                    bullet.body.FixedRotation = true;
                    meleeBullets.Add(bullet);
                    meleeBullets[meleeBullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, 0));
                    break;

                case Player.Direction.Left:
                    bullet.body.FixedRotation = true;
                    meleeBullets.Add(bullet);
                    meleeBullets[meleeBullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, 0));
                    break;

                default:
                    break;
            }

            meleeBullets[meleeBullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        /// <summary>
        /// prototyp, kulor som fienden skjuter
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="world"></param>
        /// <param name="speed"></param>
        public void NewEnemyBullet(Vector2 position, Player.Direction direction, World world, float speed, Body wheel, float damage)
        {
            this.damage = damage;
            //Allows the linear bullets to have some spread!
            float spread = random.Next(-2, 2);

            //Allows diagonal bullets to have some spread!
            float spreadDiagonal = random.Next(1, 5);
            spread /= 133;
            DrawableGameObject bullet = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(10, 4), 10, "hostile");
            bullet.body.IsBullet = true;
            bullet.body.Position = position;
            bullet.body.IgnoreGravity = true;
            bulletLifeTime = 5;
            bulletWasFired = Game1.runTime;
            bullet.body.IsSensor = true;
            bullet.body.IgnoreCollisionWith(wheel);

            switch (direction)
            {
                case Player.Direction.Right:
                    bullet.body.Rotation = MathHelper.ToRadians(180);
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed, spread * 0.2f));
                    break;

                case Player.Direction.Left:
                    bullet.body.FixedRotation = true;
                    bullets.Add(bullet);
                    bullets[bullets.Count - 1].body.ApplyLinearImpulse(new Vector2(speed * -1, spread * 0.2f));
                    break;
            }

            bullets[bullets.Count - 1].body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }
        #endregion
        #region Collisionanddraw
        /// <summary>
        /// Kollision med kulor, tar bort något träffas
        /// olika logik beroende på om det är fiende eller spelare som krockar
        /// </summary>
        /// <param name="fixtureA"></param>
        /// <param name="fixtureB"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        bool body_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (contact.IsTouching())
            {
                try
                {
                    if (fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "enemy" ||
                        fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "boss")
                    {
                        if (!removeListbody.Contains(fixtureA.Body))
                            removeListbody.Add(fixtureA.Body);
                        foreach (DrawableGameObject i in bullets)
                        {
                            if (i.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                            }
                        }
                        game.damageList.Add(new Damage(fixtureB.Body.BodyId, damage));
                        return true;
                    }

                    else if (fixtureA.UserData.ToString() == "melee" && fixtureB.UserData.ToString() == "enemy" ||
                        fixtureA.UserData.ToString() == "melee" && fixtureB.UserData.ToString() == "boss")
                    {
                        if (!meleeremoveListbody.Contains(fixtureA.Body))
                            meleeremoveListbody.Add(fixtureA.Body);
                        foreach (DrawableGameObject i in bullets)
                        {
                            if (i.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!meleeremoveList.Contains(i))
                                    meleeremoveList.Add(i);
                            }
                        }
                        game.damageList.Add(new Damage(fixtureB.Body.BodyId, damage));
                        return true;
                    }

                    else if ((fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "player") ||(fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "shield"))
                    {
                        if (!removeListbody.Contains(fixtureA.Body))
                            removeListbody.Add(fixtureA.Body);
                        foreach (DrawableGameObject i in bullets)
                        {
                            if (i.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                            }
                        }
                        game.damageList.Add(new Damage(fixtureB.Body.BodyId, damage, player.skillTree.playerDmg));
                        return true;
                    }
                    else if (fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "ground")
                    {
                        if (!removeListbody.Contains(fixtureA.Body))
                            removeListbody.Add(fixtureA.Body);
                        foreach (DrawableGameObject i in bullets)
                        {
                            if (i.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                            }
                        }
                        return true;
                    }
                    else if (fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "ground")
                    {
                        if (!removeListbody.Contains(fixtureA.Body))
                            removeListbody.Add(fixtureA.Body);
                        foreach (DrawableGameObject i in bullets)
                        {
                            if (i.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                            }
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
                }
            }
            return false;
        }

        /// <summary>
        /// Clears all bullets from all kinds of lists and from the world
        /// </summary>
        /// <param name="world"></param>
        public void Clear(World world)
        {
            foreach (DrawableGameObject d in bullets)
            {
                world.RemoveBody(d.body);
            }
            bullets.Clear();

            foreach (DrawableGameObject d in meleeBullets)
            {
                world.RemoveBody(d.body);
            }
            meleeBullets.Clear();
            meleeremoveList.Clear();
            meleeremoveListbody.Clear();
            removeList.Clear();
            removeListbody.Clear();
        }

        /// <summary>
        /// Ritar projektiler. Tar bort alla kulor som träffar något eller har varit aktiva för länge
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch, Vector2 playerPos)
        {
            try
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    bullets[i].Draw(spriteBatch);
                    if (bulletWasFired + bulletLifeTime <= Game1.runTime)
                    {
                        if (!removeList.Contains(bullets[i]))
                            removeList.Add(bullets[i]);
                        try
                        {
                            game.world.RemoveBody(bullets[i].body);
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
                        }
                    }
                }

                for (int i = 0; i < meleeBullets.Count; i++)
                {
                    meleeBullets[i].Draw(spriteBatch);
                    if (bulletWasFired + meleeBulletLifeTime <= (float)Game1.runTime)
                    {
                        if (!meleeremoveList.Contains(meleeBullets[i]))
                            meleeremoveList.Add(meleeBullets[i]);
                        try
                        {
                            game.world.RemoveBody(meleeBullets[i].body);
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
            }

            foreach (DrawableGameObject i in removeList)
            {
                bullets.Remove(i);
            }

            foreach (DrawableGameObject i in meleeremoveList)
            {
                meleeBullets.Remove(i);
            }


            foreach (Body i in removeListbody)
            {
                try
                {
                    game.world.RemoveBody(i);
                }

                catch (Exception ex)
                {
                    logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
                }
            }

            removeListbody.Clear();
            removeList.Clear();
            meleeremoveList.Clear();

        }
        #endregion
    }
}
