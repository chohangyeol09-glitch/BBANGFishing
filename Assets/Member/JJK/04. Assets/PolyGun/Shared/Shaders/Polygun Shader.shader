Shader "PolyGun/Gun Shader URP" {
    Properties {
        _CamoTexture ("Camo Texture", 2D) = "white" {}
        _CamoColor ("Camo Color", Color) = (1,1,1,1)
        _CamoMetallic ("Camo Metallic", Range(0, 1)) = 0.3
        _CamoGloss ("Camo Gloss", Range(0, 1)) = 0.2
        _Color1 ("Color 1", Color) = (0.8,0.8,0.8,1)
        _Color1Metallic ("Color 1 Metallic", Range(0, 1)) = 0.9
        _Color1Gloss ("Color 1 Gloss", Range(0, 1)) = 0.7
        _Color2 ("Color 2", Color) = (0.51,0.51,0.51,1)
        _Color2Metallic ("Color 2 Metallic", Range(0, 1)) = 1
        _Color2Gloss ("Color 2 Gloss", Range(0, 1)) = 0.05
        _Color3 ("Color 3", Color) = (0,0,0,1)
        _Color3Metallic ("Color 3 Metallic", Range(0, 1)) = 0
        _Color3Gloss ("Color 3 Gloss", Range(0, 1)) = 0.15
        _Color4 ("Color 4", Color) = (0.12,0.12,0.12,1)
        _Color4Metallic ("Color 4 Metallic", Range(0, 1)) = 0
        _Color4Gloss ("Color 4 Gloss", Range(0, 1)) = 0.15
        _Color5 ("Color 5", Color) = (0.3,0.3,0.3,1)
        _Color5Metallic ("Color 5 Metallic", Range(0, 1)) = 0.75
        _Color5Gloss ("Color 5 Gloss", Range(0, 1)) = 0.6
        _Color6 ("Color 6", Color) = (0.51,0.51,0.51,1)
        _Color6Metallic ("Color 6 Metallic", Range(0, 1)) = 0.75
        _Color6Gloss ("Color 6 Gloss", Range(0, 1)) = 0.6
        _Emission1 ("Emission 1", Color) = (1,0,0,1)
        _Emission2 ("Emission 2", Color) = (0,1,0,1)
        _Emission3 ("Emission 3", Color) = (0,0,1,1)
        [HideInInspector]_TextureMask ("Texture Mask", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }

        Pass {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _CamoTexture_ST;
                float4 _TextureMask_ST;
                half4 _CamoColor;
                half _CamoMetallic, _CamoGloss;
                half4 _Color1; half _Color1Metallic, _Color1Gloss;
                half4 _Color2; half _Color2Metallic, _Color2Gloss;
                half4 _Color3; half _Color3Metallic, _Color3Gloss;
                half4 _Color4; half _Color4Metallic, _Color4Gloss;
                half4 _Color5; half _Color5Metallic, _Color5Gloss;
                half4 _Color6; half _Color6Metallic, _Color6Gloss;
                half4 _Emission1, _Emission2, _Emission3;
            CBUFFER_END

            TEXTURE2D(_CamoTexture);  SAMPLER(sampler_CamoTexture);
            TEXTURE2D(_TextureMask);  SAMPLER(sampler_TextureMask);

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float4 shadowCoord: TEXCOORD3;
                float  fogFactor  : TEXCOORD4;
            };

            Varyings vert (Attributes v) {
                Varyings o;
                VertexPositionInputs vpi = GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs   vni = GetVertexNormalInputs(v.normalOS);
                o.positionCS = vpi.positionCS;
                o.positionWS = vpi.positionWS;
                o.normalWS   = vni.normalWS;
                o.uv         = v.uv;
                o.shadowCoord = GetShadowCoord(vpi);
                o.fogFactor  = ComputeFogFactor(vpi.positionCS.z);
                return o;
            }

            half4 frag (Varyings i) : SV_Target {
                float2 uv = i.uv;
                
                half4 node_8203 = SAMPLE_TEXTURE2D(_TextureMask, sampler_TextureMask, TRANSFORM_TEX(uv, _TextureMask));
                half3 mask1 = node_8203.rgb;

                float2 tcRcp = float2(1.0, 1.0);
                float2 uvOffA = (uv + float2(1.9339 - floor(1.9339), floor(1.9339))) * tcRcp;
                half4 node_3891 = SAMPLE_TEXTURE2D(_TextureMask, sampler_TextureMask, TRANSFORM_TEX(uvOffA, _TextureMask));
                half mask4 = 1.0 - (node_3891.r + node_3891.g + node_3891.b);

                half4 node_3179 = SAMPLE_TEXTURE2D(_TextureMask, sampler_TextureMask, TRANSFORM_TEX(uvOffA, _TextureMask));
                half3 mask2 = node_3179.rgb;

                float2 uvOffB = (uv + float2(1.868 - floor(1.868), floor(1.868))) * tcRcp;
                half4 node_3478 = SAMPLE_TEXTURE2D(_TextureMask, sampler_TextureMask, TRANSFORM_TEX(uvOffB, _TextureMask));
                half3 mask3 = node_3478.rgb;
                
                half gloss = (lerp(lerp(lerp(_CamoGloss, _Color1Gloss, mask1.r), _Color2Gloss, mask1.g), _Color3Gloss, mask1.b) * mask4)
                           + (mask2.r * _Color4Gloss + mask2.g * _Color5Gloss + mask2.b * _Color6Gloss);
                half metallic = (lerp(lerp(lerp(_CamoMetallic, _Color1Metallic, mask1.r), _Color2Metallic, mask1.g), _Color3Metallic, mask1.b) * mask4)
                              + (mask2.r * _Color4Metallic + mask2.g * _Color5Metallic + mask2.b * _Color6Metallic);
                half smoothness = gloss;
                
                half4 camoTex = SAMPLE_TEXTURE2D(_CamoTexture, sampler_CamoTexture, TRANSFORM_TEX(uv, _CamoTexture));
                half3 albedo = lerp(lerp(lerp(camoTex.rgb * _CamoColor.rgb, _Color1.rgb, mask1.r), _Color2.rgb, mask1.g), _Color3.rgb, mask1.b) * mask4
                             + (mask2.r * _Color4.rgb + mask2.g * _Color5.rgb + mask2.b * _Color6.rgb);

                half3 emission = mask3.r * _Emission1.rgb + mask3.g * _Emission2.rgb + mask3.b * _Emission3.rgb;
                
                half3 normalWS = normalize(i.normalWS);
                half3 viewDirWS = GetWorldSpaceNormalizeViewDir(i.positionWS);

                Light mainLight = GetMainLight(i.shadowCoord);
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 radiance = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation) * NdotL;

                half3 halfDir = normalize(mainLight.direction + viewDirWS);
                half NdotH = saturate(dot(normalWS, halfDir));
                half specPow = exp2(gloss * 10.0 + 1.0);
                half3 specular = radiance * pow(NdotH, specPow) * lerp(0.04, 1.0, metallic);

                half3 ambient = half3(0.12, 0.12, 0.12);
                half3 diffuse = albedo * (radiance * (1.0 - metallic) + ambient);

                half3 color = diffuse + specular + emission;
                color = MixFog(color, i.fogFactor);

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}