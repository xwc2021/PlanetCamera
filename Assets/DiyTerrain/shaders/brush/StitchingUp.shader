﻿Shader "Unlit/StitchingUp"
{
	Properties
	{
		_HeightTex ("Height", 2D) = "white" {}
		_NeighborHeightTex ("Neighbor Height", 2D) = "white" {}
		_neibhborUV ("Neighbor UV ", Vector) = (1.0,0.0,0.0,1.0)
		_neibhborOriginal ("Neighbor Original", Vector) = (0.0,0.0,0.0,0.0)
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
			float4 _neibhborUV;
			float2 _neibhborOriginal;
			float _scale;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float2 uv =1.0-v.uv; // plane的uv 右上角是(0,0)
				o.uv.xy=uv;

				return o;
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float2 uv =i.uv;
				float2 n_uv =uv*2.0-1.0;
				float scale =_scale/102.4;
				float newV = frac(1.0+(0.5*scale*n_uv.y));
				float2 newUV=float2(uv.x,newV);
				// return float4((newV%0.25),0.0,0.0,1.0);

				float h =0.0;
				if(uv.y>0.5)
					h=tex2D (_NeighborHeightTex,newUV);
				else
					h=tex2D (_HeightTex,newUV);

				// 因為TextureWrapMode 是Clamp
				float border_self_h=tex2D (_HeightTex,float2(newUV.x,1.0f));
				float border_neighbor_h=tex2D (_NeighborHeightTex,float2(newUV.x,0.0f));
				float mHeight =0.5*(border_self_h+border_neighbor_h);

				float a=1.0-abs(n_uv.y);
				float weight = a*a;

				float diff = mHeight-border_self_h;
				float finalH =h+weight*diff;
				
				return float4(finalH,0.0,0.0,1.0);

				// return float4(weight,0.0,0.0,1.0);
			}
			ENDCG
		}
	}
}
