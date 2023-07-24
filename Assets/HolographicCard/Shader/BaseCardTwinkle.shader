Shader "Custom/BaseCardTwinkle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MinusVal ("Minus Val", Range(0, 1)) = 0
        _Threshold ("Threshold", Range(0, 1)) = 0
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Stencil
        {
            Ref [_StencilRef]
            Comp Equal
        }
        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Threshold;
            float _MinusVal;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float val = tex2D(_MainTex, i.uv).r;
                val = -abs(val - _MinusVal);
                val += _Threshold;
                val = saturate(val) * 2;

                return float4(val, val, val, 1);
            }
            ENDCG
        }
    }
}