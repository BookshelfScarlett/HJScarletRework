using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace HJScarletRework.Projs.Melee
{
    public class DeepToneMinion : HJScarletFriendlyProj
    {
        public override string Texture => GetInstance<DeepToneThrownProj>().Texture;
        public override ClassCategory Category => ClassCategory.Melee;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting();
        }
        public ref float Timer => ref Projectile.ai[0];
        public ref float Osci => ref Projectile.localAI[0];
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 12;
            Projectile.noEnchantmentVisuals = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 0.8f;
            Projectile.extraUpdates = 0;
        }
        public override void AI()
        {
            //递增的值越大，锤子的摆动幅度越大
            Osci += 0.025f;
            //基本的挂机状态，此处使用了正弦曲线来让锤子常规上下偏移
            Vector2 anchorPos = new Vector2(Owner.MountedCenter.X - Owner.direction*50f, Owner.MountedCenter.Y + 60f * (MathF.Sin(Osci) / 9f)); 
            //实际更新位置
            Projectile.Center = Vector2.Lerp(Projectile.Center, anchorPos, 0.15f);
            //计算锤子需要的朝向。
            //这里会依据玩家是否按下左键来使朝向取反，即按住左键的时候，锤头朝向指针，其他情况下，锤柄朝向玩家
            float angleToWhat = Vector2.UnitY.ToRotation();
            //最后使用lerp来让锤子朝向得到修改。
            Projectile.rotation = Projectile.rotation.AngleLerp(angleToWhat, 0.18f);
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawGlowEdge(Color.DarkMagenta, 6,rotFix:-PiOver4);
            Projectile.DrawProj(Color.White, offset: 0.7f,rotFix:-PiOver4);
            return false;
        }
    }
}
