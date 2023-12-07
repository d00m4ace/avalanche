using System;
using System.Collections.Generic;

namespace HEXPLAY
{
	public class AgentSkier : Agent
	{
		public InputController.Player player;

		public int heroId;

		public Vector2 downForce = Vector2.V(0, -(Fixed)98 / 10);
		public Fixed angleTwist = 270;
		public Fixed skiSideFriction = (Fixed)4 / 10;

		public Particles skiTrail = null;

		public bool isBot;
		public Fixed safeAngle;
		public Fixed lastSpeed;

		public bool isDead;
		public Fixed timeDead;

		public int combo;

		public HEXInt coins;
		public HEXInt score;
		public HEXInt distance;
		public Fixed speed;
		public Fixed time;

		public int hitCount;

		static List<Vector2> scoredTreesPos = new List<Vector2>();

		public override void OnRest(Actor actor)
		{
			Entity2D entity = actor.entity;

			entity.friction = Game.player == entity ? (Fixed)64 / 140 : (Fixed)64 / 130;

			entity.SetPos(Vector2.V(0, 0));
			entity.SetAngle(3 * Math.PI / 2);

			lastSpeed = 0;

			isBot = false;
			safeAngle = entity.angle;

			isDead = false;
			timeDead = 0;

			combo = 0;

			coins = 0;
			score = 0;
			distance = 0;
			speed = 0;
			time = 0;

			hitCount = 0;

			if(skiTrail == null)
			{
				skiTrail = Game.collection.Get<Particles>(Game.ObjectType.Particles, Game.CollectionID.particles_snow_trail);
				skiTrail.AttachToModel(actor.entity.model, false);
			}

			List<Color> colors = new List<Color>() { Color.LightBlue, Color.LightGreen, Color.LightOrange, Color.LimeGreen, Color.RoyalBlue, Color.Yellow, Color.Pink, Color.Gold, };
			colors.Shuffle(Game.random);

			{
				skiTrail.SetColorGradientOverLifetime(new Color[] { colors[0], colors[0], colors[0] * Color.WhiteAlpha0 }, new float[] { 0, 0.5f, 1.0f });
				skiTrail.Stop();
			}

			{
				SkisTrail st = entity.model.go.GetComponent<SkisTrail>();
				if(st == null)
					st = entity.model.go.AddComponent<SkisTrail>();
				st.skisTrailColor = colors[0] * Color.WhiteAlpha75;
			}

			if(Game.player == entity)
				scoredTreesPos.Clear();
		}

		public override void OnDelete(Actor actor)
		{
		}

		public override void OnUpdateGameLogic(Actor actor)
		{
			Entity2D entity = actor.entity;
			Fixed dt = World2D.dt * (int)Game.worldIterations;

			speed = entity.vel.Length;

			if(Game.player == entity)
			{
				if(speed > (Fixed)6 / 10)
					Sound.PlayOne(Game.CollectionID.sound_skiing_loop, 1, 1 + speed / 16, true);
				else
					Sound.StopOne(Game.CollectionID.sound_skiing_loop);
			}

			if(isDead)
			{
				timeDead += dt;
				return;
			}

			if(Game.player == entity && Game.isPlayLevel && Game.iteration % 60 == 0)
			{
				scoredTreesPos.RemoveAll(v => v.y - entity.pos.y > 10);
			}

			if(speed > 2)
			{
				if(!skiTrail.IsPlaying())
					skiTrail.Play();

				float g = 100 - (float)speed * 5; if(g < 0) g = 0;
				skiTrail.SetGravityModifier(g);

				float s = (float)speed * 5;
				skiTrail.SetStartSpeed(s);
			}
			else
			{
				if(skiTrail.IsPlaying())
					skiTrail.Stop();
			}

			{
				//score += (int)(speed.Square * dt);
				distance = (int)-(entity.pos.y - 5);
				time += dt;
			}

			if(speed > 4 && entity.dir * entity.vel / speed < (Fixed)90 / 100)
			{
				if(Game.player == entity)
					Sound.PlayOne(Game.CollectionID.sound_ski_turn, 1, 1, false);
			}

			{
				if(entity.model.IsPlayingAnimation(Game.CollectionID.animation_idle))
					entity.model.StopAnimation();

				if(speed > lastSpeed)
				{
					if(!entity.model.IsPlayingAnimation())
					{
						entity.model.PlayAnimation(Game.CollectionID.animation_move);

						if(Game.player == entity)
							Sound.Play(Game.CollectionID.sound_skiing);
					}
				}

				lastSpeed = speed;
			}

			if(isBot)
			{
				Vector2 pos = entity.pos;
				Fixed angle = safeAngle;

				for(int i = 0; i < 20; i++)
				{
					if(SafeDir(entity, pos, angle))
						break;

					angle = entity.angle + (1 - Game.random.RandomFixed() * 2) * Math.PI / 4;
					angle = angle.NormalizeRad;

					if(angle > -Math.PI / 8)
						angle = -Math.PI / 8;
					else if(angle < -Math.PI + Math.PI / 8)
						angle = -Math.PI + Math.PI / 8;
				}

				safeAngle = angle;
			}
		}

