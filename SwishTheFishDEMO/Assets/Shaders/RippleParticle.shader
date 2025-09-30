Shader "Unlit/RippleParticle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Radius", float) = 0.1
        _Thickness ("Ripple Thickness", float) = 0.1
    }
    SubShader
    {
        Tags {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define TAU 6.283185307179586
            
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
            float _Radius;
            float _Thickness;

            float GetWave(float2 uv)
            {
                float2 centeredUv = uv * 2 - 1;

                float radialDistance = length(centeredUv);

                float wave = cos((radialDistance - _Time.y * 0.1) * TAU * 5);

                return wave * (1 - radialDistance);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 center = float4(0.5, 0.5, 0, 0);
                float distance = length(center - i.uv);
                

                if (abs(distance - _Radius) <= _Thickness)
                {
                    return fixed4(1, 1, 1, 1);
                }
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
