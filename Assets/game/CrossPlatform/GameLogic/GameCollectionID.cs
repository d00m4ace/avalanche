using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public partial class Game
	{
		public enum CollectionID
		{
			none = 0,

			ground = 1,
			ground_terrain,

			ground_counter_end,

			tree = ground + 100000,
			tree_01,
			tree_02,
			tree_03,

			tree_counter_end,

			rock = tree + 100000,
			rock_01,
			rock_02,

			rock_counter_end,

			character = rock + 100000,
			character_skier_01,
			character_skier_02,
			character_skier_03,
			character_skier_04,
			character_skier_05,

			character_skier_06,
			character_skier_07,
			character_skier_08,
			character_skier_09,
			character_skier_10,

			character_skier_11,
			character_skier_12,
			character_skier_13,
			character_skier_14,
			character_skier_15,

			character_skier_16,
			character_skier_17,
			character_skier_18,
			character_skier_19,
			character_skier_20,

			character_skier_21,
			character_skier_22,
			character_skier_23,
			character_skier_24,
			character_skier_25,

			character_skier_26,
			character_skier_27,
			character_skier_28,
			character_skier_29,
			character_skier_30,

			character_skier_31,
			character_skier_32,
			character_skier_33,
			character_skier_34,

			character_counter_end,

			wall = character + 100000,
			wall_counter_end,

			item = wall + 100000,
			item_coin,

			item_counter_end,

			misc = item + 1000000,
			misc_slotmachine,
			misc_avalanche,

			misc_counter_end,	

			particles = misc + 100000,
			particles_snow,
			particles_hit,
			particles_avalanche,
			particles_snow_trail,
			particles_coins,
			particles_light,
			particles_pickup,
			particles_explosion,

			particles_counter_end,

			sound = particles + 100000,

			sound_gba_gba01,
			sound_gba_gba02,
			sound_gba_gba03,
			sound_gba_gba04,
			sound_gba_gba05,
			sound_gba_gba06,
			sound_gba_gba07,
			sound_gba_gba08,
			sound_gba_gba09,
			sound_gba_gba10,
			sound_gba_gba11,
			sound_gba_gba12,
			sound_gba_gba13,
			sound_gba_gba14,
			sound_gba_gba15,
			sound_gba_gba16,

			sound_select,
			sound_click,

			sound_skiing_loop,
			sound_ski_turn,

			sound_skiing,

			sound_coin,

			sound_hurt,

			sound_avalanche,

			sound_highscore,

			sound_newhero,

			sound_hexlogo1,

			sound_avalanche2,

			sound_random,

			sound_slotmachine,
			sound_slotmachine_start,

			sound_counter_end,

			animation = sound + 100000,
			animation_none,
			animation_default,

			animation_idle,
			animation_move,
			animation_fall,

			animation_spin,
			animation_spin_1,
			animation_spin_2,
			animation_spin_3,

			animation_counter_end,
		}
	}
}