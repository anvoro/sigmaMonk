Shader "Custom/RingShader" {
    Properties {
        _RingColor ("Ring Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Range(0.1, 0.95)) = 0.5
        _RingWidth ("Ring Width", Range(0.001, 0.1)) = 0.05
        _GradWidth ("Grad Width", Range(0.0, 0.15)) = 0.05
        _MinGradAlpha ("Min Alpha", Range(0.0, 1.0)) = 0.0
        _MaxGradAlpha ("Max Alpha", Range(0.0, 1.0)) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _RingColor;
            float _Radius;
            float _RingWidth;
            float _GradWidth;
            float _MinGradAlpha;
            float _MaxGradAlpha;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            half4 frag (v2f i) : SV_Target {
                float2 uv = i.uv * 2.0 - 1.0; // Transform UVs to [-1, 1]
                float dist = length(uv);

                // Ring
                float innerRadius = _Radius - _RingWidth;
                float outerRadius = _Radius + _RingWidth;
                float ring = smoothstep(innerRadius, innerRadius + 0.01, dist) - smoothstep(outerRadius, outerRadius + 0.01, dist);
                half4 ringColor = _RingColor;
                ringColor.a *= ring;

                // Gradient
                float innerGradientRadius = innerRadius - _GradWidth;
                float outerGradientRadius = outerRadius + _GradWidth;
                float grad = smoothstep(innerGradientRadius, innerGradientRadius + 0.01, dist) - smoothstep(outerGradientRadius, outerGradientRadius + 0.01, dist);
                half4 gradColor = _RingColor;

                float gradAlpha = smoothstep(innerGradientRadius, _Radius, dist) * (1.0 - smoothstep(_Radius, outerGradientRadius, dist));
                gradAlpha = lerp(_MinGradAlpha, _MaxGradAlpha, gradAlpha);
                
                gradColor.a *= grad * gradAlpha;
                
                // Combine rings
                return ringColor + gradColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}