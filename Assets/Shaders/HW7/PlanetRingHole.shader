Shader "Custom/PlanetRingHole"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Geometry-1" "ForceNoShadowCasting"="True" }
        LOD 200
        
        Stencil
        {
            Ref 10
            Comp Always
            Pass Replace
        }

        CGPROGRAM
        
        #pragma surface surf NoLighting alpha
        
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }
        
        void surf (Input IN, inout SurfaceOutput o)
        {
        }
        
        ENDCG
    }
    FallBack "Diffuse"
}
