Shader "Unlit/Sand"
{
    Properties
    {
        _SandAlbedo (" Sand Albedo", 2D) = "white" {}
        _SandNormals ("Sand Normals", 2D) = "bump" {} // Normalmap: every pixel contains a direction
        _SandHeight ("Sand Height", 2D) = "gray" {} // centred extrusion
        _Color ("Color", Color) = (1, 1, 1, 1)
        _DisplacementIntensity ("Displaccement Intensity", Range(0,0.02)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase // Shadow addition
            #pragma multi_complile_shadowcaster // Shadow addition

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc" // Shadow addition

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT; // xyz = tangent direction, w = tangent sign
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // Shadow addition
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
                float4 pos : SV_POSITION;
                float3 tangent : TEXCOORD3;
                float3 bitangent : TEXCOORD4;
                float3 normal : TEXCOORD2;
                float3 worldPos : TEXCOORD5;
            };

            sampler2D _SandAlbedo;
            sampler2D _SandNormals;
            sampler2D _SandHeight;
            float4 _SandAlbedo_ST;
            float4 _Color;
            float _DisplacementIntensity;

            v2f vert (appdata v)
            {
                v2f o;

                //offset the vertex value
                o.uv = TRANSFORM_TEX( v.uv, _SandAlbedo );
                float height = tex2Dlod( _SandHeight, float4(o.uv, 0, 0)).x * 2 - 1; // remap from (0-1) to (-1 - 1)

                v.vertex.xyz += v.normal * (height * _DisplacementIntensity);
         
                o.pos = UnityObjectToClipPos( v.vertex );
                o.uv = TRANSFORM_TEX( v.uv, _SandAlbedo );
                o.normal = UnityObjectToWorldNormal( v.normal );
                o.worldPos = mul( unity_ObjectToWorld, v.vertex);
                o.tangent = UnityObjectToWorldDir( v.tangent.xyz );
                o.bitangent = cross( o.normal, o.tangent ) * (v.tangent.w * unity_WorldTransformParams.w); //correct the flipping/mirroring
                
                // Not sure what this does yet:
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));

                TRANSFER_SHADOW(o); // Shadow addition
                //TRANSFER_VERTEX_TO_FRAGMENT(o);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 sand = tex2D( _SandAlbedo, i.uv );

                float3 tangentSpaceNormal = UnpackNormal(tex2D( _SandNormals, i.uv)); // not in world space right now, only tangent space

                // want to contruct tangent space, we just know the direction of the normal in tangent space but not what
                // the tangent space is and what all directions are. Space --> matrix that contain the three directions we have.
                // tangent, bitangent and normal --> (x, y, z)
                float3x3 mtxTangentToWorld = {  // this cannow be used to transform from tangent space to world space
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                };

                float3 N = normalize(mul(mtxTangentToWorld, tangentSpaceNormal)); // this now gives the world space normal
                
                //Directional lights: (world space direction, 0), Other lights: (world space position, 1)
                // -> in base pass therefore directional light. 
                float3 L = normalize(_WorldSpaceLightPos0.xyz); //light vector from the surface to the light (a direction)
                
                
                //Phong-lighting - specular lighting
                    //view vector: position of the current fragment and the world space position of the camera
                    //float3 V = normalize(_WorldSpaceCameraPos - i.worldPos); //view vector: from the surface to the camera
                    //reflected vector
                    //float3 R = reflect(-L, N); //a reflected vector, incoming light is negative. 

                
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos); // Shadow addition
                float3 diffuse = saturate(dot(N, L)) * _LightColor0.rgb * atten; // Shadow addition

                //Lambertian - diffuse light
                //float3 diffuse = saturate(dot(N, L)) * _LightColor0.xyz * shadow; // same as max(0, dot(N, L)), clamped values.

                float3 finalClor = diffuse * sand.rgb * _Color.rgb;


                return float4(finalClor, 1);

            }
            ENDCG
        }
        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}

// sources: 
//  https://docs.unity3d.com/6000.2/Documentation/Manual/built-in-shader-examples-receive-shadows.html
//  Freya Holmer; Shaders for game devs, part [3] (for the albedo, height and normal maps) and [2] (for diffuse light)
