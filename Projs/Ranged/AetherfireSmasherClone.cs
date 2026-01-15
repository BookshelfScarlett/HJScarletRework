using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static HJScarletRework.Projs.Ranged.AetherfireSmasherName;

namespace HJScarletRework.Projs.Ranged
{
    public class AetherfireSmasherClone : HJScarletFriendlyProj
    {
        public override string Texture => GetInstance<AetherfireSmasherMain>().Texture;
        public override ClassCategory Category => ClassCategory.Ranged;
        private ref float AttackTimer => ref Projectile.ai[0];
        private ref float CanDamageTimer => ref Projectile.ai[1];
        private bool CanSpawnVolcano = false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.width = Projectile.height = 66;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
        }
        public override bool? CanDamage() => CanDamageTimer > 20f;
        public override void AI()
        {
            CanDamageTimer += 1f;
            if (AttackTimer is 0)
            {
                SoundEngine.PlaySound(SoundID.Item4 with { MaxInstances = 0, Pitch = 0.5f }, Owner.Center);
                AttackTimer = 1;
            }
            DrawTrailingDust();
            //冲向鼠标
            //除非你没有鼠标，不然这里肯定会在下方赋予成鼠标位置
            if (AttackTimer == 1f)
            {
                Vector2 tar = Owner.LocalMouseWorld();
                Projectile.HomingTarget(tar, 1800f, 28f, 20f);
                Rectangle mouseHitBox = new((int)tar.X, (int)tar.Y, Projectile.width, Projectile.height);
                if (Projectile.Hitbox.Intersects(mouseHitBox))
                {
                    Projectile.timeLeft = 60 * Projectile.extraUpdates;
                    AttackTimer += 1;
                }
            }
            else
            {
                //假定，冲向后正常搜索到了敌人，则冲向你的敌人
                if (Projectile.GetTargetSafe(out NPC target, Projectile.HJScarlet().GlobalTargetIndex))
                {
                    CanSpawnVolcano = true;
                    Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    Projectile.HomingTarget(target.Center, 600f, 20f, 20f);
                }
                //否则，处死射弹
                else
                {
                    SoundEngine.PlaySound(SoundID.Item4 with { MaxInstances = 0, Pitch = 0.5f }, Owner.Center);
                    Projectile.Center.CirclrDust(12, 1.8f, DustID.GemRuby, 12);
                    CanSpawnVolcano = false;
                    Projectile.Kill();
                }
            }
        }
        public void DrawTrailingDust()
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            //故意不采用循环，因为要稍微处理圆弧状态粒子，但是我技术力不够，先放着了
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 speedValue = direction * 3f;
            Vector2 spawnPosition = Projectile.Center + direction.RotatedBy(PiOver2) * 8f;
            Vector2 realVel = speedValue.RotatedBy(PiOver2);
            ShinyOrbParticle shinyOrbParticle = new ShinyOrbParticle(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f);
            shinyOrbParticle.Spawn();

            spawnPosition = Projectile.Center + direction.RotatedBy(-PiOver2) * 8f;
            realVel = speedValue.RotatedBy(-PiOver2);
            ShinyOrbParticle shinyOrbParticle2 = new ShinyOrbParticle(spawnPosition, realVel, Main.rand.NextBool() ? baseColor : targetColor, 20, 1.2f);
            shinyOrbParticle2.Spawn();
        }
        private void DirectlySpawnEruptionFireBall()
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(-PiOver4 / 4, PiOver4 / 4)) * Main.rand.NextFloat(14f, 18f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir, ProjectileType<Aetherfireball>(), Projectile.damage, Projectile.knockBack);
                proj.timeLeft = 300;
                proj.ai[0] = 12f;
                proj.extraUpdates = 1;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.whoAmI == Projectile.HJScarlet().GlobalTargetIndex && CanSpawnVolcano)
            {
                DirectlySpawnEruptionFireBall();
                Vector2 center = new Vector2(target.Center.X, target.Center.Y + 30f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, Vector2.Zero, ProjectileType<AetherfireVolcano>(), Projectile.damage * 2, Projectile.knockBack);
                proj.ai[1] = target.whoAmI;
                CanSpawnVolcano = false;
                Projectile.Kill();
            }
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.SelectedName())
            {
                case NameType.TrueScarlet:
                    baseColor = Color.Red;
                    targetColor = Color.Crimson;
                    break;
                //查 -- 金
                case NameType.WutivOrChaLost:
                    baseColor = new Color(255, 178, 36);
                    targetColor = Color.Gold;
                    break;
                case NameType.Emma:
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                //锯角 - 紫
                case NameType.SerratAntler:
                    baseColor = Color.Purple;
                    targetColor = Color.DarkViolet;
                    break;
                //Kino - 蓝
                case NameType.SherryOrAnnOrKino:
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                case NameType.Shizuku:
                    baseColor = Color.LightSkyBlue;
                    targetColor = Color.AliceBlue;
                    break;
                //绿
                case NameType.Hanna:
                    baseColor = Color.Green;
                    targetColor = Color.LimeGreen;
                    break;
                default:
                    baseColor = Color.OrangeRed;
                    targetColor = Color.Orange;
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            Color lerpColor = Color.Lerp(baseColor, targetColor, Projectile.velocity.Length() / 26);
            Projectile.DrawGlowEdge(lerpColor);
            Projectile.DrawProj(Color.White, offset: 0.5f);
            return false;
        }
    }
}
