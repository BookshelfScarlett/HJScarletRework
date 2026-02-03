using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Core.Configs;
using HJScarletRework.Globals.Enums;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class SweetStabThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<SweetSweetStab>().Texture;
        public override void SetStaticDefaults() => Type.ShimmerEach<SweetSweetStab>();
        public override void ExSD()
        {
            Item.damage = 26;
            Item.rare = ItemRarityID.Green;
            Item.useTime = Item.useAnimation = 40;
            Item.knockBack = 8;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileType<SweetStabThrownProj>();
            Item.UseSound = SoundID.Grass;
        }
        public override Color MainTooltipColor => Color.Orange;

    }
    public abstract class ThrownSpearClass : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.Melee";
        public override bool MeleePrefix() => true;
        public override bool AllowPrefix(int pre) => true;
        public static AssetCategory TexturePath => AssetCategory.Weapon;
        public override string Texture => $"HJScarletRework/Assets/Texture/Items/Weapons/{GetType().Name}";
        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 12f;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            ExSD();
        }
        public virtual void ExSD() { }
        private int GhostTimer = 0;
        private int GhostFrame = 0;
        public override void UpdateInventory(Player player)
        {
            //在UpdateInventory内更新帧图的绘制，因为tooltip的draw实际上只会执行一次
            GhostTimer++;
            if (GhostTimer > 5)
            {
                GhostFrame++;
                GhostTimer = 0;
            }
            if (GhostFrame >= 16)
                GhostFrame = 1;
            ExUpdateInventory(player);
        }
        public virtual Color MainTooltipColor { get; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string localAddress = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}");
            string path = $"{localAddress}.Tooltip";
            tooltips.ReplaceAllTooltip(path, MainTooltipColor);

            if (HJScarletMethods.HasFuckingCalamity)
            {
                string calamityPath = $"{localAddress}.CalamitySupport";
                tooltips.QuickAddTooltipDirect(calamityPath.ToLangValue(), new(220, 20, 6));
            }
            ExModifyTooltips(tooltips, localAddress);
            
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips, string localAddress)
        {

        }
        public virtual void ExUpdateInventory(Player player) { }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (ExPreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale))
                return false;
            Vector2 iconPosition = position + new Vector2(8f, 8f);
            float iconScale = 0.35f;
            Rectangle rect = new(0, GhostFrame * 44, 46, 42);
            Vector2 recorigin = new(23, 21);
            spriteBatch.Draw(HJScarletTexture.ScarletGhost.Value, iconPosition, rect, Color.White, 0f, recorigin, iconScale, SpriteEffects.None, 0f);
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public virtual bool ExPreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => false;
        public virtual void AddCustomRecipe() { }
        public override void AddRecipes()
        {
            if (HJScarletConfigServer.Instance.EnableSameItemShimmer)
                return;
            AddCustomRecipe();
        }
    }
}
