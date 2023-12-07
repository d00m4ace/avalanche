using System;

namespace HEXPLAY
{
	public struct Axis2D
	{
		public Vector2 n;
		public Fixed d;

		public static Axis2D A(Vector2 n, Fixed d) { Axis2D a; a.n = n; a.d = d; return a; } // avoid new

		public static Axis2D operator -(Axis2D a) { Axis2D axis; axis.n = -a.n; axis.d = a.d; return axis; }
	}

	public struct AABB2D
	{
		public Fixed t, b, l, r;

		public Fixed Width { get { return r - l; } }
		public Fixed Height { get { return t - b; } }
		public Vector2 Center { get { return Vector2.V(l + Width / 2, b + Height / 2); } }

		public AABB2D SetExtents(Vector2 c, Vector2 ext) { SetRange(c - ext, c + ext); return this; }
		public AABB2D SetRange(Vector2 v, Vector2 u) { b = v.y; t = u.y; l = v.x; r = u.x; return this; }

		public bool Intersect(ref AABB2D x) { return x.l <= r && l <= x.r && x.b <= t && b <= x.t; }

		public bool ContainsPoint(Vector2 x) { return x.x <= r && l <= x.x && x.y <= t && b <= x.y; }

		public static readonly AABB2D Rest = new AABB2D() { b = Fixed.PositiveInfinity, l = Fixed.PositiveInfinity, t = Fixed.NegativeInfinity, r = Fixed.NegativeInfinity };

		public void Add(ref AABB2D x)
		{
			t = Math.Max(t, x.t);
			r = Math.Max(r, x.r);
			b = Math.Min(b, x.b);
			l = Math.Min(l, x.l);
		}

		public void Add(ref Vector2 v)
		{
			t = Math.Max(t, v.y);
			r = Math.Max(r, v.x);
			b = Math.Min(b, v.y);
			l = Math.Min(l, v.x);
		}

		#region string
		public override string ToString() { return "{t:" + t + ", b:" + b + ", l:" + l + ", r:" + r + "}"; }
		public string ToString(string format, IFormatProvider formatProvider) { return "{t:" + t.ToString(format, formatProvider) + ", b:" + b.ToString(format, formatProvider) + ", l:" + l.ToString(format, formatProvider) + ", r:" + r.ToString(format, formatProvider) + "}"; }
		#endregion
	}

	public class Ray2D
	{
		public Vector2 origin, dir, normal;
		public Fixed dist, range;
		public Shape2D shape;
		public Entity2D entity;
		public AABB2D aabb;

		public Ray2D() { }

		public void Set(Vector2 origin, Vector2 dir, Fixed range)
		{
			this.origin = origin;
			this.range = dist = range;
			this.dir = dir;

			aabb = AABB2D.Rest;
			aabb.Add(ref origin);
			Vector2 end = origin + dir * range;
			aabb.Add(ref end);

			shape = null;
			entity = null;
		}

		public void Reset() { dist = range; shape = null; entity = null;  normal = Vector2.Zero; }

		public void Report(Entity2D e, Shape2D s, Fixed dist, Vector2 normal)
		{
			this.dist = dist;
			this.normal = normal;
			this.entity = e;
			this.shape = s;
		}
	}

	public abstract class Shape2D
	{
		public readonly Type type;

		public enum Type { None = 0, Circle, ConvexPolygon }

		public AABB2D aabb;

		public Fixed scale;
		public abstract void SetScale(Fixed scale);

		public Shape2D() { type = Type.None; }
		public Shape2D(Type type) { this.type = type; }

		public abstract Fixed Area { get; }

		public abstract bool ContainsPoint(Vector2 v);
		public abstract void IntersectRay(Entity2D entity, Ray2D ray);

		public abstract void Update(Vector2 pos, Vector2 dir);
	}

	public class Circle2D : Shape2D
	{
		public Fixed radius;
		public Vector2 offset, center;

		public Vector2 originOffset;
		public Fixed originRadius;

		public Circle2D() : base(Type.Circle) { }

		public Circle2D(Vector2 offset, Fixed radius) : base(Type.Circle) { originOffset = offset; originRadius = radius; SetScale(1); }

		public override void SetScale(Fixed scale)
		{
			this.scale = scale;
			radius = originRadius * scale;
			offset = originOffset * scale;
		}

		public override Fixed Area { get { return radius * radius * Math.PI; } }

		public override void Update(Vector2 pos, Vector2 dir) { aabb.SetExtents(center = pos + offset.Rotate(dir), Vector2.V(radius, radius)); }

		public override bool ContainsPoint(Vector2 v) { return (v - center).LengthSquared < radius * radius; }

