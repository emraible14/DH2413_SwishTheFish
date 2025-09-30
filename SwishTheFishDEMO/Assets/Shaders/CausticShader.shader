Shader "Unlit/CausticShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Text2 ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Radius ("Radius", Float) = 0.2
        _CausticSpeed ("Caustic Speed", float) = 0.1
        _SplitRGB ("SplitRGB", float) = 0.1
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
            sampler2D _Text2;
            float4 _MainTex_ST;
            float4 _Color;
            float _Radius;
            float _CausticSpeed;
            float _SplitRGB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 mainTextUv = i.uv - _Time.y * _CausticSpeed;
                float2 text2Uv = i.uv + _Time.y * _CausticSpeed;
                // sample the texture
                fixed4 mainText = tex2D(_MainTex, mainTextUv);
                fixed4 text2 = tex2D(_Text2, text2Uv);

                fixed s = _SplitRGB;
                fixed r = tex2D(_MainTex, mainTextUv + fixed2(+s, -s)).r;
                fixed g = tex2D(_MainTex, mainTextUv + fixed2(+s, -s)).g;
                fixed b = tex2D(_MainTex, mainTextUv + fixed2(-s, -s)).b;
                mainText += float4(r, g, b, 0.2);
                fixed r2 = tex2D(_Text2, text2Uv + fixed2(+s, +s)).r;
                fixed g2 = tex2D(_Text2, text2Uv + fixed2(+s, -s)).g;
                fixed b2 = tex2D(_Text2, text2Uv + fixed2(-s, -s)).b;
                text2 += float4(r2, g2, b2, 0.2);

                // fixed4 center = float4(0.5, 0.5, 0, 0);
                // float distance = length(center - i.uv);
                //
                // float radius = fmod(_Radius * _Time.y, 1);
                // if (abs(distance - radius) <= 0.001)
                // {
                //     return fixed4(1, 1, 1, 1);
                // }
                // return fixed4(0, 0, 0, 0);

                return min(mainText, text2) + _Color;
            }
            ENDCG
        }
    }
}
