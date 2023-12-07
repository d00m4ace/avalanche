using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class Entity2D : Collection.Element
	{
		public readonly int id;

		public Actor actor;
		public Model model;
		public Entity2DProxy proxy;

		public long restElapsedTicks;

		public Game.ObjectType objectType;
		public object gameObject;

		public int spaceID;
		public int lastGroundEntityID;

		public Vector2 pos, vel;
		public Fixed angle, rot;
		public Vector2 dir;

		public Fixed scale;

		public Fixed z, height;

		public Fixed restitution;
		public Fixed friction;

		public List<Shape2D> shapes;

		public PhysicBody2D physicBody;

		public AABB2D aabb;

		public readonly Type type;
		public int flags;

		//public Action onFree = null;

		public enum Type
		{
			None = 0,
			Ground,
			Static,
			Dynamic,
		}

		public abstract class Flags
		{
			public const int None = 0,
			IgnoreGround = 1,
			IgnoreStatic = 1 << 1,
			IgnoreDynamic = 1 << 2,
			Disabled = 1 << 3,
			NotVisible = 1 << 4,
			NoTransfer = 1 << 5,
			IgnoreIntersectRay = 1 << 6,
			Projectile = 1 << 7,
			DoNotResolveContact = 1 << 8,
			DoNotResolveCollision = 1 << 9,
			Unstoppable = 1 << 10,
			Delete = 1 << 11;
		}

		public Entity2D(int id, Type type, Vector2 pos, Fixed angle)
		{
			this.id = id;
			this.type = type;
			this.pos = pos;

			flags = Flags.Disabled;

			z = 0;
			height = 1;

			restitution = 1;
			friction = (Fixed)9 / 10;

			SetAngle(angle);

			scale = 1;

			rot = 0;
			vel = Vector2.Zero;

			objectType = 0;
			gameObject = null;

			spaceID = 0;
			lastGroundEntityID = 0;

			shapes = new List<Shape2D>();

#if SERVER
			World2D.CreateEntity2DProxy(id).Link(this);
#endif
		}

		public bool HasZOverlap(Entity2D entity)
		{
			return entity.z < z + height && z < entity.z + entity.height;
		}

		public void Rest()
		{
			SetScale(1);

			SetPos(Vector2.Zero);
			SetDir(Vector2.XAxis);

			Enable(false);

			z = 0;
			height = 1;

			rot = 0;
			vel = Vector2.Zero;

			spaceID = 0;
			lastGroundEntityID = 0;

			if(physicBody != null)
				physicBody.Rest();

			if(model != null)
				model.Rest();

			if(actor != null)
				actor.OnRest();

			restElapsedTicks = Game.updateStopwatch.ElapsedTicks;

			if(proxy != null)
			{
				proxy.Rest();
			}
		}

		public void Setup(Vector2 pos = default(Vector2), Vector2 dir = default(Vector2), Fixed z = default(Fixed))
		{
			Rest();
			SetPos(pos);
			if(dir.x != 0 || dir.y != 0) SetDir(dir);
			SetZ(z);
			Enable(true);
			Update();
		}

		public void Enable(bool enable)
		{
			if(enable)
				flags &= ~Flags.Disabled;
			else
				flags |= Flags.Disabled;
		}

		public void SetScale(Fixed scale)
		{
			this.scale = scale;

			int ic = shapes.Count;
			for(int i = 0; i < ic; i++)
				shapes[i].SetScale(scale);
		}

		public Entity2D AddPhysicBody(Fixed mass)
		{
			physicBody = new PhysicBody2D(this, mass);
			physicBody.CalcProperties();
			return this;
		}

		public void AddAngle(Fixed angle)
		{
			SetAngle(this.angle + angle);
		}

		public void SetAngle(Fixed angle)
		{
			this.angle = angle.NormalizeRad;
			dir = Vector2.Polar(this.angle);
		}

		public void SetDir(Vector2 dir)
		{
			this.dir = dir;
			angle = dir.Angle;
		}

		public void SetPos(Vector2 pos)
		{
			this.pos = pos;
		}

		public void SetZ(Fixed z)
		{
			this.z = z;
		}

		public void SetHeight(Fixed height)
		{
			this.height = height;
		}

		public void AddCircleShape(Vector2 offset, Fixed radius)
		{
			Circle2D circle = new Circle2D(offset, radius);
			shapes.Add(circle);
		}

		public void AddBoxShape(Vector2 offset, Fixed width, Fixed height, Fixed angle)
		{
			PolygonGeometry2D p = new PolygonGeometry2D();

			Vector2[] v = new Vector2[4];
			v[0].x = 0; v[0].y = 0;
			v[1].x = 0; v[1].y = height;
			v[2].x = width; v[2].y = height;
			v[3].x = width; v[3].y = 0;
			p.vertices = v;

			p.Transform(Vector2.V(-width / 2, -height / 2));
			p.Rotate(Vector2.Polar(angle));
			p.Transform(offset);

			ConvexPolygon2D poly = new ConvexPolygon2D(p.vertices); p.vertices = null;
			shapes.Add(poly);
		}

		public void Rebuild()
		{
			aabb = AABB2D.Rest;

			int ic = shapes.Count;
			for(int i = 0; i < ic; i++)
			{
				shapes[i].Update(pos, dir);
				aabb.Add(ref shapes[i].aabb);
			}

			if(physicBody != null)
				physicBody.CalcProperties();
		}

		public void Simulate()
		{
			if(actor != null)
				actor.OnUpdateWorld();

			if(physicBody != null)
				physicBody.Integrate();
			else
			{
				pos += World2D.dt * vel;
				angle += World2D.dt * rot;
				dir = Vector2.Polar(angle);
			}

			Update();
		}

		public void Update()
		{
			aabb = AABB2D.Rest;

			int ic = shapes.Count;
			for(int i = 0; i < ic; i++)
			{
				shapes[i].Update(pos, dir);
				aabb.Add(ref shapes[i].aabb);
			}

			UpdateModel();

			if(World2D.GetSpace2D(spaceID) != null)
				World2D.GetSpace2D(spaceID).flags |= (1 << (int)(type - 1));
		}

		public void UpdateModel()
		{
			if(model != null)
			{
				model.SetScale(scale);
				model.SetPos(pos);
				model.SetZ(z);
				model.SetDir(dir);

				bool isEntityDisabled = flags.Has(Flags.Disabled);
				bool isModelDisabled = model.flags.Has(Model.Flags.Disabled);

				if(isEntityDisabled != isModelDisabled)
					model.Enable(!isEntityDisabled);

				model.Update();
			}
		}

		public void OnContactEntity(ref Contact2D contact, Entity2D entity)
		{
			if(actor != null)
				actor.OnContactEntity(contact, entity);
		}

		public void ResolveCollision(ref Contact2D contact)
		{
			if(type != Type.Dynamic)
				return;

			if(!ContainsPoint(contact.point))
				return;

			pos = pos + contact.axis.n * contact.axis.d * ((Fixed)7 / 10);

			Update();
		}

		public void ResolveContact(ref Contact2D contact, Entity2D b)
		{
			Entity2D a = this;

			if(a.physicBody == null && b.physicBody == null)
			{
				if(a.type == Type.Dynamic && !a.flags.Has(Flags.Unstoppable))
					a.vel = Vector2.Zero;

				if(b.type == Type.Dynamic && !b.flags.Has(Flags.Unstoppable))
					b.vel = Vector2.Zero;

				return;
			}

			Vector2 rv = b.vel - a.vel;

			Fixed velAlongNormal = rv * contact.axis.n;

			if(velAlongNormal > 0)
				return;

			Fixed aMassInv = a.physicBody != null ? a.physicBody.massInv : 0;
			Fixed bMassInv = b.physicBody != null ? b.physicBody.massInv : 0;

			Fixed e = Math.Min(a.restitution, b.restitution);

			Fixed j = -(1 + e) * velAlongNormal / (aMassInv + bMassInv);

			Vector2 i = j * contact.axis.n;

			if(a.type == Type.Dynamic && !a.flags.Has(Flags.Unstoppable))
			{
				if(a.physicBody != null)
					a.physicBody.ApplyImpulse(-i);
				else
					a.vel = Vector2.Zero;
			}

			if(b.type == Type.Dynamic && !b.flags.Has(Flags.Unstoppable))
			{
				if(b.physicBody != null)
					b.physicBody.ApplyImpulse(i);
				else
					b.vel = Vector2.Zero;
			}
		}

		public bool ContainsPoint(Vector2 v)
		{
			if(!aabb.ContainsPoint(v))
				return false;

			int ic = shapes.Count;
			for(int i = 0; i < ic; i++)
				if(shapes[i].ContainsPoint(v))
					return true;

			return false;
		}

		public void IntersectRay(Ray2D ray)
		{
			if(!aabb.Intersect(ref ray.aabb))
				return;

			int ic = shapes.Count;
			for(int i = 0; i < ic; i++)
				shapes[i].IntersectRay(this, ray);
		}
	}
}