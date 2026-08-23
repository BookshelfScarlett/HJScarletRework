using HJScarletRework.Globals.Methods;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow.ValueContentAnalysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Players.AccessoriesSlot
{
    public class HJScarletAccessoriesSlot : ModAccessorySlot
    {
        public override string Name => "LightofHorizon";
        public override bool IsEnabled()
        {
            bool isEnable = Main.LocalPlayer.TryGetModPlayer<HJScarletPlayer>(out var value);
            if (isEnable)
                isEnable = value.LightofHorizon;
            return isEnable;
        }
    }
}
