Shader "Custom/Voxel" {

	Properties {
		_MainTex ("TextureArray", 2DArray) = "white" {}
	}

	SubShader {
		
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 100
		Lighting Off

		Pass {
		
			CGPROGRAM
				#pragma vertex vertFunction
				#pragma fragment fragFunction
				#pragma target 3.5

				#include "UnityCG.cginc"

				struct appdata {
				
					float4 vertex : POSITION;
					float3 uv : TEXCOORD0;
					float4 color : COLOR;

				};

				struct v2f {
				
					float4 vertex : SV_POSITION;
					float3 uv : TEXCOORD0;
					float4 color : COLOR;

				};

				float GlobalLightLevel;
				float minGlobalLightLevel;
				float maxGlobalLightLevel;

				v2f vertFunction (appdata v) {
				
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.color = v.color;

					return o;

				}

				UNITY_DECLARE_TEX2DARRAY(_MainTex);

				fixed4 fragFunction (v2f i) : SV_Target {
				
					fixed4 col = UNITY_SAMPLE_TEX2DARRAY (_MainTex, i.uv);

					//float shade = (maxGlobalLightLevel - minGlobalLightLevel) * GlobalLightLevel + minGlobalLightLevel;
					//shade *= i.color.a;
					//shade = clamp (1 - shade, minGlobalLightLevel, maxGlobalLightLevel);

					clip(col.a - 1);
					col = lerp(col, float4(0, 0, 0, 1), 0);

					return col;
				}

				ENDCG

		}


	}

}