using ContinentOfJourney;
using ContinentOfJourney.Projectiles;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace HJScarletRework.Projs.Melee
{
    public class EvolutionThrownProj : ThrownSpearProjClass
    {
        public override string Texture => GetInstance<Evolution>().Texture;
        public ref float Timer => ref Projectile.ai[0];
        public List<int> PortalProjList = [];
        public override void ExSSD() => Projectile.ToTrailSetting(20, 2);
        public override void ExSD()
        {
            Projectile.extraUpdates = 5;
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = 7;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void AI()
        {
            if (!Projectile.HJScarlet().FirstFrame)
            {
                Projectile.originalDamage = Projectile.damage;
                if (HJScarletMethods.HasFuckingCalamity)
                {
                    Projectile.penetrate = -1;
                    Projectile.stopsDealingDamageAfterPenetrateHits = false;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (!HJScarletMethods.OutOffScreen(Projectile.Center))
                SpawnLeafs();
            Timer++;
            //在这个过程中生成需要的材质球
            if (Timer % 15 == 0)
            {
                Vector2 portalVel = Projectile.SafeDir() * Main.rand.NextFloat(10f, 14f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, portalVel, ProjectileType<EvolutionEnergyPortal>(), 0, Projectile.knockBack, Owner.whoAmI);
                proj.originalDamage = (int)(Projectile.originalDamage * 0.75f);
                if (proj.active && proj != null)
                {
                    //存储索引而非射弹本身，避免一些编译器误判问题
                    PortalProjList.Add(proj.whoAmI);
                }
            }
            //距离过远强制处死
            if (Projectile.TooAwayFromOwner())
                Projectile.Kill();
        }

        private void SpawnLeafs()
        {
            Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(10, 10);
            Vector2 vel = Projectile.velocity * 0.8f;
            vel -= Vector2.UnitY * Main.rand.NextFloat(10f, 12f);

            Color drawColor = RandLerpColor(Color.GreenYellow, Color.LightGreen);
            new Petal(spawnPos, vel, drawColor, 60, Main.rand.NextFloat(TwoPi), 1f, 0.1f, 1f).Spawn();
            for (int i = 0; i <= 3; i++)
            {
                Color beginColor = new(77, 42, 26);
                Color endColor = new(97, 67,64);
                Color treeSkinColor = RandLerpColor(beginColor, endColor);
                Vector2 offset = Projectile.SafeDir() * i * 5f + Main.rand.NextVector2Circular(10f, 10f);
                new StarShape(Projectile.Center - offset, Projectile.SafeDir() * -1.2f, treeSkinColor, 0.3f, 60).SpawnToPriorityNonPreMult();
            }
        }

        public override bool PreKill(int timeLeft)
        {
            //先手确认当前啊列表是否合法
            if (PortalProjList.Count > 0)
            {
                //合法的情况下，遍历这个表单内所有的射弹（的索引）
                for (int i = 0; i < PortalProjList.Count; i++)
                {
                    Projectile proj = Main.projectile[PortalProjList[i]];
                    //Prekill之前更新一遍target索引
                    proj.HJScarlet().GlobalTargetIndex = Projectile.HJScarlet().GlobalTargetIndex;
                    //激活传送门发射箭矢
                    ((EvolutionEnergyPortal)proj.ModProjectile).SpawnEvolutionArrow = true;
                }
                //在玩家处生成启动音效
                SoundStyle select = HJScarletSounds.Hammer_Shoot[1];
                SoundEngine.PlaySound(select with { Pitch = 0.8f, MaxInstances = 0, Volume = 0.3f }, Owner.MountedCenter);
            }
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //将命中的第一个可用单位存储起来
            if (Projectile.HJScarlet().GlobalTargetIndex == -1 && target.CanBeChasedBy() && target.lifeMax > 5)
                Projectile.HJScarlet().GlobalTargetIndex = target.whoAmI;
            //而后，我们再考虑命中时生成的特效
            
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            //你在想什么，你以为我会给他上顶点绘制吗？
            //错啦，我要用预制素材的
            //谁tm会在这里用顶点绘制啊
            for (int i = 0; i < length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float rads = (float)i / length;
                Color drawColor = (Color.Lerp(Color.Green, Color.ForestGreen, rads) with { A = 0 }) * 0.9f * Clamp(Projectile.velocity.Length(), 0, 1) * (1 - rads);
                SB.Draw(star, Projectile.Center -Main.screenPosition + Projectile.SafeDir() * 60f - Projectile.velocity * 0.7f * i, null, drawColor * Clamp(Projectile.velocity.Length(), 0, 1), Projectile.oldRot[i] - PiOver2, star.Size() / 2, Projectile.scale * new Vector2(0.8f, 1.5f), 0, 0);
            }
            Projectile.DrawGlowEdge(Color.Green, rotFix: ToRadians(135));
            Projectile.DrawProj(Color.White, 6, 0.5f, ToRadians(135));
            return false;
        }
    }
}
