using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace HJScarletRework.Globals.Classes
{
        public abstract class HJScarletShinyRarity: ModRarity
        {
            public abstract void DrawItemName(DrawableTooltipLine line);
            public virtual void DrawFlavorTooltip(DrawableTooltipLine line) { }
            public virtual void DrawMisc(DrawableTooltipLine line) { }
        }
    }
