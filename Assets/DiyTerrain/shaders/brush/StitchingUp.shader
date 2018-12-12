Shader "Unlit/StitchingUp"
{
	Properties
	{
		_HeightTex ("Height", 2D) = "white" {}
		_NeighborHeightTex ("Neighbor Height", 2D) = "white" {}
		_uvDir ("Neighbor uv dir", Vector) = (1.0,0.0,0.0,1.0)
		_scale ("scale", float) = 6.4
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
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
			};

			sampler2D _HeightTex;
			sampler2D _NeighborHeightTex;
			float _uvDir;
			float _scale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float2 uv =1.0-v.uv; // plane的uv 右上角是(0,0)
				float newV = (1.0+(_scale*(v.vertex.z)/1024.0)) %1.0;
				o.uv.x = uv.x;
				o.uv.y = newV;
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				tex2D (_HeightTex,i.uv);

				return float4(i.uv.y,0.0,0.0,1.0);
			}
			ENDCG
		}
	}
}
