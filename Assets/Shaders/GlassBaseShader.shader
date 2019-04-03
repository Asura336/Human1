// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CookbookShaders/GlassBaseShader"
{
    Properties
    {
		_Color("Color", Color) = (1,0,0,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_BumpMap("Noise text", 2D) = "bump" {}
		_Magnitude("Magnitude", Range(0, 1)) = 0.05
	}
	SubShader
	{
		Tags {
			"Queue" = "Transparent"
			"IgnoreProject" = "True"
			"RenderType" = "Opaque"
		}
		GrabPass{ }
		Pass {
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			half4 _Color;
			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _GrabTexture;
			float _Magnitude;


			struct vertInput {
				float4 vertex: POSITION;
				float2 texcoord: TEXCOORD0;
			};

			struct vertOutput {
				float4 vertex: POSITION;
				float2 texcoord: TEXCOORD0;
				float4 uvGrab: TEXCOORD1;
			};

			vertOutput vert(vertInput input) {
				vertOutput o;
				o.vertex = UnityObjectToClipPos(input.vertex);
				o.uvGrab = ComputeGrabScreenPos(o.vertex);
				o.texcoord = input.texcoord;
				return o;
			}

			half4 frag(vertOutput i) : COLOR{
				half4 mainColor = tex2D(_MainTex, i.texcoord);
				half4 bump = tex2D(_BumpMap, i.texcoord);
				half2 distortion = UnpackNormal(bump).rg;

				i.uvGrab.xy += distortion * _Magnitude;
				fixed4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvGrab));
				return col * mainColor * _Color;
			}

			ENDCG
		}
    }
    FallBack "Diffuse"
}
