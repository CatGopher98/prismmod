using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace prismmod.Items.Consumables
{
    public class NightLite : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night-Lite");
            Tooltip.SetDefault("Changes the time to Night.");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 20;
            item.noMelee = true;
            item.consumable = true;
            item.useTime = 4;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item29;
            item.maxStack = 999;

        }

        public override bool UseItem(Player player)
        {
            if (Main.dayTime == true)
            {
                Main.time = 0;
                Main.dayTime = false;
                return true;
            }
            else {
                return false;
            }
        }
    }
}