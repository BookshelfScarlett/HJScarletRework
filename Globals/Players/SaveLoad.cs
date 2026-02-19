using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace HJScarletRework.Globals.Players
{
    public partial class HJScarletPlayer : ModPlayer
    {
        public override void SaveData(TagCompound tag)
        {
            HammerTagSave(tag);
        }
        public override void LoadData(TagCompound tag)
        {
            HammerTagLoad(tag);
        }
        private void HammerTagSave(TagCompound tag)
        {
            tag.Add(nameof(NoGuideForBinaryStars), NoGuideForBinaryStars);
            tag.Add(nameof(CanDisableGuideForGrandHammer), CanDisableGuideForGrandHammer);
            tag.Add(nameof(CanGiveFreeBinaryStars), CanGiveFreeBinaryStars);
        }

        private void HammerTagLoad(TagCompound tag)
        {
            NoGuideForBinaryStars = tag.GetBool(nameof(NoGuideForBinaryStars));
            CanDisableGuideForGrandHammer = tag.GetBool(nameof(CanDisableGuideForGrandHammer));
            CanGiveFreeBinaryStars = tag.GetBool(nameof(CanGiveFreeBinaryStars));
        }

    }
}
