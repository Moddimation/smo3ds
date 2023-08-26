// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#include "UnityShaderVariables.cginc"
#include "UnityCG.cginc"

#define USING_FOG (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))

// ES2.0 can not do loops with non-constant-expression iteration counts :(
#if defined(SHADER_API_GLES)
#define LIGHT_LOOP_LIMIT 8
#else
#define LIGHT_LOOP_LIMIT unity_VertexLightParams.x
#endif

// Compute attenuation & illumination from one light
half3 computeOneLight(int idx, float3 eyePosition, half3 eyeNormal) {

	float3 dirToLight = unity_LightPosition[idx].xyz;
	half att = 1.0;

#if defined(POINT) || defined(SPOT)
	dirToLight -= eyePosition * unity_LightPosition[idx].w;
	
	// distance attenuation
	float distSqr = dot(dirToLight, dirToLight);
	att /= (1.0 + unity_LightAtten[idx].z * distSqr);

	if (unity_LightPosition[idx].w != 0 &&
		distSqr > unity_LightAtten[idx].w) 
		att = 0.0; // set to 0 if outside of range

	dirToLight *= rsqrt(distSqr);

#if defined(SPOT)
	// spot angle attenuation
	half rho = max(dot(dirToLight, unity_SpotDirection[idx].xyz), 0.0);
	half spotAtt = (rho - unity_LightAtten[idx].x) * unity_LightAtten[idx].y;
	att *= saturate(spotAtt);
#endif

#endif
	att *= 0.5; // passed in light colors are 2x brighter than what used to be in FFP

	half NdotL = max(dot(eyeNormal, dirToLight), 0.0);
	
	// diffuse
	half3 color = att * NdotL * unity_LightColor[idx].rgb;

	return min(color, 1.0);
}

// uniforms
int4 unity_VertexLightParams; // x: light count, y: zero, z: one (y/z needed by d3d9 vs loop instruction)
float4 _MainTex_ST;

// vertex shader input data
struct appdata {
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float3 uv0 : TEXCOORD0;
#if defined(CUSTOM_LIGHTMAPPED)
	float3 uv1 : TEXCOORD1;
#endif
};

// vertex-to-fragment interpolators
struct v2f {
	fixed4 color : COLOR0;
	float2 uv0 : TEXCOORD0;
#if defined(CUSTOM_LIGHTMAPPED)
	float2 uv1 : TEXCOORD1;
#if USING_FOG
	fixed fog : TEXCOORD2;
#endif
#else
#if USING_FOG
	fixed fog : TEXCOORD1;
#endif
#endif
	float4 pos : SV_POSITION;
};

// vertex shader
v2f vert(appdata IN) {
	v2f o;
	float3 eyePos = mul(UNITY_MATRIX_MV, float4(IN.pos, 1)).xyz;
	half3 eyeNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, IN.normal).xyz);
	
	// vertex lighting
	half4 color = half4(0, 0, 0, 1);
#if defined(AMBIENT_ON) || !defined(CUSTOM_LIGHTMAPPED)
	color.rgb = glstate_lightmodel_ambient.rgb;
#endif

	for (int il = 0; il < LIGHT_LOOP_LIMIT; ++il) {
		color.rgb += computeOneLight(il, eyePos, eyeNormal);
	}
	o.color = saturate(color);
	
	// compute texture coordinates
	o.uv0 = IN.uv0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
#if defined(CUSTOM_LIGHTMAPPED)
	o.uv1 = IN.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
#endif
	
	// fog
#if USING_FOG
	float fogCoord = length(eyePos.xyz); // radial fog distance
	UNITY_CALC_FOG_FACTOR(fogCoord);
	o.fog = saturate(unityFogFactor);
#endif

	// transform position
	o.pos = UnityObjectToClipPos(float4(IN.pos, 1));
	return o;
}

// textures
sampler2D _MainTex;

// fragment shader
fixed4 frag(v2f IN) : SV_Target {
	half4 vertexLighting = IN.color * 2.0f;

#if defined(CUSTOM_LIGHTMAPPED)
	half4 lightmap = UNITY_SAMPLE_TEX2D(unity_Lightmap, IN.uv1.xy);

#if CUSTOM_LIGHTMAPPED == 1
	half4 lighting = lightmap * 2.0f + vertexLighting;
#elif CUSTOM_LIGHTMAPPED == 2
	half4 lighting = lightmap * lightmap.a * 8.0f + vertexLighting;
#endif

#else
	half4 lighting = vertexLighting;
#endif

	half4 diffuse = tex2D(_MainTex, IN.uv0.xy);

	half4 color = diffuse * lighting;
	color.a = 1;
	
	// fog
	#if USING_FOG
	color.rgb = lerp(unity_FogColor.rgb, color.rgb, IN.fog);
	#endif
	
	return color;
}
