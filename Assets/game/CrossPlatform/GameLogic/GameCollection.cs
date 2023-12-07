using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace HEXPLAY
{
	public partial class Game
	{
		public static void SetupCollection()
		{
			particlesSnow = Render.SetupParticles(CollectionID.particles_snow);
			particlesCoins = Render.SetupParticles(CollectionID.particles_coins);
			particlesLight = Render.SetupParticles(CollectionID.particles_light);


			slotmachine = CreateBoxGameObject(CollectionID.misc_slotmachine, ObjectType.Misc, Entity2D.Type.Static);


			collection.Setup(ObjectType.Item, (name) =>
			{
				Entity2D entity = CreateCircleGameObject(name, ObjectType.Item, Entity2D.Type.Static, (Fixed)50 / 100);
				entity.flags |= Entity2D.Flags.DoNotResolveCollision | Entity2D.Flags.DoNotResolveContact;
				return entity;
			});

			PreloadGameObjects(ObjectType.Item, CollectionID.item_coin);


			collection.Setup(ObjectType.Ground, (name) =>
			{
				Entity2D entity = CreateBoxGameObject(name, ObjectType.Ground, Entity2D.Type.Ground, (Fixed)128 / 10, (Fixed)128 * 7 / 10);
				return entity;
			});

			PreloadGameObjects(ObjectType.Ground, CollectionID.ground_terrain);


			collection.Setup(ObjectType.Avalanche, (name) =>
			{
				Entity2D entity = CreateBoxGameObject(name, ObjectType.Avalanche, Entity2D.Type.Dynamic, (Fixed)128 * 8 / 10, (Fixed)8 * 8 / 10, Vector2.V(-(Fixed)64 * 8 / 10, 0));
				entity.AddPhysicBody(1);
				entity.flags |= Entity2D.Flags.Unstoppable | Entity2D.Flags.NoTransfer | Entity2D.Flags.DoNotResolveCollision | Entity2D.Flags.DoNotResolveContact;

				Actor actor = new Actor();
				actor.Link(entity);
				actor.Add(new AgentAvalanche());

				return entity;
			});

			PreloadGameObjects(ObjectType.Avalanche, CollectionID.misc_avalanche);


			collection.Setup(ObjectType.Tree, (name) =>
			{
				Entity2D entity = CreateCircleGameObject(name, ObjectType.Tree, Entity2D.Type.Static, (Fixed)20 / 100);
				return entity;
			});

			PreloadGameObjects(ObjectType.Tree, CollectionID.tree_01, CollectionID.tree_02, CollectionID.tree_03);


			collection.Setup(ObjectType.Rock, (name) =>
			{
				Entity2D entity = CreateBoxGameObject(name, ObjectType.Rock, Entity2D.Type.Static, (Fixed)128 * 4 / 10, (Fixed)64 / 10, Vector2.V(-(Fixed)128 * 2 / 10, 0));
				return entity;
			});

			PreloadGameObjects(ObjectType.Rock, CollectionID.rock_01, CollectionID.rock_02);

			collection.Setup(ObjectType.Skier, (name) =>
			{
				Entity2D entity = CreateCircleGameObject(name, ObjectType.Skier, Entity2D.Type.Dynamic, (Fixed)30 / 100, Vector2.Zero);
				entity.AddPhysicBody(1);

				AgentSkier agentSkier = new AgentSkier();
				agentSkier.heroId = name - CollectionID.character_skier_01 + 1;

				Actor actor = new Actor();
				actor.Link(entity);
				actor.Add(agentSkier);

				return entity;
			});

			PreloadGameObjects(ObjectType.Skier, CollectionID.character_skier_01, CollectionID.character_skier_34);
		}
	}
}