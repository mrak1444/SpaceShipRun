Shader "Custom/Atmosphere"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _BumpMap("Norlam Map", 2D) = "bump" {}
        _AtmoColor("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1)
        _Height("Size", Float) = 0.1
        _Falloff("Falloff", Float) = 5
        _Transparency("Transparency", Float) = 15
    }
        SubShader
        {
            //Tags { "RenderType"="Opaque" }
            //LOD 100
            Cull Back

            CGPROGRAM

                #pragma surface surf Standard fullforwardshadows
                #pragma target 3.0

                sampler2D _MainTex;
                fixed _Glossiness;
                half _Metallic;
                sampler2D _BumpMap;
                fixed4 _Color;

                struct Input
                {
                    float2 uv_MainTex;
                };

                void surf(Input IN, inout SurfaceOutputStandard o)
                {
                    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                    o.Albedo = c.rgb;
                    o.Metallic = _Metallic;
                    o.Smoothness = _Glossiness;
                    o.Alpha = c.a;
                    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                }
            ENDCG

            Pass
            {
                Cull Front
                Blend SrcAlpha One

                CGPROGRAM

                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma fragmentoption ARB_fog_exp2
                    #pragma fragmentoption ARB_precision_hint_fastest
                    #include "UnityCG.cginc"

                    sampler2D _MainTex;
                    float4 _Color;
                    float4 _AtmoColor;
                    float _Height;
                    float _Falloff;
                    float _Transparency;

                    struct v2f
                    {
                        float4 pos : SV_POSITION;
                        float3 normal : TEXCOORD0;
                        float3 worldvertpos : TEXCOORD1;
                    };

                    v2f vert(appdata_base v)
                    {
                        v2f o;

                        v.vertex.xyz += v.normal * _Height;
                        o.pos = UnityObjectToClipPos(v.vertex);
                        o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                        o.worldvertpos = mul(unity_ObjectToWorld, v.vertex);

                        return o;
                    }

                    float4 frag(v2f i) : COLOR
                    {
                        i.normal = normalize(i.normal);
                        float3 viewdir = normalize(i.worldvertpos - _WorldSpaceCameraPos);

                        float4 color = _AtmoColor;
                        color.a = dot(viewdir, i.normal);
                        color.a *= dot(i.normal, _WorldSpaceLightPos0);
                        /*color.a = saturate(color.a);
                        color.a = pow(color.a, _Falloff);
                        color.a *= _Transparency;*/
                        return color;
                    }
                ENDCG
            }
        }
}
