using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor.Firearm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace HJScarletRework.Projs.Executor
{
    public class ConferenceCallBullet : HJScarletProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override EnumDamageClass Category => EnumDamageClass.Executor;
        public ref float Timer => ref Projectile.ai[0];
        public int SplitTime
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = (float)value;
        }
        public bool SetSplitBullet
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value ? 1 : 0;
        }
        public int BounceTime = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(19);
        }
        public override void ExSD()
        {
            Projectile.extraUpdates = 4;
            Projectile.penetrate = 1;
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300 * 3;
            Projectile.SetupImmnuity(30);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if ((SetSplitBullet || Projectile.HJScarlet().ExecutionStrike)&& BounceTime < 3)
            {
                Projectile.BounceOnTile(oldVelocity);
                SetGeneralParticle();
                BounceTime++;
                return false;
            }
            else
                return base.OnTileCollide(oldVelocity);
        }
        public void SetGeneralParticle()
        {
            Vector2 pos = Projectile.oldPosition + Projectile.Size / 2;
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.ShinyCrossStarECS(pos, RandVelTwoPi(1.2f, 2.2f), Color.LightGoldenrodYellow, 40, 1, 0.4f);
            }
            for (int i = 0; i < 6; i++)
            {
                ECSParticle.SmokeParticle(pos, RandVelTwoPi(1.2f, 3.2f), RandLerpColor(Color.LightGoldenrodYellow, Color.Gold), 40, 1, 1, 0.21f, blendstate: BlendState.Additive);
            }
            ECSParticle.StarShape(pos, Projectile.oldVelocity.ToSafeNormalize() * .01f, Color.LightGoldenrodYellow, 40, 1, 0.94f);
            ECSParticle.StarShape(pos, Projectile.oldVelocity.RotatedBy(PiOver2).ToSafeNormalize() * .01f, Color.LightGoldenrodYellow, 40, 1, 0.94f);
        }
        public override void OnKill(int timeLeft)
        {
            SetGeneralParticle();
        }
        public override void OnFirstFrame()
        {
            if (Projectile.HJScarlet().ExecutionStrike)
                Projectile.penetrate = 2;
        }
        public override void ProjAI()
        {
           if (Timer % (8 * Projectile.MaxUpdates) == 0 && !SetSplitBullet && Timer !=0 && SplitTime < 4 && Projectile.IsMe())
            {
                ScarletSound(HJScarletSounds.Misc_Ding, Projectile.Center, 0.4f);
                SplitTime++;
                SetGeneralParticle();
                QuickSpawnSplitBullet();
            }
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(4), DustID.GoldCoin);
                d.velocity = Projectile.velocity / 4f;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(.75f, 1.15f);
            }
            if(Projectile.HJScarlet().ExecutionStrike && Main.rand.NextBool(9))
            {
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 4f, Color.LightGoldenrodYellow, 30, 1, 0.44f, 4);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(Owner.HJScarlet().conferenceCallBuffTime<=0)
            Projectile.AddExecutionTimeImmediate(ItemType<ConferenceCall>());
            if(Projectile.penetrate !=1)
            SetGeneralParticle();
            if (!SetSplitBullet)
            {
                QuickSpawnSplitBullet();
            }
        }
        public void QuickSpawnSplitBullet()
        {
            for (int i = -1; i < 2; i += 2)
            {
                Vector2 dir = Projectile.SafeDir().RotatedBy(PiOver2 * i);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * Projectile.velocity.Length(), Type, Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                proj.ai[1] = 1;
                proj.timeLeft = 150;
                proj.HJScarlet().ExecutionStrike = Projectile.HJScarlet().ExecutionStrike;
            }
        }

        public override bool? CanDamage()
        {
            return (!SetSplitBullet) || (SetSplitBullet && Timer > 2f * Projectile.MaxUpdates);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = HJScarletTexture.Particle_SharpTear;
            Rectangle frame = tex.Frame();
            Vector2 ori = tex.Size() / 2;
            SB.EnterShaderArea();
            //绘制残影
            float oriScale = .8f;
            float scale = 0.91f;
            int length = (int)(8);
            for (int i = 0; i < length; i++)
            {
                scale *= 0.965f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.Gold, Color.LightGoldenrodYellow, (1 - rads)).ToAddColor(255) * Clamp(Projectile.velocity.Length(), 0f, 1f);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.20f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) + PiOver2;
                SB.Draw(tex, lerpPos + Projectile.PosToCenter(), null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            Vector2 pos = Projectile.Center - Main.screenPosition;
            SB.Draw(tex, pos, null, Color.LightGoldenrodYellow, Projectile.rotation + PiOver2, ori, oriScale, 0, 0);
            SB.EndShaderArea();

            return false;
        }
    }
}
