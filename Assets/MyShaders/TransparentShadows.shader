// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Exploration/Hair Soft Edge Surface ZPrimed" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "gray" {}
        _SpecularTex ("Specular (R) Gloss (G) Null (B)", 2D) = "gray" {}
        _BumpMap ("Normal (Normal)", 2D) = "bump" {}
        _AnisoDirection ("Anisotropic Direction (RGB) Anisotropic Mask (A)", 2D) = "bump" {}
        _AnisoOffset ("Anisotropic Highlight Offset", Range(-0.5,0.5)) = -0.2
        _Cutoff ("Alpha Cut-Off Threshold", Range(0,1)) = 0.5
        _Fresnel ("Fresnel Value", Float) = 0.028
    }
 
    SubShader {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}
 
        Pass {
            Name "Depth"
            Tags {"LightMode" = "Always"}
            ColorMask 0
 
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
 
                #include "UnityCG.cginc"
 
                struct appdata {
                    float4 vertex : POSITION;
                    float4 texcoord : TEXCOORD0;
                };
 
                struct v2f
                {
                    float4  pos : SV_POSITION;
                    float2  uv : TEXCOORD0;
                };
 
                float4 _MainTex_ST;
 
                v2f vert (appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }
 
                sampler2D _MainTex;
                float _Cutoff;
 
                fixed4 frag(v2f IN) : COLOR
                {
                    fixed alpha = tex2D(_MainTex, IN.uv).a;
                    clip(alpha - _Cutoff);
                    return fixed4(0.0,0.0,0.0,0.0);
                }
            ENDCG
        }
 
        ZWrite Off
 
        Blend SrcAlpha OneMinusSrcAlpha
 
        CGPROGRAM
            #pragma surface surf ExplorationSoftHair fullforwardshadows exclude_path:prepass nolightmap nodirlightmap
            #pragma target 3.0
 
            struct SurfaceOutputHair {
                fixed3 Albedo;
                fixed Alpha;
                fixed3 AnisoDir;
                fixed3 Normal;
                fixed2 Specular;
                fixed3 Emission;
            };
 
            struct Input
            {
                float2 uv_MainTex;
            };
           
            sampler2D _MainTex, _SpecularTex, _BumpMap, _AnisoDirection;
            float _Cutoff, _AnisoOffset, _Fresnel;
               
            void surf (Input IN, inout SurfaceOutputHair o)
            {
                float4 albedo = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = albedo.rgb;
                o.Alpha = albedo.a;
                o.AnisoDir = tex2D(_AnisoDirection, IN.uv_MainTex).rgb * 2 - 1;
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
                o.Specular = tex2D(_SpecularTex, IN.uv_MainTex).rg;
               
                // Stop DX11 complaining.
                o.Emission = fixed3(0.0,0.0,0.0);
            }
 
            inline fixed4 LightingExplorationSoftHair (SurfaceOutputHair s, fixed3 lightDir, fixed3 viewDir, fixed atten)
            {
                viewDir = normalize(viewDir);
                lightDir = normalize(lightDir);
                s.Normal = normalize(s.Normal);
                float NdotL = dot(s.Normal, lightDir);
                float3 h = normalize(lightDir + viewDir);
                float VdotH = dot( viewDir, h );
 
                float fresnel = pow( 1.0 - VdotH, 5.0 );
                fresnel += _Fresnel * ( 1.0 - fresnel );
                float aniso = max(0, sin(radians( (dot(normalize(s.Normal + s.AnisoDir), h) + _AnisoOffset) * 180 ) ));
               
                float spec = pow( aniso, s.Specular.g * 128 ) * s.Specular.r * fresnel;
 
                #if !defined(UNITY_PASS_FORWARDBASE)
                    // Make the resulting value black where the alpha is.
                    // Otherwise it blends through on transparent parts.
                    // Not required for forward base - only forwardadd.
                    atten *= s.Alpha;
                #endif
               
                fixed4 c;
                c.rgb = ( ((s.Albedo * _LightColor0.rgb) * (saturate(NdotL) * atten)) + ((spec * atten) * _LightColor0.rgb) ) * 2;
                c.a = s.Alpha;
               
                return c;
            }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}