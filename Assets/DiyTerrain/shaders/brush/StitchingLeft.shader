﻿Shader "Unlit/StitchingLeft"
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

			float2 trasformUV( float2 uv ){
				float2 x=_neibhborUV.xy;
				float2 y=_neibhborUV.zw;
				float2 diff = uv- _neibhborOriginal;
				return float2(dot(x,diff),dot(y,diff));
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float2 uv =i.uv;
				float2 n_uv =uv*2.0-1.0;
				float scale =_scale/102.4;
				float newU = (0.5*scale*n_uv.x);// 分兩半 >0 和 <0
				float2 newUV=float2(newU,uv.y);

				float h =0.0;
				if(uv.x>0.5)
					h=tex2D (_HeightTex ,newUV);
				else
					h=tex2D (_NeighborHeightTex,trasformUV(newUV));

				// 因為TextureWrapMode 是Clamp
				float2 borderPos =float2(0.0f,uv.y);
				float border_self_h=tex2D (_HeightTex,borderPos);
				float border_neighbor_h=tex2D (_NeighborHeightTex,trasformUV(borderPos));
				float mHeight =0.5*(border_self_h+border_neighbor_h);

				float a=1.0-abs(n_uv.x);
				float weight = a*a;
				
				float diff = mHeight-border_self_h;
				float finalH =h+weight*diff;
				return float4(finalH,0.0,0.0,1.0);
			}
			ENDCG
		}
	}
}
