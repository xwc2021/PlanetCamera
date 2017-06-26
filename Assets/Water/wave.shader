// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/wave" {
	Properties{
		_HS ("Wave Height Scale",Float) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};


				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					
					o.uv=v.uv;
					return o;
				}

				float _HS;
				float _LH;
				float4 frag(v2f i) : SV_Target
				{

					float h1 =5*_HS;
					float h2 =_HS;
					float twoPI=6.28f; 
					float4 value;
					value.r=h1*sin(2.0*twoPI*(i.uv.y+_Time));
					value.r+=h2*sin(6.0*twoPI*(i.uv.x+i.uv.y+4.0f*_Time));//45度角方向的wave
					value.r= max(value.r,_LH);
					return value;
				}
				ENDCG
			}
		}
		FallBack "Diffuse"
}
