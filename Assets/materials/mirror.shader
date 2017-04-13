// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/mirror" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				uniform float4x4 _mirror_camera_mvp;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float4 uv : TEXCOORD0;
				};

				sampler2D _MainTex;

				fixed4 _Color;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = mul(_mirror_camera_mvp, v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float2 uv = i.uv/ i.uv.w;
		
					//remapping from NDC to uv
					//opengl'uv
					uv.x = (uv.x + 1.0f)*0.5f;
					uv.y = (uv.y + 1.0f)*0.5f;

					fixed4 col = tex2D(_MainTex, uv);

					return col;
				}
				ENDCG
			}
		}
		FallBack "Diffuse"
}
