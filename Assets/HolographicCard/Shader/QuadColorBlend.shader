Shader "Custom/QuadColorBlend"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Stencil
        {
            Ref [_StencilRef]
            Comp Equal
        }
        Blend SrcAlpha OneMinusSrcAlpha

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
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            static half4 blendColors[4] = {
                half4(0.447, 0.035, 0.82, 1),
                half4(1, 1, 0, 1),
                half4(1, 0, 1, 1),
                half4(1, 0.075, 0.851, 1),
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = blendColors[v.uv.x + v.uv.y * 2];
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
