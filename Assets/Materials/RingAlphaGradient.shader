Shader "Custom/RingAlphaGradient"
{
    Properties {
        _RingColor ("Ring Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Range(0.1, 0.95)) = 0.5
        _RingWidth ("Ring Width", Range(0.001, 1.0)) = 0.05
        _MiddleRadius ("Part of Radius to Middle", Range(0.0, 1.0)) = 0.5
        _InnerAlpha ("Inner Alpha", Range(0.0, 1.0)) = 1.0
        _MiddleAlpha ("Middle Alpha", Range(0.0, 1.0)) = 1.0
        _OuterAlpha ("Outer Alpha", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _RingColor;
            float _Radius;
            float _RingWidth;
            float _MiddleRadius;
            float _InnerAlpha;
            float _MiddleAlpha;
            float _OuterAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0; // Transform UVs to [-1, 1]
                float dist = length(uv);
                
                // Ring
                float innerRadius = _Radius - _RingWidth;
                float outerRadius = _Radius + _RingWidth;
                float ring = smoothstep(innerRadius, innerRadius + 0.01, dist) - smoothstep(outerRadius, outerRadius + 0.01, dist);

                // Grad
                float innerToMiddle = smoothstep(innerRadius, _Radius * _MiddleRadius, dist);
                float middleToOuter = smoothstep(_Radius * _MiddleRadius, outerRadius, dist);
                
                float finalAlpha = lerp(_InnerAlpha, _MiddleAlpha, innerToMiddle);
                finalAlpha = lerp(finalAlpha, _OuterAlpha, middleToOuter);

                half4 ringColor = _RingColor;
                ringColor.a *= ring * finalAlpha;

                return ringColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}