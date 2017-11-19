// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaterShader" {
	Properties {
		_BaseColor ("Base Color", Color) = (1,1,1,1)
		_ColorA ("ColorA", Color) = (1,1,1,1)
		_ColorB ("ColorB", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_LerpSpeed ("Lerp Speed", Float) = 1
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_WaveMultiplier ("Wave Multiplier", Float) = 80

		//_BeginWaves ("Begin Waves", Float) = 0

		//[HideInInspector]
		//_StartTime ("Start Time", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert
		//#pragma surface surf Lambert alpha vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 localPos;
			float4 worldPos;
			float3 centerPos;
		};
		//Globals
		uniform float BEGIN_WAVES;
		uniform float START_TIME;

		//Locals changed in script
		float isIsland;

		//Properties
		half _Glossiness;
		half _Metallic;
		fixed4 _BaseColor;
		fixed4 _ColorA;
		fixed4 _ColorB;
		float _LerpSpeed;
		float _WaveMultiplier;
		
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		
		float calculateSurface(float x) {
   		 	float y = (sin(x * 1.0 + (_Time[1] - START_TIME) * 1.0) 
				+ sin(x * 2.3 + (_Time[1] - START_TIME) * 1.5) 
				+ sin(x * 3.3 + (_Time[1] - START_TIME) )) 
				* _WaveMultiplier	;
    		return y;
		}

		
		fixed4 colorLerp(fixed4 colorA, fixed4 colorB){
			float t = sin(_Time[1]) * _LerpSpeed;
			return lerp(colorA, colorB, t);
		}

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.localPos = v.vertex.xyz;
			//float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );
			o.centerPos = v.texcoord;//objectOrigin.xyz;
			float4 wpos = mul(unity_ObjectToWorld, v.vertex);
			o.worldPos = wpos;
			//float4 lpos = v.texcoord;
			
			wpos.y += calculateSurface(wpos.x);
			wpos.y += calculateSurface(wpos.z);
			//wpos.y -= calculateSurface(0.0);
			
		    v.vertex = mul(unity_WorldToObject, wpos);
		}
 

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _BaseColor;
			float factor = distance(IN.centerPos.xyz, IN.localPos.xyz) * 0.2;
			factor = clamp(factor, 0, 1);
			if(isIsland == 1){
				o.Albedo = ((lerp(_ColorB, _ColorA, factor)));
			}else{
				o.Albedo = c.rgb;
			}
			
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
