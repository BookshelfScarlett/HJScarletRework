using ContinentOfJourney.Items;
using HJScarletRework.Globals.Methods;
using HJScarletRework.ReVisual.Class;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.ReVisual.Items
{
    public abstract class ReOreKnife : ReVisualItemClass
    {
        public override void ExModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.CreateTooltip(Mod.GetLocalizationKey("SwitchWeapon.AllFix"), Color.LightGray, Mod, "HJScarlet",
                GetValue(nameof(TinKnife)) + GetValue(nameof(CopperKnife)) +
                GetValue(nameof(IronKnife)) + GetValue(nameof(LeadKnife)) +
                GetValue(nameof(TungstenKnife)) + GetValue(nameof(SilverKnife)) +
                GetValue(nameof(GoldKnife)) + GetValue(nameof(PlatinumKnife)));
        }
        private string GetValue(string name)
        {
            return $"[i:ContinentOfJourney/{name}]";
        }
        public sealed override void ExHoldItem(Item item, Player player, ReVisualPlayer vp)
        {
            vp.reVisualOreknife = !vp.reVisualOreknife;
        }
    }
    public class ReTinKnife : ReOreKnife
    {
        public override int ApplyItem => ItemType<TinKnife>();
    }
    public class ReCopperKnife : ReOreKnife
    {
        public override int ApplyItem => ItemType<CopperKnife>();
    }
    public class ReIronKnife:ReOreKnife
    {
        public override int ApplyItem => ItemType<IronKnife>();
    }
    public class ReLeadKnife : ReOreKnife
    {
        public override int ApplyItem => ItemType<LeadKnife>();

    }
    public class ReTungstunKnife : ReOreKnife
    {
        public override int ApplyItem => ItemType<TungstenKnife>();

    }
    public class ReSilverKnife : ReOreKnife
    {
        public override int ApplyItem => ItemType<SilverKnife>();

    }
    public class ReGoldKnife :ReOreKnife
    {
        public override int ApplyItem => ItemType<GoldKnife>();

    }
    public class RePlatinumKnife :ReOreKnife
    {
        public override int ApplyItem => ItemType<PlatinumKnife>();

    }
}
