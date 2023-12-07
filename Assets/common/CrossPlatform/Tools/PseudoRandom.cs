using System;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class PseudoRandom
	{
		public ulong[] seed = { 2685821657736338717UL, 1181783497276652981UL };

		// xorshift128plus https://en.wikipedia.org/wiki/Xorshift
		public ulong Random()
		{
			unchecked
			{
				ulong x = seed[0];
				ulong y = seed[1];
				seed[0] = y;
				x ^= x << 23; // a
				seed[1] = x ^ y ^ (x >> 17) ^ (y >> 26); // b, c
				return seed[1] + y;
			}
		}

		public int Random(int max) { return (int)(Random() % (ulong)max); }
		public int RandomSign(int max) { return (int)Random() % max; }
		public Fixed RandomFixed(int max = 0x7FFF) { return ((Fixed)Random(max)) / max; }

		public void Rest()
		{
			ulong ticksUtcNow = (ulong)DateTime.UtcNow.Ticks;
			seed[0] = (ticksUtcNow << 32) ^ ticksUtcNow;
			ulong ticksNow = (ulong)DateTime.Now.Ticks;
			seed[1] = (ticksNow << 32) ^ ticksNow;
		}

		public static PseudoRandom GetRest()
		{
			PseudoRandom pr = new PseudoRandom();
			pr.Rest();
			return pr;
		}
	}
}