		public override void OnUpdateWorld(Actor actor)
		{
			Entity2D entity = actor.entity;
			Fixed dt = World2D.dt;

			if(Game.player != entity && actor.entity.lastGroundEntityID == 0)
				World2D.Delete(actor.entity);

			if(isDead)
				return;

			if(Game.player == entity && Game.isPlayLevel)
			{
				const int maxD = 3;
				AABB2D aabb = AABB2D.Rest;
				aabb.SetExtents(entity.pos, Vector2.V(maxD, maxD));
				World2D.intersects.Clear();

				World2D.ignoreObjectTypeTwo[0] = Game.ObjectType.Item;
				World2D.ignoreObjectTypeTwo[1] = Game.ObjectType.Rock;
				World2D.FindAllIntersectAABB(Entity2D.Type.Static, aabb, World2D.ignoreEntityNone, World2D.ignoreObjectTypeTwo, World2D.intersects);

				for(int i = 0; i < World2D.intersects.Count; i++)
				{
					if(entity.pos.y > World2D.intersects[i].pos.y)
						continue;

					if(scoredTreesPos.FindIndex(v => v.x == World2D.intersects[i].pos.x && v.y == World2D.intersects[i].pos.y) >= 0)
						continue;

					float d = (float)(entity.pos - World2D.intersects[i].pos).LengthSquared;

					if(d > maxD * maxD)
						continue;

					int treeScore = 0;
					Color color;
					GUIAnimation anim = null;

					if(d < (0.8f * 0.8f)) { treeScore = 300; color = Color.Yellow; anim = new GUIAnimationFilter(new GUIAnimation[] { new GUIAnimationXYMove(2, int.MinValue, 50), new GUIAnimationBaseColor(2, Color.WhiteAlpha0, true), new GUIAnimationBlinkBaseColor(0.05f, Color.Zero, false, true) }); }
					else if(d < (1.0f * 1.0f)) { treeScore = 200; color = Color.Orange; }
					else if(d < (1.2f * 1.2f)) { treeScore = 150; color = Color.Gold; }
					else if(d < (1.4f * 1.4f)) { treeScore = 100; color = Color.Violet; }
					else if(d < (1.6f * 1.6f)) { treeScore = 75; color = Color.LimeGreen; }
					else if(d < (1.8f * 1.8f)) { treeScore = 50; color = Color.LightBlue; }
					else if(d < (2.0f * 2.0f)) { treeScore = 25; color = Color.Green; }
					else { treeScore = 10; color = Color.White; }

					if(anim == null)
						anim = new GUIAnimationFilter(new GUIAnimation[] { new GUIAnimationXYMove(2, int.MinValue, 50), new GUIAnimationBaseColor(2, Color.WhiteAlpha0, true) });

					score += treeScore;
					Sound.Play(Game.CollectionID.sound_gba_gba03, 1, (Fixed)(1 + (maxD - d / maxD) / 2));
					GUIBubble bubble = new GUIBubble(new GUIText(treeScore.ToString("+0"), Game.GUIStyle.MediumFont, anim, color: color), 2, World2D.intersects[i].pos, -Fixed.OneHalf);
					GUI.AddBack(bubble);

					scoredTreesPos.Add(World2D.intersects[i].pos);
				}
			}

			if(isBot)
			{
				Fixed a = (3 * Math.PI / 2).NormalizeRad;
				Fixed d = a - safeAngle;
				safeAngle += dt * d;

				if(entity.angle > safeAngle)
					entity.SetAngle(entity.angle - dt * angleTwist * Math.PI / 180);
				else if(entity.angle < safeAngle)
					entity.SetAngle(entity.angle + dt * angleTwist * Math.PI / 180);
			}
			else
			{
				InputController.Joystick joystick = Game.inputController.GetPlayerJoystick(player);

				if(joystick.GetAxis(InputController.Joystick.Axis.X) != 0)
					entity.AddAngle(dt * angleTwist * joystick.GetAxis().x * Math.PI / 180);
				else
				{
					Fixed a = (3 * Math.PI / 2).NormalizeRad;
					Fixed d = a - entity.angle;
					entity.AddAngle(dt * d);
				}

				{
					Fixed angle = entity.angle;

					if(angle < Math.PI && angle > Math.PI / 2)
						angle = Math.PI;
					else if(angle > 0 && angle < Math.PI / 2)
						angle = 0;

					if(angle != entity.angle)
						entity.SetAngle(angle);
				}
			}

			if(entity.vel.LengthSquared != 0)
			{
				Fixed sina = (entity.dir - entity.vel.Normalize).Left.y;
				entity.model.SetWorldRotation(entity.dir, Math.PI * sina / 8);
				entity.model.Update();

				Vector2 n = entity.vel.Right.Normalize;
				entity.physicBody.AddForce(Fixed.OneHalf * (entity.dir * n) * n * skiSideFriction * entity.vel.LengthSquared);
			}

			entity.physicBody.AddForce((downForce * entity.dir) * entity.dir);
		}

