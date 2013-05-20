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
        private float FireLifeTime;
        private float bulletWasFired;
        private float damage;

        
        private List<DrawableGameObject> bullets = new List<DrawableGameObject>();
        private List<DrawableGameObject> meleeBullets = new List<DrawableGameObject>();
        private List<DrawableGameObject> fire = new List<DrawableGameObject>();

        private List<Bullet> newbullets = new List<Bullet>();
        //Lists of things to remove
        private List<DrawableGameObject> meleeremoveList = new List<DrawableGameObject>();
        private List<DrawableGameObject> fireremoveList = new List<DrawableGameObject>();
        private List<Bullet> removeList = new List<Bullet>();
        private List<Body> meleeremoveListbody = new List<Body>();
        private List<Body> fireremoveListbody = new List<Body>();
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
            FireLifeTime = 1.0f;
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

            DrawableGameObject Obj = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(11, 8), 10, "shot");
            Obj.body.IsBullet = true;
            Obj.body.Position = position;
            Obj.body.IgnoreGravity = true;
            Obj.body.IsSensor = true;
            Obj.body.IgnoreCollisionWith(wheel);
            Obj.body.IgnoreCollisionWith(torso);
            
            switch (direction)
            {
                case Player.Direction.Right:
                    Obj.body.Rotation = MathHelper.ToRadians(180);
                    Obj.body.FixedRotation = true;              
                    Obj.body.ApplyLinearImpulse(new Vector2(speed, spread * 0.2f));
                    break;

                case Player.Direction.Left:
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed * -1, spread * 0.2f));
                    break;

                case Player.Direction.Down:
                    Obj.body.Rotation = MathHelper.ToRadians(270);
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed));
                    break;

                case Player.Direction.Up:
                    Obj.body.Rotation = MathHelper.ToRadians(90);
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(spread * 0.2f, speed * -1.0f));
                    break;

                case Player.Direction.Upright:
                    Obj.body.Rotation = MathHelper.ToRadians(135);
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed / 2, speed / 2 * -1.0f) * spreadDiagonal);
                    break;

                case Player.Direction.Upleft:
                    Obj.body.Rotation = MathHelper.ToRadians(45);
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2 * -1.0f) * spreadDiagonal);
                    break;

                case Player.Direction.Downright:
                    Obj.body.Rotation = MathHelper.ToRadians(-135);
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed / 2, speed / 2) * spreadDiagonal);
                    break;

                case Player.Direction.Downleft:
                    Obj.body.Rotation = MathHelper.ToRadians(-45);
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed / 2 * -1.0f, speed / 2) * spreadDiagonal);
                    break;

                default:
                    break;

            }
            newbullets.Add(new Bullet(Obj, damage, game, world, bulletLifeTime));
            newbullets[newbullets.Count - 1].Obj.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
        }

        /// <summary>
        /// Create a sniper bullet in the direction of the crosshair
        /// </summary>
        /// <param name="position">Position to shoot from</param>
        /// <param name="direction">Direction of bullet</param>
        /// <param name="world"></param>
        /// <param name="wheel"></param>
        /// <param name="damage">Bullet damage</param>
        public void NewBullet(Vector2 position, Vector2 direction, World world, Body wheel, Body torso, float damage, bool player)
        {
            
            DrawableGameObject Obj;
            if(player)
                Obj = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(11, 8), 10, "shot");
            else
                Obj = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(11, 8), 10, "hostile");

            Obj.body.IsBullet = true;
            Obj.body.Position = position;
            Obj.body.IgnoreGravity = true;
            
            Obj.body.IsSensor = true;
            Obj.body.IgnoreCollisionWith(wheel);
            Obj.body.IgnoreCollisionWith(torso);

            //calculate direction and rotation
            float dotProd = Vector2.Dot(Vector2.UnitY, direction);
            float rotation = (direction.X > 0) ? -(float)Math.Acos(dotProd) + MathHelper.ToRadians(-90) : (float)Math.Acos(dotProd) + MathHelper.ToRadians(-90);
            Obj.body.Rotation = rotation;
            Obj.body.FixedRotation = true;

            Obj.body.ApplyLinearImpulse(direction / 20);
            

            newbullets.Add(new Bullet(Obj, damage, game, world, bulletLifeTime));
            newbullets[newbullets.Count - 1].Obj.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
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
            
            DrawableGameObject Obj = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Fist"), new Vector2(22, 14), 10, "melee");
            Obj.body.IsBullet = true;
            Obj.body.Position = position;
            Obj.body.IgnoreGravity = true;
            Obj.body.IsSensor = true;
            Obj.body.IgnoreCollisionWith(wheel);

            switch (direction)
            {
                case Player.Direction.Right:
                    //bullet.body.Rotation = MathHelper.ToRadians(180);
                    Obj.texture = game.Content.Load<Texture2D>("Projectiles/FistFlip");
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed, 0));
                    break;

                case Player.Direction.Left:
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed * -1, 0));
                    break;

                default:
                    break;
            }

           
            newbullets.Add(new Bullet(Obj, damage, game, world, meleeBulletLifeTime));
            newbullets[newbullets.Count - 1].Obj.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
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
            //Allows the linear bullets to have some spread!
            float spread = random.Next(-2, 2);

            //Allows diagonal bullets to have some spread!
            float spreadDiagonal = random.Next(1, 5);
            spread /= 133;
            DrawableGameObject Obj = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(10, 4), 10, "hostile");
            Obj.body.IsBullet = true;
            Obj.body.Position = position;
            Obj.body.IgnoreGravity = true;
            Obj.body.IsSensor = true;
            Obj.body.IgnoreCollisionWith(wheel);

            switch (direction)
            {
                case Player.Direction.Right:
                    Obj.body.Rotation = MathHelper.ToRadians(180);
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed, spread * 0.2f));
                    break;

                case Player.Direction.Left:
                    Obj.body.FixedRotation = true;
                    Obj.body.ApplyLinearImpulse(new Vector2(speed * -1, spread * 0.2f));
                    break;
            }

            newbullets.Add(new Bullet(Obj, damage, game, world, bulletLifeTime));

            newbullets[newbullets.Count - 1].Obj.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);


        }

        public void FireBreath(Vector2 position, Player.Direction direction, World world, float speed, float damage)
        {
          

            for (int i = 0; i < 5; i++)
            {
                DrawableGameObject Obj = new DrawableGameObject(world, game.Content.Load<Texture2D>("Projectiles/Bullet"), new Vector2(22, 14), 10, "melee");
                Obj.body.IsBullet = true;
                Obj.body.Position = position;
                Obj.body.IgnoreGravity = true;
                Obj.body.IsSensor = true;
                
                switch (direction)
                {
                    case Player.Direction.Right:
                        Obj.body.Rotation = MathHelper.ToRadians(180);
                        Obj.body.FixedRotation = true;
                        Obj.body.ApplyLinearImpulse(new Vector2(speed, -0.4f + (float)i/5));
                        break;

                    case Player.Direction.Left:
                        Obj.body.FixedRotation = true;
                        Obj.body.ApplyLinearImpulse(new Vector2(speed * -1, 0.4f - (float)i/5));
                        break;
                }

                newbullets.Add(new Bullet(Obj, damage, game, world, FireLifeTime));

                newbullets[newbullets.Count - 1].Obj.body.OnCollision += new OnCollisionEventHandler(body_OnCollision);
            }
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
                        foreach (Bullet i in newbullets)
                        {
                            if (i.Obj.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                                game.damageList.Add(new Damage(fixtureB.Body.BodyId, i.Damage));
                            }
                        }
                        return true;
                    }

                    else if (fixtureA.UserData.ToString() == "melee" && fixtureB.UserData.ToString() == "enemy" ||
                        fixtureA.UserData.ToString() == "melee" && fixtureB.UserData.ToString() == "boss")
                    {
                        foreach (Bullet i in newbullets)
                        {
                            if (i.Obj.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                            }
                            game.damageList.Add(new Damage(fixtureB.Body.BodyId, i.Damage));
                        }
                        
                        return true;
                    }


                    else if (fixtureA.UserData.ToString() == "fire" && fixtureB.UserData.ToString() == "enemy" ||
                    fixtureA.UserData.ToString() == "fire" && fixtureB.UserData.ToString() == "boss")
                    {
                        foreach (Bullet i in newbullets)
                        {
                            if (i.Obj.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                                game.damageList.Add(new Damage(fixtureB.Body.BodyId, i.Damage));
                            }
                        }
                        
                        return true;
                    }


                    else if ((fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "player") ||(fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "shield"))

                    {
                        foreach (Bullet i in newbullets)
                        {
                            if (i.Obj.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                                game.damageList.Add(new Damage(fixtureB.Body.BodyId, i.Damage, player.skillTree.playerDmg));
                            }
                        }
                        return true;
                    }

                    else if (fixtureA.UserData.ToString() == "shot" && fixtureB.UserData.ToString() == "ground")
                    {
                        foreach (Bullet i in newbullets)
                        {
                            if (i.Obj.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                            }
                        }
                        return true;
                    }

                    else if (fixtureA.UserData.ToString() == "fire" && fixtureB.UserData.ToString() == "ground")
                    {
                        foreach (Bullet i in newbullets)
                        {
                            if (i.Obj.body.BodyId == fixtureA.Body.BodyId)
                            {
                                if (!removeList.Contains(i))
                                    removeList.Add(i);
                            }
                        }
                        return true;
                    }

                    else if (fixtureA.UserData.ToString() == "hostile" && fixtureB.UserData.ToString() == "ground")
                    {
                        foreach (Bullet i in newbullets)
                        {
                            if (i.Obj.body.BodyId == fixtureA.Body.BodyId)
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
                for (int i = 0; i < newbullets.Count; i++)
                {
                    newbullets[i].Obj.Draw(spriteBatch);
                    if (newbullets[i].WasFired + newbullets[i].LifeTime <= Game1.runTime)
                    {
                        if (!removeList.Contains(newbullets[i]))
                            removeList.Add(newbullets[i]);
                        /*try
                        {
                            game.world.RemoveBody(bullets[i].body);
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
                        }*/
                    }
                }

                /*for (int i = 0; i < meleeBullets.Count; i++)
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

                for (int i = 0; i < fire.Count; i++)
                {
                    fire[i].Draw(spriteBatch);
                    if (bulletWasFired + FireLifeTime <= (float)Game1.runTime)
                    {
                        if (!fireremoveList.Contains(fire[i]))
                            fireremoveList.Add(fire[i]);
                        try
                        {
                            game.world.RemoveBody(fire[i].body);
                        }
                        catch (Exception ex)
                        {
                            logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
                        }
                    }
                }*/
            }

            catch (Exception ex)
            {
                logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
            }

            foreach (Bullet i in removeList)
            {
                newbullets.Remove(i);
            }

            /*foreach (DrawableGameObject i in meleeremoveList)
            {
                meleeBullets.Remove(i);
            }

            foreach (DrawableGameObject i in fireremoveList)
            {
                fire.Remove(i);
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

            foreach (Body i in meleeremoveListbody)
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

            foreach (Body i in fireremoveListbody)
            {
                try
                {
                    game.world.RemoveBody(i);
                }

                catch (Exception ex)
                {
                    logger.Fatal(ex.Message + "  " + ex.TargetSite + "  " + ex.StackTrace);
                }
            }*/

            removeListbody.Clear();
            removeList.Clear();
            meleeremoveList.Clear();
            meleeremoveListbody.Clear();
            fireremoveList.Clear();
            fireremoveListbody.Clear();

        }
        #endregion
    }
}
