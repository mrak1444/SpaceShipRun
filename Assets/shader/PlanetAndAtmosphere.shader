Shader "Custom/PlanetAndAtmosphere"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1) //
        _MainTex("Albedo (RGB)", 2D) = "white" {} //
        _AtmoColor("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1) //
        _HeightAtmosphere("Size", Float) = 0.1 //
        _Falloff("Falloff", Float) = 5 //
        _Transparency("Transparency", Float) = 15 //
        _Emission("Emission", Color) = (1,1,1,1) //
        _HeightPlanet("Height", Range(-1,1)) = 0 //
        _Seed("Seed", Range(0,10000)) = 10 //
    }
        SubShader
        {
            //Tags { "RenderType"="Opaque" }
            //LOD 100
            Cull Back

            CGPROGRAM

                #pragma surface surf Lambert noforwardadd noshadow vertex:vert
                #pragma target 3.0

                sampler2D _MainTex;
                fixed4 _Color;
                float4 _Emission;
                float _HeightPlanet;
                float _Seed;

                struct Input
                {
                    float2 uv_MainTex;
                    float4 color: COLOR;
                };

                float hash(float2 st)
                {
                    return frac(sin(dot(st.xy, float2(12.9898, 78.233))) * 43758.5453123);
                }

                float noise(float2 p, float size)
                {
                    float result = 0;
                    p *= size;
                    float2 i = floor(p + _Seed);
                    float2 f = frac(p + _Seed / 739);
                    float2 e = float2(0, 1);
                    float z0 = hash((i + e.xx) % size);
                    float z1 = hash((i + e.yx) % size);
                    float z2 = hash((i + e.xy) % size);
                    float z3 = hash((i + e.yy) % size);
                    float2 u = smoothstep(0, 1, f);
                    result = lerp(z0, z1, u.x) + (z2 - z0) * u.y * (1.0 - u.x) + (z3 - z1) *
                    u.x * u.y;
                    return result;
                }

                void vert(inout appdata_full v)
                {
                    float height = noise(v.texcoord, 5) * 0.75 + noise(v.texcoord, 30) *
                    0.125 + noise(v.texcoord, 50) * 0.125;
                    v.color.r = height + _HeightPlanet;
                }

                void surf(Input IN, inout SurfaceOutput o)
                {
                    fixed4 color = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                    float height = IN.color.r;

                    if (height < 0.45)
                    {
                        color.x = 0.10;
                        color.y = 0.30;
                        color.z = 0.50;
                    }
                    else if (height < 0.75)
                    {
                        color.x = 0.10;
                        color.y = 0.60;
                        color.z = 0.30;
                    }
                    else
                    {
                        color.x = 0.60;
                        color.y = 0.30;
                        color.z = 0.30;
                    }

                    o.Albedo = color.rgb;
                    o.Emission = _Emission.xyz;
                    o.Alpha = color.a;
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
                    float _HeightAtmosphere;
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

                        v.vertex.xyz += v.normal * _HeightAtmosphere;
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