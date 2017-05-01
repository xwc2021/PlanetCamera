
Shader "Custom/fetchWaveDataMirror" {
	Properties{	
		_W ("WaterWidth",Float) = -256
		_H ("WaterHeight", Float) = 256
		_WaveTex("WaveTex", 2D) = "white" {}
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

				float _W;
				float _H;
				sampler2D _WaveTex;
				sampler2D _MirrorTex;

				v2f vert(appdata v)
				{
					//get height
					float2 uv;
					uv.x = v.vertex.x /_W;//分子和分母都是負值
					uv.y = v.vertex.y /_H;
					float height = tex2Dlod (_WaveTex, float4(uv,0,0)).r;
					v.vertex.z +=height;

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
