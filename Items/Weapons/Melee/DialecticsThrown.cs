using ContinentOfJourney.Items;
using HJScarletRework.Assets.Registers;
using HJScarletRework.Globals.Classes;
using HJScarletRework.Globals.Methods;
using HJScarletRework.Projs.Melee;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static HJScarletRework.Projs.Melee.DialecticsThrownProj;

namespace HJScarletRework.Items.Weapons.Melee
{
    public class DialecticsThrown : ThrownSpearClass
    {
        public override string Texture => HJScarletItemProj.Item_DialecticsThrown.Path;
        public override void SetStaticDefaults() => Type.ShimmerEach<Dialectics>();
        public int UsePhase = 0;
        public override void ExSD()
        {
            Item.damage = 120;
            Item.rare = ItemRarityID.Red;
            //不需要声音，在shoot里手动控制
            Item.UseSound = null;
            Item.shootSpeed = 15f;
            Item.autoReuse = true;
            Item.useTime = Item.useAnimation = 40;
            Item.shoot = ProjectileType<DialecticsThrownProj>();
        }
        public override Color MainTooltipColor => Color.LightBlue;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var curStyle = UsePhase switch
            {
                0 => WaveStyle.Sin,
                1 => WaveStyle.Square,
                2 => WaveStyle.ParaSquare,
                _ => WaveStyle.Paraline,
            };
            if (curStyle != WaveStyle.Paraline)
                SoundEngine.PlaySound(HJScarletSounds.Dialectics_Throw with { MaxInstances = 0, Pitch = 0.5f, Volume =0.7f });
            else
                SoundEngine.PlaySound(HJScarletSounds.Atom_Strike[0] with { MaxInstances = 1 });
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            ((DialecticsThrownProj)proj.ModProjectile).CurWaveStyle = curStyle;
            UsePhase++;
            if (UsePhase > 3)
                UsePhase = 0;
            return false;
        }
    }
}
