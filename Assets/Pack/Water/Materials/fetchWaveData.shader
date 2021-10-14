// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/fetchWaveData" {
	Properties{
		
		_dW ("diff Width",Float) =-1
		_dH ("diff Height", Float) = 1
		_W ("WaterWidth",Float) = -256
		_H ("WaterHeight", Float) = 256
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Smoothness ("Smoothness", Range(0,1)) = 0.0
		_WaveTex("WaveTex", 2D) = "white" {}
	}
		SubShader{
		Tags { "RenderType"="Opaque"}


			CGPROGRAM
			#pragma surface surf Standard  vertex:vert
			//#pragma vertex vert
			//#pragma fragment frag

			struct Input {
				fixed3 normal;
			};

			sampler2D _WaveTex;

			float _W;
			float _H;
			float _A;
			float _Metallic;
			float _Smoothness;

			float _dW;
			float _dH;

			void vert(inout appdata_full v, out Input data)
			{
				UNITY_INITIALIZE_OUTPUT(Input,data);
				float2 uv;
				uv.x = v.vertex.x /_W;//分子和分母都是負值
				uv.y = v.vertex.y /_H;

				float height = tex2Dlod (_WaveTex, float4(uv,0,0)).r;
						
				v.vertex.z +=height;
				//計算normal

				float2 uvDiff = float2(_dW,_dH)/float2(_W,_H);

				float2 right = float2(uvDiff.x,0);
				float2 left = float2(-uvDiff.x,0);
				float2 up = float2(0,uvDiff.y);
				float2 down = float2(0,-uvDiff.y);
				float right_height = tex2Dlod (_WaveTex, float4(uv+right,0,0)).r;
				float left_height = tex2Dlod (_WaveTex, float4(uv+left,0,0)).r;
				float up_height = tex2Dlod (_WaveTex, float4(uv+up,0,0)).r;
				float down_height = tex2Dlod (_WaveTex, float4(uv+down,0,0)).r;

				//因為_dW是負值
				float3 temp =cross(float3(-_dW,0,right_height)-float3(_dW,0,left_height),float3(0,_dH,up_height)-float3(0,-_dH,down_height));
				v.normal =normalize(temp);	
			}

			void surf (Input IN, inout SurfaceOutputStandard o) {
				o.Albedo= fixed4(0.72,1,1,1);
				o.Metallic =_Metallic;
				o.Smoothness=_Smoothness;
			}

		
			ENDCG
		}
		FallBack "Diffuse"
}