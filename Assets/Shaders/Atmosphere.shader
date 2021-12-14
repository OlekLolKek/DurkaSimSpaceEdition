Shader "Custom/Atmosphere"
{
    Properties
    {
        [MainColor] _Color("Main Color", COLOR) = (1,1,1,1)
        _Size("Size", Range(0,20)) = 0.5
        _Transparency("Transparency", Range(0, 20)) = 1
        [HideInInspector] _ClampLower("ClampLower", Range(-10, 10)) = 0
        [HideInInspector] _ClampUpper("ClampUpper", Range(-10, 10)) = 0.75
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite OFF

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            float4 _Color;
            float _Size;
            sampler2D _CameraDepthTexture;
            float _Transparency;
            float _ClampLower;
            float _ClampUpper;

            struct MeshData
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 wPos : TEXCOORD2;
            };

            Interpolators vert(MeshData v)
            {
                Interpolators result;
                float radius = _Size / 2;
                v.vertex.xyz = radius * v.normal;
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.normal = UnityObjectToWorldNormal(v.normal);
                result.wPos = mul(unity_ObjectToWorld, v.vertex);
                return result;
            }

            float4 frag(Interpolators i) : SV_Target
            {
                const float3 N = normalize(i.normal);
                const float3 V = normalize(_WorldSpaceCameraPos - i.wPos);

                float fresnel = dot (V, N) + 0.1; // +0.1 is used to remove the black edges
                fresnel = tanh(fresnel); // tanh is used to distribute the opacity more evenly
                float4 color = _Color;
                color.w = pow(clamp(fresnel, _ClampLower, _ClampUpper), _Transparency);
                return color;
            }

            ENDCG
        }
    }
}