Shader "Custom/VertexDynamicLightmap" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" { }
		[Toggle(AMBIENT_ON)] _AmbientOn("Ambient Lighting", Float) = 0
	}

	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 80

		// Non-lightmapped
		Pass {
			Tags { "LightMode" = "Vertex" "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_fog
			// Compile specialized variants for when positional (point/spot) and spot lights are present
			#pragma multi_compile __ POINT SPOT

			#include "VertexLightmapCommon.cginc"

			ENDCG
		}

		// Lightmapped, encoded as dLDR
		Pass {
			Tags { "LightMode" = "VertexLM" "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_fog
			// Compile specialized variants for when positional (point/spot) and spot lights are present
			#pragma multi_compile __ POINT SPOT
			#pragma multi_compile __ AMBIENT_ON

			#define CUSTOM_LIGHTMAPPED 1
			#include "VertexLightmapCommon.cginc"

			ENDCG
		}

		// Lightmapped, encoded as RGBM
		Pass {
			Tags { "LightMode" = "VertexLMRGBM" "RenderType" = "Opaque" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			// Compile specialized variants for when positional (point/spot) and spot lights are present
			#pragma multi_compile __ POINT SPOT
			#pragma multi_compile __ AMBIENT_ON

			#define CUSTOM_LIGHTMAPPED 2
			#include "VertexLightmapCommon.cginc"

			ENDCG
		}
		
		// Pass to render object as a shadow caster
		Pass {
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On
			ZTest LEqual
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v) {
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target {
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
}
