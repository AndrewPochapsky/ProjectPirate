  Shader "Custom/Test" {
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf Lambert
      struct Input {
          float4 color : COLOR;
      };
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = float3(0,1,0);
      }
      ENDCG
    }
    //Fallback "Diffuse"
  }