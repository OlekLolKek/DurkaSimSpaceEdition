Shader "Custom/UnlitTextureMix"
{
    Properties
    {
        _Texture1 ("Texture1", 2D) = "white" {}
        _Texture2 ("Texture2", 2D) = "white" {}
        _MixValue("MixValue", Range(0,1)) = 0.5
        _Color("Main Color", COLOR) = (1,1,1,1)
        _Height("Height", Range(0,20)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Texture1;
            float4 _Texture1_ST;

            sampler2D _Texture2;
            float2 _Texture2_ST;

            float _MixValue;
            float4 _Color;
            float _Height;

            struct v2f
            {
                float2 uv : TEXCOORD;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_full v)
            {
                v2f result;
                v.vertex.xyz -= v.normal * _Height * (0.5 - v.texcoord.x) * (0.5 - v.texcoord.x);
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.uv = TRANSFORM_TEX(v.texcoord, _Texture1);
                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color;
                color = tex2D(_Texture1, i.uv) * _MixValue;
                color += tex2D(_Texture2, i.uv) * (1 - _MixValue);
                color = color * _Color;
                return color;
            }

            ENDCG
        }
    }
}