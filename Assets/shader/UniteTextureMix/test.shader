Shader "Custom/test"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Height("Height", Range(-20,20)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Geometry-1" "ForceNoShadowCasting" = "True" }
        //Tags { "RenderType" = "Opaque" }
        LOD 200

        Stencil
        {
            Ref 10
            Comp Always
            Pass Replace
        }

            CGPROGRAM
                #pragma surface surf NoLighting alpha

                fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
                {
                    fixed4 c;
                    c.rgb = s.Albedo;
                    c.a = s.Alpha;
                    return c;
                }

                struct Input
                {
                    float2 uv_MainTex;
                };

                void surf(Input IN, inout SurfaceOutput o)
                {
                }

            ENDCG

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Height;

            v2f vert (appdata_full v)
            {
                v2f o;
                v.vertex.x += v.normal.x * _Height;
                v.vertex.z += v.normal.z * _Height;
                v.vertex.y = 0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
