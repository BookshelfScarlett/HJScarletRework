using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
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
        private ref int StrikeTimes => ref GetInstance<SpearofDarknessThrown>().StrikeTime;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12,2);
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
            Projectile.velocity *= 0.95f;
            //强制置零速度，并在这生成后续的追踪矛
            if (Projectile.velocity.Length() <= 0.1f)
            {
                Projectile.velocity *= 0f;
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileType<SpearofDarknessShadow>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
                proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
                proj.ai[0] = StrikeTimes;
                proj.rotation = Projectile.rotation;
                Projectile.netUpdate = true;
                //临时刷新一下生命值
                Projectile.Kill();
                StrikeTimes++;
                if (StrikeTimes > 16)
                    StrikeTimes = 1;
            }
        }

        private void DoAttack()
        {
            //对，这个函数只做了这些
            Projectile.rotation = Projectile.velocity.ToRotation();
            new ShinyOrbParticle(Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 1.2f, Color.DarkMagenta, 40, 0.4f).Spawn();
        }

        private void SpawnDarkParticle()
        {
            //根据速度情况减少下方的粒子生成
            if (Projectile.velocity.Length() < Main.rand.NextFloat(5f))
                return;
            //火焰
            Color Firecolor2 = Color.Lerp(Color.Purple, Color.DarkViolet, Main.rand.NextFloat(0, 1));
            Vector2 fireOffset = Projectile.rotation.ToRotationVector2() * 20f + Main.rand.NextVector2Circular(4f, 4f);
            new Fire(Projectile.Center - fireOffset, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(0.2f, 1.2f) * 1.2f, Firecolor2, 30, Main.rand.NextFloat(TwoPi), 1, 0.1f * Projectile.Opacity).SpawnToPriorityNonPreMult();

            //挥发性粒子
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(11, 11);
            Color Firecolor = Color.Lerp(Color.Black, Color.DarkViolet, Main.rand.NextFloat(0, 1));
            new TurbulenceShinyOrb(spawnPos, 1f, Firecolor, 40, 0.20f * Projectile.Opacity, Main.rand.NextFloat(TwoPi)).SpawnToNonPreMult();
        }
        public override bool PreKill(int timeLeft)
        {
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
            if (AttackType == Styles.Attack)
            {
                AttackType = Styles.Decay;
                Projectile.netUpdate = true; 
            }
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            for (int i = 0; i < length; i++)
            {
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.Purple, Color.DarkOrchid, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                Main.spriteBatch.Draw(star, Projectile.oldPos[i] + Projectile.PosToCenter(), null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.8f, 1.5f), 0, 0);
            }
            Projectile.DrawGlowEdge(Color.DarkMagenta * Projectile.Opacity, drawTime: 16, posMove: 2.5f,rotFix: PiOver4);
            Projectile.DrawProj(Color.White * Projectile.Opacity, rotFix: PiOver4);
            return false;
        }
    }
}
