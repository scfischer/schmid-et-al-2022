Shader "Alveolus/GradientStandardShader20Colors"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _GradientStrength("Gradient Strength", Range(0,1)) = 0.75
        _Color0("Color 0", Color) = (0,0,1,0)
        _Color1("Color 1", Color) = (1,0,0,0)
        _Color2("Color 2", Color) = (0,0,1,0)
        _Color3("Color 3", Color) = (1,0,0,0)
        _Color4("Color 4", Color) = (0,0,1,0)
        _Color5("Color 5", Color) = (1,0,0,0)
        _Color6("Color 6", Color) = (0,0,1,0)
        _Color7("Color 7", Color) = (1,0,0,0)
        _Color8("Color 8", Color) = (0,0,1,0)
        _Color9("Color 9", Color) = (1,0,0,0)
        _Color10("Color 10", Color) = (0,0,1,0)
        _Color11("Color 11", Color) = (1,0,0,0)
        _Color12("Color 12", Color) = (0,0,1,0)
        _Color13("Color 13", Color) = (1,0,0,0)
        _Color14("Color 14", Color) = (0,0,1,0)
        _Color15("Color 15", Color) = (1,0,0,0)
        _Color16("Color 16", Color) = (0,0,1,0)
        _Color17("Color 17", Color) = (1,0,0,0)
        _Color18("Color 18", Color) = (0,0,1,0)
        _Color19("Color 19", Color) = (1,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        #define NUM_STEPS 20
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _GradientStrength;
        fixed4 _Color0;
        fixed4 _Color1;
        fixed4 _Color2;
        fixed4 _Color3;
        fixed4 _Color4;
        fixed4 _Color5;
        fixed4 _Color6;
        fixed4 _Color7;
        fixed4 _Color8;
        fixed4 _Color9;
        fixed4 _Color10;
        fixed4 _Color11;
        fixed4 _Color12;
        fixed4 _Color13;
        fixed4 _Color14;
        fixed4 _Color15;
        fixed4 _Color16;
        fixed4 _Color17;
        fixed4 _Color18;
        fixed4 _Color19;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 gradient;
            float uv_y = 1.0 - IN.uv_MainTex.y; // quick hack as the UV is the wrong way round in the mesh
            int section = NUM_STEPS * uv_y;
            float blending = NUM_STEPS * uv_y - section;
            switch (section)
            {
            case 0:
                gradient = lerp(_Color0, _Color1, blending);
                break;
            case 1:
                gradient = lerp(_Color1, _Color2, blending);
                break;
            case 2:
                gradient = lerp(_Color2, _Color3, blending);
                break;
            case 3:
                gradient = lerp(_Color3, _Color4, blending);
                break;
            case 4:
                gradient = lerp(_Color4, _Color5, blending);
                break;
            case 5:
                gradient = lerp(_Color5, _Color6, blending);
                break;
            case 6:
                gradient = lerp(_Color6, _Color7, blending);
                break;
            case 7:
                gradient = lerp(_Color7, _Color8, blending);
                break;
            case 8:
                gradient = lerp(_Color8, _Color9, blending);
                break;
            case 9:
                gradient = lerp(_Color9, _Color10, blending);
                break;
            case 10:
                gradient = lerp(_Color10, _Color11, blending);
                break;
            case 11:
                gradient = lerp(_Color11, _Color12, blending);
                break;
            case 12:
                gradient = lerp(_Color12, _Color13, blending);
                break;
            case 13:
                gradient = lerp(_Color13, _Color14, blending);
                break;
            case 14:
                gradient = lerp(_Color14, _Color15, blending);
                break;
            case 15:
                gradient = lerp(_Color15, _Color16, blending);
                break;
            case 16:
                gradient = lerp(_Color16, _Color17, blending);
                break;
            case 17:
                gradient = lerp(_Color17, _Color18, blending);
                break;
            case 18:
                gradient = lerp(_Color18, _Color19, blending);
                break;
            default:
                gradient = lerp(_Color19, _Color19, blending);
                break;
            }

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            c = lerp(c, gradient, _GradientStrength);

            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}