Shader "Custom/CircleIndicator" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Radius", Float) = 0.5
        _EdgeSharpness ("Edge Sharpness", Float) = 100
    }
    SubShader {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 100

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

            fixed4 _Color;
            float _Radius;
            float _EdgeSharpness;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float dist = length(i.uv - float2(0.5, 0.5)) * 2; 
                float alpha = smoothstep(_Radius, _Radius - (_Radius / _EdgeSharpness), dist);
                alpha *= _Color.a;
                return fixed4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
