using System;
using System.Collections;
using System.Collections.Generic;

#if !SERVER
using UnityEngine;
#endif

namespace HEXPLAY
{
	public class GUIWindow : GUIElement
	{
		GUIStyle back;
		GUIStyle backShine;

		public List<GUIElement> elements;

		public int spacing = 1;
		public int paddingWidth, paddingHeight;

		public int xOffset = 0, yOffset = 0;

		public Color backColor;

		public enum Align
		{
			None,
			TopLeft,
			TopCenter,
			TopRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			BottomLeft,
			BottomCenter,
			BottomRight,
		};

		public Align align;

		public enum VerticalAlign
		{
			Center,
			Left,
			Right,
		};

		public VerticalAlign verticalAlign;

		public bool contentEqualWidth;
		public bool contentEqualHeight;

		public bool noClip;

		public bool focusWindow = false;

		public Color shadowBackTextureColor;
		public Texture2D shadowBackTexture;

		public ContentLayout contentLayout;
		public WidthLayout widthLayout;
		public HeightLayout heightLayout;

		public int fixedWidth;
		public int fixedHeight;

		public enum ContentLayout
		{
			Horizontal,
			Vertical,
		};

		public enum WidthLayout
		{
			ContentWidth,
			FixedWidth,
			PercentWidth,
		};

		public enum HeightLayout
		{
			ContentHeight,
			FixedHeight,
			PercentHeight,
		};

		public GUIWindow(GUIElement[] elements = default(GUIElement[]), Game.GUIStyle style = Game.GUIStyle.Default, GUIAnimation animation = default(GUIAnimation), int width = -1, int height = -1, int minWidth = int.MinValue, int minHeight = int.MinValue, int maxWidth = int.MaxValue, int maxHeight = int.MaxValue) : base(minWidth, minHeight, maxWidth, maxHeight)
		{
			if(animation != null)
				this.animation = animation;

			this.elements = elements != null ? new List<GUIElement>(elements) : new List<GUIElement>();

			SetWidthLayout(WidthLayout.ContentWidth);
			SetHeightLayout(HeightLayout.ContentHeight);

			this.style = style;
			SetStyle();

			if(width != -1)
				this.minWidth = this.maxWidth = width;
			if(height != -1)
				this.minHeight = this.maxHeight = height;
		}

		public override bool IsPlayingAnimation()
		{
			if(isDisabled)
				return false;

			if(animation.IsPlaying())
				return true;

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				if(!elements[i].isDisabled && elements[i].IsPlayingAnimation())
					return true;

			return false;
		}

		public override void StopgAnimation()
		{
			animation.Stop();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].StopgAnimation();
		}

