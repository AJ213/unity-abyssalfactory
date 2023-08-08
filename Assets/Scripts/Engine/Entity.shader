Shader "Instanced/Entity" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader {

		Pass {
			Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
			LOD 100
			Lighting Off

			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#pragma instancing_options assumeuniformscaling
				#pragma target 4.5

				#include "UnityCG.cginc"

				struct appdata {
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 pos : POSITION;
					float2 uv : TEXCOORD0;
					float4 color: COLOR;
				};
				struct v2f {
				
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 color : COLOR;
				};

				float GlobalLightLevel;
				float minGlobalLightLevel;
				float maxGlobalLightLevel;
				sampler2D _MainTex;

				fixed4 _Color;

				StructuredBuffer<float4x4> positionBuffer;
				v2f vert (appdata v, uint instanceID : SV_InstanceID) {
					
					v2f o;
					
					UNITY_SETUP_INSTANCE_ID(v);
					float4x4 data = positionBuffer[instanceID];
					o.pos = mul(UNITY_MATRIX_VP, mul(data, float4(v.pos.xyz, 1.0)));
					o.uv = v.uv;
					o.color = v.color;
					return o;
				}


				fixed4 frag(v2f i) : SV_Target {
					fixed4 col = tex2D (_MainTex, i.uv)* _Color;

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