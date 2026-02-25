using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Ranged
{
    public class BinaryStars : ThrownHammerItem
    {
        public override int ShootProjID => ProjectileType<BinaryStarsMain>();
        public override int NeedFocusStrikeTime => 30;
        public override void ExSSD()
        {
        }
        public override void ExSD()
        {
            Item.width = Item.height = 86;
            Item.damage = 321;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 20f;
            Item.rare = ItemRarityID.Red;
            Item.consumable = false;
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            //为锤子添加描边，并时刻更新大小
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + ToRadians(i * 60f).ToRotationVector2() * 2.4f, null, Color.White with { A = 0 }, 0f, tex.Size() / 2, scale, 0, 0f);
            //然后绘制锤子本身。
            spriteBatch.Draw(tex, position, iFrame, Color.White, 0f, tex.Size() / 2, scale, 0f, 0f);
            return false;
        }
        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            return base.PreDrawTooltipLine(line, ref yOffset);
        }
        public override bool AltFunctionUse(Player player)
        {
            return false;
            //bool canAltFunction = !player.UCA().CanDisableGuideForGrandHammer && DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas;
            //return true;
            //return canAltFunction;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //正常情况下， 他应该只会执行一次…… 
            /*
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, new Vector2(0f, -28f), ProjectileType<DivineHammerFlyingUpProj>(), 0, 0f,player.whoAmI);
                return false;
            }
            */
            bool focusStrike = player.HJScarlet().FocusStrikeTime > NeedFocusStrikeTime;
            Projectile st = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            st.rotation = (Main.MouseWorld - player.MountedCenter).ToRotation();
            st.HJScarlet().UseFocusStrikeMechanic = true;
            if (focusStrike)
            {
                st.HJScarlet().FocusStrike = true;
                player.HJScarlet().FocusStrikeTime = 0;
            }
            return false;
        }
        public override bool ConsumeItem(Player player)
        {
            return false;
        }
    }

    public abstract class ThrownHammerItem : HJScarletWeapon
    {
        public virtual int NeedFocusStrikeTime { get; }
        public override ClassCategory Category => ClassCategory.Ranged;
        public virtual int ShootProjID { get; }
        public override bool WeaponPrefix() => true;
        public override bool RangedPrefix() => true;
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ExSSD();
        }
        public virtual void ExSSD() { }
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ShootProjID;
            Item.knockBack = 18f;
            ExSD();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
        }
        public virtual float FocusDamageAddictive => 0.5f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool stealth = player.HJScarlet().FocusStrikeTime > NeedFocusStrikeTime;
            //锤子的潜伏固定1.5倍伤害
            damage = (int)(damage * (1 + FocusDamageAddictive * stealth.ToInt()));
            Projectile st = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            st.HJScarlet().UseFocusStrikeMechanic = true;
            if (stealth)
            {
                st.HJScarlet().FocusStrike = true;
                player.HJScarlet().FocusStrikeTime = 0;
            }
            return false;
        }
    }
}
    /*
    public class GodsHammerShimmerIL : ModSystem
    {
        public override void OnModLoad()
        {
            On_ShimmerTransforms.IsItemTransformLocked += ShimmerRequirementHandler;
        }stealthStrike 
        public static bool ShimmerRequirementHandler(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
        {
            if (type == ModContent.ItemType<NightmareHammer>())
                return !DownedBossSystem.downedDoG;
            return orig(type);
        }
    }
    */
