﻿Shader "Custom/RotateShader" {

Properties{
        _Color("Color", Color) = (1, 1, 1, 1)
        [PerRendererData]_MainTex("Main Texture", 2D) = "white"{}

        _RSpeed("Rotate Speed", Range(1, 100)) = 10

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
}

SubShader{
        tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

    Pass{
        Name "RotateShader"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };


        float4 _Color;
        sampler2D _MainTex;
        float4 _MainTex_ST;

        float _RSpeed;


        v2f vert (appdata_t v){
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            OUT.worldPosition = v.vertex;
            OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
            OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
            return OUT;
        }

        half4 frag(v2f IN) : SV_Target{
        
            float2 uv = IN.texcoord.xy - float2(0.5, 0.5);
            uv = float2(uv.x * cos(_RSpeed * _Time.x) - uv.y * sin(_RSpeed * _Time.x), 
            uv.x * sin(_RSpeed * _Time.x) + uv.y * cos(_RSpeed * _Time.x));
            uv += float2(0.5, 0.5);
            half4 c = tex2D(_MainTex, uv) * _Color;

            float2 uv2 = IN.texcoord.xy - float2(0.5, 0.5);
            uv2 = float2(uv2.x * cos(-_RSpeed * _Time.x) - uv2.y * sin(-_RSpeed * _Time.x), 
            uv2.x * sin(-_RSpeed * _Time.x) + uv2.y * cos(-_RSpeed * _Time.x));
            uv2 += float2(0.5, 0.5);
            half4 c2 = tex2D(_MainTex, uv2) * _Color;

            return (c*0.7) + (c2*0.7);
        }
        ENDCG
    }

    }

}