using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.IDSets;
using HJScarletRework.Globals.List;
using HJScarletRework.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class TheSonBuff : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            HJScarletList.ShinyRarityItemDictionary.Add(Type, Globals.Enums.ShinyRarityType.FateGolden);
        }
        public override void ExSD()
        {
            Item.master = true;
            Item.SetUpRarityPrice(ItemRarityID.Blue);
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.HJScarlet().weaponUpgradePostSon = !player.HJScarlet().weaponUpgradePostSon;
        }
        public override void HoldItem(Player player)
        {
            player.HJScarlet().drawUseableItemIcon = Type;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (!item.IsLegal())
                    continue;
                if (ScarletItemIDSets.GrantsBoosterAfterSon[item.type])
                    item.HJScarlet().setTintIcon = true;
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player p = Main.LocalPlayer;
            Color c = p.HJScarlet().weaponUpgradePostSon? Color.LightGreen : Color.Coral;
            int index = tooltips.FindLineIndex("Tooltip0");
            string text = this.GetLocalizationKey("EnableTooltips").ToLangValue().ToFormatValue(p.HJScarlet().weaponUpgradePostSon.ToString());
            var executionLine = new TooltipLine(Mod, "EnableTooltipsName", text)
            {
                OverrideColor = c
            };
            tooltips.Insert(index, executionLine);
        }

        public override bool ConsumeItem(Player player) => false;
    }
}
