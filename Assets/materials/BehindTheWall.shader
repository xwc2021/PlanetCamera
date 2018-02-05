Shader "Unlit/BehindTheWall"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BaseColor ("BaseColor",Color) =(1,1,1,1)
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 vertex2 : TEXCOORD1;
			};

			sampler2D _DepthTexture;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _BaseColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex2 = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//SV_POSITION不給用
				float2 uv = i.vertex2 / i.vertex2.w;
				uv.x = (uv.x + 1.0f)*0.5f;
				uv.y = (uv.y + 1.0f)*0.5f;

				#if SHADER_API_D3D11
					uv.y = 1.0f - uv.y;//DX11 上下會反過來
				#endif

				
				float zBuffer = tex2D(_DepthTexture, uv);
						
#if SHADER_API_D3D11
				float z = (i.vertex2.z / i.vertex2.w);
				float bias = 0.005f;
				//靠近Camera是1，遠離是0
				z += bias;//往鏡頭靠近，才不會判定在身體後面
				if (z > zBuffer)
					discard;
#endif

#if SHADER_API_GLCORE
				float z = (i.vertex2.z / i.vertex2.w);
				z = (z + 1.0f)*0.5f;
				float bias = 0.005f;
				//靠近Camera是0，遠離是1
				z -= bias;//往鏡頭靠近，才不會判定在身體後面
				if (z < zBuffer)
					discard;
#endif

				return _BaseColor;
			}
			ENDCG
		}
	}
}
