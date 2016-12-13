Shader "Custom/GlowShader"
{
	Properties
	{
		_ColorTint("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_TexActive("Tex Active", Int) = 1
		_RimActive("Rim Active", Int) = 1
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimPower("Rim Power", Range(1.0, 6.0)) = 3.0
	}
		SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		CGPROGRAM
#pragma surface surf Lambert alpha

	struct Input {

		float4 color : Color;
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 viewDir;

	};

	float4 _ColorTint;
	sampler2D _MainTex;
	sampler2D _BumpMap;
	int _TexActive;
	int _RimActive;
	float4 _RimColor;
	float _RimPower;

	void surf(Input IN, inout SurfaceOutput o)
	{

		IN.color = _ColorTint;

		if (_TexActive == 1) {
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * IN.color;
		} else {
			o.Albedo = IN.color;
		}

		o.Alpha = IN.color.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));

		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
		o.Emission = 0.0;
		if (_RimActive == 1) {
			o.Emission = _RimColor.rgb * pow(rim, _RimPower);
		}


	}
	ENDCG
	}
		FallBack "Diffuse"
}﻿