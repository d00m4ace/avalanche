using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class PhysicBody2D
	{
		public Entity2D entity;

		public Fixed mass, massInv, inertiaInv;

		public Vector2 force;
		public Fixed torque;

		public Fixed damping;

		public PhysicBody2D(Entity2D entity, Fixed mass)
		{
			this.entity = entity;

			force = Vector2.Zero;
			torque = 0;

			SetMass(mass);

			damping = (Fixed)999 / 1000;
		}

		public void Rest()
		{
			force = Vector2.Zero;
			torque = 0;
		}

		public void SetMass(Fixed mass)
		{
			this.mass = mass;
			massInv = mass == 0 ? 0 : 1 / mass;
		}

		public void AddForce(Vector2 force)
		{
			this.force += force;
		}

		public void AddTorque(Fixed torque)
		{
			this.torque += torque;
		}

		public void AddForce(Vector2 force, Vector2 point)
		{
			this.force += force;
			this.torque += Vector2.Cross(entity.pos - point, force);
		}

		public void ApplyImpulse(Vector2 j) { entity.vel += massInv * j; }

		public void ApplyImpulse(Vector2 j, Vector2 r) { entity.vel += massInv * j; entity.rot -= inertiaInv * Vector2.Cross(j, r); }

		public void CalcProperties()
		{
			if(mass == 0)
			{
				inertiaInv = 0;
				return;
			}

			Fixed inertia = 0;


			int sc = entity.shapes.Count;
			for(int s = 0; s < sc; s++)
			{
				if(entity.shapes[s].type == Shape2D.Type.Circle)
				{
					Circle2D c = (Circle2D)entity.shapes[s];

					Fixed sInertia = c.radius * c.radius / 2 + c.offset.LengthSquared;

					inertia += sInertia;
				}
				else if(entity.shapes[s].type == Shape2D.Type.ConvexPolygon)
				{
					Fixed s1 = 0, s2 = 0;

					PolygonGeometry2D local = ((ConvexPolygon2D)entity.shapes[s]).local;

					int len = local.vertices.Length;

					for(int i = 0; i < len; i++)
					{
						Vector2 v = local.vertices[i], u = local.vertices[(i + 1) % len];
						Fixed a = Vector2.Cross(u, v), b = v * v + v * u + u * u;
						s1 += a * b; s2 += a;
					}

					Fixed sInertia = s1 / (6 * s2);

					inertia += sInertia;
				}
			}

			inertiaInv = 1 / (inertia * mass);
		}

		public void Integrate()
		{
			entity.vel *= damping;
			entity.rot *= damping;

			Vector2 totalForce = force * massInv;
			Fixed totalTorque = torque * inertiaInv;

			Fixed totalFriction = entity.friction;
			if(entity.lastGroundEntityID != 0)
			{
				Entity2D groundEntity = World2D.GetEntity2D(entity.lastGroundEntityID);
				totalFriction = Math.Min(groundEntity.friction, totalFriction);
			}
			totalForce = totalForce - entity.vel * totalFriction;

			IntegrateForces(World2D.dt, totalForce, totalTorque, Fixed.OneHalf);
			IntegrateVelocity(World2D.dt);
			IntegrateForces(World2D.dt, totalForce, totalTorque, Fixed.OneHalf);

			ClearForces();
		}

		void IntegrateForces(Fixed dt, Vector2 force, Fixed torque, Fixed mult)
		{
			entity.vel += dt * force * mult;
			entity.rot -= dt * torque * mult;
		}

		void IntegrateVelocity(Fixed dt)
		{
			entity.pos += dt * entity.vel;
			entity.angle += dt * entity.rot;
			entity.dir = Vector2.Polar(entity.angle);
		}

		void ClearForces()
		{
			force = Vector2.Zero;
			torque = 0;
		}
	}

