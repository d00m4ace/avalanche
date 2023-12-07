#if false
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;

namespace HEXPLAY
{
	public static class World2DHelper
	{
		public static void DrawWorld2D()
		{
			int color = 0;
			Color[] colors = { Color.red, Color.blue };

			foreach(Space2D space in World2DTest.gameLogic.world2d.spacies)
				Draw(space, colors[(color++) % 2]);

			Draw(World2DTest.gameLogic.world2d.collision.contactPool.Used);
		}

		public static void Draw(Space2D space, Color color)
		{
			foreach(Entity2D entity in space.dynamicEntities)
				Draw(entity, color);

			foreach(Entity2D entity in space.staticEntities)
				Draw(entity, color);

			foreach(Entity2D entity in space.groundEntities)
				Draw(entity, color);
		}

		public static void Draw(IEnumerable<Entity2DContact> contacts)
		{
			foreach(Entity2DContact contact in contacts)
			{
				Draw(contact);
				Gizmos.color = Color.red;
				DrawAABB(contact.a.aabb);
				Gizmos.color = Color.red;
				DrawAABB(contact.b.aabb);
			}
		}

		public static void Draw(World2D.Ray2D ray)
		{
			FixedPoint.Vector2 end = ray.origin + ray.dir * ray.dist;

			Gizmos.color = Color.red;
			Gizmos.DrawLine(ToVector3(ray.origin), ToVector3(end));

			//Handles.color = Color.red;
			//Handles.DrawWireDisc(ToVector3(end), Vector3.forward, 0.1f);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(ToVector3(end), ToVector3(end + ray.normal));
		}

		public static void Draw(Entity2DContact contact)
		{
			int len = contact.contactsCount;

			for(int i = 0; i < len; i++)
				Draw(contact.contacts[i]);
		}

		public static void Draw(Contact2D contact)
		{
			//Handles.color = Color.red;
			//Handles.DrawWireDisc(ToVector3(contact.point), Vector3.forward, 0.1f);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(ToVector3(contact.point), ToVector3(contact.point + contact.axis.n));

			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(ToVector3(contact.point), ToVector3(contact.point + contact.axis.n * contact.axis.d));
		}

		public static void Draw(Entity2D entity, Color color)
		{
			//Handles.color = Color.yellow;
			//Handles.DrawWireDisc(ToVector3(entity.pos), Vector3.forward, 0.1f);

			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(ToVector3(entity.pos), ToVector3(entity.pos + entity.dir * (Fixed.One / 5)));

			Gizmos.color = Color.blue;
			Gizmos.DrawLine(ToVector3(entity.pos), ToVector3(entity.pos + entity.vel));

			foreach(Shape2D shape in entity.shapes)
				Draw(shape, color);

			Gizmos.color = Color.green;
			DrawAABB(entity.aabb);
		}

		public static Vector3 ToVector3(FixedPoint.Vector2 v)
		{
			Vector3 v3;
			v3.x = (float)v.x;
			v3.y = (float)v.y;
			v3.z = 0;
			return v3;
		}

		public static void Draw(Shape2D shape, Color color)
		{
			if(shape.type == Shape2D.Type.Circle)
			{
				Circle2D circle = (Circle2D)shape;
				//Handles.color = color;
				//Handles.DrawWireDisc(ToVector3(circle.center), Vector3.forward, (float)circle.radius);
			}
			else if(shape.type == Shape2D.Type.ConvexPolygon)
			{
				ConvexPolygon2D polygon = (ConvexPolygon2D)shape;
				Gizmos.color = color;
				DrawPolygon(polygon.world.vertices);
				Gizmos.color = Color.cyan;
				DrawPolygonAxis(polygon.world.vertices, polygon.world.axis);
			}
		}

		public static void DrawPolygon(FixedPoint.Vector2[] vertices)
		{
			int len = vertices.Length;

			for(int i = 0; i < len; i++)
			{
				FixedPoint.Vector2 v = vertices[i], u = vertices[(i + 1) % len];
				Gizmos.DrawLine(ToVector3(v), ToVector3(u));
			}
		}

		public static void DrawPolygonAxis(FixedPoint.Vector2[] vertices, Axis2D[] axis)
		{
			int len = vertices.Length;

			for(int i = 0; i < len; i++)
			{
				FixedPoint.Vector2 v = vertices[i], u = vertices[(i + 1) % len];
				FixedPoint.Vector2 m = (v + u) * Fixed.OneHalf;
				Gizmos.DrawLine(ToVector3(m), ToVector3(m + axis[i].n * (Fixed.One / 5)));
			}
		}

		public static void DrawAABB(AABB2D aabb)
		{
			FixedPoint.Vector2[] vertices = {
			FixedPoint.Vector2.V(aabb.l, aabb.b),
			FixedPoint.Vector2.V(aabb.l, aabb.t),
			FixedPoint.Vector2.V(aabb.r, aabb.t),
			FixedPoint.Vector2.V(aabb.r, aabb.b) };
			DrawPolygon(vertices);
		}
	}
}
#endif