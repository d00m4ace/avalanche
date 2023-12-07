using System.Collections;
using System;

using UnityEngine;

namespace HEXPLAY
{
	public class GameObjectHelper : MonoBehaviour
	{
		public Model model;

		void OnEnable()
		{
			//RemoveEntityShapes();
			//AddEntityShapes();
		}

		public void RemoveEntityShapes()
		{
			for(int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);

				if(child.name == "entityShape")
				{
					child.parent = null;
					Destroy(child.gameObject);
				}
			}
		}

		public void AddEntityShapes()
		{
			Entity2D entity = model.entity;

			int sc = entity.shapes.Count;
			for(int s = 0; s < sc; s++)				
			{
				if(entity.shapes[s].type == Shape2D.Type.Circle)
				{
					Circle2D circle = entity.shapes[s] as Circle2D;
					GameObject go = AddCircleShape(circle.originOffset, circle.originRadius);
					go.name = "entityShape";
				}
				else if(entity.shapes[s].type == Shape2D.Type.ConvexPolygon)
				{
					ConvexPolygon2D polygon = entity.shapes[s] as ConvexPolygon2D;

					Vector2[] vertices = new Vector2[polygon.origin.Length];
					for(int i = 0; i < polygon.origin.Length; i++)
					{
						vertices[i].x = polygon.origin[i].y;
						vertices[i].y = polygon.origin[i].x;
					}

					GameObject go = AddPolygonShape(vertices);
					go.name = "entityShape";
				}
			}
		}

		public GameObject AddCircleShape(Vector2 pos, Fixed radius)
		{
			GameObject go = CreateSprite(Render.GetTexture("textures/circle25"));
			go.transform.parent = this.transform;
			go.transform.position = this.transform.position;
			go.transform.rotation = this.transform.rotation;

			//!set local transform after parent link
			go.transform.localPosition = new Vector3((float)pos.x, 0, -(float)pos.y);
			go.transform.localRotation = Quaternion.LookRotation(new Vector3(0, 1, 0));
			go.transform.localScale = new Vector3((float)radius, (float)radius, (float)radius);

			SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
			sr.material = new Material(Render.GetMaterial("VLit"));
			sr.material.color = UnityEngine.Color.magenta;

			return go;
		}

		public GameObject AddPolygonShape(Vector2[] vertices)
		{
			GameObject go = CreateLinePolygon(vertices, (Fixed)1 / 20);
			go.transform.parent = this.transform;
			go.transform.position = this.transform.position;
			go.transform.rotation = this.transform.rotation;

			MeshRenderer mr = go.GetComponent<MeshRenderer>();
			mr.material = new Material(Render.GetMaterial("VLit"));
			mr.material.color = UnityEngine.Color.magenta;

			return go;
		}

		public static GameObject CreateSprite(Texture2D texture, UnityEngine.Color color = default(UnityEngine.Color))
		{
			GameObject go = new GameObject();

			SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
			sr.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new UnityEngine.Vector2(0.5f, 0.5f), texture.width / 2);
			sr.color = color;
			sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			sr.receiveShadows = false;

			return go;
		}

		public static GameObject CreateLinePolygon(Vector2[] v, Fixed thickness, UnityEngine.Color color = default(UnityEngine.Color))
		{
			GameObject go = new GameObject();

			MeshFilter mf = go.AddComponent<MeshFilter>();
			Mesh mesh = new Mesh();
			mf.mesh = mesh;

			Func<Vector2, Vector3> v2v3 = (v2) => new Vector3((float)v2.x, 0, (float)v2.y);

			Func<Vector2, Vector2, Vector2, Vector2, Vector2> intersect = (l1v, l1u, l2v, l2u) =>
			{
				//Line1
				Fixed A1 = l1u.y - l1v.y;
				Fixed B1 = l1v.x - l1u.x;
				Fixed C1 = A1 * l1v.x + B1 * l1v.y;

				//Line2
				Fixed A2 = l2u.y - l2v.y;
				Fixed B2 = l2v.x - l2u.x;
				Fixed C2 = A2 * l2v.x + B2 * l2v.y;

				Fixed det = A1 * B2 - A2 * B1;

				if(Math.Approximately(det, 0)) //parallel lines
					return l1u;
				else
					return Vector2.V((B2 * C1 - B1 * C2) / det, (A1 * C2 - A2 * C1) / det);
			};

			int len = v.Length;
			Vector3[] vertices = new Vector3[len * 2];
			for(int i = 0; i < len; i++)
			{
				Vector2 v0 = v[i], u0 = v[(i + 1) % len], n0 = (u0 - v0).Left.Normalize;
				Vector2 v1 = u0, u1 = v[(i + 2) % len], n1 = (u1 - v1).Left.Normalize;
				Vector2 p0 = intersect(v0 + n0 * thickness, u0 + n0 * thickness, v1 + n1 * thickness, u1 + n1 * thickness);
				Vector2 p1 = intersect(v0 - n0 * thickness, u0 - n0 * thickness, v1 - n1 * thickness, u1 - n1 * thickness);
				vertices[((i + 1) % len) * 2 + 0] = v2v3(p0);
				vertices[((i + 1) % len) * 2 + 1] = v2v3(p1);
			}
			mesh.vertices = vertices;

			int len2 = len * 2;
			int[] tri = new int[len * 6];
			for(int i = 0; i < len; i++)
			{
				tri[(i * 6) + 0] = (i * 2 + 0) % len2;
				tri[(i * 6) + 1] = (i * 2 + 2) % len2;
				tri[(i * 6) + 2] = (i * 2 + 1) % len2;

				tri[(i * 6) + 3] = (i * 2 + 2) % len2;
				tri[(i * 6) + 4] = (i * 2 + 3) % len2;
				tri[(i * 6) + 5] = (i * 2 + 1) % len2;
			}
			mesh.triangles = tri;

			Vector3[] n3 = new Vector3[len * 2];
			for(int i = 0; i < len; i++)
				n3[i] = Vector3.up;
			mesh.normals = n3;

			UnityEngine.Vector2[] uv = new UnityEngine.Vector2[len * 2];
			for(int i = 0; i < len; i++)
			{
				if(i % 2 == 0)
				{
					uv[(i * 2) + 0] = new UnityEngine.Vector2(0, 0);
					uv[(i * 2) + 1] = new UnityEngine.Vector2(1, 0);
				}
				else
				{
					uv[(i * 2) + 0] = new UnityEngine.Vector2(0, 1);
					uv[(i * 2) + 1] = new UnityEngine.Vector2(1, 1);
				}
			}
			mesh.uv = uv;

			MeshRenderer mr = go.AddComponent<MeshRenderer>();
			mr.material = Render.GetMaterial("SpritesDefault");
			mr.material.color = color;
			mr.receiveShadows = false;
			mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

			return go;
		}
	}
}