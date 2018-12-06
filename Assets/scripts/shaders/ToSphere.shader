Shader "Unlit/ToSphere"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
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
				float3 nV = normalize(v);
				float R=510.0;
				v = (R)*nV-_local_pos;

				// Todo: 讀取高度圖
				

				v2f o;
				o.vertex = UnityObjectToClipPos(v);
				o.uv = TRANSFORM_TEX(V.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return float4(col.rgb,1.0);
			}
			ENDCG
		}
	}
}