		public override void OnContactEntity(Actor actor, Contact2D contact, Entity2D entity)
		{
			if(entity.flags.Has(Entity2D.Flags.Delete))
				return;

			if(entity.objectType == Game.ObjectType.Item)
			{
				if(Game.player != actor.entity)
					Sound.PlayAtPoint(Game.CollectionID.sound_coin, 1, entity.pos);
				else
					Sound.Play(Game.CollectionID.sound_coin);

				if(Game.player == actor.entity && Game.isPlayLevel)
				{
					GUIBubble bubble = new GUIBubble(new GUIImage("gui/images/game/coin", scale: 1.5f, animation: new GUIAnimationFilter(new GUIAnimation[] { new GUIAnimationXYMove(1, int.MinValue, 50), new GUIAnimationBaseColor(1, Color.WhiteAlpha0, true) })), 1, entity.pos, 0);
					GUI.AddBack(bubble);
				}

				Particles particles = Game.collection.Get<Particles>(Game.ObjectType.Particles, Game.CollectionID.particles_pickup);
				particles.Rest();
				particles.SetPos(entity.pos, (Fixed)1 / 2);
				particles.SetStartColor(Color.Yellow);
				particles.Play();

				World2D.Delete(entity);
				coins++;
				return;
			}

			if(entity.objectType == Game.ObjectType.Tree)
			{
				Fixed angle = actor.entity.vel.Length * Math.PI / 40;
				entity.model.SetWorldRotation(contact.axis.n.Right, angle);
				entity.model.Update();
			}

			bool skierAgentIsDead = false;
			if(entity.objectType == Game.ObjectType.Skier)
			{
				AgentSkier skierAgent = entity.actor.GetAgent<AgentSkier>();
				skierAgentIsDead = skierAgent.timeDead > 0;
			}

			if(!isDead && (Game.player == actor.entity && entity.objectType == Game.ObjectType.Skier && !skierAgentIsDead))
			{
				Sound.Play(Game.CollectionID.sound_hurt);
				return;
			}

			if(Game.player == actor.entity && entity.objectType != Game.ObjectType.Avalanche)
				Render.ShakeCamera(0.45f, (float)actor.entity.vel.Length / 3);

			if(!isDead && (Game.player == entity || skierAgentIsDead || entity.objectType == Game.ObjectType.Tree || entity.objectType == Game.ObjectType.Avalanche || entity.objectType == Game.ObjectType.Rock))
			{
				isDead = true;

				if(entity.objectType == Game.ObjectType.Skier || entity.objectType == Game.ObjectType.Avalanche)
					actor.entity.physicBody.ApplyImpulse((actor.entity.pos - entity.pos).Normalize * entity.vel.Length);

				AgentSkier playerAgentSkier = Game.player.actor.GetAgent<AgentSkier>();
				AgentSkier entitySkierAgent = entity.objectType == Game.ObjectType.Skier ? entity.actor.GetAgent<AgentSkier>() : null;

				if((Game.player == entity || (entitySkierAgent != null && entitySkierAgent.combo > 0)) && Game.isPlayLevel)
				{
					combo = 1 + ((entitySkierAgent != null && entitySkierAgent.combo > 0) ? entitySkierAgent.combo : 0);

					HEXInt skierScore = 500 * combo;
					playerAgentSkier.score += skierScore;

					Sound.Play(Game.CollectionID.sound_gba_gba04);
					Color color = Color.Yellow;
					//GUIAnimation anim = new GUIAnimationFilter(new GUIAnimation[] { new GUIAnimationXYMove(2, int.MinValue, 100), new GUIAnimationBaseColor(2, Color.WhiteAlpha0, true), new GUIAnimationBlinkBaseColor(0.05f, Color.Zero, false, true) });
					GUIAnimation anim = new GUIAnimationXYMove(3, int.MinValue, GUI.GetScreenHeight() / 2);

					if(Game.heroSet[heroId - 1].heroTokens < Game.heroSet[heroId - 1].heroTokensTotal)
					{
						if(!Game.isReplay)
							Game.heroSet[heroId - 1].heroTokens += combo;

						string heroIcon = "gui/images/coins/coin" + heroId.ToString("000");

						if(!Game.isReplay && Game.heroSet[heroId - 1].heroTokens >= Game.heroSet[heroId - 1].heroTokensTotal)
						{
							Game.heroSet[heroId - 1].heroTokens = Game.heroSet[heroId - 1].heroTokensTotal;
							Game.heroSet[heroId - 1].heroReceived = true;
						}

						GUIBubble bubble = new GUIBubble(new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.Combo.GetText() + " X" + combo.ToString(), Game.GUIStyle.MediumFont, color: color), new GUIImage(heroIcon), new GUIText(skierScore.ToString("+0"), Game.GUIStyle.MediumFont, color: color) }, animation: anim), 3, GUI.GetScreenWidth() / 2, GUI.GetScreenHeight() / 2);
						GUI.AddBack(bubble);
					}
					else
					{
						GUIBubble bubble = new GUIBubble(new GUILabel(new GUIElement[] { new GUIText(Game.TEXT.Combo.GetText() + " X" + combo.ToString(), Game.GUIStyle.MediumFont, color: color), new GUIText(skierScore.ToString("+0"), Game.GUIStyle.MediumFont, color: color) }, animation: anim), 3, GUI.GetScreenWidth() / 2, GUI.GetScreenHeight() / 2);
						GUI.AddBack(bubble);
					}
				}

