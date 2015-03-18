//#<!--
//#    CRT-simple shader
//#
//#    Copyright (C) 2011 DOLLS. Based on cgwg's CRT shader.
//#
//#    This program is free software; you can redistribute it and/or modify it
//#    under the terms of the GNU General Public License as published by the Free
//#    Software Foundation; either version 2 of the License, or (at your option)
//#    any later version.
//#    -->
 
Shader "Custom/CRT"
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
        #pragma fragment frag
        #include "UnityCG.cginc"
        #define CURVATURE
        #pragma target 3.0
        #define PI 3.141592653589
 
        uniform sampler2D _MainTex;
        uniform float2 _InputSize;
        uniform float2 _OutputSize;
        uniform float2 _TextureSize;
        uniform float2 _One;
        uniform float2 _Texcoord;
        uniform float _Factor;
        uniform float _Distortion; // 0.1f
        uniform float _InputGamma; // 2.4f
        uniform float _OutputGamma; //2.2f
 
        float2 RadialDistortion(float2 coord)
        {
            coord *= _TextureSize / _InputSize;
            float2 cc = coord - 0.5f;
            float dist = dot(cc, cc) * _Distortion;
            return (coord + cc * (1.0f + dist) * dist) * _InputSize / _TextureSize;
        }
       
        float4 ScanlineWeights(float distance, float4 color)
        {
            float4 width = 2.0f + 2.0f * pow(color, float4(4.0f, 4.0f, 4.0f, 4.0f));
            float4 weights = float4(distance / 0.3f, distance / 0.3f, distance / 0.3f, distance / 0.3f);
            return 1.4f * exp(-pow(weights * rsqrt(0.5f * width), width)) / (0.6f + 0.2f * width);
        }
       
        float4 frag(v2f_img i) : COLOR
        {
            _Texcoord = i.uv;
            _One = 1.0f / _TextureSize;
            _OutputSize = _TextureSize;
            _InputSize = _TextureSize;
            _Factor = _Texcoord.x * _TextureSize.x * _OutputSize.x / _InputSize.x;
           
            float4 ScreenGamma = pow(tex2D(_MainTex, _Texcoord), _InputGamma);
           
            #ifdef CURVATURE
            float2 xy = RadialDistortion(_Texcoord);
            #else
            float2 xy = _Texcoord;
            #endif
           
            float2 ratio = xy * _TextureSize - float2(0.5f, 0.5f);
            float2 uvratio = frac(ratio);
           
            xy.y = (floor(ratio.y) + 0.5f) / _TextureSize;
            float4 col = tex2D(_MainTex, xy);
            float4 col2 = tex2D(_MainTex, xy + float2(0.0f, _One.y));
           
            float4 weights = ScanlineWeights(uvratio.y, col);
            float4 weights2 = ScanlineWeights(1.0f - uvratio.y, col2);
            float3 res = (col * weights + col2 * weights2).rgb;
           
            float3 rgb1 = float3(1.0f, 0.7f, 1.0f);
            float3 rgb2 = float3(0.7f, 1.0f, 0.7f);
           
            float3 dotMaskWeights = lerp(rgb1, rgb2, floor(fmod(_Factor, 2.0f)));
            res *= dotMaskWeights;
           
            return float4(pow(res, float3(1.0f / _OutputGamma, 1.0f / _OutputGamma, 1.0f / _OutputGamma)), 1.0f);
           
           
        }
 
        ENDCG
        }
    }
}