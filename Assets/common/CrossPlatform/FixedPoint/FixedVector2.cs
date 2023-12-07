using System;

namespace HEXPLAY
{
	public struct Vector2 : IComparable, IFormattable, IComparable<Vector2>, IEquatable<Vector2>
	{
		public Fixed x;
		public Fixed y;

		public static Vector2 V(Fixed x, Fixed y) { Vector2 v; v.x = x; v.y = y; return v; } // avoid new

		#region const
		public static readonly Vector2 Zero = V(0, 0);
		public static readonly Vector2 One = V(1, 1);
		public static readonly Vector2 OneHalf = V(Fixed.OneHalf, Fixed.OneHalf);

		public static readonly Vector2 XAxis = V(1, 0);
		public static readonly Vector2 YAxis = V(0, 1);
		public static readonly Vector2 XYAxis = V(1, 1).Normalize;

		public static readonly Vector2 MinValue = V(Fixed.MinValue, Fixed.MinValue);
		public static readonly Vector2 MaxValue = V(Fixed.MaxValue, Fixed.MaxValue);
		#endregion

		#region properties
		public Fixed LengthSquared { get { return x * x + y * y; } }
		public Fixed Length { get { return Math.Sqrt(x * x + y * y); } }

		public Vector2 Right { get { return V(y, -x); } }
		public Vector2 Left { get { return V(-y, x); } }
		#endregion

		#region methods
		public Vector2 Normalize { get { return (1 / Length) * this; } }

		public Vector2 Scale(Vector2 s) { return V(x * s.x, y * s.y); }

		public Vector2 Project(Vector2 to) { return to * ((this * to) / to.LengthSquared); }

		public Fixed Angle { get { return Math.Atan2(y, x); } }

		public Vector2 Rotate(Vector2 b) { return V(x * b.x - y * b.y, y * b.x + x * b.y); }
		#endregion

		#region static methods
		public static Vector2 Polar(Fixed angle) { return V(Math.Cos(angle), Math.Sin(angle)); }

		public static Fixed Cross(Vector2 a, Vector2 b) { return a * b.Right; }
		public static Fixed Dot(Vector2 a, Vector2 b) { return a.x * b.x + a.y * b.y; }

		public static Vector2 Lerp(Vector2 from, Vector2 to, Fixed t) { return (to - from) * t + from; }

		public static Vector2 Abs(Vector2 v) { return V(Math.Abs(v.x), Math.Abs(v.y)); }

		public static Vector2 Min(Vector2 a, Vector2 b) { return V(Math.Min(a.x, b.x), Math.Min(a.y, b.y)); }
		public static Vector2 Max(Vector2 a, Vector2 b) { return V(Math.Max(a.x, b.x), Math.Max(a.y, b.y)); }

		public static bool Approximately(Vector2 a, Vector2 b) { return Math.Approximately(a.x, b.x) && Math.Approximately(a.y, b.y); }
		#endregion

		#region operators
		public static Vector2 operator *(Vector2 v, Fixed f) { return V(v.x * f, v.y * f); }
		public static Vector2 operator *(Fixed f, Vector2 v) { return V(v.x * f, v.y * f); }
		public static Vector2 operator *(Vector2 v, int f) { return V(v.x * f, v.y * f); }
		public static Vector2 operator *(int f, Vector2 v) { return V(v.x * f, v.y * f); }

		public static Vector2 operator /(Vector2 v, Fixed f) { return V(v.x / f, v.y / f); }
		public static Vector2 operator /(Vector2 v, int f) { return V(v.x / f, v.y / f); }

		public static Fixed operator *(Vector2 a, Vector2 b) { return a.x * b.x + a.y * b.y; } // aka DOT

		public static Vector2 operator +(Vector2 a, Vector2 b) { return V(a.x + b.x, a.y + b.y); }
		public static Vector2 operator -(Vector2 a, Vector2 b) { return V(a.x - b.x, a.y - b.y); }

		public static Vector2 operator -(Vector2 v) { return V(-v.x, -v.y); }

		public static bool operator ==(Vector2 a, Vector2 b) { return a.x == b.x && a.y == b.y; }
		public static bool operator !=(Vector2 a, Vector2 b) { return a.x != b.x || a.y != b.y; }
		#endregion

		#region IEquatable
		public bool Equals(Vector2 other) { return x == other.x && y == other.y; }
		public override bool Equals(object obj) { return obj is Vector2 ? Equals((Vector2)obj) : false; }
		public override int GetHashCode() { return x.GetHashCode() + y.GetHashCode(); }
		#endregion

		#region IComparable
		public int CompareTo(object obj) { return obj is Vector2 ? CompareTo((Vector2)obj) : -1; }
		public int CompareTo(Vector2 value) { return this.LengthSquared.CompareTo(value.LengthSquared); }
		#endregion

		#region string
		public override string ToString() { return "{" + x + ", " + y + "}"; }
		public string ToString(string format, IFormatProvider formatProvider) { return "{" + x.ToString(format, formatProvider) + ", " + y.ToString(format, formatProvider) + "}"; }
		#endregion
	}
}