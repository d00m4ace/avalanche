Shader "VEmission"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Sprite Texture", 2D) = "white" {}
	}

		Category
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Always
			Fog { Color(0,0,0,0) }

		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader
		{
			Pass
			{
				SetTexture[_MainTex]
				{
					combine texture * primary Double, texture * primary
				}
			}
		}
	}
}
