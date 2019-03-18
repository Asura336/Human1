// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ZShader"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white"{}
		_Color("Far color", Color) = (1,1,1,1)
		_NColor("Near color", Color) = (1,1,1,1)
		_MinL("Min value of lightness", Range(0, 1)) = 0.5
		_nZ("Offset of change", Range(-1, 1)) = 0
		_dZ("Power of change", Range(0.001, 50)) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCg.cginc"
			#include "UnityLightingCommon.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _NColor;
			fixed _MinL;
			half _nZ;
			half _dZ;

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv0 : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 diffuseLigntingColor : COLOR0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv0 = v.uv0;
				// use Lambert lighting
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half NdotL = dot(worldNormal, _WorldSpaceLightPos0.xyz);
				NdotL = clamp(NdotL, _MinL, 1);
				o.diffuseLigntingColor = NdotL * _LightColor0 * 2;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//float z = i.pos.z;
				float z = i.pos.z * _dZ + _nZ;
				float l = clamp(z, 0, 1);
				fixed4 color = fixed4(lerp(_Color, _NColor, l));

				fixed4 mainTexColor = tex2D(_MainTex, i.uv0) * color;
				return fixed4(mainTexColor.rgb * i.diffuseLigntingColor.rgb, 1);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
