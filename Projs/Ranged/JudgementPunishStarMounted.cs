using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace HJScarletRework.Projs.Ranged
{
    public class JudgementPunishStarMounted : HJScarletFriendlyProj
    {
        public override string Texture => HJScarletTexture.InvisAsset.Path;
        public override ClassCategory UseDamage => ClassCategory.Ranged;
        private ref float MountedX => ref Projectile.localAI[0];
        private ref float MountedY => ref Projectile.localAI[1];
        public ref float AttackTimer => ref Projectile.HJScarlet().ExtraAI[1];
        private bool CanSpawnStar
        {
            get => Projectile.HJScarlet().ExtraAI[0] == 1f;
            set => Projectile.HJScarlet().ExtraAI[0] = value ? 1f : 0f;
        }
        private ref float AniProgress => ref Projectile.ai[0];
        private ref float MountedIndex => ref Projectile.ai[2];
        private bool ShouldGrowUp = true;
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.knockBack = 0;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile mountedProj = Main.projectile[(int)MountedIndex];
            //玩家是否死亡与挂载射弹是否存在
            if (!Owner.dead && mountedProj.active)
                Projectile.timeLeft = 2;
            //时刻更新射弹位置为原本挂载锤子的中心点
            //没了。
            Projectile.Center = new Vector2(MountedX, MountedY);
            Projectile.rotation += ToRadians(1.2f);
            //如果挂载弹挂载的锤子没有搜索到任何敌人，不要让其生成星星
            if (CanSpawnStar)
                UpdateAnimation();
        }
        private void UpdateAnimation()
        {
            //总控圣光新星挂载射弹的放缩
            if (ShouldGrowUp)
            {
                float t = AttackTimer / 30f;
                float progress = EaseInOutExpo(t);
                AttackTimer += 1f;
                AniProgress = Clamp(progress, 0f, 1f);
                if (t >= 1f)
                {
                    AttackTimer = 10f;
                    AniProgress = 1f;
                    ShouldGrowUp = false;
                    CanSpawnHolyStar();
                }
            }
            else
            {
                float t = AttackTimer / 10f;
                float progress = EaseInOutExpo(t);
                AttackTimer -= 1f;
                AniProgress = Clamp(progress, 0f, 1f);
                if (t <= 0f)
                {
                    AttackTimer = 0f;
                    AniProgress = 0f;
                    ShouldGrowUp = true;
                }
            }
        }
        private void CanSpawnHolyStar()
        {
            Projectile.Center.CirclrDust(36, Main.rand.NextFloat(1.2f, 1.8f), DustID.HallowedWeapons, 4, 16f);
            SoundEngine.PlaySound(SoundID.Item82 with { Pitch = 0.8f }, Projectile.Center);
            const int TotalProjCounts = 4;
            float beginAngle = Projectile.rotation;
            for (int i = 0; i < TotalProjCounts; i++)
            {
                float totalOffset = i * TwoPi / TotalProjCounts;
                Vector2 dir = Vector2.UnitX.RotatedBy(beginAngle + totalOffset);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * 8f, ProjectileType<JudgementPunishStar>(), (int)(Projectile.damage * 0.80f), Projectile.knockBack, Projectile.owner);
                //标记ai2为1f，让这个射弹在发起追踪前进行一段圆弧运动
                proj.ai[2] = 1f;
            }
            Projectile.netUpdate = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //无法生成神圣新星的话，不绘制下方的所有东西
            if (!CanSpawnStar)
                return false;

            //这里的放缩会被lerp进行一次总控。
            Vector2 dynamicBackgroundScale = Vector2.Lerp(Vector2.Zero, new Vector2(1.0f,1.0f), AniProgress) * Projectile.scale * 1.1f;
            Vector2 dynamicBloomScale = Vector2.Lerp(Vector2.Zero, new Vector2(0.5f,0.5f), AniProgress) * Projectile.scale * 1.1f;
            Texture2D tex = HJScarletTexture.Particle_HRStar.Value;
            Vector2 ori = tex.Size() / 2;
            //最后我们实际绘制他。
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Gold, Projectile.rotation, ori, dynamicBackgroundScale, SpriteEffects.None, 0.1f);
            SB.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, ori, dynamicBloomScale, SpriteEffects.None, 0.1f);

            SB.End();
            SB.BeginDefault();
            return false;
        }
    }
}