#if false
	RIGID BODY FOR FUTURE
	public class PhysicSpace2D : Space2D
	{
		public static class Config
		{
			public static Vector2 gravity = Vector2.ToVector2(Fixed.Zero, -(Fixed)8 / 10);
		}

		public PhysicSpace2D() : base()
		{
		}

		public override void FindAllContacts()
		{
			foreach(Entity2D e in entites)
			{
				if(e is PhysicBody2D)
					FindAllContacts(e);
			}
		}

		public override bool CheckContacts(Entity2D a, Entity2D b)
		{
			if(a.aabb.Intersect(b.aabb))
			{
				PhysicBody2DContacts e2dc = new PhysicBody2DContacts(a, b);

				if(e2dc.contacts.Count > 0)
				{
					contacts.Add(e2dc);
					return true;
				}
			}

			return false;
		}

		public override void Update(Fixed dt)
		{
			foreach(Entity2D entity in entites)
			{
				if(entity is PhysicBody2D)
					((PhysicBody2D)entity).AddForce(Config.gravity);

				entity.Update(dt);
			}

			ClearAllContacts();
			FindAllContacts();

			ResolveContacts();

			ResolveCollisions((a, b, c) => true, e => e is PhysicBody2D);

			ClearAllContacts();
			FindAllContacts();

			ResolveCollisions((a, b, c) => ((a is PhysicBody2D) != (b is PhysicBody2D)), e => e is PhysicBody2D);

			UpdateAABB();
		}

		public void ResolveCollision(PhysicBody2D body, Contact2D contact)
		{
			body.vel = body.vel - 2 * contact.axis.n * (contact.axis.n * body.vel);
		}

		public void ResolveContacts()
		{
			foreach(Entity2DContacts c in contacts)
			{
				PhysicBody2DContacts pb2c = (PhysicBody2DContacts)c;
				pb2c.Prestep();
			}

			Physic2D.elasticity = (Fixed)1;

			for(int i = 0; i < Physic2D.Config.iterations * 1 / 3; i++)
			{
				foreach(Entity2DContacts c in contacts)
				{
					PhysicBody2DContacts pb2c = (PhysicBody2DContacts)c;
					pb2c.Perform();
				}
			}

			foreach(Entity2DContacts c in contacts)
			{
				PhysicBody2DContacts pb2c = (PhysicBody2DContacts)c;
				pb2c.PerformCached();
			}

			Physic2D.elasticity = (Fixed)0;

			for(int i = 0; i < Physic2D.Config.iterations * 2 / 3; i++)
			{
				foreach(Entity2DContacts c in contacts)
				{
					PhysicBody2DContacts pb2c = (PhysicBody2DContacts)c;
					pb2c.Perform();
				}
			}
		}
	}

	public class Physic2D : World2D
	{
		public static class Config
		{
			public static Fixed
				areaMassRatio = (Fixed)1 / 100,

				resolveSlop = (Fixed)1 / 100,
				resolveRate = (Fixed)1 / 10,

				defaultRestitution = (Fixed)3 / 10,
				defaultFriction = (Fixed)7 / 10,

				damping = (Fixed)999 / 1000;

			public static int iterations = 5;
		}

		public static Fixed elasticity;

		public Physic2D() : base()
		{
		}

		public override void Update(Fixed dt)
		{
			foreach(Space2D space in spacies)
			{
				space.Update(dt);
			}
		}
	}

	public class PhysicContact2D : Contact2D
	{
		public Vector2 toA;
		public Vector2 toB;

		public Fixed nMass;
		public Fixed tMass;
		public Fixed restitution;
		public Fixed bias;
		public Fixed jBias;

		public Fixed cachedNormalImpulse;
		public Fixed cachedTangentImpulse;

		public PhysicContact2D(Contact2D contact)
		{
			point = contact.point;
			axis = contact.axis;

			toA = Vector2.Zero;
			toB = Vector2.Zero;

			nMass = Fixed.Zero;
			tMass = Fixed.Zero;
			restitution = Fixed.Zero;
			bias = Fixed.Zero;
			jBias = Fixed.Zero;

			cachedNormalImpulse = Fixed.Zero;
			cachedTangentImpulse = Fixed.Zero;
		}

		public void Prestep(PhysicBody2DContacts manifold)
		{
			PhysicBody2D bodyA = manifold.bodyA;
			PhysicBody2D bodyB = manifold.bodyB;

			toA = point - bodyA.pos;
			toB = point - bodyB.pos;

			nMass = Fixed.One / KScalar(bodyA, bodyB, axis.n);
			tMass = Fixed.One / KScalar(bodyA, bodyB, axis.n.Left);

			bias = BiasDist(axis.d);
			jBias = Fixed.Zero;

			restitution = manifold.restitution * axis.n * RelativeVelocity(bodyA, bodyB);
		}

		public void Perform(PhysicBody2DContacts manifold)
		{
			PhysicBody2D bodyA = manifold.bodyA;
			PhysicBody2D bodyB = manifold.bodyB;
			Fixed elasticity = Physic2D.elasticity;

			// Calculate relative bias velocity
			Vector2 vb1 = bodyA.velBias + (bodyA.rotBias * toA.Left);
			Vector2 vb2 = bodyB.velBias + (bodyB.rotBias * toB.Left);
			Fixed vbn = (vb1 - vb2) * axis.n;

			// Calculate and clamp the bias impulse
			Fixed jbn = nMass * (vbn - bias);
			jbn = Math.Max(-jBias, jbn);
			jBias += jbn;

			// Apply the bias impulse
			ApplyNormalBiasImpulse(bodyA, bodyB, jbn);

			// Calculate relative velocity
			Vector2 vr = RelativeVelocity(bodyA, bodyB);
			Fixed vrn = vr * axis.n;

			// Calculate and clamp the normal impulse
			Fixed jn = nMass * (vrn + (restitution * elasticity));
			jn = Math.Max(-cachedNormalImpulse, jn);
			cachedNormalImpulse += jn;

			// Calculate the relative tangent velocity
			Fixed vrt = vr * axis.n.Left;

			// Calculate and clamp the friction impulse
			Fixed jtMax = manifold.friction * cachedNormalImpulse;
			Fixed jt = vrt * tMass;
			Fixed result = Math.Clamp(cachedTangentImpulse + jt, -jtMax, jtMax);
			jt = result - cachedTangentImpulse;
			cachedTangentImpulse = result;

			// Apply the normal and tangent impulse
			ApplyContactImpulse(bodyA, bodyB, jn, jt);
		}

		public void PerformCached(PhysicBody2DContacts manifold)
		{
			ApplyContactImpulse(manifold.bodyA, manifold.bodyB, cachedNormalImpulse, cachedTangentImpulse);
		}

		public Vector2 RelativeVelocity(PhysicBody2D bodyA, PhysicBody2D bodyB)
		{
			return (bodyA.rot * toA.Left + bodyA.vel) - (bodyB.rot * toB.Left + bodyB.vel);
		}

		public void ApplyNormalBiasImpulse(PhysicBody2D bodyA, PhysicBody2D bodyB, Fixed normalBiasImpulse)
		{
			Vector2 impulse = normalBiasImpulse * axis.n;
			bodyA.ApplyBias(-impulse, toA);
			bodyB.ApplyBias(impulse, toB);
		}

		public void ApplyContactImpulse(PhysicBody2D bodyA, PhysicBody2D bodyB, Fixed normalImpulseMagnitude, Fixed tangentImpulseMagnitude)
		{
			Vector2 impulseWorld = Vector2.ToVector2(normalImpulseMagnitude, tangentImpulseMagnitude);
			Vector2 impulse = impulseWorld.Rotate(axis.n);
			bodyA.ApplyImpulse(-impulse, toA);
			bodyB.ApplyImpulse(impulse, toB);
		}

		public Fixed KScalar(PhysicBody2D bodyA, PhysicBody2D bodyB, Vector2 normal)
		{
			return (bodyA.massInv + bodyB.massInv) + bodyA.inertiaInv * Vector2.Cross(toA, normal).Square + bodyB.inertiaInv * Vector2.Cross(toB, normal).Square;
		}

		static Fixed BiasDist(Fixed dist)
		{
			return Physic2D.Config.resolveRate * Math.Min(Fixed.Zero, dist + Physic2D.Config.resolveSlop);
		}
	}

	public class PhysicBody2DContacts : Entity2DContacts
	{
		public PhysicBody2D bodyA, bodyB;

		public Fixed restitution;
		public Fixed friction;

		public PhysicBody2DContacts(Entity2D a, Entity2D b) : base(a, b)
		{
			if(contacts.Count == 0)
				return;

			if(a is PhysicBody2D)
				bodyA = (PhysicBody2D)a;
			else
				bodyA = new PhysicBody2D(a.pos, a.dir);

			if(b is PhysicBody2D)
				bodyB = (PhysicBody2D)b;
			else
				bodyB = new PhysicBody2D(b.pos, b.dir);

			int len = contacts.Count;

			for(int i = 0; i < len; i++)
				contacts[i] = new PhysicContact2D(contacts[i]);

			restitution = Math.Sqrt(bodyA.restitution * bodyB.restitution);
			friction = Math.Sqrt(bodyA.friction * bodyB.friction);
		}

		public void Prestep()
		{
			int len = contacts.Count;
			for(int i = 0; i < len; i++)
				((PhysicContact2D)contacts[i]).Prestep(this);
		}

		public void Perform()
		{
			int len = contacts.Count;
			for(int i = 0; i < len; i++)
				((PhysicContact2D)contacts[i]).Perform(this);
		}

		public void PerformCached()
		{
			int len = contacts.Count;
			for(int i = 0; i < len; i++)
				((PhysicContact2D)contacts[i]).PerformCached(this);
		}
	}

	public class PhysicBody2D : Entity2D
	{
		// motion components
		public Vector2 vel = Vector2.Zero;
		public Fixed angle = Fixed.Zero, rot = Fixed.Zero;

		// mass and inertia
		public Fixed massInv = (Fixed)1 / 100000, inertiaInv = (Fixed)1 / 100000;

		public Fixed density = Fixed.One;
		public Fixed mass = Fixed.Zero;

		public Fixed friction = Physic2D.Config.defaultFriction;
		public Fixed restitution = Physic2D.Config.defaultRestitution;

		public Vector2 velBias = Vector2.Zero;
		public Fixed rotBias = Fixed.Zero;

		public Vector2 force = Vector2.Zero;
		public Fixed torque = Fixed.Zero;

		public PhysicBody2D(Vector2 pos, Vector2 dir) : base(pos, dir)
		{
		}

		public void AddTorque(Fixed torque)
		{
			this.torque += torque;
		}

		public void AddForce(Vector2 force)
		{
			this.force += force;
		}

		public void AddForce(Vector2 force, Vector2 point)
		{
			this.force += force;
			this.torque += Vector2.Cross(pos - point, force);
		}

		public void ApplyImpulse(Vector2 j) { vel += massInv * j; }

		public void ApplyImpulse(Vector2 j, Vector2 r) { vel += massInv * j; rot -= inertiaInv * Vector2.Cross(j, r); }

		public void ApplyImpulseAtPoint(Vector2 j, Vector2 p) { ApplyImpulse(j, p - pos); }

		public void ApplyBias(Vector2 j, Vector2 r) { velBias += massInv * j; rotBias -= inertiaInv * Vector2.Cross(j, r); }

		/// <summary>
		/// Computes forces and dynamics and applies them to position and angle.
		/// </summary>
		public void Integrate(Fixed dt)
		{
			// Apply damping
			vel *= Physic2D.Config.damping;
			rot *= Physic2D.Config.damping;

			// Calculate total force and torque
			Vector2 totalForce = force * massInv;
			Fixed totalTorque = torque * inertiaInv;

			// See http://www.niksula.hut.fi/~hkankaan/Homepages/gravity.html
			IntegrateForces(dt, totalForce, totalTorque, Fixed.OneHalf);
			IntegrateVelocity(dt);
			IntegrateForces(dt, totalForce, totalTorque, Fixed.OneHalf);

			ClearForces();
		}

		public void IntegrateForces(Fixed dt, Vector2 force, Fixed torque, Fixed mult)
		{
			vel += dt * force * mult;
			rot -= dt * torque * mult;
		}

		public void IntegrateVelocity(Fixed dt)
		{
			pos += dt * vel + velBias;
			angle += dt * rot + rotBias;
			dir = Vector2.Polar(angle);
		}

		public void ClearForces()
		{
			force = Vector2.Zero;
			torque = Fixed.Zero;
			velBias = Vector2.Zero;
			rotBias = Fixed.Zero;
		}

		public override void Update(Fixed dt)
		{
			Integrate(dt);

			base.Update(dt);
		}

		public override void Rebuild()
		{
			base.Rebuild();
			CalcProperties();
		}

		public void CalcProperties()
		{
			Fixed inertia = Fixed.Zero;
			mass = Fixed.Zero;

			foreach(Shape2D s in shapes)
			{
				Fixed sMass = density * s.Area * Physic2D.Config.areaMassRatio;

				mass += sMass;

				if(s.shapeType == Shape2D.ShapeType.Circle)
				{
					Circle2D c = (Circle2D)s;

					Fixed sInertia = c.radius * c.radius / 2 + c.offset.LengthSquared;

					inertia += sMass * sInertia;
				}
				else if(s.shapeType == Shape2D.ShapeType.ConvexPolygon)
				{
					Fixed s1 = Fixed.Zero, s2 = Fixed.Zero;

					PolygonGeometry2D local = ((ConvexPolygon2D)s).local;

					int len = local.vertices.Length;

					for(int i = 0; i < len; i++)
					{
						Vector2 v = local.vertices[i], u = local.vertices[(i + 1) % len];
						Fixed a = Vector2.Cross(u, v), b = v * v + v * u + u * u;
						s1 += a * b; s2 += a;
					}

					Fixed sInertia = s1 / (6 * s2);

					inertia += sMass * sInertia;
				}
			}

			massInv = Fixed.One / mass; inertiaInv = Fixed.One / inertia;
		}
	}

#endif
}