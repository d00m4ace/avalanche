using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public partial class Game
	{
		public enum ObjectType
		{
			None,

			Ground,

			Character,
			Skier,

			Wall,
			Tree,
			Rock,

			Avalanche,

			Item,

			Misc,

			Particles,
			Sound,
		}
	}
}