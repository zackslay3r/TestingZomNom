Shader "Custom/NewSurfaceShader" {
	Properties {
		_Tint("Tint", Color) = (1, 1, 1, 1)
		_MainTex("Albedo", 2D) = "white" {}

	[NoScaleOffset] _NormalMap("Normals", 2D) = "bump" {}
	_BumpScale("Bump Scale", Float) = 1

		[NoScaleOffset] _MetallicMap("Metallic", 2D) = "white" {}
	[Gamma] _Metallic("Metallic", Range(0, 1)) = 0
		_Smoothness("Smoothness", Range(0, 1)) = 0.1

		[NoScaleOffset] _EmissionMap("Emission", 2D) = "black" {}
	_Emission("Emission", Color) = (0, 0, 0)

		_DetailTex("Detail Albedo", 2D) = "gray" {}
	[NoScaleOffset] _DetailNormalMap("Detail Normals", 2D) = "bump" {}
	_DetailBumpScale("Detail Bump Scale", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	CustomEditor "CustomShaderGUI"
	FallBack "Diffuse"
}
