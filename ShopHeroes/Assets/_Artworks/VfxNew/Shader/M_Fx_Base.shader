// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ShopHero/M_Fx_Base"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][HDR]_BaseColor1("BaseColor", Color) = (1,1,1,1)
		_BaseTex1("BaseTex", 2D) = "white" {}
		_BaseTexSpeedX1("BaseTexSpeedX", Float) = 0
		_BaseTexSpeedY1("BaseTexSpeedY", Float) = 0
		[Toggle(_AR1_ON)] _AR1("A><R", Float) = 0
		_MaskTex1("MaskTex", 2D) = "white" {}
		_MaskSpeedX1("MaskSpeedX", Float) = 0
		_MaskSpeedY1("MaskSpeedY", Float) = 0
		_NoiseTex1("NoiseTex", 2D) = "white" {}
		_Noise1SpeedX1("Noise1SpeedX", Float) = 0
		_Noise1SpeedY1("Noise1SpeedY", Float) = 0
		_Noise2SpeedX1("Noise2SpeedX", Float) = 0
		_Noise2SpeedY1("Noise2SpeedY", Float) = 0
		_NoiseBlend1("NoiseBlend", Float) = 0
		_DissolveTex1("DissolveTex", 2D) = "white" {}
		_DissolveTexSpeedX1("DissolveTexSpeedX", Float) = 0
		_DissolveTexSpeedY1("DissolveTexSpeedY", Float) = 0
		_Dissolve1("Dissolve", Range( 0 , 1)) = 0
		_Hard1("Hard", Float) = 0
		_DistBase1("Dist Base", Range( 0 , 0.5)) = 0
		_DistMask1("Dist Mask", Range( 0 , 0.5)) = 0
		_DistDossolve1("Dist Dossolve", Range( 0 , 0.5)) = 0
		_DissolveDirt1("DissolveDirt", 2D) = "white" {}
		[ASEEnd][Toggle(_DIRTDISSOLVE1_ON)] _DirtDissolve1("DirtDissolve", Float) = 0

	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 
		ENDHLSL

		
		Pass
		{
			Name "Unlit"
			

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM

			#define ASE_SRP_VERSION 70701


			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS_SPRITEUNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _AR1_ON
			#pragma shader_feature_local _DIRTDISSOLVE1_ON


			sampler2D _BaseTex1;
			sampler2D _NoiseTex1;
			sampler2D _MaskTex1;
			sampler2D _DissolveTex1;
			sampler2D _DissolveDirt1;
			CBUFFER_START( UnityPerMaterial )
			float4 _BaseColor1;
			float4 _DissolveDirt1_ST;
			float4 _BaseTex1_ST;
			float4 _DissolveTex1_ST;
			float4 _NoiseTex1_ST;
			float4 _MaskTex1_ST;
			float _DistDossolve1;
			float _DissolveTexSpeedY1;
			float _DissolveTexSpeedX1;
			float _DistMask1;
			float _MaskSpeedY1;
			float _BaseTexSpeedX1;
			float _Dissolve1;
			float _DistBase1;
			float _NoiseBlend1;
			float _Noise1SpeedY1;
			float _Noise1SpeedX1;
			float _Noise2SpeedY1;
			float _Noise2SpeedX1;
			float _BaseTexSpeedY1;
			float _MaskSpeedX1;
			float _Hard1;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord2 = v.ase_texcoord1;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.normal = v.normal;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.vertex.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.clipPos = vertexInput.positionCS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 appendResult37 = (float2(_BaseTexSpeedX1 , _BaseTexSpeedY1));
				float2 uv_BaseTex1 = IN.texCoord0.xy * _BaseTex1_ST.xy + _BaseTex1_ST.zw;
				float2 appendResult30_g144 = (float2(IN.texCoord0.z , IN.texCoord0.w));
				float2 panner21_g144 = ( 1.0 * _Time.y * appendResult37 + ( uv_BaseTex1 + appendResult30_g144 ));
				float2 appendResult7 = (float2(_Noise2SpeedX1 , _Noise2SpeedY1));
				float2 uv_NoiseTex1 = IN.texCoord0.xy * _NoiseTex1_ST.xy + _NoiseTex1_ST.zw;
				float2 appendResult30_g141 = (float2(0.0 , 0.0));
				float2 panner21_g141 = ( 1.0 * _Time.y * appendResult7 + ( uv_NoiseTex1 + appendResult30_g141 ));
				float2 appendResult8 = (float2(_Noise1SpeedX1 , _Noise1SpeedY1));
				float2 appendResult30_g140 = (float2(0.0 , 0.0));
				float2 panner21_g140 = ( 1.0 * _Time.y * appendResult8 + ( uv_NoiseTex1 + appendResult30_g140 ));
				float4 tex2DNode59 = tex2D( _BaseTex1, ( panner21_g144 + ( ( ( tex2D( _NoiseTex1, panner21_g141 ).g + tex2D( _NoiseTex1, panner21_g140 ).r ) + _NoiseBlend1 ) * _DistBase1 ) ) );
				float4 base78 = ( tex2DNode59 * _BaseColor1 );
				#ifdef _AR1_ON
				float staticSwitch61 = tex2DNode59.r;
				#else
				float staticSwitch61 = tex2DNode59.a;
				#endif
				float basealpha63 = staticSwitch61;
				float2 appendResult41 = (float2(_MaskSpeedX1 , _MaskSpeedY1));
				float2 uv_MaskTex1 = IN.texCoord0.xy * _MaskTex1_ST.xy + _MaskTex1_ST.zw;
				float2 appendResult30_g145 = (float2(0.0 , 0.0));
				float2 panner21_g145 = ( 1.0 * _Time.y * appendResult41 + ( uv_MaskTex1 + appendResult30_g145 ));
				float Mask65 = tex2D( _MaskTex1, ( panner21_g145 + ( ( ( tex2D( _NoiseTex1, panner21_g141 ).g + tex2D( _NoiseTex1, panner21_g140 ).r ) + _NoiseBlend1 ) * _DistMask1 ) ) ).r;
				float3 temp_cast_0 = 0;
				float2 appendResult22 = (float2(_DissolveTexSpeedX1 , _DissolveTexSpeedY1));
				float2 uv_DissolveTex1 = IN.texCoord0.xy * _DissolveTex1_ST.xy + _DissolveTex1_ST.zw;
				float2 appendResult30_g142 = (float2(0.0 , 0.0));
				float2 panner21_g142 = ( 1.0 * _Time.y * appendResult22 + ( uv_DissolveTex1 + appendResult30_g142 ));
				float temp_output_43_0 = ( tex2D( _DissolveTex1, ( panner21_g142 + ( ( ( tex2D( _NoiseTex1, panner21_g141 ).g + tex2D( _NoiseTex1, panner21_g140 ).r ) + _NoiseBlend1 ) * _DistDossolve1 ) ) ).r * 0.55 );
				float2 uv_DissolveDirt1 = IN.texCoord0.xy * _DissolveDirt1_ST.xy + _DissolveDirt1_ST.zw;
				float2 appendResult30_g143 = (float2(0.0 , 0.0));
				float2 panner21_g143 = ( 1.0 * _Time.y * float2( 0,0 ) + ( uv_DissolveDirt1 + appendResult30_g143 ));
				#ifdef _DIRTDISSOLVE1_ON
				float staticSwitch83 = ( tex2D( _DissolveDirt1, panner21_g143 ).r + temp_output_43_0 );
				#else
				float staticSwitch83 = temp_output_43_0;
				#endif
				float3 temp_cast_1 = (staticSwitch83).xxx;
				float Dissolve66 = saturate( ( 1.0 - saturate( ( ( distance( temp_cast_0 , temp_cast_1 ) - ( 1.0 - ( _Dissolve1 + IN.ase_texcoord2.z ) ) ) / ( 1.0 - _Hard1 ) ) ) ) );
				
				float4 Color = ( ( ( base78 * IN.color ) * ( IN.color.a * ( ( basealpha63 * Mask65 ) * Dissolve66 ) ) ) * 1.0 );

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				Color *= IN.color;

				return Color;
			}

			ENDHLSL
		}
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=18935
558;771;1961;1129;1392.332;149.2654;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;1;-8023.957,626.592;Inherit;False;2431.151;911.4285;Comment;15;19;17;14;13;12;11;10;9;8;7;6;5;4;3;2;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-7839.608,1153.406;Inherit;False;Property;_Noise1SpeedX1;Noise1SpeedX;10;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-7836.12,1263.13;Inherit;False;Property;_Noise1SpeedY1;Noise1SpeedY;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-7968.957,819.5921;Inherit;False;Property;_Noise2SpeedY1;Noise2SpeedY;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-7973.957,676.592;Inherit;False;Property;_Noise2SpeedX1;Noise2SpeedX;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;8;-7590.155,1219.875;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;7;-7729.041,776.3371;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;6;-7818.535,946.4849;Inherit;True;Property;_NoiseTex1;NoiseTex;9;0;Create;True;0;0;0;False;0;False;95882c86f1e0f4e438d7dd6a3e81ecad;95882c86f1e0f4e438d7dd6a3e81ecad;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;9;-7345.249,780.4305;Inherit;False;MF_Common_Panner;-1;;141;a13e4cfc70e7faf43abf7ead1b6e7d65;0;4;28;SAMPLER2D;0,0,0,0;False;31;FLOAT;0;False;32;FLOAT;0;False;25;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;10;-7374.072,1123.856;Inherit;False;MF_Common_Panner;-1;;140;a13e4cfc70e7faf43abf7ead1b6e7d65;0;4;28;SAMPLER2D;0,0,0,0;False;31;FLOAT;0;False;32;FLOAT;0;False;25;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;11;-6959.157,739.097;Inherit;True;Property;_TextureSample2;Texture Sample 1;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-7002.347,974.9619;Inherit;True;Property;_TextureSample3;Texture Sample 2;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;15;-4565.749,389.5767;Inherit;False;3066.478;1014.695;Dissolve;24;83;66;64;60;57;56;55;54;52;48;47;43;40;32;31;30;29;25;24;22;21;20;18;16;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-6445.649,1033.516;Inherit;False;Property;_NoiseBlend1;NoiseBlend;14;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-6580.135,784.819;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-4469.332,900.7119;Inherit;False;Property;_DissolveTexSpeedY1;DissolveTexSpeedY;17;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;-6296.48,807.7265;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-4462.817,759.6311;Inherit;False;Property;_DissolveTexSpeedX1;DissolveTexSpeedX;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;20;-4149.16,665.0981;Inherit;True;Property;_DissolveTex1;DissolveTex;15;0;Create;True;0;0;0;False;0;False;95882c86f1e0f4e438d7dd6a3e81ecad;95882c86f1e0f4e438d7dd6a3e81ecad;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.WireNode;19;-5931.156,857.3113;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-4202.548,859.376;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-4083.508,1144.043;Inherit;False;Property;_DistDossolve1;Dist Dossolve;22;0;Create;True;0;0;0;False;0;False;0;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;25;-3865.687,764.9399;Inherit;False;MF_Common_Panner;-1;;142;a13e4cfc70e7faf43abf7ead1b6e7d65;0;4;28;SAMPLER2D;0,0,0,0;False;31;FLOAT;0;False;32;FLOAT;0;False;25;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;23;-3776.749,-560.5246;Inherit;False;1996.355;853.181;Base1;15;82;78;76;63;61;59;51;49;46;38;37;36;34;28;27;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-3784.173,1005.988;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-3600.838,-243.8181;Inherit;False;Property;_BaseTexSpeedY1;BaseTexSpeedY;4;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;26;-4150.247,1650.797;Inherit;False;1895.344;607.597;mask;10;65;62;58;53;50;45;44;41;35;33;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-3593.838,-346.8185;Inherit;False;Property;_BaseTexSpeedX1;BaseTexSpeedX;3;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;29;-3798.418,416.7456;Inherit;True;Property;_DissolveDirt1;DissolveDirt;23;0;Create;True;0;0;0;False;0;False;95882c86f1e0f4e438d7dd6a3e81ecad;95882c86f1e0f4e438d7dd6a3e81ecad;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;30;-3539.578,853.6628;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;31;-3340.754,773.59;Inherit;True;Property;_TextureSample1;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-3162.676,212.3058;Inherit;False;Property;_DistBase1;Dist Base;20;0;Create;True;0;0;0;False;0;False;0;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-4093.73,1796.503;Float;False;Property;_MaskSpeedX1;MaskSpeedX;7;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;36;-3725.749,-107.6516;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;39;-5633.817,261.0818;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;37;-3284.922,-244.0734;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;32;-3513.025,478.8134;Inherit;False;MF_Common_Panner;-1;;143;a13e4cfc70e7faf43abf7ead1b6e7d65;0;4;28;SAMPLER2D;0,0,0,0;False;31;FLOAT;0;False;32;FLOAT;0;False;25;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-4102.247,1937.584;Float;False;Property;_MaskSpeedY1;MaskSpeedY;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;34;-3209.393,-506.4975;Inherit;True;Property;_BaseTex1;BaseTex;2;0;Create;True;0;0;0;False;0;False;95882c86f1e0f4e438d7dd6a3e81ecad;531e54f1dc6e6214d8dd2297d5f6bb9c;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-2719.777,816.5037;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.55;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;46;-2948.186,-327.1244;Inherit;False;MF_Common_Panner;-1;;144;a13e4cfc70e7faf43abf7ead1b6e7d65;0;4;28;SAMPLER2D;0,0,0,0;False;31;FLOAT;0;False;32;FLOAT;0;False;25;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-2899.593,3.467298;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;47;-3724.04,1150.425;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;45;-3776.649,1697.797;Inherit;True;Property;_MaskTex1;MaskTex;6;0;Create;True;0;0;0;False;0;False;95882c86f1e0f4e438d7dd6a3e81ecad;95882c86f1e0f4e438d7dd6a3e81ecad;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DynamicAppendNode;41;-3833.46,1896.248;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-3798.085,2173.196;Float;False;Property;_DistMask1;Dist Mask;21;0;Create;True;0;0;0;False;0;False;0;0;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;42;-5702.745,1992.934;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3597.416,984.9338;Inherit;False;Property;_Dissolve1;Dissolve;18;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;-3188.994,426.6474;Inherit;True;Property;_TextureSample5;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-2655.316,488.6262;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-3373.242,2101.815;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-3470.709,1079.418;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;50;-3476.015,1828.591;Inherit;False;MF_Common_Panner;-1;;145;a13e4cfc70e7faf43abf7ead1b6e7d65;0;4;28;SAMPLER2D;0,0,0,0;False;31;FLOAT;0;False;32;FLOAT;0;False;25;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-2724.27,-184.2473;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;83;-2310.277,595.7631;Inherit;False;Property;_DirtDissolve1;DirtDissolve;24;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-3235.056,1911.212;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-3203.065,1127.436;Inherit;False;Property;_Hard1;Hard;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;57;-3271.281,1039.836;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;59;-2554.215,-358.1435;Inherit;True;Property;_BaseMap1;BaseMap;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;56;-3073.18,961.8761;Inherit;False;Constant;_Int1;Int 0;22;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.SamplerNode;62;-3055.086,1736.21;Inherit;True;Property;_TextureSample4;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;60;-2192.251,871.9028;Inherit;False;MF_SphereMask;-1;;146;1a6e7d942db424f4d83b369f4547ddaa;0;4;15;FLOAT3;0,0,0;False;16;FLOAT3;0,0,0;False;17;FLOAT;0;False;21;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;61;-2134.768,-45.17555;Inherit;False;Property;_AR1;A><R;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;64;-1898.49,936.1578;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;63;-1972.782,-19.31617;Inherit;False;basealpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;82;-2563.415,-106.9434;Inherit;False;Property;_BaseColor1;BaseColor;1;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;65;-2579.613,1764.289;Inherit;False;Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;-1450.058,529.6993;Inherit;False;65;Mask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-1455.229,413.1374;Inherit;False;63;basealpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-1858.954,845.5027;Inherit;False;Dissolve;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-2112.394,-342.7565;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1239.344,436.5623;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;70;-1231.175,602.6993;Inherit;False;66;Dissolve;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-1967.116,-279.9513;Inherit;False;base;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-1308.907,4.123731;Inherit;False;78;base;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-923.3443,445.5623;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;71;-1071.344,214.5623;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;73;-754.3438,398.5623;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;-864.3439,20.5623;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-129.3438,169.5623;Inherit;False;Constant;_Float1;Float 0;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;111;-270.332,-1.265396;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;77;-533.2158,382.7215;Inherit;False;Property;_Clip1;Clip;0;0;Create;True;0;0;0;False;0;False;0.3333;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-0.343811,73.5623;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;103;-419.332,499.7346;Inherit;False;Constant;_Float0;Float 0;25;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;108;135,-7;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;13;ShopHero/M_Fx_Base;cf964e524c8e69742b1d21fbe2ebcc4a;True;Unlit;0;0;Unlit;3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;17;d3d9;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;xboxseries;ps4;playstation;psp2;n3ds;wiiu;switch;nomrt;0;False;True;2;5;False;-1;10;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;False;0;Hidden/InternalErrorShader;0;0;Standard;1;Vertex Position;1;0;0;1;True;False;;False;0
WireConnection;8;0;5;0
WireConnection;8;1;4;0
WireConnection;7;0;2;0
WireConnection;7;1;3;0
WireConnection;9;28;6;0
WireConnection;9;25;7;0
WireConnection;10;28;6;0
WireConnection;10;25;8;0
WireConnection;11;0;6;0
WireConnection;11;1;9;0
WireConnection;12;0;6;0
WireConnection;12;1;10;0
WireConnection;14;0;11;2
WireConnection;14;1;12;1
WireConnection;17;0;14;0
WireConnection;17;1;13;0
WireConnection;19;0;17;0
WireConnection;22;0;16;0
WireConnection;22;1;18;0
WireConnection;25;28;20;0
WireConnection;25;25;22;0
WireConnection;24;0;19;0
WireConnection;24;1;21;0
WireConnection;30;0;25;0
WireConnection;30;1;24;0
WireConnection;31;0;20;0
WireConnection;31;1;30;0
WireConnection;39;0;19;0
WireConnection;37;0;28;0
WireConnection;37;1;27;0
WireConnection;32;28;29;0
WireConnection;43;0;31;1
WireConnection;46;28;34;0
WireConnection;46;31;36;3
WireConnection;46;32;36;4
WireConnection;46;25;37;0
WireConnection;49;0;39;0
WireConnection;49;1;38;0
WireConnection;41;0;35;0
WireConnection;41;1;33;0
WireConnection;42;0;19;0
WireConnection;40;0;29;0
WireConnection;40;1;32;0
WireConnection;54;0;40;1
WireConnection;54;1;43;0
WireConnection;53;0;42;0
WireConnection;53;1;44;0
WireConnection;52;0;48;0
WireConnection;52;1;47;3
WireConnection;50;28;45;0
WireConnection;50;25;41;0
WireConnection;51;0;46;0
WireConnection;51;1;49;0
WireConnection;83;1;43;0
WireConnection;83;0;54;0
WireConnection;58;0;50;0
WireConnection;58;1;53;0
WireConnection;57;0;52;0
WireConnection;59;0;34;0
WireConnection;59;1;51;0
WireConnection;62;0;45;0
WireConnection;62;1;58;0
WireConnection;60;15;56;0
WireConnection;60;16;83;0
WireConnection;60;17;57;0
WireConnection;60;21;55;0
WireConnection;61;1;59;4
WireConnection;61;0;59;1
WireConnection;64;0;60;0
WireConnection;63;0;61;0
WireConnection;65;0;62;1
WireConnection;66;0;64;0
WireConnection;76;0;59;0
WireConnection;76;1;82;0
WireConnection;69;0;68;0
WireConnection;69;1;67;0
WireConnection;78;0;76;0
WireConnection;72;0;69;0
WireConnection;72;1;70;0
WireConnection;73;0;71;4
WireConnection;73;1;72;0
WireConnection;75;0;81;0
WireConnection;75;1;71;0
WireConnection;111;0;75;0
WireConnection;111;1;73;0
WireConnection;79;0;111;0
WireConnection;79;1;74;0
WireConnection;108;1;79;0
ASEEND*/
//CHKSM=105774AE4A5AEE8E29C9812BF45291EB50101437