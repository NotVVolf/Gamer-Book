Shader "Custom/CardShader"
{
    Properties
    {
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
    }

    SubShader
    {
        ColorMask 0
        ZWrite Off
        Tags
        {
            "RenderType"="Opaque"
        }
        Stencil
        {
            Ref [_StencilRef]
            Comp Always
            Pass Replace
        }
        Pass
        {

            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_TARGET
            {
                return half4(1, 0, 0, 0.5);
            }
            ENDCG
        }
    }
}