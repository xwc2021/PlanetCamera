Shader "Unlit/BehindTheWallMask"
{
	Properties
	{
		_TestColor("TestColor",Color) = (1,0,0,1)
	}
	SubShader
	{
		//Tags { "Queue"="Geometry+49" "RenderType" = "Opaque" }
		ColorMask 0 //不寫入FrameBuffer
		ZWrite Off
		ZTest Less
		Stencil{
			Ref 1   
			Comp  Always
			Pass Replace
		}

		CGINCLUDE
		struct appdata
		{
			float4 vertex : POSITION;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			return o;
		}

		fixed4 _TestColor;

		fixed4 frag(v2f i) : SV_Target
		{
			return _TestColor;
		}

		// MRT shader
		struct FragmentOutput
		{
			fixed4 dest0 : SV_Target0;
		};

		//https://github.com/keijiro/UnityMrtTest/blob/master/Assets/MrtTest.shader
		FragmentOutput frag_mrt(v2f i) : SV_Target
		{
			FragmentOutput o;
			o.dest0 = _TestColor;
			return o;
		}

		ENDCG

		Pass
		{
			Tags{ "LightMode" = "Deferred" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_mrt
			ENDCG
		}
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				ENDCG
		}

		
	}
}
