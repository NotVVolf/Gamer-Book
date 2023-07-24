Shader "Custom/PlanetariumSphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Offset ("Offset", float) = 0

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
        Blend SrcAlpha OneMinusSrcAlpha

        Cull Front

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 position : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Offset;

            static half4 blendColors[4] = {
                half4(0.447, 0.035, 0.82, 1),
                half4(1, 1, 0, 1),
                half4(1, 0, 1, 1),
                half4(1, 0.075, 0.851, 1),
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.position = normalize(UnityObjectToWorldDir(v.vertex) + half3(0, _Offset, 0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float square(const float v) { return v * v; }
            fixed4 frag(v2f i) : SV_Target
            {
                return lerp(
                    lerp(blendColors[0], blendColors[1], i.position.x),
                    lerp(blendColors[2], blendColors[3], i.position.y),
                    i.position.z) * (tex2D(_MainTex, i.uv).r + 0.5);
            }
            ENDCG
        }
    }
}