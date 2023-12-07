using System.Collections;
using System;

using UnityEngine;

namespace HEXPLAY
{
	public class SkisTrail : MonoBehaviour
	{
		LineRenderer skiLeftRender;
		LineRenderer skiRightRender;

		const int MAX_TRAIL_POINTS = 200;

		Vector3[] skiLeftTrail;
		Vector3[] skiRightTrail;

		public float skiWidth = 0.4f;
		public Fixed skiSpace = (Fixed)3 / 10;

		public Color skisTrailColor;

		void Awake()
		{
			skiLeftTrail = new Vector3[MAX_TRAIL_POINTS];
			skiRightTrail = new Vector3[MAX_TRAIL_POINTS];

			GameObject goSkiLeft = new GameObject();
			skiLeftRender = goSkiLeft.AddComponent<LineRenderer>();
			skiLeftRender.material = new Material(Render.GetMaterial("SpritesDefault"));
			skiLeftRender.SetWidth(skiWidth, skiWidth);
			skiLeftRender.SetVertexCount(0);
			goSkiLeft.transform.parent = this.transform;
			goSkiLeft.transform.position = this.transform.position;
			goSkiLeft.transform.rotation = this.transform.rotation;
			goSkiLeft.name = "skiLeftTrail";

			GameObject goSkiRight = new GameObject();
			skiRightRender = goSkiRight.AddComponent<LineRenderer>();
			skiRightRender.material = new Material(Render.GetMaterial("SpritesDefault"));
			skiRightRender.SetWidth(skiWidth, skiWidth);
			skiRightRender.SetVertexCount(0);
			goSkiRight.transform.parent = this.transform;
			goSkiRight.transform.position = this.transform.position;
			goSkiRight.transform.rotation = this.transform.rotation;
			goSkiRight.name = "skiRightTrail";
		}

		void OnEnable()
		{
			Func<Vector2, Vector3> v2v3 = (v2) => new Vector3((float)v2.x, 0.1f, (float)v2.y);

			GameObjectHelper goh = gameObject.GetComponent<GameObjectHelper>();
			Vector2 skiLeftPos = goh.model.Pos + goh.model.Dir.Left * skiSpace;
			Vector2 skiRightPos = goh.model.Pos + goh.model.Dir.Right * skiSpace;

			skiLeftTrail[0] = v2v3(skiLeftPos);
			skiRightTrail[0] = v2v3(skiRightPos);

			for(int i = 1; i < MAX_TRAIL_POINTS; i++)
			{
				skiLeftTrail[i] = skiLeftTrail[0];
				skiRightTrail[i] = skiRightTrail[0];
			}

			skiLeftRender.SetVertexCount(MAX_TRAIL_POINTS);
			skiRightRender.SetVertexCount(MAX_TRAIL_POINTS);
			skiLeftRender.SetPositions(skiLeftTrail);
			skiRightRender.SetPositions(skiRightTrail);

			{
				Color32 c32 = skisTrailColor.GetColor32();
				skiLeftRender.SetColors(c32, c32);
				skiRightRender.SetColors(c32, c32);
			}
		}

		void Update()
		{
			GameObjectHelper goh = gameObject.GetComponent<GameObjectHelper>();
			Vector2 skiLeftPos = goh.model.Pos + goh.model.Dir.Left * skiSpace;
			Vector2 skiRightPos = goh.model.Pos + goh.model.Dir.Right * skiSpace;

			Func<Vector2, Vector3> v2v3 = (v2) => new Vector3((float)v2.x, 0.1f, (float)v2.y);

			if((skiLeftTrail[0] - v2v3(skiLeftPos)).sqrMagnitude > 0.1f)
			{
				skiLeftTrail[0] = v2v3(skiLeftPos);
				skiRightTrail[0] = v2v3(skiRightPos);

				for(int i = 1; i < MAX_TRAIL_POINTS; i++)
				{
					skiLeftTrail[MAX_TRAIL_POINTS - i] = skiLeftTrail[MAX_TRAIL_POINTS - i - 1];
					skiRightTrail[MAX_TRAIL_POINTS - i] = skiRightTrail[MAX_TRAIL_POINTS - i - 1];
				}
			}

			skiLeftRender.SetVertexCount(MAX_TRAIL_POINTS);
			skiRightRender.SetVertexCount(MAX_TRAIL_POINTS);
			skiLeftRender.SetPositions(skiLeftTrail);
			skiRightRender.SetPositions(skiRightTrail);
		}
	}
}