
Shader "Enviro/EnviroFogRendering" 
{
	Properties
	{ 
		_EnviroVolumeLightingTex("Volume Lighting Tex",  Any) = ""{}
		_Source("Source",  2D) = "black"{}
	}
	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma target 3.0
	#pragma multi_compile ENVIROVOLUMELIGHT

	#include "UnityCG.cginc" 
	#include "../Core/EnviroVolumeLightCore.cginc"
	#include "../Core/EnviroFogCore.cginc"

	uniform sampler2D _MainTex;
	uniform float4 _MainTex_TexelSize;
    uniform float _noiseScale;
	uniform half _noiseIntensity;
	uniform sampler2D _Clouds;
	uniform float _SkyFogHeight;
	uniform float _SunBlocking;

	struct appdata_t 
	{
		float4 vertex : POSITION;
		float3 texcoord : TEXCOORD0;
	};

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float3 texcoord : TEXCOORD0;
		float3 sky : TEXCOORD1;
		float2 uv : TEXCOORD2;
		float2 uv_depth : TEXCOORD3;
		float4 interpolatedRay : TEXCOORD4;
	};

	v2f vert(appdata_img v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		half index = v.vertex.z;
		v.vertex.z = 0.1;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy;
		o.uv_depth = v.texcoord.xy;
		o.interpolatedRay = _FrustumCornersWS[(int)index];
		o.interpolatedRay.w = index;
		o.sky.x = saturate(_SunDir.y + 0.25);   
        o.sky.y = saturate(clamp(1.0 - _SunDir.y,0.0,0.5));
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
		i.uv.y = 1 - i.uv.y;
#endif 


		float rawDepth;
		rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(i.uv_depth));
		float dpth = Linear01Depth(rawDepth);
		float4 wsDir = dpth * i.interpolatedRay;
		float4 wsPos = float4(_WorldSpaceCameraPos,0) + wsDir;
		float3 viewDir = normalize(wsDir);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		half fogFac = 0;
		float4 finalFog = 0;
		float g = _DistanceParams.x;
		half gHeight = 0;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	    
		//Scene
		if (dpth < 0.99999) 
		{
			// Calculate Distance Fog
			if (_EnviroParams.y > 0)
			{
				g += ComputeDistance(wsDir, dpth);
				g *= _distanceFogIntensity;
			}

			if (_EnviroParams.z > 0)
			{
				gHeight = ComputeHalfSpaceWithNoise(wsDir);
				//gAdd = ComputeHalfSpace(wsDir);
			}

			// Add Height Fog
			g += gHeight;

			// Compute fog amount
			fogFac = ComputeFogFactor(max(0.0, g));
			fogFac = lerp(_maximumFogDensity, 1.0f, fogFac);

			finalFog = ComputeScatteringScene(viewDir, i.sky.xy);
		}
		else //SKY
		{
			float4 clouds = tex2D(_Clouds, UnityStereoTransformScreenSpaceTex(i.uv));
			
			if (_EnviroParams.z > 0)
			{
				gHeight = ComputeHalfSpace(wsDir);
			}

			half fogFacSky = ComputeFogFactor(max(0.0, gHeight));

			float f = saturate((_SkyFogHeight * dot(normalize(wsPos - _WorldSpaceCameraPos.xyz), float3(0, 1, 0))));
			f = pow(f, _skyFogIntensity);
			fogFac = (clamp(f, 0, 1));

			if (fogFac > fogFacSky)
				fogFac = fogFacSky;

			float sunMask = 1;
			if (clouds.a <= 0.05) sunMask = lerp(0.1, 1.0, clouds.a);
			else sunMask = 0;

			float cosTheta = dot(viewDir, _SunDir);
			float4 sunD = min(2, pow((1 - cosTheta) * (_SunDiskSize * 100), -2));
			float4 skyFog = ComputeScattering(viewDir, i.sky.xy);

			finalFog = lerp(skyFog, lerp(skyFog, skyFog + ((clouds * clouds) * sunMask), sunD), _SunBlocking);
		}

		// Color bandińg fix
		float2 wcoord = (wsPos.xy/wsPos.w) * _noiseScale;
		float4 dither = ( dot( float2( 171.0f, 231.0f ), wcoord.xy ) );
		dither.rgb = frac( dither / float3( 103.0f, 71.0f, 97.0f ) ) - float3( 0.5f, 0.5f, 0.5f );
		finalFog =  finalFog + (dither/255.0f) * _noiseIntensity;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		float4 final;
		float4 source = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(i.uv));
		
		#if defined (ENVIROVOLUMELIGHT)
			float4 volumeLighting = tex2D(_EnviroVolumeLightingTex, UnityStereoTransformScreenSpaceTex(i.uv));
			volumeLighting *= _EnviroParams.x; 
			final = lerp (lerp(finalFog, finalFog + volumeLighting, _EnviroVolumeDensity), lerp(source, source + volumeLighting, _EnviroVolumeDensity), fogFac);
		#else
			final = lerp (finalFog, source, fogFac);
		#endif

		return final;

		}
		ENDCG
		}
	}
	Fallback Off
}