		public override void ResumeAnimation()
		{
			animation.Resume();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].ResumeAnimation();
		}

		public override void RestartAnimation()
		{
			animation.Restart();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].RestartAnimation();
		}

		public override void PlayInverseAnimation()
		{
			animation.PlayInverse();

			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].PlayInverseAnimation();
		}

		public void SetAlign(Align align)
		{
			this.align = align;
		}

		public void SetVerticalAlign(VerticalAlign verticalAlign)
		{
			this.verticalAlign = verticalAlign;
		}

		public void SetShadowBackTexture(string image)
		{
			shadowBackTexture = Render.GetTexture(image);
		}

		public override void SetStyle()
		{
			back = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].windowBackStyle];
			backShine = Render.guiSkin.customStyles[(int)GUI.styles[(int)style].windowBackShineStyle];

			backColor = GUI.styles[(int)style].windowBackColor;

			shadowBackTextureColor = GUI.styles[(int)style].windowShadowTextureColor;
			shadowBackTexture = GUI.styles[(int)style].windowShadowTexture;

			spacing = GUI.styles[(int)style].windowSpacing;

			minWidth = GUI.styles[(int)style].windowMinWidth;
			minHeight = GUI.styles[(int)style].windowMinHeight;
			maxWidth = GUI.styles[(int)style].windowMaxWidth;
			maxHeight = GUI.styles[(int)style].windowMaxHeight;
		}

		public void SetContentLayout(ContentLayout contentLayout)
		{
			this.contentLayout = contentLayout;
		}

		public void SetWidthLayout(WidthLayout widthLayout, int fixedWidth = 100)
		{
			this.widthLayout = widthLayout;
			this.fixedWidth = fixedWidth;
		}

		public void SetHeightLayout(HeightLayout heightLayout, int fixedHeight = 100)
		{
			this.heightLayout = heightLayout;
			this.fixedHeight = fixedHeight;
		}

		public void Add(GUIElement element)
		{
			elements.Add(element);
		}

		public override void RestSize()
		{
			width = height = 0;
			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].RestSize();
		}

		public void FreeAllElements()
		{
			while(elements.Count > 0)
			{
				elements[0] = null;
				elements.RemoveAt(0);
			}
		}

		public override void CalcWidth()
		{
			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].CalcWidth();

			if(contentEqualWidth)
			{
				int w = 0;

				ic = elements.Count;
				for(int i = 0; i < ic; i++)
				{
					int itemWidth = elements[i].GetWidth();
					w = itemWidth > w ? itemWidth : w;
				}

				ic = elements.Count;
				for(int i = 0; i < ic; i++)
					elements[i].width = w;
			}

			if(width == 0)
			{
				if(widthLayout == WidthLayout.FixedWidth)
					width = fixedWidth;
				else if(widthLayout == WidthLayout.PercentWidth)
					width = (int)(fixedWidth * GUI.PeekGuiClip().width) / 100;
				else if(widthLayout == WidthLayout.ContentWidth)
				{
					if(contentLayout == ContentLayout.Horizontal)
					{
						ic = elements.Count;
						for(int i = 0; i < ic; i++)
							width += elements[i].GetWidth() + spacing;
						if(elements.Count > 0) width -= spacing;
					}
					else
					{
						ic = elements.Count;
						for(int i = 0; i < ic; i++)
						{
							int itemWidth = elements[i].GetWidth();
							width = itemWidth > width ? itemWidth : width;
						}
					}

					width += back.border.left + back.border.right + paddingWidth * 2;
				}
			}

			{
				int flexibleSpaceCount = 0;
				int contentWidth = 0;

				ic = elements.Count;
				for(int i = 0; i < ic; i++)
				{
					if(elements[i] is FlexibleSpace)
					{
						flexibleSpaceCount++;
						continue;
					}

					if(contentLayout == ContentLayout.Horizontal)
						contentWidth += elements[i].GetWidth() + spacing;
					else
					{
						int itemWidth = elements[i].GetWidth();
						contentWidth = itemWidth > contentWidth ? itemWidth : contentWidth;
					}
				}

				if(flexibleSpaceCount != 0)
				{
					int flexibleSpaceWidth = (GetWidth() - contentWidth - back.border.left - back.border.right - paddingWidth * 2) / flexibleSpaceCount;

					if(contentLayout == ContentLayout.Horizontal)
						flexibleSpaceWidth -= spacing;

					if(flexibleSpaceWidth < 0)
						flexibleSpaceWidth = 0;

					ic = elements.Count;
					for(int i = 0; i < ic; i++)
						if(elements[i] is FlexibleSpace)
							elements[i].width = flexibleSpaceWidth;
				}
			}
		}

		public override void CalcHeight()
		{
			int ic = elements.Count;
			for(int i = 0; i < ic; i++)
				elements[i].CalcHeight();

			if(contentEqualHeight)
			{
				int h = 0;

				ic = elements.Count;
				for(int i = 0; i < ic; i++)
				{
					int itemHeight = elements[i].GetHeight();
					h = itemHeight > h ? itemHeight : h;
				}

				ic = elements.Count;
				for(int i = 0; i < ic; i++)
					elements[i].height = h;
			}

			if(height == 0)
			{
				if(heightLayout == HeightLayout.FixedHeight)
					height = fixedHeight;
				else if(heightLayout == HeightLayout.PercentHeight)
					height = (int)(fixedHeight * GUI.PeekGuiClip().height) / 100;
				else if(heightLayout == HeightLayout.ContentHeight)
				{
					if(contentLayout == ContentLayout.Horizontal)
					{
						ic = elements.Count;
						for(int i = 0; i < ic; i++)
						{
							int itemHeight = elements[i].GetHeight();
							height = itemHeight > height ? itemHeight : height;
						}
					}
					else
					{
						ic = elements.Count;
						for(int i = 0; i < ic; i++)
							height += elements[i].GetHeight() + spacing;
						if(elements.Count > 0) height -= spacing;
					}

					height += back.border.top + back.border.bottom + paddingHeight * 2;
				}
			}

			{
				int flexibleSpaceCount = 0;
				int contentHeight = 0;

				ic = elements.Count;
				for(int i = 0; i < ic; i++)
				{
					if(elements[i] is FlexibleSpace)
					{
						flexibleSpaceCount++;
						continue;
					}

					if(contentLayout == ContentLayout.Horizontal)
					{
						int itemHeight = elements[i].GetHeight();
						contentHeight = itemHeight > contentHeight ? itemHeight : contentHeight;
					}
					else
						contentHeight += elements[i].GetHeight() + spacing;
				}

				if(flexibleSpaceCount != 0)
				{
					int flexibleSpaceHeight = (GetHeight() - contentHeight - back.border.top - back.border.bottom - paddingHeight * 2) / flexibleSpaceCount;

					if(contentLayout == ContentLayout.Vertical)
						flexibleSpaceHeight -= spacing;

					if(flexibleSpaceHeight < 0)
						flexibleSpaceHeight = 0;

					ic = elements.Count;
					for(int i = 0; i < ic; i++)
						if(elements[i] is FlexibleSpace)
							elements[i].height = flexibleSpaceHeight;
				}
			}
		}

		public override void OnGUI()
		{
			animation.OnGUIBegin();

			Color guiBaseColor = GUI.baseColor;
			GUI.baseColor = GUI.baseColor * animation.AnimateBaseColor(color);

			CalcWidth();
			CalcHeight();

			if(shadowBackTexture != null)
			{
				GUI.SetColor(shadowBackTextureColor);
				Rect rect = GUI.PeekGuiClip();
				rect.x = 0; rect.y = 0;
				UnityEngine.GUI.DrawTextureWithTexCoords(rect, shadowBackTexture, new Rect(0, 0, rect.width / (shadowBackTexture.width * GUI.scale), rect.height / (shadowBackTexture.height * GUI.scale)));
			}

			int w = animation.AnimateW(GetWidth());
			int h = animation.AnimateH(GetHeight());

			switch(align)
			{
				case Align.None:
				break;
				case Align.TopLeft:
				x = (int)GUI.PeekGuiClip().x;
				y = (int)GUI.PeekGuiClip().y;
				break;
				case Align.TopCenter:
				x = (int)GUI.PeekGuiClip().x + (int)(GUI.PeekGuiClip().width - w) / 2;
				y = (int)GUI.PeekGuiClip().y;
				break;
				case Align.TopRight:
				x = (int)GUI.PeekGuiClip().x + (int)(GUI.PeekGuiClip().width - w);
				y = (int)GUI.PeekGuiClip().y;
				break;
				case Align.MiddleLeft:
				x = (int)GUI.PeekGuiClip().x;
				y = (int)GUI.PeekGuiClip().y + (int)(GUI.PeekGuiClip().height - h) / 2;
				break;
				case Align.MiddleCenter:
				x = (int)GUI.PeekGuiClip().x + (int)(GUI.PeekGuiClip().width - w) / 2;
				y = (int)GUI.PeekGuiClip().y + (int)(GUI.PeekGuiClip().height - h) / 2;
				break;
				case Align.MiddleRight:
				x = (int)GUI.PeekGuiClip().x + (int)(GUI.PeekGuiClip().width - w);
				y = (int)GUI.PeekGuiClip().y + (int)(GUI.PeekGuiClip().height - h) / 2;
				break;
				case Align.BottomLeft:
				x = (int)GUI.PeekGuiClip().x;
				y = (int)GUI.PeekGuiClip().y + (int)(GUI.PeekGuiClip().height - h);
				break;
				case Align.BottomCenter:
				x = (int)GUI.PeekGuiClip().x + (int)(GUI.PeekGuiClip().width - w) / 2;
				y = (int)GUI.PeekGuiClip().y + (int)(GUI.PeekGuiClip().height - h);
				break;
				case Align.BottomRight:
				x = (int)GUI.PeekGuiClip().x + (int)(GUI.PeekGuiClip().width - w);
				y = (int)GUI.PeekGuiClip().y + (int)(GUI.PeekGuiClip().height - h);
				break;
			}

			Rect pos = new Rect(animation.AnimateX(x) + xOffset, animation.AnimateY(y) + yOffset, w, h);

			if(back.normal.background != null)
			{
				GUI.SetColor(animation.AnimateBackColor(backColor));
				UnityEngine.GUI.Box(pos, "", back);
			}

			if(backShine.normal.background != null)
			{
				GUI.SetColor(Color.White);
				UnityEngine.GUI.Box(pos, "", backShine);
			}

			Rect window = new Rect(pos.x + back.border.left + paddingWidth, pos.y + back.border.top + paddingHeight, w - back.border.left - back.border.right - paddingWidth * 2, h - back.border.top - back.border.bottom - paddingHeight * 2);

			if(!noClip)
				GUI.BeginClip(window);

			int xOffs = 0, yOffs = 0;

			if(noClip)
			{
				xOffs = (int)window.x;
				yOffs = (int)window.y;
			}

			if(contentLayout == ContentLayout.Horizontal)
			{
				int elementsWidth = 0;
				int ic = elements.Count;
				for(int i = 0; i < ic; i++)
					elementsWidth += elements[i].GetWidth() + spacing;

				if(elementsWidth > window.width)
					elementsWidth = (int)window.width;

				xOffs += (int)(window.width - elementsWidth) / 2;

				List<GUIElement> anims = animation.AnimateElements(elements);
				ic = anims.Count;
				for(int i = 0; i < ic; i++)
				{
					anims[i].SetPos(xOffs, yOffs + (height - back.border.top - back.border.bottom - paddingHeight * 2 - anims[i].GetHeight()) / 2);
					if(!anims[i].isDisabled)
						anims[i].OnGUI();
					xOffs += anims[i].GetWidth() + spacing;
				}
			}
			else if(contentLayout == ContentLayout.Vertical)
			{
				int elementsHeight = 0;
				int ic = elements.Count;
				for(int i = 0; i < ic; i++)
					elementsHeight += elements[i].GetHeight() + spacing;

				if(elementsHeight > window.height)
					elementsHeight = (int)window.height;

				yOffs += (int)(window.height - elementsHeight) / 2;

				List<GUIElement> anims = animation.AnimateElements(elements);
				ic = anims.Count;
				for(int i = 0; i < ic; i++)
				{
					if(verticalAlign == VerticalAlign.Center)
						anims[i].SetPos(xOffs + (width - back.border.left - back.border.right - paddingWidth * 2 - anims[i].GetWidth()) / 2, yOffs);
					else if(verticalAlign == VerticalAlign.Left)
						anims[i].SetPos(xOffs + back.border.left + paddingWidth, yOffs);
					else
						anims[i].SetPos(xOffs + width - back.border.right - paddingWidth - anims[i].GetWidth(), yOffs);

					if(!anims[i].isDisabled)
						anims[i].OnGUI();

					yOffs += anims[i].GetHeight() + spacing;
				}
			}

			if(!noClip)
				GUI.EndClip();

			GUI.baseColor = guiBaseColor;

			animation.OnGUIEnd();
		}
	}
}