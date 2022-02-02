Shader "Custom/ToSphere"
{
    Properties
    { 
        _MainTex("Base Map", 2D) = "white"
        _HeightTex ("Height Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline" }

        Pass
        {
            HLSLPROGRAM
            #define sphereMapping
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            
            struct Attributes
            {
                float4 vertex   : POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
                half3 normal        : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
                half3 normal        : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_HeightTex);
            SAMPLER(sampler_HeightTex);

            CBUFFER_START(UnityPerMaterial)
               float4 _MainTex_ST;
               float3 _local_pos;
            CBUFFER_END

            Varyings vert(Attributes V)
            {
                Varyings o;
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                o.uv = TRANSFORM_TEX(V.uv, _MainTex);

                float _BoxWidth = 1023.0;
				float _resolution = 1024.0;

                float3 v = V.vertex.xyz+_local_pos; // 計算normal的原點，是parent物件的中心點
				float halfResolution =0.5*_BoxWidth;
				float2 index =v.xz+float2(halfResolution,halfResolution);// 0~1023

				// 讀取高度圖
				
				float onePixel = 1.0/_resolution;
				float halfPixel = 0.5*onePixel;
				float2 hUV = float2(halfPixel,halfPixel)+index*float2(onePixel,onePixel);
				float h = SAMPLE_TEXTURE2D_LOD(_HeightTex, sampler_HeightTex, hUV, 0).r;
				h =h*2.0-1.0; // remap to -1~1

				#ifdef sphereMapping
					float3 nV = normalize(v);
					float R=1023.0*0.5;
					//球面
					v = (R+h*256.0f)*nV-_local_pos;
					o.positionHCS = TransformObjectToHClip(v);
					o.normal = TransformObjectToWorldNormal(nV);
				#else
					// 平面
                    v = float3(V.vertex.x,h*128.0f,V.vertex.z);
                    o.positionHCS = TransformObjectToHClip(v);
				#endif

                return o;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // The SAMPLE_TEXTURE2D marco samples the texture with the given
                // sampler.
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return color;
            }
            ENDHLSL
        }
    }
}