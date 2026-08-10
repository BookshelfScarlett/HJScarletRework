using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(terraRecipe_EatenFoodList), terraRecipe_EatenFoodList);
            tag.Add(nameof(terraRecipe_NotEatenFoodList), terraRecipe_NotEatenFoodList);
            tag.Add(nameof(terraRecipe), terraRecipe);
            tag.Add(nameof(terraRecipe_EatenFoodCounts), terraRecipe_EatenFoodCounts);
            tag.Add(nameof(terraRecipe_LifeMaxMultTime), terraRecipe_LifeMaxMultTime);
            tag.Add(nameof(givePaper), givePaper);
            tag.Add(nameof(firstTimeCraftGaia), firstTimeCraftGaia);
            tag.Add(nameof(ruShiWoWenBanMinionNameTrashList), ruShiWoWenBanMinionNameTrashList);
            tag.Add(nameof(ruShiWoWenBanMinionNameList), ruShiWoWenBanMinionNameList);
        }
        public override void LoadData(TagCompound tag)
        {
            terraRecipe_EatenFoodList = (List<int>)tag.GetList<int>(nameof(terraRecipe_EatenFoodList));
            terraRecipe_NotEatenFoodList = (List<int>)tag.GetList<int>(nameof(terraRecipe_NotEatenFoodList));
            ruShiWoWenBanMinionNameList = (List<string>)tag.GetList<string>(nameof(ruShiWoWenBanMinionNameList));
            ruShiWoWenBanMinionNameTrashList = (List<string>)tag.GetList<string>(nameof(ruShiWoWenBanMinionNameTrashList));
            terraRecipe = tag.GetBool(nameof(terraRecipe));
            terraRecipe_EatenFoodCounts = tag.GetInt(nameof(terraRecipe_EatenFoodCounts));
            terraRecipe_LifeMaxMultTime = tag.GetInt(nameof(terraRecipe_LifeMaxMultTime));
            givePaper = tag.GetBool(nameof(givePaper));
            firstTimeCraftGaia = tag.GetBool(nameof(firstTimeCraftGaia));
        }
    }
}
