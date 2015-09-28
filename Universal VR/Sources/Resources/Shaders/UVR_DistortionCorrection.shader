Shader "Demonixis/Filter/SimpleBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Pass
        {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment PixelShaderFunction
			#include "UnityCG.cginc"
	 
			uniform sampler2D _MainTex;
			uniform half2 _LensCenter;
			uniform half2 _Scale;
			uniform half2 _ScaleIn;
			uniform half4 _HmdWarpParam;
		
			half2 HmdWarp(half2 in01)
			{
				half2 theta = (in01 - _LensCenter) * _ScaleIn; // _Scales to [-1, 1]
				half rSq = theta.x * theta.x + theta.y * theta.y;
				half2 rvector = theta * (_HmdWarpParam.x + _HmdWarpParam.y * rSq + _HmdWarpParam.z * rSq * rSq + _HmdWarpParam.w * rSq * rSq * rSq);
				return _LensCenter + _Scale * rvector;
			}

			half4 PixelShaderFunction(v2f_img input) : COLOR
			{
				fixed2 tc = HmdWarp(input.uv);
				
				if (tc.x < 0.0 || tc.x > 1.0 || tc.y < 0.0 || tc.y > 1.0)
					return half4(0.0, 0.0, 0.0, 1.0);
				else
					return half4(tex2D(_MainTex, tc));
			}
			ENDCG
        }
    }
}