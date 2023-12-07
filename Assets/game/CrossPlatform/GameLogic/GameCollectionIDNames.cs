using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public partial class Game
	{
		public static Dictionary<CollectionID, string> CollectionIDNames = new Dictionary<CollectionID, string>()
		{
			{ CollectionID.ground_terrain, "models/ground/terrain" },		

			{ CollectionID.item_coin, "models/items/coin" },

			{ CollectionID.tree_01, "models/obstacles/trees/tree01" },
			{ CollectionID.tree_02, "models/obstacles/trees/tree02" },
			{ CollectionID.tree_03, "models/obstacles/trees/tree03" },

			{ CollectionID.rock_01, "models/obstacles/rocks/rocks01" },
			{ CollectionID.rock_02, "models/obstacles/rocks/rocks02" },

			{ CollectionID.character_skier_01, "models/characters/set1/hero001" },
			{ CollectionID.character_skier_02, "models/characters/set1/hero002" },
			{ CollectionID.character_skier_03, "models/characters/set1/hero003" },
			{ CollectionID.character_skier_04, "models/characters/set1/hero004" },
			{ CollectionID.character_skier_05, "models/characters/set1/hero005" },

			{ CollectionID.character_skier_06, "models/characters/set2/hero006" },
			{ CollectionID.character_skier_07, "models/characters/set2/hero007" },
			{ CollectionID.character_skier_08, "models/characters/set2/hero008" },
			{ CollectionID.character_skier_09, "models/characters/set2/hero009" },
			{ CollectionID.character_skier_10, "models/characters/set2/hero010" },

			{ CollectionID.character_skier_11, "models/characters/set3/hero011" },
			{ CollectionID.character_skier_12, "models/characters/set3/hero012" },
			{ CollectionID.character_skier_13, "models/characters/set3/hero013" },
			{ CollectionID.character_skier_14, "models/characters/set3/hero014" },
			{ CollectionID.character_skier_15, "models/characters/set3/hero015" },

			{ CollectionID.character_skier_16, "models/characters/set4/hero016" },
			{ CollectionID.character_skier_17, "models/characters/set4/hero017" },
			{ CollectionID.character_skier_18, "models/characters/set4/hero018" },
			{ CollectionID.character_skier_19, "models/characters/set4/hero019" },
			{ CollectionID.character_skier_20, "models/characters/set4/hero020" },

			{ CollectionID.character_skier_21, "models/characters/set5/hero021" },
			{ CollectionID.character_skier_22, "models/characters/set5/hero022" },
			{ CollectionID.character_skier_23, "models/characters/set5/hero023" },
			{ CollectionID.character_skier_24, "models/characters/set5/hero024" },
			{ CollectionID.character_skier_25, "models/characters/set5/hero025" },

			{ CollectionID.character_skier_26, "models/characters/set6/hero026" },
			{ CollectionID.character_skier_27, "models/characters/set6/hero027" },
			{ CollectionID.character_skier_28, "models/characters/set6/hero028" },
			{ CollectionID.character_skier_29, "models/characters/set6/hero029" },
			{ CollectionID.character_skier_30, "models/characters/set6/hero030" },

			{ CollectionID.character_skier_31, "models/characters/set7/hero031" },
			{ CollectionID.character_skier_32, "models/characters/set7/hero032" },
			{ CollectionID.character_skier_33, "models/characters/set7/hero033" },
			{ CollectionID.character_skier_34, "models/characters/set7/hero034" },

			{ CollectionID.misc_avalanche, "models/obstacles/avalanche/avalanche" },
			{ CollectionID.misc_slotmachine, "models/items/slotmachine" },

			{ CollectionID.particles_snow, "particles/snow" },
			{ CollectionID.particles_hit, "particles/hit" },
			{ CollectionID.particles_avalanche, "particles/avalanche" },
			{ CollectionID.particles_snow_trail, "particles/snow_trail" },
			{ CollectionID.particles_coins, "particles/coins" },
			{ CollectionID.particles_light, "particles/light" },
			{ CollectionID.particles_pickup, "particles/pickup" },
			{ CollectionID.particles_explosion, "particles/explosion" },

			{ CollectionID.sound_gba_gba01, "sound/gba/gba01" },
			{ CollectionID.sound_gba_gba02, "sound/gba/gba02" },
			{ CollectionID.sound_gba_gba03, "sound/gba/gba03" },
			{ CollectionID.sound_gba_gba04, "sound/gba/gba04" },
			{ CollectionID.sound_gba_gba05, "sound/gba/gba05" },
			{ CollectionID.sound_gba_gba06, "sound/gba/gba06" },
			{ CollectionID.sound_gba_gba07, "sound/gba/gba07" },
			{ CollectionID.sound_gba_gba08, "sound/gba/gba08" },
			{ CollectionID.sound_gba_gba09, "sound/gba/gba09" },
			{ CollectionID.sound_gba_gba10, "sound/gba/gba10" },
			{ CollectionID.sound_gba_gba11, "sound/gba/gba11" },
			{ CollectionID.sound_gba_gba12, "sound/gba/gba12" },
			{ CollectionID.sound_gba_gba13, "sound/gba/gba13" },
			{ CollectionID.sound_gba_gba14, "sound/gba/gba14" },
			{ CollectionID.sound_gba_gba15, "sound/gba/gba15" },
			{ CollectionID.sound_gba_gba16, "sound/gba/gba16" },

			{ CollectionID.sound_select, "sound/select" },
			{ CollectionID.sound_click, "sound/click" },
			{ CollectionID.sound_skiing_loop, "sound/skiing-loop" },
			{ CollectionID.sound_ski_turn, "sound/ski-turn" },
			{ CollectionID.sound_skiing, "sound/skiing" },
			{ CollectionID.sound_coin, "sound/coin" },
			{ CollectionID.sound_hurt, "sound/hurt" },
			{ CollectionID.sound_avalanche, "sound/avalanche" },
			{ CollectionID.sound_highscore, "sound/highscore" },
			{ CollectionID.sound_newhero, "sound/newhero" },
			{ CollectionID.sound_hexlogo1, "sound/hexlogo1" },
			{ CollectionID.sound_avalanche2, "sound/avalanche2" },
			{ CollectionID.sound_random, "sound/random" },
			{ CollectionID.sound_slotmachine, "sound/slotmachine" },
			{ CollectionID.sound_slotmachine_start, "sound/slotmachine-start" },

			{ CollectionID.animation_none, "none" },

			{ CollectionID.animation_default, "default" },

			{ CollectionID.animation_idle, "Idle" },
			{ CollectionID.animation_move, "Move" },
			{ CollectionID.animation_fall, "Fall" },
			{ CollectionID.animation_spin, "Spin" },

			{ CollectionID.animation_spin_1, "1" },
			{ CollectionID.animation_spin_2, "2" },
			{ CollectionID.animation_spin_3, "3" },

		};
	}
}
