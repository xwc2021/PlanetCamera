// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/fetchWaveData" {
	Properties{
		_A ("Transparent",Float) = 0.2
		_W ("WaterWidth",Float) = 222.3952
		_H ("WaterHeight", Float) = 202.5132
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha

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

				sampler2D _MainTex;

				float _W;
				float _H;
				float _A;

				v2f vert(appdata v)
				{
					float2 uv;
					uv.x = v.vertex.x /_W;//分子和分母都是負值
					uv.y = v.vertex.y /_H;

					float4 height = tex2Dlod (_MainTex, float4(uv,0,0));
					v2f o;
					o.vertex = v.vertex;
					o.vertex.z =height;
					o.vertex = UnityObjectToClipPos(o.vertex);
					return o;
				}

				

				float4 frag(v2f i) : SV_Target
				{
					return float4(0.72,1,1,_A);
				}
				ENDCG
			}
		}
		FallBack "Diffuse"
}