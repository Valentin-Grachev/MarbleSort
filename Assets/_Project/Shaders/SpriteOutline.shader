Shader "MarbleSort/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 32)) = 2
        _AlphaCutoff ("Alpha Cutoff", Range(0, 1)) = 0.2
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="False"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            Name "Mask"
            ColorMask 0

            Stencil
            {
                Ref 1
                ReadMask 1
                WriteMask 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment MaskFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #include "UnitySprites.cginc"

            float _AlphaCutoff;

            fixed4 MaskFrag(v2f IN) : SV_Target
            {
                fixed4 sprite = SampleSpriteTexture(IN.texcoord) * IN.color;
                clip(sprite.a - _AlphaCutoff);
                return 0;
            }
            ENDCG
        }

        Pass
        {
            Name "Outline"

            Stencil
            {
                Ref 1
                ReadMask 1
                WriteMask 1
                Comp NotEqual
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #include "UnitySprites.cginc"

            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _AlphaCutoff;

            struct v2f_outline
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float spriteAlpha : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            static const float2 kOutlineRing[16] =
            {
                float2( 1.0000,  0.0000),
                float2( 0.9239,  0.3827),
                float2( 0.7071,  0.7071),
                float2( 0.3827,  0.9239),
                float2( 0.0000,  1.0000),
                float2(-0.3827,  0.9239),
                float2(-0.7071,  0.7071),
                float2(-0.9239,  0.3827),
                float2(-1.0000,  0.0000),
                float2(-0.9239, -0.3827),
                float2(-0.7071, -0.7071),
                float2(-0.3827, -0.9239),
                float2( 0.0000, -1.0000),
                float2( 0.3827, -0.9239),
                float2( 0.7071, -0.7071),
                float2( 0.9239, -0.3827)
            };

            v2f_outline OutlineVert(appdata_t IN)
            {
                v2f_outline OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 expanded = IN.vertex.xyz;
                float2 uv = IN.texcoord;
                float2 uvDelta = uv - 0.5;
                float2 absUv = abs(uvDelta);
                float2 absPos = abs(expanded.xy);
                float2 texel = _MainTex_TexelSize.xy;

                float localTexel = 0;
                if (absUv.x >= absUv.y && absUv.x > 1e-4)
                    localTexel = (absPos.x / absUv.x) * texel.x;
                else if (absUv.y > 1e-4)
                    localTexel = (absPos.y / absUv.y) * texel.y;

                float posRadius = length(expanded.xy);
                if (localTexel > 0 && posRadius > 1e-4 && _OutlineWidth > 0)
                {
                    float expand = (_OutlineWidth + 1.0) * localTexel;
                    float2 deltaPos = (expanded.xy / posRadius) * expand;
                    float2 size = localTexel / max(texel, float2(1e-6, 1e-6));
                    expanded.xy += deltaPos;
                    uv += deltaPos / size;
                }

                OUT.vertex = UnityObjectToClipPos(UnityFlipSprite(expanded, _Flip));
                OUT.texcoord = uv;
                OUT.spriteAlpha = IN.color.a * _Color.a * _RendererColor.a;
                return OUT;
            }

            float SampleCoverage(float2 uv)
            {
                float2 inside = step(0.0, uv) * step(uv, 1.0);
                return SampleSpriteTexture(saturate(uv)).a * inside.x * inside.y;
            }

            fixed4 OutlineFrag(v2f_outline IN) : SV_Target
            {
                if (_OutlineWidth <= 0.001)
                    discard;

                float coverage = SampleCoverage(IN.texcoord);
                float2 texelStep = _MainTex_TexelSize.xy * _OutlineWidth;

                [unroll]
                for (int i = 0; i < 16; i++)
                {
                    coverage = max(coverage, SampleCoverage(IN.texcoord + kOutlineRing[i] * texelStep));
                    coverage = max(coverage, SampleCoverage(IN.texcoord + kOutlineRing[i] * texelStep * 0.5));
                }

                coverage *= IN.spriteAlpha;
                clip(coverage - _AlphaCutoff);

                fixed4 outline = _OutlineColor;
                outline.rgb *= outline.a;
                return outline;
            }
            ENDCG
        }

        Pass
        {
            Name "Sprite"

            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #include "UnitySprites.cginc"
            ENDCG
        }
    }

    Fallback "Sprites/Default"
}
