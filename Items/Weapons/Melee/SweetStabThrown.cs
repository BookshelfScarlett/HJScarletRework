using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
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
        public virtual Color MainTooltipColor { get; }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string localAddress = Mod.GetLocalizationKey($"{LocalizationCategory}.{GetType().Name}");
            string path = $"{localAddress}.Tooltip";
            tooltips.ReplaceAllTooltip(path, MainTooltipColor);
            if (!NotHomewardJourneySpear)
            {
                string keyPath = Mod.GetLocalizationKey($"SwitchWeaponTooltip");
                tooltips.QuickAddTooltipDirect(keyPath.ToLangValue(), Color.Lerp(Color.LawnGreen,Color.LightGreen,0.5f));
            }
            ExModifyTooltips(tooltips, localAddress);
            
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips, string localAddress)
        {

        }
    }
}
