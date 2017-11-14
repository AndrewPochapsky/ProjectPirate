Shader "Custom/WaterShader" {
	Properties {
		_ColorA ("ColorA", Color) = (1,1,1,1)
		_ColorB ("ColorB", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_LerpSpeed ("Lerp Speed", Float) = 1
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_WaveMultiplier ("Wave Multiplier", Float) = 80
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
		sampler2D _NoiseTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float2 uv_BumpMap;
			float2 uv_NoiseTex;
			INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
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
   		 	float y = (sin(x * 1.0 + _Time[1] * 1.0) + sin(x * 2.3 + _Time[1] * 1.5) + sin(x * 3.3 + _Time[1] )) * _WaveMultiplier	;
    		return y;
		}

		fixed4 colorLerp(fixed4 colorA, fixed4 colorB){
			float t = sin(_Time[1]) * _LerpSpeed;
			return lerp(colorA, colorB, t);
		}

		void vert (inout appdata_full v) {
			float4 wpos = mul(unity_ObjectToWorld, v.vertex);
			//fixed4 noise = tex2D(_NoiseTex, float2(wpos.y / 50, _Time[1]/20) ).r;
			//float y = (sin(wpos.z * 1.0 + _Time[1] * 1.0) + sin(wpos.x * 2.3 + _Time[1] * 1.5) + sin(wpos.x * 3.3 + _Time[1] * 0.4)) / 3.0;
    		float phase = _Time[1] * 10;
   			float offset1 = (wpos.x + (wpos.z * 0.2)) * 0.5; 
			
			wpos.y = calculateSurface(wpos.x);//sin(wpos.z + phase) * _WaveDampener + sin(offset1 + phase) * _WaveDampener;
			wpos.y += calculateSurface(wpos.z);
			wpos.y -= calculateSurface(0.0);
		    v.vertex = mul(unity_WorldToObject, wpos);
		}
 

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _ColorA;
			o.Albedo = c.rgb;
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
