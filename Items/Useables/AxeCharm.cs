using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Handlers;
using HJScarletRework.Globals.Methods;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Items.Useables
{
    public class AxeCharm : HJScarletItemClass
    {
        public override string AssetPath => AssetHandler.Useables;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void ExSD()
        {
            Item.SetUpRarityPrice(ItemRarityID.Green);
            Item.accessory = true;
            Item.maxStack = 1;
        }
        public override bool CanRightClick() => true;
        public override void RightClick(Player player)
        {
            player.HJScarlet().tacticalExecutionManual = !player.HJScarlet().tacticalExecutionManual;
        }
        public override bool ConsumeItem(Player player) => false;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Player p = Main.LocalPlayer;
            Color c = p.HJScarlet().tacticalExecution ? Color.LightGreen : Color.Coral;
            int executionProgressIndex = tooltips.FindIndex(line => line.Name == "Tooltip0" && line.Mod == "Terraria");
            string text = this.GetLocalizationKey("EnableTooltips").ToLangValue().ToFormatValue(p.HJScarlet().tacticalExecution.ToString());
            var executionLine = new TooltipLine(Mod, "EnableTooltipsName", text)
            {
                OverrideColor = c
            };
            tooltips.Insert(executionProgressIndex, executionLine);
        }
    }
}
