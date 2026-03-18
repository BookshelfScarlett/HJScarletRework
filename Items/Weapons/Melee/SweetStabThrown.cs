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
        public override void ExSD()
        {
            Item.damage = 36;
            Item.rare = ItemRarityID.Green;
            Item.useTime = Item.useAnimation = 40;
            Item.knockBack = 6f;
            Item.shootSpeed = 14f;
            Item.shoot = ProjectileType<SweetStabThrownProj>();
            Item.UseSound = SoundID.Grass;
        }
        public override Color MainTooltipColor => Color.Orange;

    }
    public abstract class ThrownSpearClass : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.Melee";
        public virtual bool HasLegendary => false;
        public override bool MeleePrefix() => HasLegendary;
        public override bool AllowPrefix(int pre) => true;
        public static AssetCategory TexturePath => AssetCategory.Weapon;
        public virtual bool NotHomewardJourneySpear => false;
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
            Item.value = Item.buyPrice(gold: 15);
            Item.HJScarlet().CanDrawIcon = true;
            ExSD();
        }
        public virtual void ExSD() { }
        private int GhostTimer = 0;
        private int GhostFrame = 0;
        public override void UpdateInventory(Player player)
        {
            ExUpdateInventory(player);
        }
        public virtual Color MainTooltipColor { get; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string localAddress = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}");
            string path = $"{localAddress}.Tooltip";
            tooltips.ReplaceAllTooltip(path, MainTooltipColor);
            if (!NotHomewardJourneySpear)
            {
                string keyPath = Mod.GetLocalizationKey($"SwitchWeaponTooltip");
                tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), Color.Lime);
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
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
        public virtual bool ExPreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) => false;
    }
}
