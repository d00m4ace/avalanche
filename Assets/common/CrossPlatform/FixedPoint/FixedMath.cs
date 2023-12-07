using System;

namespace HEXPLAY
{
	public static class Math
	{
		public static readonly Fixed E = (Fixed)2718281 / 1000000;

		public static readonly Fixed PI = (Fixed)3141592 / 1000000;
		public static readonly Fixed PIDouble = PI * 2;
		public static readonly Fixed PIHalf = PI / 2;
		public static readonly Fixed InversePI = 1 / PI;

		public static readonly Fixed Epsilon = (Fixed)1 / 1000;

		public static Fixed Abs(Fixed f) { return f.raw < 0 ? -f : f; }

		public static Fixed Max(Fixed val1, Fixed val2) { return val1 >= val2 ? val1 : val2; }
		public static Fixed Min(Fixed val1, Fixed val2) { return val1 <= val2 ? val1 : val2; }

		public static int Sign(Fixed f) { return f.raw < 0 ? -1 : f.raw > 0 ? 1 : 0; }

		public static bool Approximately(Fixed a, Fixed b) { return Abs(a - b) <= Epsilon; }

		public static Fixed Clamp(Fixed f, Fixed min, Fixed max) { return f > max ? max : f < min ? min : f; }

		// Returns the largest integer less than or equal to the specified number.
		// Just zero out the fractional part
		public static Fixed Floor(Fixed f) { return Fixed.F((long)((ulong)f.raw & Fixed.INTEGER_BITS)); }

		// Returns the smallest integral value that is greater than or equal to the specified number.
		public static Fixed Ceiling(Fixed f) { return ((ulong)f.raw & Fixed.FRACTIONAL_BITS) != 0 ? Floor(f) + Fixed.One : f; }

		public static Fixed Round(Fixed f)
		{
			if(f.raw >= 0)
				return Fixed.F((long)((ulong)(f.raw + Fixed.OneHalf.raw) & Fixed.INTEGER_BITS));
			else
				return Fixed.F(-(long)((ulong)(-f.raw + Fixed.OneHalf.raw) & Fixed.INTEGER_BITS));
		}

		public static Fixed Sqrt(Fixed f)
		{
			if(f.raw < 0)
				throw new ArgumentOutOfRangeException("Negative value passed to Sqrt", f.ToString());

			// https://en.wikipedia.org/wiki/Methods_of_computing_square_roots#Binary_numeral_system_.28base_2.29
			long v = f.raw << Fixed.SHIFT_BITS;
			long r = 0L;
			long bit = 1L << 62;

			while(bit > v)
				bit >>= 2;

			while(bit != 0)
			{
				if(v >= r + bit)
				{
					v -= r + bit;
					r = (r >> 1) + bit;
				}
				else
					r >>= 1;
				bit >>= 2;
			}

			return Fixed.F(r);
		}

		public static Fixed Sin(Fixed angle)
		{
			// https://en.wikipedia.org/wiki/Trigonometric_functions#Series_definitions
			long pi = PI.raw;
			long pi2 = PIDouble.raw;

			// Normalize rad to [-π, +π]
			long rad = angle.raw % pi2;
			if(rad > pi)
				rad -= pi2;
			else if(rad < -pi)
				rad += pi2;

			long square = (rad * rad) >> Fixed.SHIFT_BITS;

			long r = rad;                // x
			rad = (rad * square) >> Fixed.SHIFT_BITS;
			r -= rad / 6L;              // - x^3 / 3!
			rad = (rad * square) >> Fixed.SHIFT_BITS;
			r += rad / 120L;            // + x^5 / 5!
			rad = (rad * square) >> Fixed.SHIFT_BITS;
			r -= rad / 5040L;           // - x^7 / 7!
			rad = (rad * square) >> Fixed.SHIFT_BITS;
			r += rad / 362880L;         // + x^9 / 9!
			rad = (rad * square) >> Fixed.SHIFT_BITS;
			r -= rad / 39916800L;       // - x^11 / 11!
			rad = (rad * square) >> Fixed.SHIFT_BITS;
			r += rad / 6227020800L;     // + x^13 / 13!

			return Fixed.F(r);
		}

		public static Fixed Asin(Fixed d)
		{
			if(d > Fixed.One || d < -Fixed.One)
				throw new NotSupportedException();

			return Atan(d / Sqrt(Fixed.One - d * d));
		}

		public static Fixed Cos(Fixed angle) { return Sin(PIHalf - angle); }

		public static Fixed Acos(Fixed d) { return PIHalf - Asin(d); }

		public static Fixed Tan(Fixed angle) { return Sin(angle) / Cos(angle); }

		public static Fixed Atan(Fixed d)
		{
			// https://en.wikipedia.org/wiki/Inverse_trigonometric_functions
			// Make sure |d| <= 1
			bool inversed;
			if(Abs(d) > Fixed.One)
			{
				d = Fixed.One / d;
				inversed = true;
			}
			else
				inversed = false;

			long v = d.raw;
			long square = (v * v) >> Fixed.SHIFT_BITS;

			long r = v;
			long n = 3L;

			for(int i = 0; i < 15; i++)
			{
				v = (v * square) >> Fixed.SHIFT_BITS;
				r -= v / n;
				n += 2;

				v = (v * square) >> Fixed.SHIFT_BITS;
				r += v / n;
				n += 2;
			}

			if(inversed)
			{
				if(r > 0)
					r = PIHalf.raw - r;
				else
					r = -PIHalf.raw - r;
			}

			return Fixed.F(r);
		}

		public static Fixed Atan2(Fixed y, Fixed x)
		{
			// https://en.wikipedia.org/wiki/Atan2#Definition_and_computation
			if(x > Fixed.Zero)
			{
				return Atan(y / x);
			}
			else if(x < Fixed.Zero)
			{
				if(y >= Fixed.Zero)
					return Atan(y / x) + PI;
				else
					return Atan(y / x) - PI;
			}
			else
			{
				if(y > Fixed.Zero)
					return PIHalf;
				else if(y < Fixed.Zero)
					return -PIHalf;
				else
					throw new NotSupportedException();
			}
		}
	}
}