				skiTrail.Stop();

				actor.entity.model.PlayAnimation(Game.CollectionID.animation_fall);

				Particles particles = Game.collection.Get<Particles>(Game.ObjectType.Particles, Game.CollectionID.particles_hit);
				particles.Rest();
				particles.AttachToModel(actor.entity.model);
				particles.SetPos(Vector2.Zero, (Fixed)1 / 2);
				particles.SetStartColor(Color.Red);
				particles.Play();

				if(entity.objectType == Game.ObjectType.Avalanche)
					Sound.Play(Game.CollectionID.sound_avalanche);
			}

			if(entity.objectType != Game.ObjectType.Avalanche && entity.objectType != Game.ObjectType.Skier)
			{
				Particles particles = Game.collection.Get<Particles>(Game.ObjectType.Particles, Game.CollectionID.particles_explosion);
				particles.Rest();
				particles.SetPos(actor.entity.pos, (Fixed)1 / 2);
				Color[] colors = new Color[] { Color.LightBlue, Color.LightGreen, Color.LightOrange, Color.LightSlateGray, Color.LimeGreen, Color.RoyalBlue, Color.Silver, Color.Yellow, Color.Pink, Color.Gold, };
				colors.Shuffle(Game.random);
				particles.SetStartColor(colors[0]);
				particles.Play();

				if(Game.maxHitCount < ++hitCount)
					Game.maxHitCount = hitCount;

				//Console.WriteLine(hitCount);
			}

			if(entity.objectType != Game.ObjectType.Avalanche)
			{
				if(Game.player != actor.entity)
					Sound.PlayAtPoint(Game.CollectionID.sound_hurt, 1, actor.entity.pos);
				else
					Sound.Play(Game.CollectionID.sound_hurt);
			}
		}

		static Ray2D ray = new Ray2D();

		public bool SafeDir(Entity2D entity, Entity2D.Type type, Vector2 pos, Vector2 dir)
		{
			ray.Set(pos, dir, 8);
			World2D.ignoreEntityOne[0] = entity;
			World2D.ignoreObjectTypeOne[0] = Game.ObjectType.Item;
			World2D.IntersectRay(type, ray, World2D.ignoreEntityOne, World2D.ignoreObjectTypeOne);
			return ray.shape == null;
		}

		Vector2 safeDir;
		public bool SafeDir(Entity2D entity, Vector2 pos, Fixed angle)
		{
			safeDir = Vector2.Polar(angle);
			if(!SafeDir(entity, Entity2D.Type.Static, pos, safeDir) || !SafeDir(entity, Entity2D.Type.Dynamic, pos, safeDir))
				return false;

			safeDir = Vector2.Polar(angle + 5 * Math.PI / 180);
			if(!SafeDir(entity, Entity2D.Type.Static, pos, safeDir) || !SafeDir(entity, Entity2D.Type.Dynamic, pos, safeDir))
				return false;

			safeDir = Vector2.Polar(angle - 5 * Math.PI / 180);
			if(!SafeDir(entity, Entity2D.Type.Static, pos, safeDir) || !SafeDir(entity, Entity2D.Type.Dynamic, pos, safeDir))
				return false;

			return true;
		}
	}
}