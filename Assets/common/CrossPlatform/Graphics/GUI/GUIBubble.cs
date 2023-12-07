using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIBubble
	{
		public float startTime, lifeTime;
		public int x, y;
		public GUIElement element;

		public Vector2 pos;
		public Fixed z;
		public bool posToScreenOnGUI;

		public GUIBubble(GUIElement element, float lifeTime, int x, int y)
		{
			this.lifeTime = lifeTime;
			this.element = element;
			SetPos(x, y);
		}

		public GUIBubble(GUIElement element, float lifeTime, Vector2 pos, Fixed z, bool posToScreenOnGUI = true)
		{
			this.lifeTime = lifeTime;
			this.element = element;
			this.posToScreenOnGUI = posToScreenOnGUI;
			SetPos(pos, z);
		}

		public virtual void SetPos(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public virtual void SetPos(Vector2 pos, Fixed z)
		{
			this.pos = pos;
			this.z = z;
			PosToScreen();
		}

		public virtual void PosToScreen()
		{
			Vector3 v = new Vector3((float)pos.x, (float)z, (float)pos.y);
			Vector3 s = Camera.main.WorldToScreenPoint(v);
			s.y = Screen.height - s.y;
			SetPos((int)(s.x / GUI.scale), (int)(s.y / GUI.scale));
		}

		public virtual void OnGUI()
		{
			if(startTime == 0)
			{
				startTime = Time.time;
				element.CalcWidth();
				element.CalcHeight();
			}

			if(posToScreenOnGUI)
				PosToScreen();

			element.SetPos(x - element.GetWidth() / 2, y - element.GetHeight() / 2);

			element.OnGUI();
		}

		public virtual void RestSize()
		{
			element.RestSize();
		}

		public virtual bool IsAlive()
		{
			return lifeTime < 0 || Time.time - startTime < lifeTime;
		}
	}
}