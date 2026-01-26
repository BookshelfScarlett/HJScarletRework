using ContinentOfJourney.Projectiles;
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
        public List<NPC> LegalTargetList = [];
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
            //启用这个Timer，我们开始准备让仆从发起需要的攻击。
            AttackAI();
        }

        private void AttackAI()
        {
            Timer++;
            //5秒一次低吼射弹
            if(Timer >300)
            {
                //先固定Timer
                //如果当前没有这个射弹，我们开始尝试发射这个射弹。嗯。
                if(!Owner.HasProj<DeepToneShockwave>(out int projID))
                {
                    //朝向玩家位置发射。
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Owner.ToMouseVector2() * 12f, projID, Projectile.originalDamage, Projectile.knockBack, Owner.whoAmI);
                    ((DeepToneShockwave)proj.ModProjectile).MoutedProjIndex = Projectile.whoAmI;
                }
                //我们在这里等160帧，然后我们再遍历表单查看可用目标
                if (Timer < 300 + 160)
                    return;
                //如果附近没有目标，立刻重置Timer，并返回
                if(LegalTargetList.Count == 0)
                {
                    Timer = 0;
                    return;
                }
                //否则。遍历这个表单并查看合格单位
                for (int i =0;i<LegalTargetList.Count;i++)
                {
                    NPC target = LegalTargetList[i];
                    if(target != null && target.active && target.CanBeChasedBy())
                    {
                        //触手。
                        SpawnTenctacle_Portal(target);
                    }
                }
                //遍历结束之后我们立刻清空这个表单，并重置Timer
                LegalTargetList.Clear();
                Timer = 0;

                
            }
        }
        private void SpawnTenctacle_Portal(NPC target)
        {
            for (int j = 0; j < 3; j++)
            {
                //随机生成的位置方向
                Vector2 realspawnPos = target.Center + Vector2.UnitY.RotatedBy(Main.rand.NextFloat(PiOver4) * Main.rand.NextBool().ToDirectionInt()) * -30f;
                //什么玩意，为啥咋生成出来的位置都是一样的
                Vector2 dir = (realspawnPos - target.Center).SafeNormalize(Vector2.UnitX);
                for (int k = 0; k < 3; k++)
                {
                    //让开始的方向有一定的偏差
                    Vector2 randDir = dir.RotatedBy(Main.rand.NextFloat(ToRadians(10f) * Main.rand.NextBool().ToDirectionInt()));
                    float tentacleYDirection = Main.rand.Next(10, 120) * 0.001f;
                    if (Main.rand.NextBool())
                    {
                        tentacleYDirection *= -1f;
                    }
                    float tentacleXDirection = Main.rand.Next(10, 120) * 0.001f;
                    if (Main.rand.NextBool())
                    {
                        tentacleXDirection *= -1f;
                    }
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), realspawnPos, randDir * Main.rand.NextFloat(12f, 18f), ProjectileType<DeepToneTenctacle>(), Projectile.damage, 0f, Owner.whoAmI, tentacleXDirection, tentacleYDirection);
                    proj.HJScarlet().GlobalTargetIndex = target.whoAmI;
                    ((DeepToneTenctacle)proj.ModProjectile).InitPos = realspawnPos;
                }
            }
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
