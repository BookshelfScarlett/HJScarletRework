using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class SpearofDarknessThrownProj : ThrownSpearProjClass
    {
        public override string Texture => HJScarletItemProj.Proj_SpearofDarkness.Path;
        public int TargetIndex = -1;
        private enum Styles
        {
            Attack,
            Decay
        }
        private Styles AttackType
        {
            get => (Styles)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public ref float Timer => ref Projectile.ai[0];
        private ref int StrikeTimes => ref GetInstance<SpearofDarknessThrown>().StrikeTime;
        private float Speed = 0f;
        private float SpinMoveTime = 30f;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12, 2);
        }
        public override void ExSD()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 240;
        }
        public bool AlreadyHit = false;
        public override void AI()
        {
            SpawnDarkParticle();
            switch (AttackType)
            {
                case Styles.Attack:
                    DoAttack();
                    break;
                case Styles.Decay:
                    DoDecay();
                    break;
            }
        }

        private void DoDecay()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer < SpinMoveTime)
                Projectile.velocity = Projectile.SafeDir() * Speed * (1f - Timer / (SpinMoveTime + 1));
            //强制置零速度，并在这生成后续的追踪矛
            else
            {
                Projectile.velocity *= 0.98f;
                if (Timer < SpinMoveTime + 30f)
                    return;
                //这里的i固定成这样，因为会同时控制三个不同的黑暗矛的位置的。
                if (Projectile.GetTargetSafe(out NPC target,canPassWall:true) && Projectile.Opacity == 1)
                {
                    for (int i = -1; i < 2; i += 1)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<SpearofDarknessShadow>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);

                        proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                        ((SpearofDarknessShadow)proj.ModProjectile).MountedVec = Projectile.SafeDirByRot().RotatedBy(ToRadians(360f / 4f * StrikeTimes));
                        proj.ai[0] = i;
                        proj.rotation = Projectile.rotation;
                    }
                }
                StrikeTimes += 1;
                if (StrikeTimes > 3)
                    StrikeTimes = 0;
                Projectile.Opacity -= 0.1f;
                if (Projectile.Opacity == 0)
                    Projectile.Kill();
            }
        }

        private void DoAttack()
        {
            //对，这个函数只做了这些
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
            if (Timer > 15f)
            {
                AttackType = Styles.Decay;
                Timer = 0f;
                Projectile.netUpdate = true;
                Speed = Projectile.velocity.Length();
            }
        }

        private void SpawnDarkParticle()
        {
            //根据速度情况减少下方的粒子生成
            float generalProgress = AttackType == Styles.Attack ? 1f : (1f - Timer / SpinMoveTime);
            generalProgress = Clamp(generalProgress, 0f, 1f);
            //火焰
            Color Firecolor2 = RandLerpColor(Color.Purple, Color.DarkViolet);
            Vector2 fireOffset = Projectile.rotation.ToRotationVector2() * 20f + Main.rand.NextVector2Circular(4f, 4f);
            new Fire(Projectile.Center - fireOffset, RandVelTwoPi(0.2f, 1.2f) * 1.2f, Firecolor2, 30, RandRotTwoPi, 1, 0.1f * generalProgress).SpawnToPriorityNonPreMult();

            //挥发性粒子
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(11, 11);
            Color Firecolor = RandLerpColor(Color.Black, Color.DarkViolet);
            new TurbulenceGlowOrb(spawnPos, 1f, Firecolor, 40, 0.20f * generalProgress, RandRotTwoPi).SpawnToNonPreMult();
            new ShinyOrbParticle(Projectile.Center.ToRandCirclePos(4f), RandVelTwoPi(0.2f, 1.2f) * 1.2f, RandLerpColor( Color.DarkMagenta, Color.Magenta), 40, 0.4f * generalProgress).Spawn();
        }
        public override bool PreKill(int timeLeft)
        {
            for (int j = 0; j < 15; j++)
            {
                new SmokeParticle(Projectile.Center.ToRandCirclePos(16f) + Projectile.SafeDirByRot() * Main.rand.NextFloat(-60f, 61f), RandVelTwoPi(0.2f, 1.2f), RandLerpColor(Color.DarkMagenta, Color.Black), 40, RandRotTwoPi, 1f, .24f).SpawnToPriorityNonPreMult();
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(16f) + Projectile.SafeDirByRot() * Main.rand.NextFloat(-60f, 61f);
                float rot = RandRotTwoPi;
                new Fire(pos, RandVelTwoPi(2f), RandLerpColor(Color.DarkMagenta, Color.Black), 40, rot, 1f, 0.1f).SpawnToNonPreMult();
            }

            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
            if (AttackType == Styles.Attack)
            {
                AttackType = Styles.Decay;
                Projectile.netUpdate = true;
                Speed = Projectile.velocity.Length();
                Timer = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            float generalProgress = AttackType == Styles.Attack ? 1f : (1f - Timer / SpinMoveTime);
            generalProgress = Clamp(generalProgress, 0f, 1f);
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.Purple, Color.DarkOrchid, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads) * generalProgress * Projectile.Opacity;
                Main.spriteBatch.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter(), null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.8f, 1.5f), 0, 0);
            }
            Projectile.DrawGlowEdge(Color.DarkMagenta * Projectile.Opacity, drawTime: 16, posMove: 2.5f, rotFix: PiOver4);
            Projectile.DrawProj(Color.Lerp( Color.Black, Color.White, generalProgress) * Projectile.Opacity, rotFix: PiOver4);
            return false;
        }
    }
}
