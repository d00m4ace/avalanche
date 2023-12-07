using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public struct Contact2D
	{
		public Vector2 point;
		public Axis2D axis;
	}

	public class Entity2DContact
	{
		public const int MAX_CONTACTS = 3;

		public Entity2D a;
		public Entity2D b;
		public Contact2D[] contacts;
		public int contactsCount;

		public Entity2DContact()
		{
			contacts = new Contact2D[MAX_CONTACTS];
			contactsCount = 0;
		}

		public int FindContacts(Entity2D a, Entity2D b)
		{
			this.a = a;
			this.b = b;

			contactsCount = 0;

			int sac = a.shapes.Count, sbc = b.shapes.Count;
			for(int sa = 0; sa < sac; sa++)
				for(int sb = 0; sb < sbc; sb++)
				{
					if(contactsCount >= MAX_CONTACTS)
						return contactsCount;
					Contact(a.shapes[sa], b.shapes[sb]);
				}

			return contactsCount;
		}

		public int FindContacts(Shape2D a, Shape2D b)
		{
			contactsCount = 0;
			Contact(a, b);
			return contactsCount;
		}

		public Contact2D Median()
		{
			if(contactsCount == 1)
				return contacts[0];

			Contact2D c;

			c.point = c.axis.n = Vector2.Zero;
			c.axis.d = 0;

			for(int i = 0; i < contactsCount; i++)
			{
				c.point = c.point + contacts[i].point;
				c.axis.n = c.axis.n + contacts[i].axis.n;
				c.axis.d = c.axis.d + contacts[i].axis.d;
			}

			Fixed div = Fixed.One / contactsCount;

			c.point = c.point * div;
			c.axis.n = c.axis.n.Normalize;
			c.axis.d *= div;

			return c;
		}

		bool AddContact(ref Contact2D contact)
		{
			if(contactsCount < MAX_CONTACTS)
			{
				contacts[contactsCount] = contact;
				return ++contactsCount < MAX_CONTACTS;
			}

			return false;
		}

		// Check contact b against a, contact normals used from a 
		bool Contact(Shape2D a, Shape2D b)
		{
			if(a.type == Shape2D.Type.Circle && b.type == Shape2D.Type.Circle)
				return Circle2Circle((Circle2D)a, (Circle2D)b);

			if(a.type == Shape2D.Type.Circle && b.type == Shape2D.Type.ConvexPolygon)
				return Circle2ConvexPolygon((Circle2D)a, (ConvexPolygon2D)b);

			if(a.type == Shape2D.Type.ConvexPolygon && b.type == Shape2D.Type.Circle)
				return Circle2ConvexPolygon((Circle2D)b, (ConvexPolygon2D)a, true);

			if(a.type == Shape2D.Type.ConvexPolygon && b.type == Shape2D.Type.ConvexPolygon)
				return ConvexPolygon2ConvexPolygon((ConvexPolygon2D)a, (ConvexPolygon2D)b);

			return false;
		}

		bool Circle2Circle(Circle2D a, Circle2D b)
		{
			return Circle2Circle(a.center, a.radius, b.center, b.radius);
		}

		bool Circle2Circle(Vector2 ac, Fixed ar, Vector2 bc, Fixed br, bool inverseN = false)
		{
			Vector2 r = ac - bc; Fixed min = ar + br, dist = r.LengthSquared, distInv;

			if(dist >= min * min)
				return false;

			dist = Math.Sqrt(dist);

			if(dist == 0)
				return false;

			distInv = 1 / dist;

			Contact2D contact;
			contact.point = ac - (Fixed.OneHalf + distInv * (ar - min / 2)) * r;
			contact.axis.n = inverseN ? distInv * r : -distInv * r;
			contact.axis.d = dist - min;
			AddContact(ref contact);
			return true;
		}

		bool Circle2ConvexPolygon(Circle2D circle, ConvexPolygon2D poly, bool inverseN = false)
		{
			int len = poly.world.axis.Length, ix = 0;
			Fixed max = Fixed.NegativeInfinity;

			for(int i = 0; i < len; i++)
			{
				Fixed dist = poly.world.axis[i].n * circle.center - poly.world.axis[i].d - circle.radius;

				if(dist > 0)
					return false;

				if(dist > max)
				{
					max = dist; ix = i;
				}
			}

			Vector2 v = poly.world.vertices[ix], u = poly.world.vertices[(ix + 1) % len]; Axis2D a = poly.world.axis[ix];

			Fixed d = Vector2.Cross(a.n, circle.center);

			if(d > Vector2.Cross(a.n, v))
				return Circle2Circle(circle.center, circle.radius, v, 0, inverseN);
			if(d < Vector2.Cross(a.n, u))
				return Circle2Circle(circle.center, circle.radius, u, 0, inverseN);

			Contact2D contact;
			contact.point = circle.center - (circle.radius + max / 2) * a.n;
			contact.axis.n = inverseN ? a.n : -a.n;
			contact.axis.d = max;
			AddContact(ref contact);
			return true;
		}

		bool ConvexPolygon2ConvexPolygon(ConvexPolygon2D sa, ConvexPolygon2D sb)
		{
			Axis2D a1, a2;

			if(!(MinSepAxis(sa, sb, out a1) && MinSepAxis(sb, sa, out a2)))
				return false;

			if(a2.d > a1.d)
				FindVerts(sb, sa, a2, true);
			else
				FindVerts(sa, sb, a1, false);

			return true;
		}

		bool MinSepAxis(ConvexPolygon2D sa, ConvexPolygon2D sb, out Axis2D axis)
		{
			axis.n = Vector2.Zero;
			axis.d = Fixed.NegativeInfinity;			

			int ic = sa.world.axis.Length;
			for(int i = 0; i < ic; i++)
			{
				Fixed min = Fixed.PositiveInfinity;

				int jc = sb.world.vertices.Length;
				for(int j = 0; j < jc; j++)					
					min = Math.Min(min, sa.world.axis[i].n * sb.world.vertices[j]);

				min -= sa.world.axis[i].d;

				if(min > 0)
					return false;

				if(min > axis.d)
				{
					axis.n = sa.world.axis[i].n;
					axis.d = min;
				}
			}

			return true;
		}

		void FindVerts(ConvexPolygon2D sa, ConvexPolygon2D sb, Axis2D a, bool inverseN)
		{
			bool found = false;

			int ic = sa.world.vertices.Length;
			for(int i = 0; i < ic; i++)
			{
				if(sb.ContainsPoint(sa.world.vertices[i]))
				{
					Contact2D contact;
					contact.point = sa.world.vertices[i];
					contact.axis = a;
					if(inverseN)
						contact.axis.n = -contact.axis.n;
					if(!AddContact(ref contact))
						return;
					found = true;
				}
			}

			ic = sb.world.vertices.Length;
			for(int i = 0; i < ic; i++)
			{
				if(sa.ContainsPoint(sb.world.vertices[i]))
				{
					Contact2D contact;
					contact.point = sb.world.vertices[i];
					contact.axis = a;
					if(inverseN)
						contact.axis.n = -contact.axis.n;
					if(!AddContact(ref contact))
						return;
					found = true;
				}
			}

			// Fallback to check the degenerate "Star of David" case
			if(found == false)
			{
				ic = sa.world.vertices.Length;
				for(int i = 0; i < ic; i++)
				{
					if(sb.ContainsPointPartial(sa.world.vertices[i], a.n))
					{
						Contact2D contact;
						contact.point = sa.world.vertices[i];
						contact.axis = a;
						if(inverseN)
							contact.axis.n = -contact.axis.n;
						if(!AddContact(ref contact))
							return;
					}
				}

				ic = sb.world.vertices.Length;
				for(int i = 0; i < ic; i++)
				{
					if(sa.ContainsPointPartial(sb.world.vertices[i], -a.n))
					{
						Contact2D contact;
						contact.point = sb.world.vertices[i];
						contact.axis = a;
						if(inverseN)
							contact.axis.n = -contact.axis.n;
						if(!AddContact(ref contact))
							return;
					}
				}
			}
		}
	}
}