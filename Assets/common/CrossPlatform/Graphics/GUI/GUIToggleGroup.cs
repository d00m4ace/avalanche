using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIToggleGroup : GUILabel
	{
		public int toggle;

		public GUIToggleGroup(GUIElement[] elements = default(GUIElement[]), Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), ContentLayout contentLayout = ContentLayout.Horizontal, int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(elements, style, animation, contentLayout, width, height, minWidth, minHeight, maxWidth, maxHeight)
		{
			SetToggle(0);
		}

		public void SetToggle(int toggle)
		{
			this.toggle = toggle = toggle % elements.Count;

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				((GUIToggle)elements[i]).SetToggle(elements[i] == elements[toggle] ? 1 : 0);
		}

		public override void OnGUI()
		{
			base.OnGUI();

			int togglePushed = -1;

			if(GUI.buttonPushed != null)
				for(int i = 0; i < elements.Count; i++)
					if(GUI.buttonPushed == elements[i])
						togglePushed = i;

			if(togglePushed != -1)
				SetToggle(togglePushed);
		}
	}
}