using System;

namespace HEXPLAY
{
	public struct Fixed : IComparable, IFormattable, IComparable<Fixed>, IEquatable<Fixed>
	{
		long bits;

		public static Fixed F(long fixedPoint) { Fixed fp; fp.bits = fixedPoint; return fp; }

		#region properties
		public long raw { get { return bits; } }
		public Fixed Square { get { return this * this; } }
		public Fixed NormalizeRad { get { Fixed r = this % Math.PIDouble; if(r > Math.PI) r -= Math.PIDouble; else if(r < -Math.PI) r += Math.PIDouble; return r; } } // Normalize rad to [-π, +π]
		#endregion

		#region const
		public const int SHIFT_BITS = 14;
		public const int DOUBLE_SHIFT_BITS = SHIFT_BITS * 2;

		public const ulong INTEGER_BITS = 0xFFFFFFFFFFFFC000;
		public const ulong FRACTIONAL_BITS = 0x3FFF;

		public static readonly Fixed Zero = 0;
		public static readonly Fixed One = 1;
		public static readonly Fixed OneHalf = One / 2;

		public static readonly Fixed MaxValue = F(0x7FFFFFFFL);
		public static readonly Fixed MinValue = F(-0x7FFFFFFFL);

		public static readonly Fixed PositiveInfinity = F(0x7FFFFFFFFFFFFFFFL);
		public static readonly Fixed NegativeInfinity = F(-0x7FFFFFFFFFFFFFFFL);
		#endregion

		#region operators
		public static implicit operator Fixed(int value) { return F(unchecked((long)value << SHIFT_BITS)); }

		public static explicit operator Fixed(long value) { return F(unchecked(value << SHIFT_BITS)); }
		public static explicit operator long(Fixed value) { return unchecked(value.bits >= 0 ? value.bits >> SHIFT_BITS : -(-value.bits >> SHIFT_BITS)); }

		public static explicit operator Fixed(float value) { return F(unchecked((long)(value * One.bits))); }
		public static explicit operator float(Fixed value) { return unchecked((float)value.bits / One.bits); }

		public static Fixed operator *(Fixed one, Fixed other) { return F(unchecked((one.bits * other.bits) >> SHIFT_BITS)); }
		public static Fixed operator *(Fixed one, int other) { return F(unchecked(one.bits * other)); }
		public static Fixed operator *(int other, Fixed one) { return F(unchecked(other * one.bits)); }

		public static Fixed operator /(Fixed one, Fixed other) { return F(unchecked((one.bits << SHIFT_BITS) / other.bits)); }
		public static Fixed operator /(Fixed one, int other) { return F(unchecked(one.bits / other)); }
		public static Fixed operator /(int other, Fixed one) { return F(unchecked(((long)other << DOUBLE_SHIFT_BITS) / one.bits)); }

		public static Fixed operator %(Fixed one, Fixed other) { return F(unchecked(one.bits % other.bits)); }
		public static Fixed operator %(Fixed one, int other) { return F(unchecked(one.bits % ((long)other << SHIFT_BITS))); }
		public static Fixed operator %(int other, Fixed one) { return F(unchecked(((long)other << SHIFT_BITS) % one.bits)); }

		public static Fixed operator +(Fixed one, Fixed other) { return F(unchecked(one.bits + other.bits)); }
		public static Fixed operator +(Fixed one, int other) { return F(unchecked(one.bits + ((long)other << SHIFT_BITS))); }
		public static Fixed operator +(int other, Fixed one) { return F(unchecked(((long)other << SHIFT_BITS) + one.bits)); }

		public static Fixed operator -(Fixed one, Fixed other) { return F(unchecked(one.bits - other.bits)); }
		public static Fixed operator -(Fixed one, int other) { return F(unchecked(one.bits - ((long)other << SHIFT_BITS))); }
		public static Fixed operator -(int other, Fixed one) { return F(unchecked(((long)other << SHIFT_BITS) - one.bits)); }

		public static Fixed operator -(Fixed one) { return F(-one.bits); }

		public static Fixed operator ++(Fixed one) { return one + One; }
		public static Fixed operator --(Fixed one) { return one - One; }

		public static bool operator ==(Fixed one, Fixed other) { return one.bits == other.bits; }
		public static bool operator ==(Fixed one, int other) { return unchecked(one.bits == ((long)other << SHIFT_BITS)); }
		public static bool operator ==(int other, Fixed one) { return unchecked(((long)other << SHIFT_BITS) == one.bits); }

		public static bool operator !=(Fixed one, Fixed other) { return unchecked(one.bits != other.bits); }
		public static bool operator !=(Fixed one, int other) { return unchecked(one.bits != ((long)other << SHIFT_BITS)); }
		public static bool operator !=(int other, Fixed one) { return unchecked(((long)other << SHIFT_BITS) != one.bits); }

		public static bool operator >=(Fixed one, Fixed other) { return one.bits >= other.bits; }
		public static bool operator >=(Fixed one, int other) { return unchecked(one.bits >= ((long)other << SHIFT_BITS)); }
		public static bool operator >=(int other, Fixed one) { return unchecked(((long)other << SHIFT_BITS) >= one.bits); }

		public static bool operator <=(Fixed one, Fixed other) { return one.bits <= other.bits; }
		public static bool operator <=(Fixed one, int other) { return unchecked(one.bits <= ((long)other << SHIFT_BITS)); }
		public static bool operator <=(int other, Fixed one) { return unchecked(((long)other << SHIFT_BITS) <= one.bits); }

		public static bool operator >(Fixed one, Fixed other) { return one.bits > other.bits; }
		public static bool operator >(Fixed one, int other) { return unchecked(one.bits > ((long)other << SHIFT_BITS)); }
		public static bool operator >(int other, Fixed one) { return unchecked(((long)other << SHIFT_BITS) > one.bits); }

		public static bool operator <(Fixed one, Fixed other) { return one.bits < other.bits; }
		public static bool operator <(Fixed one, int other) { return unchecked(one.bits < ((long)other << SHIFT_BITS)); }
		public static bool operator <(int other, Fixed one) { return unchecked(((long)other << SHIFT_BITS) < one.bits); }

		public static Fixed operator <<(Fixed one, int amount) { return F(unchecked(one.bits << amount)); }
		public static Fixed operator >>(Fixed one, int amount) { return F(unchecked(one.bits >> amount)); }
		#endregion

		#region string
		public override string ToString() { return ((float)this).ToString(); }
		public string ToString(string format, IFormatProvider formatProvider) { return ((float)this).ToString(format, formatProvider); }
		public string ToString(string format) { return ((float)this).ToString(format); }
		public string ToBinaryString() { return Convert.ToString(bits, 2).PadLeft(64, '0'); }
		#endregion

		#region IComparable
		public int CompareTo(object obj) { return obj is Fixed ? CompareTo((Fixed)obj) : -1; }
		public int CompareTo(Fixed value) { return bits.CompareTo(value.bits); }
		#endregion

		#region IEquatable
		public override bool Equals(object obj) { return obj is Fixed ? ((Fixed)obj).bits == bits : false; }
		public bool Equals(Fixed value) { return bits == value.bits; }
		public override int GetHashCode() { return bits.GetHashCode(); }
		#endregion
	}
}