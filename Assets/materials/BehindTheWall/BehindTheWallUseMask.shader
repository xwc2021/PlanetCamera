Shader "Unlit/BehindTheWallUseMask"
{
	Properties
	{
		_BaseColor("BaseColor",Color) = (0,1,0,1)
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent-2" "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			ZTest Always
			ZWrite Off
			Stencil{
				Ref 1
				Comp notequal
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			sampler2D _DepthTexture;
			float4 _BaseColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return _BaseColor;
			}
			ENDCG
		}
	}
}
