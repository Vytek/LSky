

Shader "AC/LSky/Skybox"
{

	Properties{}

	SubShader
	{

		Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
		Cull Off ZWrite Off

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_MOON_INFLUENCE
			#pragma multi_compile __ LSKY_NIGHT_COLOR_ATMOSPHERIC
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_ENABLE_SUN_DISC
			#pragma multi_compile __ LSKY_ENABLE_MOON
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_ENABLE_STARS
			#pragma multi_compile __ LSKY_ENABLE_NEBULA
			//-------------------------------------------------------

			#pragma multi_compile __ LSKY_HDR
			#pragma multi_compile __ LSKY_GAMMA_COLOR_SPACE
			//-------------------------------------------------------

			#pragma target 3.0
			//-------------------------------------------------------

			#include "UnityCG.cginc"
			#include "LSkyVariablesInc.cginc"  
			#include "LSkyInc.cginc"  
			#include "LSkyAtmosphereInc.cginc"  
			//-------------------------------------------------------


			uniform half3 _GroundColor;
			//-------------------------------------------------------

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID 
			};

			struct v2f
			{
				float3 worldPos             : TEXCOORD0;
				half3  inScatter            : TEXCOORD1;
				half4  outScatter           : TEXCOORD2;
				float3 moonCoords           : TEXCOORD3;
				float3 outerSpaceCoords     : TEXCOORD4;
				float3 starsNoiseCoords     : TEXCOORD5;
				half   extinction           : TEXCOORD6;

				float4 vertex               : SV_POSITION;

				UNITY_VERTEX_OUTPUT_STEREO 
			};


			v2f vert (appdata v)
			{
				v2f o;

			
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				//------------------------------------------------------------------------------

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
				//------------------------------------------------------------------------------

				AtmosphericScattering(o.worldPos, o.inScatter, o.outScatter);
				//------------------------------------------------------------------------------

				o.moonCoords = MOON_COORDS(v.vertex);
				//------------------------------------------------------------------------------
				float3 sunCoords   = SUN_COORDS(v.vertex.xyz);
				o.outerSpaceCoords = OUTER_SPACE_COORDS(sunCoords);
				o.starsNoiseCoords = STARS_NOISE_COORDS(sunCoords);
				//------------------------------------------------------------------------------

				o.extinction = saturate((o.worldPos.y-0.005)*5);
				//------------------------------------------------------------------------------

				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{

				float3 ray = normalize(i.worldPos);
				//----------------------------------------------------------

				float  sunCosTheta  = dot(ray, LSky_SunDir.xyz);
				float  moonCosTheta = dot(ray, LSky_MoonDir.xyz); 
				//-------------------------------------------------------- --

			
				half3 color = i.inScatter;


				// Sun
				color += MiePhase(sunCosTheta, LSky_SunBetaMiePhase, LSky_SunMieScattering,  LSky_SunMieColor) * i.outScatter.rgb;

				#ifdef LSKY_ENABLE_SUN_DISC
				half3 sunDisc  = SunDisc(ray-LSky_SunDir) * i.outScatter.rgb;
				color += sunDisc;
				#endif
				//---------------------------------------------------------------------------------------------------------------------------------------

				// Moon.
				color.rgb  += MiePhaseSimplified(moonCosTheta, LSky_MoonBetaMiePhase, LSky_MoonMieScattering,  LSky_MoonMieColor) * i.outScatter.a;

				#ifdef LSKY_ENABLE_MOON
				half4 moon  = Moon(i.moonCoords, moonCosTheta) * i.outScatter.a;
				color  += moon.rgb * i.extinction;
				#endif
				//---------------------------------------------------------------------------------------------------------------------------------------


				// Outer Space
				half3 outerSpace = OuterSpace(i.outerSpaceCoords, i.starsNoiseCoords) * i.outScatter.a;

				#ifdef LSKY_ENABLE_SUN_DISC
				outerSpace *= saturate(1.0 - sunDisc.r);
				#endif

				#ifdef LSKY_ENABLE_MOON
				outerSpace *= moon.a;
				#endif

				color += outerSpace * i.extinction;
				//---------------------------------------------------------------------------------------------------------------------------------------

				ColorCorrection(color, _GroundColor);

				color = lerp(color, _GroundColor, saturate((-ray.y - 0.01) * 30));

				return half4(color,1);
			}
			ENDCG
		}

	}
}
