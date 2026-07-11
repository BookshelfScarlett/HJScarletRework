using ContinentOfJourney.Items.GemOriented.Onyx;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class PureYinyoBlack : HJScarletProj
    {
        public override string Texture => ProjPath + "PureYinyoProj";
        public override ClassCategory Category => ClassCategory.Executor;
        public enum State
        {
            Shoot,
            ReadyBack,
            Back
        }
        public State AttackState
        {
            get => (State)(Projectile.ai[1]);
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        public AnimationStruct Helper = new(2);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.SetupImmnuity(-1);
        }
        public override void OnFirstFrame()
        {
            Helper.MaxProgress[0] = 10;
        }
        public override void ProjAI()
        {
            UpdateAttackAI();
            UpdateParticle();
        }

        public void UpdateParticle()
        {
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(8))
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(30f), Projectile.velocity / 4f, RandLerpColor(Color.Black, Color.Black), Main.rand.Next(30, 70), RandRotTwoPi, 1, Projectile.scale * Main.rand.NextFloat(.80f, 1.1f) * .3f, false, BlendState.NonPremultiplied);
            if (Main.rand.NextBool(4))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(25f);
                Vector2 vel = Projectile.velocity / 8f;
                int lifeTime = Main.rand.Next(30, 70);
                float scale = Projectile.scale * Main.rand.NextFloat(0.75f, 1.1f) * .14f;
                ECSParticle.HRShinyOrb(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.WhiteSmoke, .12f)), lifeTime, 1, scale, .5f, BlendState.NonPremultiplied);
            }

        }

        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.ReadyBack:
                    DoReadyBack();
                    break;
                case State.Back:
                    DoBack();
                    break;
            }
        }
        public void DoShoot()
        {
            Timer++;
            Projectile.rotation += .2f;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 15))
            {
                Projectile.velocity = Projectile.Center.GetNormalVector2(Owner.Center) * 5f;
                AttackState = State.Back;
                Projectile.netUpdate = true;
                Projectile.tileCollide = false;
                //AttackState = State.ReadyBack;
                Projectile.netUpdate = true;
            }
        }
        public void DoReadyBack()
        {
            if (Helper.IsDone[0])
            {
                
            }
            else
            {

                Projectile.velocity *= 0.927f;
                //Projectile.rotation = Projectile.rotation.AngleLerp((Projectile.velocity).ToRotation(), 0.05f);
            }
        }
        public void DoBack()
        {
            Projectile.rotation += .2f;
            Projectile.HomingTarget(Owner.Center, -1, 20f, 10f);
            if (Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                Projectile.Kill();
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (AttackState == State.Back)
                return false;
                AttackState = State.Back;
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(3);
                Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                int lifeTime = Main.rand.Next(30, 70);
                float rot = RandRotTwoPi;
                new SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, .8f, 0.35f, true).SpawnToPriorityNonPreMult();
                new SmokeParticle(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.DarkViolet, .10f)), lifeTime, rot, 1f, 0.3f, true).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(16);
                Vector2 dir = Projectile.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .4f, .15f);
            }
            new CrossGlow(Projectile.Center, Color.WhiteSmoke, 40, 1, .24f).Spawn();
            SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with { MaxInstances = 0,Pitch = -.8f ,Volume = .4f},Projectile.Center);
            SoundEngine.PlaySound(HJScarletSounds.TheMars_Hit with { MaxInstances = 0,Pitch = -.8f, Volume = .4f },Projectile.Center);
            Projectile.velocity = -Projectile.oldVelocity;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.AddExecutionTimeImmediate(ItemType<PureYinyo>());
            for (int i = -2; i < 3; i++)
            {
                Vector2 dir = RandDirTwoPi;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, dir * Main.rand.NextFloat(10f, 12.5f), ProjectileType<PureYinyoShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                ((PureYinyoShard)proj.ModProjectile).CurTarget = target;
                ((PureYinyoShard)proj.ModProjectile).BlackShard = true;
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = target.Center.ToRandCirclePos(3);
                Vector2 dir = target.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                int lifeTime = Main.rand.Next(30, 70);
                float rot = RandRotTwoPi;
                new SmokeParticle(pos, vel, Color.WhiteSmoke, lifeTime, rot, .8f, 0.35f, true).SpawnToPriorityNonPreMult();
                new SmokeParticle(pos, vel, RandLerpColor(Color.Black, Color.Lerp(Color.Black, Color.DarkViolet, .10f)), lifeTime, rot, 1f, 0.3f, true).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 32; i++)
            {
                Vector2 pos = target.Center.ToRandCirclePos(16);
                Vector2 dir = target.Center.GetNormalVector2(pos);
                Vector2 vel = dir * Main.rand.NextFloat(0.3f, 7f);
                ECSParticle.ShinyCrossStarECS(pos, vel, RandLerpColor(Color.Silver, Color.WhiteSmoke), Main.rand.Next(30, 70), 1, Main.rand.NextFloat(.7f, 1.1f) * .4f, .15f);
            }
            new CrossGlow(target.Center, Color.WhiteSmoke, 40, 1, .24f).Spawn();
            SoundEngine.PlaySound(HJScarletSounds.Misc_Ding with { MaxInstances = 0,Pitch = -.8f });
            SoundEngine.PlaySound(HJScarletSounds.TheMars_Hit with { MaxInstances = 0,Pitch = -.8f });
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Rectangle frame = tex.Frame(1, 2, 0, 1);
            Vector2 origin = frame.Size() / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 16; i++)
            {
                SB.Draw(tex, drawPos + ToRadians(360f / 16 * i).ToRotationVector2() * 2f, frame, Color.White.ToAddColor(), Projectile.rotation+ PiOver4, origin, Projectile.scale, 0, 0);
            }
            Color drawColor = AttackState == State.Back ? Color.White : Color.Lerp(Color.White, Color.Black, Helper.GetAniProgress(0));
            SB.Draw(tex, drawPos, frame, drawColor, Projectile.rotation + PiOver4, origin, Projectile.scale, 0, 0);

            return false;
        }
    }
}
