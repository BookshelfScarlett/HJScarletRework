using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class StormSpearSpinProj : HJScarletFriendlyProj
    {
        public override string Texture => ProjPath + nameof(StormSpear);
        public List<Vector2> PosList = [];
        public List<float> RotList = [];
        public override void SetStaticDefaults() => Projectile.ToTrailSetting(16, 2);
        public ref float AniProgress => ref Projectile.localAI[0];
        public Vector2 FirstPos = Vector2.Zero;
        public float OffsetDistance = 40f;
        public override void ExSD()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.noEnchantmentVisuals = true;
        }
        public override void AI()
        {
            if(!Projectile.HJScarlet().FirstFrame)
            {
                FirstPos = Projectile.rotation.ToRotationVector2() * OffsetDistance;
            }
            Projectile.rotation += ToRadians(10f);
            float dist = Vector2.Distance(Projectile.Center, Owner.LocalMouseWorld());
            Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.LocalMouseWorld(), dist > 20f ? 0.2f : 1f);
                //是的孩子们，这里还是他妈的硬编码
            StoredVector2PosData();
        }

        private void StoredVector2PosData()
        {
            AniProgress += 1f;
            if (AniProgress < 1f)
                return;
            //将当前需要的位置信息存储进去
            Vector2 nextPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * OffsetDistance;
            PosList.Add(nextPos);
            RotList.Add(Projectile.rotation);
            //如果超过了16个需要绘制的点，删除第一个点
            if (PosList.Count > 16)
            {
                PosList.RemoveAt(0);
                RotList.RemoveAt(0);
            }
            //重置进程
            AniProgress = 0;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetProjDrawData(out Texture2D projTex, out Vector2 drawPos, out Vector2 ori);
            //自转，不用考虑转角修正
            Projectile.DrawGlowEdge(Color.White);
            Projectile.DrawProj(Color.White, 8, 0.7f, useOldPos: true);
            //在这里尝试绘制需要的刀光。
            //也？
            return false;

        }

        private void DrawNeedBladeGlow()
        {
        }
    }
}
