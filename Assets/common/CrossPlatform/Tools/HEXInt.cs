using System;

namespace HEXPLAY
{
	public struct HEXInt : IComparable, IFormattable, IComparable<HEXInt>, IEquatable<HEXInt>
	{
		static PseudoRandom pr = PseudoRandom.GetRest();
		static readonly ulong salt = pr.Random();
		public static int hexed = 0;

		ulong r;
		int a, b;

		#region HEX
		static int CalcA(int value, ulong r) { return unchecked(value ^ (int)(r >> 32) ^ (int)(salt & 0xFFFFFFFF)); }
		static int CalcB(int value, ulong r) { return unchecked(value ^ (int)(r & 0xFFFFFFFF) ^ (int)(salt >> 32)); }
		static int CalcHex() { return unchecked((int)(salt & 0xFFFFFFFF) ^ (int)(salt >> 32)); }

		static HEXInt Rest(int value = 0)
		{
			HEXInt h;
			h.r = pr.Random();
			h.a = CalcA(value, h.r);
			h.b = CalcB(value, h.r);
			return h;
		}

		public bool Check()
		{
			int ta = CalcA(a, r);
			int tb = CalcB(b, r);
			return ta == tb;
		}

		int GetValue()
		{
			int ta = CalcA(a, r);
			int tb = CalcB(b, r);

			if(ta == tb)
				return ta;

			hexed = (ta ^ tb ^ CalcHex()) | 1;
			return 0;
		}

		void SetValue(int value)
		{
			int ta = CalcA(a, r);
			int tb = CalcB(b, r);

			if(ta == tb)
			{
				r = pr.Random();
				a = CalcA(value, r);
				b = CalcB(value, r);
				return;
			}

			hexed = (ta ^ tb ^ CalcHex()) | 1;
		}
		#endregion

		#region operators
		public static implicit operator HEXInt(int value) { return Rest(value); }
		public static implicit operator int(HEXInt value) { return value.GetValue(); }

		public static explicit operator Fixed(HEXInt value) { return value.GetValue(); }

		public static explicit operator HEXInt(long value) { return Rest((int)value); }
		public static explicit operator HEXInt(float value) { return Rest((int)value); }

		public static HEXInt operator *(HEXInt one, HEXInt other) { return one.GetValue() * other.GetValue(); }
		public static HEXInt operator *(HEXInt one, int other) { return one.GetValue() * other; }
		public static HEXInt operator *(int other, HEXInt one) { return other * one.GetValue(); }

		public static HEXInt operator /(HEXInt one, HEXInt other) { return one.GetValue() / other.GetValue(); }
		public static HEXInt operator /(HEXInt one, int other) { return one.GetValue() / other; }
		public static HEXInt operator /(int other, HEXInt one) { return other / one.GetValue(); }

		public static HEXInt operator %(HEXInt one, HEXInt other) { return one.GetValue() % other.GetValue(); }
		public static HEXInt operator %(HEXInt one, int other) { return one.GetValue() % other; }
		public static HEXInt operator %(int other, HEXInt one) { return other % one.GetValue(); }

		public static HEXInt operator ^(HEXInt one, HEXInt other) { return one.GetValue() ^ other.GetValue(); }
		public static HEXInt operator ^(HEXInt one, int other) { return one.GetValue() ^ other; }
		public static HEXInt operator ^(int other, HEXInt one) { return other ^ one.GetValue(); }

		public static HEXInt operator &(HEXInt one, HEXInt other) { return one.GetValue() & other.GetValue(); }
		public static HEXInt operator &(HEXInt one, int other) { return one.GetValue() & other; }
		public static HEXInt operator &(int other, HEXInt one) { return other & one.GetValue(); }

		public static HEXInt operator |(HEXInt one, HEXInt other) { return one.GetValue() | other.GetValue(); }
		public static HEXInt operator |(HEXInt one, int other) { return one.GetValue() | other; }
		public static HEXInt operator |(int other, HEXInt one) { return other | one.GetValue(); }

		public static HEXInt operator +(HEXInt one, HEXInt other) { return one.GetValue() + other.GetValue(); }
		public static HEXInt operator +(HEXInt one, int other) { return one.GetValue() + other; }
		public static HEXInt operator +(int other, HEXInt one) { return other + one.GetValue(); }

		public static HEXInt operator -(HEXInt one, HEXInt other) { return one.GetValue() - other.GetValue(); }
		public static HEXInt operator -(HEXInt one, int other) { return one.GetValue() - other; }
		public static HEXInt operator -(int other, HEXInt one) { return other - one.GetValue(); }

		public static HEXInt operator -(HEXInt one) { return one.GetValue(); }
		public static HEXInt operator ~(HEXInt one) { return ~one.GetValue(); }

		public static HEXInt operator ++(HEXInt one) { return one.GetValue() + 1; }
		public static HEXInt operator --(HEXInt one) { return one.GetValue() - 1; }

		public static bool operator ==(HEXInt one, HEXInt other) { return one.GetValue() == other.GetValue(); }
		public static bool operator ==(HEXInt one, int other) { return one.GetValue() == other; }
		public static bool operator ==(int other, HEXInt one) { return other == one.GetValue(); }

		public static bool operator !=(HEXInt one, HEXInt other) { return one.GetValue() != other.GetValue(); }
		public static bool operator !=(HEXInt one, int other) { return one.GetValue() != other; }
		public static bool operator !=(int other, HEXInt one) { return other != one.GetValue(); }

		public static bool operator >=(HEXInt one, HEXInt other) { return one.GetValue() >= other.GetValue(); }
		public static bool operator >=(HEXInt one, int other) { return one.GetValue() >= other; }
		public static bool operator >=(int other, HEXInt one) { return other >= one.GetValue(); }

		public static bool operator <=(HEXInt one, HEXInt other) { return one.GetValue() <= other.GetValue(); }
		public static bool operator <=(HEXInt one, int other) { return one.GetValue() <= other; }
		public static bool operator <=(int other, HEXInt one) { return other <= one.GetValue(); }

		public static bool operator >(HEXInt one, HEXInt other) { return one.GetValue() > other.GetValue(); }
		public static bool operator >(HEXInt one, int other) { return one.GetValue() > other; }
		public static bool operator >(int other, HEXInt one) { return other > one.GetValue(); }

		public static bool operator <(HEXInt one, HEXInt other) { return one.GetValue() < other.GetValue(); }
		public static bool operator <(HEXInt one, int other) { return one.GetValue() < other; }
		public static bool operator <(int other, HEXInt one) { return other < one.GetValue(); }

		public static HEXInt operator <<(HEXInt one, int amount) { return one.GetValue() << amount; }
		public static HEXInt operator >>(HEXInt one, int amount) { return one.GetValue() >> amount; }
		#endregion

		#region string
		public override string ToString() { return GetValue().ToString(); }
		public string ToString(string format, IFormatProvider formatProvider) { return GetValue().ToString(format, formatProvider); }
		public string ToString(string format) { return GetValue().ToString(format); }
		#endregion

		#region IComparable
		public int CompareTo(object obj) { return obj is HEXInt ? CompareTo((HEXInt)obj) : -1; }
		public int CompareTo(HEXInt value) { return GetValue().CompareTo(value.GetValue()); }
		#endregion

		#region IEquatable
		public override bool Equals(object obj) { return obj is HEXInt ? ((HEXInt)obj).GetValue() == GetValue() : false; }
		public bool Equals(HEXInt value) { return GetValue() == value.GetValue(); }
		public override int GetHashCode() { return GetValue().GetHashCode(); }
		#endregion
	}
}