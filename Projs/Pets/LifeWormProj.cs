using HJScarletRework.Assets.Registers;
using HJScarletRework.Buffs.Pets;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Projs.Pets
{
    public class LifeWormProj : ScarletPetProjClass
    {
        public enum State
        {
            Idle,
            Catch
        }
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public int CurrentFetching
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public Vector2 CatchPosition = Vector2.Zero;
        public float CatchLerp = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(201, 0);
            HJScarletMethods.PetStaticDefaultsCommon(Type, 3);
        }
        public override void ExSD()
        {
            Projectile.ignoreWater = true;
            Projectile.scale = 1.1f;
        }
        public override void SimplePetFunction()
        {
            if (Owner.dead)
                Owner.HJScarlet().petLifeWorm = false;
            if (Owner.HJScarlet().petLifeWorm)
                Projectile.timeLeft = 2;
        }
        public override void PetAI()
        {
            HJScarletMethods.PetCommonBuffCheck(Projectile, BuffType<LifeWormBuff>());
            var player = Main.player[Projectile.owner];
            if (player.miscCounter % 10 == 0&&AttackState == State.Idle)
            {
                float dist = 16f * 500;
                //后续改为“WorldItem”，需注意
                foreach (var item in Main.ActiveItems)
                {
                    if (item.noGrabDelay == 0 && !item.beingGrabbed && Owner.CanPullItem(item, Owner.ItemSpace(item)) && ItemID.Sets.IsFood[item.type])
                    {
                        var dist2 = (Projectile.Center - item.Center).LengthSquared();
                        if (dist2 > dist * dist)
                            continue;
                        CurrentFetching = item.whoAmI;
                        dist = dist2;
                    }
                }
                if (CurrentFetching > -1)
                {
                    AttackState = State.Catch;
                }
                else if (AttackState == State.Catch)
                {
                    AttackState = State.Idle;
                }
            }
            switch (AttackState)
            {
                case State.Catch:
                    var worldItem = Main.item[CurrentFetching];
                    var direction = Projectile.Center.DirectionTo(worldItem.Center);

                    Vector2 catchingSpot = Projectile.Center + (Projectile.rotation+PiOver2).ToRotationVector2()* -25f;
                    if (!worldItem.active || worldItem.beingGrabbed || !Owner.CanPullItem(worldItem, Owner.ItemSpace(worldItem)))
                    {
                        CurrentFetching = 0;
                        AttackState = State.Idle;
                        break;
                    }
                    CatchPosition = worldItem.Center;
                    CatchLerp = Lerp(CatchLerp, 1.01f, 0.1f);
                    Lighting.AddLight(CatchPosition, Color.White.ToVector3() * CatchLerp * 2f);
                    float dist = Min((worldItem.Center - catchingSpot).Length(), (worldItem.Center - Projectile.Center).Length());
                    if (dist < 36)
                    {
                        Projectile.velocity += Projectile.DirectionTo(Owner.Center) * .5f;
                        Projectile.velocity *= .95f;
                        worldItem.Center = catchingSpot;
                        worldItem.velocity = Vector2.Zero;
                    }
                    else
                    {
                        Projectile.velocity += Projectile.DirectionTo(worldItem.Center) * .95f;
                        Projectile.velocity *= .95f;
                    }
                    Projectile.rotation = Projectile.velocity.ToRotation() + PiOver2;

                    break;

                case State.Idle:
                    CatchPosition = Vector2.Zero;
                    CatchLerp = Lerp(CatchLerp, 0f, 0.1f);
                    CurrentFetching = -1;
                    Vector2 center = player.Center;
                    float distanceToOwner = (center - Projectile.Center).Length();
                    if (distanceToOwner > 2000f)
                    {
                        Projectile.Center = center;
                        Projectile.velocity = Vector2.Zero;
                        Projectile.netUpdate = true;
                    }

                    float ownerVelocity = Clamp(player.velocity.Length(), 4, 12);
                    Projectile.velocity.Length();
                    if (Projectile.velocity == Vector2.Zero)
                    {
                        Projectile.velocity.X = 2f * player.direction;
                        Vector2 vector = Projectile.position;
                        for (int i = 0; i < Projectile.oldPos.Length; i++)
                        {
                            vector -= Projectile.velocity;
                            Projectile.oldPos[i] = vector;
                        }
                    }

                    if (distanceToOwner >= 120f)
                    {
                        float targetAngle = Projectile.AngleTo(center);
                        float f = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(5f));
                        Projectile.velocity = f.ToRotationVector2() * ownerVelocity;
                    }

                    if (Projectile.velocity.Length() > ownerVelocity)
                        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * ownerVelocity;

                    if (Math.Abs(Projectile.velocity.Y) < 1f)
                        Projectile.velocity.Y -= 0.1f;

                    Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2f;
                    int num3 = Projectile.direction;
                    Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : (-1);
                    if (num3 != Projectile.direction)
                        Projectile.netUpdate = true;

                    Projectile.position.X = Clamp(Projectile.position.X, 160f, Main.maxTilesX * 16 - 160);
                    Projectile.position.Y = Clamp(Projectile.position.Y, 160f, Main.maxTilesY * 16 - 160);

                    break;
            }
        }
        /// <summary>
        /// 蠕虫宠物体节数量，每100生命值对应一额外体节
        /// </summary>
        public int WormUnitCount => Math.Clamp(Main.player[Projectile.owner].statLife / 100 + 3, 3, 21);
        /// <summary>
        /// 蠕虫宠物单节长度
        /// </summary>
        public float WormUnitLength => 18;
        protected virtual int GetWormUnitFrame(int index, int max)
        {
            if (index == 0)
                return 0;
            if (index == max - 1)
                return Main.projFrames[Type] - 1;
            else
                return 1;
                //return index == 0
                //    ? 0
                //    : (
                //        index == max - 1
                //        ? Main.projFrames[Type] - 1
                //        : 1
                //      );
            //return index == max - 1 ? Main.projFrames[Type] - 1 : 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //这下面叽里咕噜一大堆都是画蠕虫的
            Projectile proj = Projectile;
            Texture2D value = TextureAssets.Projectile[proj.type].Value;
            int count = WormUnitCount;
            float unitLength = WormUnitLength;
            SpriteEffects effects = (proj.spriteDirection != 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle rectangle = value.Frame(1, Main.projFrames[proj.type], 0, GetWormUnitFrame(0, count));
            Vector2 origin = rectangle.Size() / 2f;
            Vector2 position = proj.Center - Main.screenPosition;
            Color alpha = Color.White;
            Color color = Color.White * (Main.mouseTextColor / 255f);
            Vector2 vector = proj.Center;
            for (int i = 1; i < count; i++)
            {
                int frameY = GetWormUnitFrame(i, count);
                Rectangle value2 = value.Frame(1, Main.projFrames[proj.type], 0, frameY);
                Vector2 vector2 = proj.oldPos[i * 10] + proj.Size / 2f;
                float num5 = (vector - vector2).ToRotation();
                vector2 = vector - new Vector2(unitLength, 0f).RotatedBy(num5, Vector2.Zero);
                num5 = (vector - vector2).ToRotation() + (float)Math.PI / 2f;
                Vector2 position2 = vector2 - Main.screenPosition;
                SpriteEffects effects2 = (!(vector2.X < vector.X)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                vector = vector2;
                Main.EntitySpriteDraw(value, position2, value2, Color.White, num5, origin, proj.scale, effects2);
            }

            Main.EntitySpriteDraw(value, position, rectangle, alpha, proj.rotation, origin, proj.scale, effects);
            //这下面我们才画别的东西
            if (CatchPosition.Equals(Vector2.Zero))
                return false;
            //Main.spriteBatch.EnterShaderArea();
            //Texture2D glow = HJScarletTexture.Particle_CrossGlow.Value;
            //Vector2 glowPos = CatchPosition - Main.screenPosition;
            //float scale = .26f;
            //float opa = CatchLerp;

            //Main.spriteBatch.FastDraw(glow, glowPos, Color.DarkGreen*opa, 0, glow.Size() / 2f, scale, 0);
            //Main.spriteBatch.FastDraw(glow, glowPos, Color.LimeGreen*opa, 0, glow.Size() / 2f, scale*.98f, 0);
            //Main.spriteBatch.FastDraw(glow, glowPos, Color.White * opa, 0, glow.Size() / 2f, scale*.95f, 0);
            //Projectile.SetCrossStar(CatchPosition, 1.15f, 0, Color.LimeGreen*opa);
            //Main.spriteBatch.EndShaderArea();
            return false;
        }
    }
}
