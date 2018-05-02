// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Demonixis/LavaShader" {
	Properties {
		_MainTexture ("Lava texture", 2D) = "white" {}
		_BumpTexture ("Bump texture", 2D) = "white" {}
		_DiffuseColor ("Diffuse Color", Color) = (1, 1, 1, 1)
		_EmissiveColor ("Emissive Color", Color) = (0, 0, 0, 1)
		_Tiling ("Texture tiling", Vector) = (1, 1, 0)
		_Offset ("Texture offset", Vector) = (0, 0, 0)  
		_WeaveSpeed ("_WeaveSpeed", float) = 1                                   
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
			sampler2D _BumpTexture;
			float4 _DiffuseColor;
			float4 _EmissiveColor;
			float2 _Tiling;
			float2 _Offset;
			float _WeaveSpeed;
													
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
																						
			VertexShaderOutput VertexShaderFunction (VertexShaderInput input) 
			{
				VertexShaderOutput output;
				output.Position = UnityObjectToClipPos(input.Position);
				output.UV = half2(0.5, 0.5) * input.UV;
				return output;
			}
														
			half4 FragmentShaderFunction(VertexShaderOutput input) : COLOR
			{
				float speed = _Time * _WeaveSpeed;
				float4 noise = tex2D(_BumpTexture, (input.UV + _Offset) * _Tiling);
				float2 T1 = input.UV + float2(1.5, -1.5) * speed * 0.02;
				float2 T2 = input.UV + float2(-0.5, 2.0) * speed * 0.01;
				
				T1.x += noise.x * 2.0;
				T1.y += noise.y * 2.0;
				T2.x -= noise.y * 0.2;
				T2.y += noise.z * 0.2;
				
				float p = tex2D(_BumpTexture, ((T1 * 3.0) + _Offset) * _Tiling).a;
				
				float4 color = tex2D(_MainTexture, ((T2 * 4.0) + _Offset) * _Tiling);
				float4 temp = color * (half4(p, p, p, p) * 2.0) + (color * color - 0.1);
				
				if (temp.r > 1.0)
					temp.bg += clamp(temp.r - 2.0, 0.0, 100.0);
				
				if (temp.g > 1.0)
					temp.rb += temp.g - 1.0;
				
				if (temp.b > 1.0)
					temp.rg += temp.b - 1.0;
					
				return UNITY_LIGHTMODEL_AMBIENT + (_DiffuseColor * temp) + _EmissiveColor;
			}
			ENDCG
		}
	}
Fallback "Diffuse"
}