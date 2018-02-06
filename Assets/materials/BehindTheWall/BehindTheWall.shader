Shader "Unlit/BehindTheWall"
{
	Properties
	{
		_BaseColor("BaseColor",Color) = (0,0,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent-2" "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			ZTest Always
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _BaseColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return _BaseColor;
			}
			ENDCG
		}
	}
}