		public override void IntersectRay(Entity2D entity, Ray2D ray)
		{
			if(!aabb.Intersect(ref ray.aabb))
				return;

			Vector2 r = center - ray.origin;

			Fixed slope = r * ray.dir;
			if(slope < 0)
				return;

			Fixed D = radius * radius + slope * slope - r * r;
			if(D < 0)
				return;

			Fixed dist = slope - Math.Sqrt(D);
			if(dist < 0)
				return;

			if(ray.dist > dist)
				ray.Report(entity, this, dist, (-r + dist * ray.dir).Normalize);
		}
	}

	public class PolygonGeometry2D
	{
		public Vector2[] vertices;
		public Axis2D[] axis;

		public PolygonGeometry2D() { }

		public void Transform(Vector2 offset)
		{
			int len = vertices.Length;

			for(int i = 0; i < len; i++)
				vertices[i] = vertices[i] + offset;
		}

		public void Rotate(Vector2 dir)
		{
			int len = vertices.Length;

			for(int i = 0; i < len; i++)
				vertices[i] = vertices[i].Rotate(dir);
		}

		public void CalclAxis()
		{
			int len = vertices.Length;

			for(int i = 0; i < len; i++)
			{
				Vector2 v = vertices[i], u = vertices[(i + 1) % len], n = (u - v).Left.Normalize;
				axis[i] = Axis2D.A(n, n * v);
			}
		}
	}

	public class ConvexPolygon2D : Shape2D
	{
		public Vector2[] origin;
		public PolygonGeometry2D local;
		public PolygonGeometry2D world;

		public ConvexPolygon2D() : base(Type.ConvexPolygon) { }

		public override void SetScale(Fixed scale)
		{
			this.scale = scale;

			Vector2 originCenter = Vector2.Zero, originOffset;

			for(int i = 0; i < origin.Length; i++)
				originCenter += origin[i];

			originCenter *= (Fixed)1 / origin.Length;
			originOffset = originCenter * scale;

			for(int i = 0; i < origin.Length; i++)
				local.vertices[i] = originOffset + (origin[i] - originCenter) * scale;

			local.CalclAxis();
		}

		public ConvexPolygon2D(Vector2[] vertices) : base(Type.ConvexPolygon)
		{
			origin = vertices;

			local = new PolygonGeometry2D();
			world = new PolygonGeometry2D();
			int len = vertices.Length;
			local.vertices = new Vector2[len];
			world.vertices = new Vector2[len];
			local.axis = new Axis2D[len];
			world.axis = new Axis2D[len];

			SetScale(1);
		}

		public override Fixed Area
		{
			get
			{
				Fixed s = Fixed.Zero; int len = local.vertices.Length;

				for(int i = 0; i < len; i++)
				{
					Vector2 v = local.vertices[i], u = local.vertices[(i + 1) % len], w = local.vertices[(i + 2) % len];
					s += u.x * (v.y - w.y);
				}

				return s / 2;
			}
		}

		public override void Update(Vector2 pos, Vector2 dir)
		{
			aabb = AABB2D.Rest;

			for(int i = 0; i < local.vertices.Length; i++)
			{
				world.vertices[i] = pos + local.vertices[i].Rotate(dir);
				aabb.Add(ref world.vertices[i]);
			}

			for(int i = 0; i < local.axis.Length; i++)
			{
				world.axis[i].n = local.axis[i].n.Rotate(dir);
				world.axis[i].d = world.axis[i].n * pos + local.axis[i].d;
			}
		}

		public override bool ContainsPoint(Vector2 v) { int ic = world.axis.Length; for(int i = 0; i < ic; i++) if(world.axis[i].n * v > world.axis[i].d) return false; return true; }

		public bool ContainsPointPartial(Vector2 v, Vector2 n) { int ic = world.axis.Length; for(int i = 0; i < ic; i++) if(world.axis[i].n * n <= Fixed.Zero && world.axis[i].n * v > world.axis[i].d) return false; return true; }

		public override void IntersectRay(Entity2D entity, Ray2D r)
		{
			if(!aabb.Intersect(ref r.aabb))
				return;

			int len = world.vertices.Length, ix = -1;
			Fixed inner = Fixed.PositiveInfinity, outer = 0;

			for(int i = 0; i < len; i++)
			{
				Axis2D a = world.axis[i];

				Fixed slope = -a.n * r.dir;
				Fixed proj = a.n * r.origin - a.d;

				if(Math.Approximately(slope, 0))
				{
					if(proj > 0)
						return;
					else
						continue;
				}

				Fixed dist = proj / slope;

				if(slope > 0)
				{
					if(dist > inner)
						return;

					if(dist > outer)
					{
						outer = dist;
						ix = i;
					}
				}
				else
				{
					if(dist < outer)
						return;

					if(dist < inner)
						inner = dist;
				}
			}

			if(ix == -1)
				return;

			if(r.dist > outer)
				r.Report(entity, this, outer, world.axis[ix].n);
		}
	}
}