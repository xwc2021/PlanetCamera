Shader "Unlit/BehindTheWallUseMaskAndDepthTexture"
{
	Properties
	{
		_BaseColor ("BaseColor",Color) =(0,1,0,1)
	}
	SubShader
	{
		Tags { "Queue" = "Transparent-2" "RenderType"="Opaque" }
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 vertex2 : TEXCOORD1;
			};

			sampler2D _DepthTexture;
			float4 _BaseColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex2 = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//SV_POSITION不給用
				float2 uv = i.vertex2 / i.vertex2.w;
				uv.x = (uv.x + 1.0f)*0.5f;
				uv.y = (uv.y + 1.0f)*0.5f;

				float zBuffer = tex2D(_DepthTexture, uv);
						
				float z = (i.vertex2.z / i.vertex2.w);
				z = (z + 1.0f)*0.5f;
				float bias = 0.001f;
				//靠近Camera是0，遠離是1
				z -= bias;//往鏡頭靠近，才不會判定在身體後面
				if (z < zBuffer)
					discard;

				return _BaseColor;
			}
			ENDCG
		}
	}
}
