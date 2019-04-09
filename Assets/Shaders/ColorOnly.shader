Shader "Custom/ColorOnly"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader
	{
		Tags { 
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
		}
		LOD 200
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			fixed4 _Color;
			struct v2f {
				float4 vertex: POSITION;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				return o;
			}

			fixed4 frag(v2f i):COLOR{
				return  _Color;
			}
			ENDCG
		}
    }
    FallBack "Diffuse"
}
