// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaterShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_OceanWaterTex ("Ocean Water", 2D) = "white" {}
		_IslandWaterTex ("Island Water", 2D) = "white" {}
		
		//_BaseColor ("Base Color", Color) = (1,1,1,1)
		//_ColorA ("ColorA", Color) = (1,1,1,1)
		//_ColorB ("ColorB", Color) = (1,1,1,1)
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_LerpSpeed ("Lerp Speed", Float) = 1
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_OceanWaveModifier ("Ocean Wave Modifier", Float) = 80
		_IslandWaveModifier ("Island Wave Modifier", Float) = 5
		//_BeginWaves ("Begin Waves", Float) = 0

		//[HideInInspector]
		//_StartTime ("Start Time", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		//LOD 300
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert vertex:vert
		//#pragma surface surf Lambert alpha vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 localPos;
			float4 worldPos;
			float3 centerPos;
		};

		sampler2D _MainTex;
		sampler2D _OceanWaterTex;
		sampler2D _IslandWaterTex;
		sampler2D _BumpMap;

		//Globals
		//uniform float BEGIN_WAVES;
		uniform float START_TIME;

		//Locals changed in script
		float isIsland;
		float4 center;

		//Properties
		half _Glossiness;
		half _Metallic;
		fixed4 _BaseColor;
		fixed4 _ColorA;
		fixed4 _ColorB;
		float _LerpSpeed;
		float _OceanWaveModifier;
		float _IslandWaveModifier;
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		float calculateSurface(float x, float modifier) {
   		 	float y = (sin(x * 1.0 + (_Time[1] - START_TIME) * 1.0) 
				+ sin(x * 2.3 + (_Time[1] - START_TIME) * 1.5) 
				+ sin(x * 3.3 + (_Time[1] - START_TIME) )) 
				* modifier	;
    		return y;
		}

		fixed4 colorLerp(fixed4 colorA, fixed4 colorB){
			float t = sin(_Time[1]) * _LerpSpeed;
			return lerp(colorA, colorB, t);
		}


		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			//o.localPos = v.vertex.xyz;
			o.localPos = v.texcoord.xyz;
			float4 objectOrigin = mul(unity_WorldToObject, float4(0.0,0.0,0.0,1.0) );
			o.centerPos = objectOrigin;

			float4 wpos = mul(unity_ObjectToWorld, v.vertex);
			o.worldPos = wpos;

			if(isIsland == 1 && v.texcoord.x == 0 || v.texcoord.x == 1 || v.texcoord.y == 0 || v.texcoord.y == 1){
				wpos.y += calculateSurface(wpos.x, _OceanWaveModifier);
				wpos.y += calculateSurface(wpos.z, _OceanWaveModifier);
			}else if(isIsland == 1){
				//Stuff done in the centre of the mesh if island
				wpos.y += calculateSurface(wpos.x, _IslandWaveModifier);
				wpos.y += calculateSurface(wpos.z, _IslandWaveModifier);
			}else if(isIsland == 0){
				wpos.y += calculateSurface(wpos.x, _OceanWaveModifier);
				wpos.y += calculateSurface(wpos.z, _OceanWaveModifier);
			}
			
			//wpos.y -= calculateSurface(0.0);
			
		    v.vertex = mul(unity_WorldToObject, wpos);
		}
 

		void surf (Input IN, inout SurfaceOutput o) {
			
			
			if(isIsland == 1){
				o.Albedo = tex2D (_IslandWaterTex, IN.uv_MainTex).rgb;
			}else{
				o.Albedo = tex2D (_OceanWaterTex, IN.uv_MainTex).rgb;
			}
			
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
