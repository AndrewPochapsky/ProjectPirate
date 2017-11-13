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
			INTERNAL_DATA
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _ColorA;
		fixed4 _ColorB;
		float _LerpSpeed;
		
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		
		fixed4 colorLerp(fixed4 colorA, fixed4 colorB){
			float t = sin(_Time[1]) * _LerpSpeed;
			return lerp(colorA, colorB, t);
		}

		void vert (inout appdata_full v) {
			float4 wpos = mul(unity_ObjectToWorld, v.vertex);

    		float phase = _Time[1] * 10;
   			float offset = (wpos.x + (wpos.z * 0.2)) * 0.5;
    		wpos.y = sin(phase + offset) * 0.2;
			
            v.vertex = mul(unity_WorldToObject, wpos);
			
		}
 

		void surf (Input IN, inout SurfaceOutputStandard o) {
			
			//float lerpValue = tex2Dlod( _NoiseTex, float4(0,IN.vertex.y / 100  + _Time[1]/25 ,0,0)).r;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * colorLerp(_ColorA, _ColorB);
			
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
