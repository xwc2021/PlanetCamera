Shader "Unlit/ToSphere"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_HeightTex ("Height Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members normal)
			#pragma exclude_renderers d3d11
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"


			uniform float3 _local_pos;

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float height: TEXCOORD1;
				float2 index: TEXCOORD2;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _HeightTex;
			float4 _MainTex_ST;

			float fract(float x){
				return x-floor(x);
			}

			float N(float t) {
				return fract(sin(t*12345.564)*7658.76);
			}

			
			v2f vert (appdata V)
			{
				float3 v = V.vertex+_local_pos;

				// Todo: 讀取高度圖
				
				float resolution =1024.0;
				float halfResolution =0.5*resolution;
				float onePixel = 1.0/resolution;
				float halfPixel = 0.5*onePixel;
				float2 index =v.xz+float2(halfResolution,halfResolution);
				float2 hUV = float2(halfPixel,halfPixel)+index*float2(onePixel,onePixel);
				float h = tex2Dlod(_HeightTex, float4(hUV,0,0)).r;
				h =h*2.0-1.0; // remap to -1~1

				float3 nV = normalize(v);
				float R=510.0;
				//球面
				v = (R)*nV-_local_pos;
				v = (R+h*256.0f)*nV-_local_pos;

				// 平面
				// v =V.vertex;
				// v.y = h*128.0f;

				v2f o;
				o.vertex = UnityObjectToClipPos(v);
				o.uv = TRANSFORM_TEX(V.uv, _MainTex);
				// o.index =index;
				o.height =h;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				// float2 indexColor = i.index.xy % 2.0;
				float hColor = 0.5+0.5*sin(60.0*i.height);
				// return float4(hColor,hColor,hColor,1.0);
				float3 newColor=float3(0.5,0.5,0.5);
				return float4(newColor*col.rgb,1.0);
				
			}
			ENDCG
		}
	}
}
