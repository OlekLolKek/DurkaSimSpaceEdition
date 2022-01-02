Shader "Custom/PlanetRing"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Cutoff ("Cutoff", Range(-5, 5)) = 0.5
        _Width ("Width", Range(0, 1)) = 0.5
        _InnerWidth ("InnerWidth", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "ForceNoShadowCasting"="True" "Queue"="Geometry+1"}
        LOD 200
        Cull off
        ZTest Less

        CGPROGRAM
        
        #pragma surface surf NoLighting noshadow noforwardadd alpha
        
        #pragma target 3.0
        
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };
        
        fixed4 _Color;
        float _Width;
        float _InnerWidth;

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = _Color;
            o.Albedo = c.rgb;
            float2 uv = IN.uv_MainTex;
            float magnitude = sqrt(pow(uv.x - 0.5, 2) + pow(uv.y - 0.5, 2));

            o.Alpha = c.a;
            
            if (magnitude < _InnerWidth / 2)
            {
                o.Alpha = 0;
            }
            if (magnitude > _Width / 2)
            {
                o.Alpha = 0;
            }
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
