Shader "Custom/CardOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _XSize ("XSize", float) = 0
        _YSize ("YSize", float) = 0
        _BarOffset ("Bar Offset", float) = 0
        _BarHeight ("Bar Height", float) = 0
        _Outline ("Outline", float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 0)
        _ContentOutlineWidth ("Content Outline Width", float) = 0
        _ContentOutlineHeight ("Content Outline Height", float) = 0
        _ContentOutlineOffset ("Content Outline Offset", float) = 0

        [IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Tags
        {
            "RenderType"="Opaque"
        }
        Stencil
        {
            Ref [_StencilRef]
            Comp Equal
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "SDFShapes.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _XSize, _YSize, _Outline, _BarOffset, _BarHeight;
            float4 _OutlineColor;
            float _ContentOutlineWidth, _ContentOutlineHeight, _ContentOutlineOffset;

            half4 frag(v2f i) : SV_Target
            {
                half4 color = half4(0, 0, 0, 0);
                const float rectangle_sdf = _Outline - abs(-CurvedRectangle(i.uv, float2(_XSize, _YSize) / 4, 0.2));
                const float bar_sdf = -CurvedRectangle(i.uv + half2(0, _BarOffset), float2(_XSize, _BarHeight) / 4, 0);
                const float content_sdf = -CurvedRectangle(i.uv + half2(0, _ContentOutlineOffset), half2(_ContentOutlineWidth, _ContentOutlineHeight), 0.15);
                DrawSDF4(rectangle_sdf, color, _OutlineColor);
                DrawSDF4(bar_sdf, color, _OutlineColor);
                DrawSDF4(content_sdf, color, half4(1, 1, 1, 1));
                return color;
            }
            ENDCG
        }
    }
}