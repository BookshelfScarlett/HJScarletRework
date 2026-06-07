using ContinentOfJourney.Buffs;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Globals.Players.Dashes;
using HJScarletRework.Globals.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Accessories
{
    [AutoloadEquip(EquipType.Shield)]
    public class SlimeGodShield : HJScarletItemClass
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override string AssetPath => AssetHandler.Equips;
        public override void SetStaticDefaults()
        {
            HJScarletList.SunlightRarityHashSet.Add(Type);
        }
        public override void ExSD()
        {
            Item.accessory = true;
            Item.defense = 10;
            Item.SetUpRarityPrice(ItemRarityID.Red);
            Item.master = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ApplyDash(GetInstance<SlimeGodShieldDash>().Type);
            player.buffImmune[BuffType<IcarusBuff>()] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.Burning] = true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}
