// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/fetchWaveData" {
	Properties{
		_W ("WaterWidth",Float) = 222.3952
		_H ("WaterHeight", Float) = 202.5132
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags { "RenderType"="Opaque"}


			CGPROGRAM
			#pragma surface surf Standard  vertex:vert
			//#pragma vertex vert
			//#pragma fragment frag

			struct Input {
				float2 a;
			};

			sampler2D _MainTex;

			float _W;
			float _H;
			float _A;
			float _Metallic;

			void vert(inout appdata_full v, out Input data)
			{
				UNITY_INITIALIZE_OUTPUT(Input,data);
				float2 uv;
				uv.x = v.vertex.x /_W;//分子和分母都是負值
				uv.y = v.vertex.y /_H;

				float4 height = tex2Dlod (_MainTex, float4(uv,0,0));
						
				v.vertex.z =height;
				//似乎會自己計算normal
					
			}

			void surf (Input IN, inout SurfaceOutputStandard o) {
				o.Albedo= fixed4(0.72,1,1,1);
				o.Metallic =_Metallic;
				o.Smoothness=1.0f;
			}

		
			ENDCG
		}
		FallBack "Diffuse"
}