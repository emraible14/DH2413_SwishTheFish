Shader "Unlit/learning"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)

        
    }
    SubShader // passes through all the pixels
    {
        Tags { "RenderType"="Opaque" } // determines the order in the rendering pipeline. 
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // the pixels of the texture
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex; 
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); // vertex coordinate into pixel coordinates (screen coordinates)
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // -/-?
                return o;
            }

            
            // You can add functions like thissss!!!!!
            float Circle(fixed2 uv, float2 position, float radius, float blur) {
                float distance = length(uv - position); // length(uv) is the distance from origin
                float c = smoothstep(radius, radius-blur, distance);

                return c;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = tex2D(_MainTex, i.uv); // maps, the uv is the coordinates of the mesh.

                /*fixed2 centre = float2(0.5, 0.5);
                float radius = 0.5;
                float distance = length(centre - i.uv);
                if (abs(distance - radius)<= 0.01){
                    return 1;
                } return 0;*/
                
                //fixed4 interpolated = float4(i.uv.x-0.5, i.uv.y-0.5, 0, 1);
                //clamped, negative is black and over one is white. 
                
                /*if (i.uv.x - i.uv.y <= 0.01) {
                    return 1;
                }
                return 0;*/

                //i.uv -= 0.5;
                
                //float aspect = _ScreenParams.x / _ScreenParams.y;
                //i.uv.x *= aspect; How do you find the aspect ratio of a quad??
                
                float c = Circle(i.uv, (.5, .5), .4, .05);
                c -= Circle(i.uv, (.3, .7), .07, .01);
                c -= Circle(i.uv, (.7, .3), .07, .01);

                //if (d < 0.3) c = 1; else c = 0; // A circle

                fixed4 col = float4(c, c, c, 1);

                return col;
                
            }
            ENDCG
        }
    }
}
