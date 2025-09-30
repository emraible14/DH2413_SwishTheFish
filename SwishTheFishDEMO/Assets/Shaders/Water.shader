Shader"Unlit/Water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "gray" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Amplitude ("Amplitude", float) = .01
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
                float4 normals : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Amplitude;
            float4 _Color;

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
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                float height = tex2Dlod(_MainTex, float4(o.uv, 0, 0)).r;
                
                v.vertex.y = height * _Amplitude;

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainText = tex2D(_MainTex, i.uv);
                
                return mainText.r;
            }
            ENDCG
        }
    }
}
