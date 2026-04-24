using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class SleepyBubbles : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory Category => ClassCategory.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public bool IsRevers = false;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void ExSD()
        {
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 16;
            Projectile.SetupImmnuity(-1);
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = GetSeconds(10);
        }
        public override void OnFirstFrame()
        {
            IsRevers = Main.rand.NextBool();
            for (int i =0;i<18;i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6f), Main.rand.NextBool() ? DustID.GreenTorch : DustID.Terra);
                d.scale *= 0.86f;
                d.noGravity = true;
                d.velocity = RandVelTwoPi(0f, 3f);
            }

        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            UpdateParticle();
            if(Projectile.MeetMaxUpdatesFrame(Timer, 30))
            {
                if (Projectile.GetTargetSafe(out NPC target, searchDistance: 800, canPassWall: false))
                {
                    Projectile.HomingTarget(target.Center, -1, 18, 20);
                }
                else
                    Projectile.velocity *= 0.94f;
            }
            else
            {
                Projectile.velocity *= 0.96f;
                Projectile.velocity.RotatedBy(ToRadians(5f) * IsRevers.ToDirectionInt());
            }
            if (Projectile.FinalUpdate())
                Projectile.netUpdate = true;
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen() || Main.rand.NextBool())
                return;
            

            if (Projectile.velocity.LengthSquared() < Main.rand.NextFloat() * (5f * 5f))
            {
if (Main.rand.NextBool(4))
            {
                Vector2 pos = Projectile.ToRandRec();
                new ShinyCrossStar(pos, -Vector2.UnitY*Main.rand.NextFloat(0.8f,1.8f), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, 0, 1, 0.48f, false).Spawn();
            }
                return;
            }
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TerraBlade);
                d.scale *= 0.76f;
                d.velocity = -Projectile.velocity.ToRandVelocity(ToRadians(5f), 1.4f, 3.4f);
                d.noGravity = true;
            }
            if(Main.rand.NextBool(3))
            {
                new ShinyCrossStar(Projectile.Center.ToRandCirclePosEdge(4f), -Projectile.velocity.ToRandVelocity(ToRadians(5f), 1.4f, 3.4f), RandLerpColor(Color.DarkGreen, Color.LimeGreen), 40, 0, 1, 0.48f, false).Spawn();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i =0;i<18;i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(6f), Main.rand.NextBool() ? DustID.GreenTorch : DustID.Terra);
                d.scale *= 0.86f;
                d.noGravity = true;
                d.velocity = RandVelTwoPi(0f, 3f);
            }
        }
        public override bool? CanDamage()
        {
            return Projectile.MeetMaxUpdatesFrame(Timer, 30);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 projPos, out Vector2 ori);
            projTex = HJScarletTexture.Particle_ShinyOrbHard.Value;
            ori = HJScarletTexture.Particle_ShinyOrbHard.Origin;
            Texture2D starShape = HJScarletTexture.Particle_SharpTear;
            //绘制残影
            float oriScale = 0.64f;
            float scale = 1f;
            Vector2 shapeScale = new Vector2(0.8f, 1.5f);
            int length = 6;
            for (int i = 0; i < length; i++)
            {
                scale *= 0.975f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.DarkGreen, Color.Green, (1 - rads)).ToAddColor(20) * Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.60f) + Projectile.PosToCenter();
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f);
                SB.Draw(projTex, lerpPos, null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
                SB.Draw(projTex, lerpPos, null, Color.White.ToAddColor(20) *Clamp(Projectile.velocity.Length(), 0f, 1f) * (1 - rads) , rot, ori, oriScale * scale * Projectile.scale * 0.65f, 0, 0);
            }
            SB.Draw(projTex, projPos, null, Color.DarkGreen.ToAddColor(50), Projectile.rotation, ori, oriScale * Projectile.scale * 1f, 0, 0);
            SB.Draw(projTex, projPos, null, Color.White.ToAddColor(0), Projectile.rotation, ori, oriScale * Projectile.scale * 0.85f, 0, 0);

            return false;
        }
    }
}
