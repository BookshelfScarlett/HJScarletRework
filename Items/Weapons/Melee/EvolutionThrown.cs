using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using HJScarletRework.Rarity.RarityShiny;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class EvolutionThrown : ThrownSpearClass
    {
        public override string Texture => GetInstance<Evolution>().Texture;
        public override bool HasLegendary => true;
        public override void SetStaticDefaults()
        {
            HJScarletList.MiscRarityDrawDictionary.Add(Type, LivingRarity.DrawRarity);
        }
        public override void ExSD()
        {
            Item.damage = 345;
            Item.useTime = Item.useAnimation = 28;
            Item.UseSound = HJScarletSounds.Evolution_Thrown with { MaxInstances = 0 };
            Item.shootSpeed = 16;
            Item.shoot = ProjectileType<EvolutionThrownProj>();
            Item.SetUpRarityPrice(ItemRarityID.Purple);
        }
        public override Color MainTooltipColor => Color.LightGreen;
    }
}
