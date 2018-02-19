Shader "Custom/WaterShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_OceanWaterTex ("Ocean Water", 2D) = "white" {}
		_IslandWaterTex ("Island Water", 2D) = "white" {}
		
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_LerpSpeed ("Color Lerp Speed", Float) = 1
		_OceanWaveModifier ("Ocean Wave Modifier", Float) = 80
		_IslandWaveModifier ("Island Wave Modifier", Float) = 5

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		//LOD 300
		
		CGPROGRAM

		#pragma surface surf Lambert vertex:vert

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
		uniform float START_TIME;

		//Locals changed in script
		float isIsland;
		float4 center;

		//Properties
		float _LerpSpeed;
		float _OceanWaveModifier;
		float _IslandWaveModifier;
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		//Uses compound sin waves inorder to create realistic looking waves
		float calculateSurface(float x, float modifier) {
   		 	float y = (sin(x * 1.0 + (_Time[1] - START_TIME) * 1.0) 
				+ sin(x * 2.3 + (_Time[1] - START_TIME) * 1.5) 
				+ sin(x * 3.3 + (_Time[1] - START_TIME) )) 
				* modifier	;
    		return y;
		}

		//Lerps from one color to another overtime
		fixed4 colorLerp(fixed4 colorA, fixed4 colorB){
			float t = sin(_Time[1]) * _LerpSpeed;
			return lerp(colorA, colorB, t);
		}

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
		
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
		    v.vertex = mul(unity_WorldToObject, wpos);
		}
 

		void surf (Input IN, inout SurfaceOutput o) {
			
			fixed4 c;
			if(isIsland == 1){
			 	c = tex2D (_IslandWaterTex, IN.uv_MainTex);
			}else{
				c = tex2D (_OceanWaterTex, IN.uv_MainTex);
			}
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
