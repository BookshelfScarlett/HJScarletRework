using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.ParticleECS;
using HJScarletRework.Core.ScreenEffect;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Graphics.Metaballs;
using HJScarletRework.Globals.Graphics.Particles;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Executor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;

namespace HJScarletRework.Projs.Executor
{
    public class GaiaStrikerProj : HJScarletProj
    {
        public override ClassCategory Category => ClassCategory.Executor;
        public override string Texture => GetInstance<GaiaStriker>().Texture;
        public enum State
        {
            Shoot,
            Buffer,
            Return
        }
        public ref float Timer => ref Projectile.ai[0];
        public float LerpTimer = 0;
        public State AttackState
        {
            get => (State)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public AnimationStruct Helper = new(3);
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.SetupImmnuity(60);
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.scale = 0.78f;
            Projectile.ownerHitCheck = true;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            LerpTimer = Lerp(LerpTimer, 1.05f, 0.05f);
            UpdateAttackAI();
            if (Projectile.IsOutScreen())
                return;
            //一堆随变化发生的数据，巴拉巴拉。
            float bloodDropSpeedMult = Clamp(Lerp(1f, 3.4f, LerpTimer), 1f, 3.4f);
            float bloodLifeTimeMult = Clamp(Lerp(1f, 1.4f, LerpTimer), 1, 1.4f);
            float bloodAngleSpread = Clamp(Lerp(PiOver4, ToRadians(0.5f), EaseOutBack(LerpTimer)), PiOver4, ToRadians(.5f));
            float smokePosSpreadMult = Clamp(Lerp(1f, 0.0f, (EaseOutBack(LerpTimer))), 1, 0f);
            if (LerpTimer < Main.rand.NextFloat(0, 1.1f))
            {
                int DustCount = 2;
                for (int i = 0; i < DustCount; i++)
                {
                    BloodyMetaball.SpawnParticle(Projectile.Center.ToRandCirclePos(22) + Projectile.velocity / DustCount * i,
                        Projectile.rotation.ToRotationVector2(),
                        Main.rand.NextFloat(0.90f, 1.1f) * .61f,
                        Projectile.velocity.ToRotation());
                }
            }
            else
            {
                //即将结束的时候这里会堆叠更多，更多的粒子。
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePosEdge(30);
                    Vector2 vel = (-Vector2.UnitY).ToRandVelocity(PiOver4 * bloodAngleSpread, 0.6f, 6f) * bloodDropSpeedMult;
                    float scale = Main.rand.NextFloat(0.95f, 1.175f) * 0.1f;
                    Color c = RandLerpColor(Color.DarkRed, Color.Black);
                    ECSParticle.BloodDrop(pos, vel, c, Main.rand.Next((int)(60 * bloodLifeTimeMult), (int)(90 * bloodLifeTimeMult)), 1, scale, 1, blendstate: BlendState.AlphaBlend);
                }
                if (Main.rand.NextBool(3))
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(60 * smokePosSpreadMult);
                    float rot = RandRotTwoPi;
                    float scale = Main.rand.NextFloat(.26f, .38f) * 0.615f;
                    Vector2 speed = RandVelTwoPi(0.1f, 2.4f) * smokePosSpreadMult * 1.18f;
                    new Fire(pos, speed, RandLerpColor(Color.Crimson, Color.DarkRed), 40, rot, 0.75f, scale * 1.05f).SpawnToPriorityNonPreMult();
                }
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(60 * smokePosSpreadMult);
                float rot = RandRotTwoPi;
                float scale = Main.rand.NextFloat(.26f, .38f) * 0.615f;
                Vector2 speed = RandVelTwoPi(0.1f, 2.4f) * smokePosSpreadMult * 1.18f;
                new Fire(pos, speed, RandLerpColor(Color.Crimson, Color.DarkRed), 40, rot, 0.75f, scale * 1.05f).SpawnToPriorityNonPreMult();
            }
            if (Main.rand.NextBool(4))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePosEdge(30);
                Vector2 vel = (-Vector2.UnitY).ToRandVelocity(PiOver4 * bloodAngleSpread, 0.6f, 6f) * bloodDropSpeedMult;
                float scale = Main.rand.NextFloat(0.95f, 1.175f) * 0.1f;
                Color c = RandLerpColor(Color.DarkRed, Color.Black);
                ECSParticle.BloodDrop(pos, vel, c, Main.rand.Next(60, 90), 1, scale, 1, blendstate: BlendState.AlphaBlend);
            }
        }
        public void UpdateAttackAI()
        {
            switch (AttackState)
            {
                case State.Shoot:
                    DoShoot();
                    break;
                case State.Buffer:
                    DoBuffer();
                    break;
                case State.Return:
                    DoReturn();
                    break;
            }
        }

        public void DoReturn()
        {
            Projectile.HomingTarget(Owner.Center, -1, 20, 12);
            if (Projectile.IntersectOwnerByDistance(70))
                Projectile.Kill();
        }

        public void DoBuffer()
        {
            Projectile.rotation = Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.12f);
            Projectile.velocity *= .92f;
            Projectile.position += Main.rand.NextVector2Circular(4, 4);
            if (Projectile.FinalUpdate())
                Timer++;
            if (Timer % (15) == 0)
            {
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Charge with { MaxInstances = 1, Pitch = 0.1f * Timer / 15f, Volume = .67f }, Projectile.Center);
                for (int i = 0; i < 36; i++)
                    ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(0), RandVelTwoPi(.7f, 21f), RandLerpColor(Color.Red, Color.DarkRed), 40, 1, 0.25f, Main.rand.NextFloat(0.9f, 1.1f) * 0.40f, Main.rand.NextBool(), BlendState.NonPremultiplied);
            }
            if (Timer > 55 && Projectile.IsMe())
            {
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = RandVelTwoPi(0.9f, 6.4f);
                    BloodyMetaball.SpawnParticle(pos, vel, 0.35f, RandRotTwoPi, true);
                }
                for (int i = 0; i < 36; i++)
                {
                    Vector2 pos = Projectile.Center.ToRandCirclePos(3.6f);
                    Vector2 vel = RandVelTwoPi(0.9f, 9.4f);
                    BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.75f, vel.ToRotation(), false);
                    BloodyMetaball.SpawnParticle(pos, vel * 2.7f, 0.15f, RandRotTwoPi, true);

                }
                int bloodBulletCount = GaiaStriker.BloodBulletCount;
                int healType = Main.rand.Next(0, bloodBulletCount);
                int healType2 = Main.rand.Next(0, bloodBulletCount);
                if (healType == healType2)
                {
                    if (healType2 != (bloodBulletCount -1))
                        healType2 += 1;
                    else
                        healType2 -= 1;
                }
                for (int i = 0; i < bloodBulletCount; i++)
                {

                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(6), RandVelTwoPi(6f, 10f), ProjectileType<GaiaStrikerBloodyBullet>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    if (i != healType && i != healType2)
                        proj.HJScarlet().HasExecutionMechanic = true;
                }
                if (Projectile.HJScarlet().ExecutionStrike && !Owner.HasProj<GaiaStrikerHeldProj>() && !Owner.HasProj<GaiaStrikerMountedProj>())
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center.ToRandCirclePos(6), Vector2.Zero, ProjectileType<GaiaStrikerMountedProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    proj.rotation = Projectile.rotation;
                    //No.
                    ((GaiaStrikerMountedProj)proj.ModProjectile).ShouldCreate = false;
                }
                new CrossGlow(Projectile.Center, Color.Red, 40, 1, 0.30f).Spawn();
                new CrossGlow(Projectile.Center, Color.DarkRed, 40, 1, 0.28f).Spawn();
                ScreenShakeSystem.AddScreenShakes(Projectile.Center, 12, 20, Projectile.rotation, 0.15f, easingFunc: EaseOutBack);
                SoundEngine.PlaySound(HJScarletSounds.Gaia_Explosion with { MaxInstances = 0, Pitch = 0.4f,PitchVariance = 0.1f, Volume = .57f }, Projectile.Center);
                Projectile.Kill();
            }
        }
        public void DoShoot()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.FinalUpdate())
                Projectile.velocity *= .92f;
            Timer++;
            if (Projectile.MeetMaxUpdatesFrame(Timer, 12))
                UpdateNextState(State.Buffer);
        }

        public void UpdateNextState(State id)
        {
            AttackState = id;
            Projectile.netUpdate = true;
            Timer = 0;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //一堆基础数据，巴拉巴拉。
            Texture2D tex = Projectile.GetTexture();
            Vector2 ori = tex.ToOrigin();
            int length = Projectile.oldPos.Length;
            float rotFixer = Projectile.velocity.X > 0 ? PiOver4 : -PiOver4 + Pi;
            SpriteEffects se = Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float lerpRatios = 0f;

            if (AttackState == State.Buffer)
                lerpRatios = Timer / (45f);
            length = (int)(length * lerpRatios);
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 pos = Projectile.oldPos[i] + Projectile.PosToCenter();
                float rot = Projectile.oldRot[i];
                float ratios = (1 - (float)i / length);
                int aValue = (int)(Lerp(180, 255, EaseInCubic(ratios)));
                Color c = Color.Lerp(Color.Red, Color.White, ratios) with { A = (byte)aValue } * ratios;
                SB.Draw(tex, pos, null, c, rot + rotFixer, ori, Projectile.scale, se, 0);
            }
            Color edgeMainColor = Color.Red.ToAddColor();
            Color edgeTargetColor = Color.DarkRed;
            Color edgeDrawColor = Color.Lerp(edgeMainColor, edgeTargetColor, lerpRatios) with { A = (byte)(Lerp(0, 250, lerpRatios)) };
            for (int i = 0; i < 16; i++)
                SB.Draw(tex, Projectile.Center - Main.screenPosition + (TwoPi / 16f * i).ToRotationVector2() * 2f, null, edgeDrawColor, Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            Color mainColor = Color.Lerp(Color.White, Color.DarkRed, 0.14f);
            Color targetColor = Color.Lerp(Color.White, Color.Red, 0.75f);
            Color drawColor = Color.Lerp(mainColor, targetColor, lerpRatios) with { A = (byte)(Lerp(255, 0, lerpRatios)) };
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation + rotFixer, ori, Projectile.scale, se, 0);
            return false;
        }
    }
}
