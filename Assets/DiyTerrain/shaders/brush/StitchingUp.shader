Shader "Unlit/StitchingUp"
{
	Properties
	{
		_HeightTex ("Height", 2D) = "white" {}
		_NeighborHeightTex ("Neighbor Height", 2D) = "white" {}
		_uvDir ("Neighbor uv dir", Vector) = (1.0,0.0,0.0,1.0)
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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				// sample the texture
				

				return float4(1.0,0.0,0.0,1.0);
			}
			ENDCG
		}
	}
}
