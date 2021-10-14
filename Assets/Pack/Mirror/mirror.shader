
Shader "Custom/mirror" {
	Properties{
		_MirrorTex("MirrorTex", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				uniform float4x4 _mirror_camera_vp;

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

				sampler2D _MirrorTex;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					float4x4 M = mul(_mirror_camera_vp,unity_ObjectToWorld);
					o.uv = mul(M, v.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float2 uv = i.uv/ i.uv.w;
		
					//remapping from NDC to uv
					//opengl'uv
					uv.x = (uv.x + 1.0f)*0.5f;
					uv.y = (uv.y + 1.0f)*0.5f;

					fixed4 col = tex2D(_MirrorTex, uv);

					return col;
				}
				ENDCG
			}
		}
		FallBack "Diffuse"
}
