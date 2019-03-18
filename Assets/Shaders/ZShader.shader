// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ZShader"
{
	Properties
	{
		_MainTex("Albedo", 2D) = "white"{}
		_Color("Far color", Color) = (1,1,1,1)
		_NColor("Near color", Color) = (1,1,1,1)
		_nZ("Offset to change", Range(-1, 1)) = 0
		_dZ("Power of change", Range(0.001, 50)) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert fullforwardshadows

		struct Input
		{
			float2 uv_Maintex;
		};
		sampler2D _MainTex;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_Maintex);
			o.Albedo = c.rgb;
		}
		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			fixed4 _Color;
			fixed4 _NColor;
			float _nZ;
			float _dZ;

			struct appdata
			{
				float4 pos : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				return o;
			}

			fixed4 frag(v2f i : WPOS) : SV_Target
			{
				//float z = i.pos.z;
				float z = i.pos.z * _dZ + _nZ;
				float l = clamp(z, 0, 1);
								
				return fixed4(lerp(_Color, _NColor, l));
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
