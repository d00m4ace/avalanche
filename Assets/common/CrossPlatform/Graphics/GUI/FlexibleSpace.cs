using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class FlexibleSpace : GUIElement
	{
		public FlexibleSpace(int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
		}

		public override void CalcWidth()
		{
			width = 0;
		}

		public override void CalcHeight()
		{
			height = 0;
		}
	}
}