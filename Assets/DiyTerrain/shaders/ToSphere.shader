Shader "Unlit/ToSphere"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_HeightTex ("Height Texture", 2D) = "white" {}
		_BoxWidth ("BoxWidth", float) = 1023.0
		_resolution ("resolution", float) = 1024.0
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
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _HeightTex;
			float _BoxWidth;
			float _resolution;
			
			v2f vert (appdata V)
			{
				float3 v = V.vertex+_local_pos;
				float halfResolution =0.5*_BoxWidth;
				float2 index =v.xz+float2(halfResolution,halfResolution);// 0~1023

				// Todo: 讀取高度圖
				
				float onePixel = 1.0/_resolution;
				float halfPixel = 0.5*onePixel;
				float2 hUV = float2(halfPixel,halfPixel)+index*float2(onePixel,onePixel);
				float h = tex2Dlod(_HeightTex, float4(hUV,0,0)).r;
				h =h*2.0-1.0; // remap to -1~1

				float3 nV = normalize(v);
				float R=1023.0*0.5;
				//球面
				v = (R)*nV-_local_pos;
				v = (R+h*256.0f)*nV-_local_pos;

				// 平面
				// v =V.vertex;
				// v.y = h*128.0f;

				v2f o;
				o.vertex = UnityObjectToClipPos(v);
				o.uv = V.uv;
				// o.index =index;
				o.height =h;
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
