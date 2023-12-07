using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class AgentAvalanche : Agent
	{
		Vector2 downForce = Vector2.V(0, -(Fixed)98 / 10) * 2;

		Fixed sideFriction = 0;

		public Particles avalancheParticles = null;

		public override void OnRest(Actor actor)
		{
			Entity2D entity = actor.entity;
			entity.friction = (Fixed)36 / 120;

			if(avalancheParticles == null)
			{
				avalancheParticles = Game.collection.Get<Particles>(Game.ObjectType.Particles, Game.CollectionID.particles_avalanche);
				avalancheParticles.SetPos(Vector2.Zero, 1);
				avalancheParticles.AttachToModel(actor.entity.model, false);
			}
		}

		public override void OnUpdateWorld(Actor actor)
		{
			Entity2D entity = actor.entity;
			Entity2D player = Game.player;

			if(!avalancheParticles.IsPlaying())
				avalancheParticles.Play();

			if(entity.pos.y - player.pos.y > 12 * 4)
				entity.pos.y = player.pos.y + 12 * 4;
			else if(entity.pos.y - player.pos.y < -12 * 4)
				entity.pos.y = player.pos.y - 12 * 4;

			if(World2D.iteration % 60*3 == 0)
				sideFriction = entity.friction * Game.random.RandomFixed();

			if(entity.vel.LengthSquared != 0)
				entity.physicBody.AddForce(Fixed.OneHalf * -entity.vel.Normalize * sideFriction * entity.vel.LengthSquared);

			entity.physicBody.AddForce((downForce * entity.dir) * entity.dir);
		}
	}
}