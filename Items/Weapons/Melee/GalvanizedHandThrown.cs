using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class GalvanizedHandThrown : ThrownSpearClass
    {
        public Projectile CurrentProj = null;
        public override string Texture => GetInstance<GalvanizedHand>().Texture;
        public override bool HasLegendary => true;
        public override void ExSD()
        {
            Item.damage = 512;
            Item.useTime = Item.useAnimation = 25;
            Item.knockBack = 12f;
            Item.UseSound = HJScarletSounds.GalvanizedHand_Toss with { MaxInstances = 1 };
            Item.shootSpeed = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<GalvanizedHandThrownProj>();
            Item.rare = RarityType<TimeRarity>();
        }
        public override Color MainTooltipColor => Color.Yellow;
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                TimeRarity.DrawRarity(line);
                return false;
            }
            return base.PreDrawTooltipLine(line, ref yOffset);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.HasProj(Item.shoot))
            {
                //如果CurrentProj并非为null，转效果
                if (CurrentProj != null)
                {
                    GalvanizedHandThrownProj proj = ((GalvanizedHandThrownProj)CurrentProj.ModProjectile);
                    if (proj.AttackType == GalvanizedHandThrownProj.Style.Stab)
                    {
                        Item.useStyle = ItemUseStyleID.Shoot;
                        Item.useTime = Item.useAnimation = 15;
                        return true;
                    }
                }
                return true;
            }
            else
            {
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useTime = Item.useAnimation = 25;
                return true;
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float spawnPosX = player.LocalMouseWorld().X + (player.MountedCenter.X - player.LocalMouseWorld().X) * 1.25f;
            float spawnPosY = player.MountedCenter.Y - 1000f - Main.rand.NextFloat(120f);
            Vector2 spawnPos = new Vector2(spawnPosX, spawnPosY);
            Vector2 dir = (spawnPos - player.LocalMouseWorld()).ToSafeNormalize();
            Vector2 dirToVel = dir * -28f;

            if (player.HasProj(type))
            {
                ShootSideProj(player, source, spawnPos, dirToVel, ProjectileType<GalvanizedHandSideProj>(), damage, knockback);
            }
            else
                CurrentProj = Projectile.NewProjectileDirect(source, spawnPos, dirToVel, type, damage * 2, knockback, player.whoAmI);
            return false;
        }
        public void ShootSideProj(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //遍历玩家当前拥有的射弹，搜这把武器
            //这里的遍历是有必要的，因为使用全局的CurrentProj并不能确保获取Target的安全性
            NPC target = null;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.type != Item.shoot || proj.owner != player.whoAmI)
                    continue;
                //搜寻成功后保存当前射弹，然后干掉这个循环避免一直吃玩家的性能。
                GalvanizedHandThrownProj setProj = ((GalvanizedHandThrownProj)proj.ModProjectile);
                //确认射弹是否处于挂载状态，如果是，将当前挂载敌人的索引取出来
                //这里最好检查一下这个射弹的存活，如果没存活的情况下立刻重置为null

                if (setProj.AttackType == GalvanizedHandThrownProj.Style.Stab)
                {
                    target = Main.npc[proj.HJScarlet().GlobalTargetIndex];
                    if (target != null && target.CanBeChasedBy())
                    {
                        position = new Vector2(position.X + Main.rand.NextFloat(-300f, 300f), position.Y);
                        velocity = HJScarletMethods.PredictAimToTarget(position, target.Center, target.velocity, 28f, 0);
                    }
                }
            }
            //实际发射射弹。
            Projectile secProj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (target != null)
                secProj.HJScarlet().GlobalTargetIndex = target.whoAmI;
        }
    }
}
