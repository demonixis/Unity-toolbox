// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Demonixis/PlasmaShader" {
	Properties {
		_MainTexture ("Lava texture", 2D) = "white" {}
		_Size ("Size", float) = 256.0
		_Shift ("Shift", float) = 1.0
		_Speed ("Speed", float) = 2.5
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
													
		Pass {
			CGPROGRAM
			#pragma vertex VertexShaderFunction
			#pragma fragment FragmentShaderFunction
			#include "UnityCG.cginc"
			
			sampler2D _MainTexture;
			float _Size;
			float _Shift;
			float _Speed;
			static const float PI = 3.14159265f;
													
			struct VertexShaderInput 
			{
				float4 Position : POSITION;
				float2 UV : TEXCOORD0;
			};
			
			struct VertexShaderOutput 
			{
				float4 Position : POSITION;
				float2 UV : TEXCOORD0;
			};
			
			float Distance(float a, float b, float c, float d)  
			{
				return sqrt(((a - c) * (a - c) + (b - d) * (b - d)));
			}

			// Map the value to the range [0, size - 1]
			int Normalize(float _value) 
			{
				float result;
				
				result = (_value + 1.0f) / 2.0f;
				result = floor((_Size - 1.0f) * result);
				result = round(result);

				return (int)result;
			}

			// f(x, t) = sin(x / 40.74 + t) 
			// http://www.mennovanslooten.nl/blog/post/72
			float F1(float time, int x, int y) 
			{
				float divider = _Size / (2.0f * PI); // Make the lifetime = size
				return sin(x / divider + time);
			}

			// f(x, y, t) = sin(distance(x, y, (128 * sin(-t) + 128), (128 * cos(-t) + 128)) / 40.74)
			// http://www.mennovanslooten.nl/blog/post/72
			float F2(float time, int x, int y) 
			{
				float distance = Distance(x, y, sin(-time) * 128.0f + 128.0f, cos(-time) * 128.0f + 128.0f);
				float divider  = _Size / (2.0f * PI); // Make the lifetime = size

				return sin(distance / divider);
			}
																						
			VertexShaderOutput VertexShaderFunction (VertexShaderInput input) 
			{
				VertexShaderOutput output;
				output.Position = UnityObjectToClipPos(input.Position);
				output.UV = half2(0.5, 0.5) * input.UV;
				return output;
			}
														
			half4 FragmentShaderFunction(VertexShaderOutput input) : COLOR
			{
				int x = round(input.UV.x * (_Size - 1)); // Map to [0, 255]
				int y = round(input.UV.y * (_Size - 1)); // Map to [0, 255]

				float index = 0.0f;

				index += F1(_Time * _Speed, x, y);
				index += F2(_Time * _Speed, x, y);
				index /= 2.0f;
				index = (index + _Shift) % 1.0f; // Shift the palette

				return UNITY_LIGHTMODEL_AMBIENT + tex2D(_MainTexture, half2(index, 0)); // Only the first row contains the palette data.
			}
			ENDCG
		}
	}
Fallback "Diffuse"
}