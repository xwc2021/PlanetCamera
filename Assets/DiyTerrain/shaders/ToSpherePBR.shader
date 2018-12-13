// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ToSpherePBR" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_HeightTex ("Height Texture", 2D) = "white" {}
		_BoxWidth ("BoxWidth", float) = 1023.0
		_resolution ("resolution", float) = 1024.0

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert fullforwardshadows
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		
		sampler2D _HeightTex;
		float _BoxWidth;
		float _resolution;

		uniform float3 _local_pos;

		struct Input {
			float2 uv_MainTex;
			float height;
		};

		void vert(inout appdata_full V, out Input o)
		{
			float3 v = V.vertex+_local_pos; // 計算normal的原點，是parent物件的中心點
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
			// v = (R)*nV-_local_pos;
			v = (R+h*256.0f)*nV-_local_pos;
			V.vertex.xyz =v; // Unity會自動作MVP
			V.normal = nV;

			// 平面
			// v.y = h*128.0f;
			// v =V.vertex;

			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.height =h;
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			// float h= sin(160.0*IN.height);
			// o.Albedo = float4(h,h,h,1.0);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
