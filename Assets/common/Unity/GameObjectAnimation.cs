using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

namespace HEXPLAY
{
	public class GameObjectAnimation : MonoBehaviour
	{
		public Model model;

		public Vector3 position;
		public Quaternion rotation;
		public Vector3 localScale;

		public GOAnimation goAnimation;

		void Update()
		{
			position = Vector3.zero;
			rotation = Quaternion.identity;
			localScale = Vector3.one;

			if(goAnimation != null)
				goAnimation.Update(this);

			transform.position = model.position + position;
			transform.rotation = model.rotation * rotation;
			transform.localScale = Vector3.Scale(model.localScale, localScale);
		}
	}

	public class GOAnimation
	{
		public virtual void Update(GameObjectAnimation goa) { }
		public virtual bool IsEnded() { return true; }
	}

	public class GOAnimationSequence : GOAnimation
	{
		public List<GOAnimation> sequence;

		public GOAnimationSequence(params GOAnimation[] anims)
		{
			sequence = new List<GOAnimation>(anims);
		}

		public override void Update(GameObjectAnimation goa)
		{
			for(int i = 0; i < sequence.Count;)
			{
				if(sequence[i].IsEnded())
				{
					sequence.RemoveAt(i);
					continue;
				}

				sequence[i].Update(goa);
				i++;
				break;
			}
		}

		public override bool IsEnded() { return sequence.Count == 0; }
	}

	public class GOAnimationFilter : GOAnimation
	{
		public List<GOAnimation> sequence;

		public GOAnimationFilter(params GOAnimation[] anims)
		{
			sequence = new List<GOAnimation>(anims);
		}

		public override void Update(GameObjectAnimation goa)
		{
			for(int i = 0; i < sequence.Count;)
			{
				if(sequence[i].IsEnded())
				{
					sequence.RemoveAt(i);
					continue;
				}

				sequence[i].Update(goa);
				i++;
			}
		}

		public override bool IsEnded() { return sequence.Count == 0; }
	}

	public class GOAnimationScalar : GOAnimation
	{
		public float scalar = 0;
		public float speed = 0;
		public float max = 0;
		public float min = 0;

		public bool isEnded = false, endOnMax = false, endOnMin = false;

		public GOAnimationScalar(float scalar, float speed, float min, float max, bool endOnMin = false, bool endOnMax = false)
		{
			this.scalar = scalar;
			this.speed = speed;
			this.min = min;
			this.max = max;
			this.endOnMax = endOnMax;
			this.endOnMin = endOnMin;
		}

		public override void Update(GameObjectAnimation goa)
		{
			scalar += speed * Time.deltaTime;

			if(scalar > max)
			{
				scalar = max;
				speed = -speed;

				if(endOnMax)
					isEnded = true;
			}

			if(scalar < min)
			{
				scalar = min;
				speed = -speed;

				if(endOnMin)
					isEnded = true;
			}
		}

		public override bool IsEnded() { return false; }
	}

	public class GOAnimationXRotation : GOAnimation
	{
		public float rotationSpeedDegrees = 0;
		public Quaternion rotation;

		public GOAnimationXRotation(float rotationSpeedDegrees) { this.rotationSpeedDegrees = rotationSpeedDegrees; rotation = Quaternion.identity; }
		public override void Update(GameObjectAnimation goa) { rotation *= Quaternion.Euler(rotationSpeedDegrees * Time.deltaTime, 0, 0); goa.rotation *= rotation; }
		public override bool IsEnded() { return false; }
	}

	public class GOAnimationYRotation : GOAnimation
	{
		public float rotationSpeedDegrees = 0;
		public Quaternion rotation;

		public GOAnimationYRotation(float rotationSpeedDegrees) { this.rotationSpeedDegrees = rotationSpeedDegrees; rotation = Quaternion.identity; }
		public override void Update(GameObjectAnimation goa) { rotation *= Quaternion.Euler(0, 0, rotationSpeedDegrees * Time.deltaTime); goa.rotation *= rotation; }
		public override bool IsEnded() { return false; }
	}

	public class GOAnimationZRotation : GOAnimation
	{
		public float rotationSpeedDegrees = 0;
		public Quaternion rotation;

		public GOAnimationZRotation(float rotationSpeedDegrees) { this.rotationSpeedDegrees = rotationSpeedDegrees; rotation = Quaternion.identity; }
		public override void Update(GameObjectAnimation goa) { rotation *= Quaternion.Euler(0, rotationSpeedDegrees * Time.deltaTime, 0); goa.rotation *= rotation; }
		public override bool IsEnded() { return false; }
	}


	public class GOAnimationXposition : GOAnimationScalar
	{
		public GOAnimationXposition(float scalar, float speed, float min, float max, bool endOnMin = false, bool endOnMax = false) : base(scalar, speed, min, max, endOnMin, endOnMax) { }
		public override void Update(GameObjectAnimation goa) { base.Update(goa); goa.position.x += scalar; }
	}

	public class GOAnimationYposition : GOAnimationScalar
	{
		public GOAnimationYposition(float scalar, float speed, float min, float max, bool endOnMin = false, bool endOnMax = false) : base(scalar, speed, min, max, endOnMin, endOnMax) { }
		public override void Update(GameObjectAnimation goa) { base.Update(goa); goa.position.z += scalar; }
	}

	public class GOAnimationZposition : GOAnimationScalar
	{
		public GOAnimationZposition(float scalar, float speed, float min, float max, bool endOnMin = false, bool endOnMax = false) : base(scalar, speed, min, max, endOnMin, endOnMax) { }
		public override void Update(GameObjectAnimation goa) { base.Update(goa); goa.position.y += scalar; }
	}


	public class GOAnimationXScale : GOAnimationScalar
	{
		public GOAnimationXScale(float scalar, float speed, float min, float max, bool endOnMin = false, bool endOnMax = false) : base(scalar, speed, min, max, endOnMin, endOnMax) { }
		public override void Update(GameObjectAnimation goa) { base.Update(goa); goa.localScale.x *= scalar; }
	}

	public class GOAnimationYScale : GOAnimationScalar
	{
		public GOAnimationYScale(float scalar, float speed, float min, float max, bool endOnMin = false, bool endOnMax = false) : base(scalar, speed, min, max, endOnMin, endOnMax) { }
		public override void Update(GameObjectAnimation goa) { base.Update(goa); goa.localScale.z *= scalar; }
	}

	public class GOAnimationZScale : GOAnimationScalar
	{
		public GOAnimationZScale(float scalar, float speed, float min, float max, bool endOnMin = false, bool endOnMax = false) : base(scalar, speed, min, max, endOnMin, endOnMax) { }
		public override void Update(GameObjectAnimation goa) { base.Update(goa); goa.localScale.y *= scalar; }
	}
}