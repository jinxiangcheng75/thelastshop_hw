// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ShopHero/P_Add"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin][HDR]_BaseColor("BaseColor", Color) = (1,1,1,0)
		_BaseMap("BaseMap", 2D) = "white" {}
		[Toggle(_AR_ON)] _AR("A><R", Float) = 0
		[Toggle(_MASKSW_ON)] _MaskSW("MaskSW", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		[Toggle(_PANNERSWITCH_ON)] _PannerSwitch("PannerSwitch", Float) = 0
		_speed("speed", Vector) = (0,0,0,0)
		_UVTime("UVTime", Float) = 1
		[Toggle(_UV_NOISE_DISTORTION_SW_ON)] _UV_Noise_Distortion_SW("UV_Noise_Distortion_SW", Float) = 0
		_Noise("Noise", 2D) = "white" {}
		_ND_SP_Time("ND_SP_Time", Float) = 1
		_ND_Speed("ND_Speed", Vector) = (0,0,0,0)
		_ND_Noise("ND_Noise", Range( 0 , 1)) = 0
		_ND_Blend("ND_Blend", Float) = -0.48
		[Toggle(_FLIPBOOKSW_ON)] _FlipBookSW("FlipBookSW", Float) = 0
		_FlipbookY("FlipbookY", Float) = 0
		_FlipbookX("FlipbookX", Float) = 0
		[ASEEnd]_FlipbookTime("FlipbookTime", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		ENDHLSL

		
		Pass
		{
			Name "Unlit"
			

			Blend One One, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			

			HLSLPROGRAM
			#define ASE_SRP_VERSION 70301

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

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
			#pragma shader_feature_local _MASKSW_ON
			#pragma shader_feature_local _FLIPBOOKSW_ON
			#pragma shader_feature_local _UV_NOISE_DISTORTION_SW_ON
			#pragma shader_feature_local _PANNERSWITCH_ON
			#pragma shader_feature_local _AR_ON


			sampler2D _BaseMap;
			sampler2D _Noise;
			sampler2D _Mask;
			CBUFFER_START( UnityPerMaterial )
			float4 _BaseMap_ST;
			float4 _BaseColor;
			float4 _Mask_ST;
			float2 _speed;
			float2 _ND_Speed;
			float _UVTime;
			float _ND_SP_Time;
			float _ND_Blend;
			float _ND_Noise;
			float _FlipbookX;
			float _FlipbookY;
			float _FlipbookTime;
			CBUFFER_END


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				
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

				float2 uv_BaseMap = IN.texCoord0.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;
				float2 appendResult31 = (float2(( uv_BaseMap.x + IN.texCoord0.z ) , ( uv_BaseMap.y + IN.texCoord0.w )));
				float mulTime66 = _TimeParameters.x * _UVTime;
				float2 panner22 = ( mulTime66 * _speed + uv_BaseMap);
				#ifdef _PANNERSWITCH_ON
				float2 staticSwitch24 = panner22;
				#else
				float2 staticSwitch24 = appendResult31;
				#endif
				float temp_output_61_0 = ( ( _TimeParameters.x ) * _ND_SP_Time );
				float2 texCoord38 = IN.texCoord0.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner39 = ( temp_output_61_0 * _ND_Speed + texCoord38);
				float2 panner44 = ( ( temp_output_61_0 * 0.5 ) * ( _ND_Speed * -1.0 ) + texCoord38);
				#ifdef _UV_NOISE_DISTORTION_SW_ON
				float2 staticSwitch55 = ( staticSwitch24 + ( ( tex2D( _Noise, panner39 ).r + tex2D( _Noise, panner44 ).g + _ND_Blend ) * (0.0 + (_ND_Noise - 0.0) * (0.5 - 0.0) / (1.0 - 0.0)) ) );
				#else
				float2 staticSwitch55 = staticSwitch24;
				#endif
				float temp_output_4_0_g1 = _FlipbookX;
				float temp_output_5_0_g1 = _FlipbookY;
				float2 appendResult7_g1 = (float2(temp_output_4_0_g1 , temp_output_5_0_g1));
				float totalFrames39_g1 = ( temp_output_4_0_g1 * temp_output_5_0_g1 );
				float2 appendResult8_g1 = (float2(totalFrames39_g1 , temp_output_5_0_g1));
				float clampResult42_g1 = clamp( 0.0 , 0.0001 , ( totalFrames39_g1 - 1.0 ) );
				float temp_output_35_0_g1 = frac( ( ( ( _FlipbookTime * ( _TimeParameters.x ) ) + clampResult42_g1 ) / totalFrames39_g1 ) );
				float2 appendResult29_g1 = (float2(temp_output_35_0_g1 , ( 1.0 - temp_output_35_0_g1 )));
				float2 temp_output_15_0_g1 = ( ( IN.texCoord0.xy / appendResult7_g1 ) + ( floor( ( appendResult8_g1 * appendResult29_g1 ) ) / appendResult7_g1 ) );
				#ifdef _FLIPBOOKSW_ON
				float2 staticSwitch75 = temp_output_15_0_g1;
				#else
				float2 staticSwitch75 = staticSwitch55;
				#endif
				float4 tex2DNode2 = tex2D( _BaseMap, staticSwitch75 );
				float4 temp_output_3_0 = ( tex2DNode2 * ( _BaseColor * IN.color ) );
				#ifdef _AR_ON
				float staticSwitch19 = tex2DNode2.r;
				#else
				float staticSwitch19 = tex2DNode2.a;
				#endif
				float temp_output_7_0 = ( staticSwitch19 * IN.color.a );
				float2 uv_Mask = IN.texCoord0.xy * _Mask_ST.xy + _Mask_ST.zw;
				#ifdef _MASKSW_ON
				float4 staticSwitch35 = ( temp_output_3_0 * ( temp_output_7_0 * tex2D( _Mask, uv_Mask ).r ) );
				#else
				float4 staticSwitch35 = ( temp_output_3_0 * temp_output_7_0 );
				#endif
				
				float4 Color = staticSwitch35;

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
Version=18900
935.2;775.2;1494;890;-178.2372;551.6165;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;58;-3330.194,629.0931;Inherit;False;1997.701;787.8241;Nosie dis;19;53;54;51;52;43;37;39;42;44;38;50;62;45;61;49;63;60;47;64;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-2995.805,1107.517;Inherit;False;Property;_ND_SP_Time;ND_SP_Time;10;0;Create;True;0;0;0;False;0;False;1;2.74;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;60;-3113.728,796.9095;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-2798.53,913.014;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;45;-3044.389,960.0125;Inherit;False;Property;_ND_Speed;ND_Speed;11;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;49;-2914.894,1288.517;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-3006.517,1188.816;Inherit;False;Constant;_Float2;Float 2;14;0;Create;True;0;0;0;False;0;False;-1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2703.472,1179.621;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;59;-2787.21,-487.9343;Inherit;False;1358.237;739.7999;UVpanner;10;30;21;29;28;27;32;22;31;24;66;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;38;-2951.67,679.0931;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-2828.517,1015.816;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-2580.21,-437.9343;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;39;-2627.14,689.8537;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;30;-2737.21,41.06567;Inherit;False;0;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;42;-2605.912,838.1245;Inherit;True;Property;_Noise;Noise;9;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;32;-2674.33,-174.6744;Inherit;False;Property;_UVTime;UVTime;7;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;44;-2498.893,1090.517;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;37;-2225.506,688.2509;Inherit;True;Property;_TextureSample0;Texture Sample 0;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-2288.21,118.0657;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1853.793,1181.517;Inherit;False;Property;_ND_Noise;ND_Noise;12;0;Create;True;0;0;0;False;0;False;0;0.24;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;27;-2625.21,-319.9343;Inherit;False;Property;_speed;speed;6;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;66;-2493.601,-132.8814;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;28;-2306.21,0.06568909;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;43;-2221.257,930.3348;Inherit;True;Property;_TextureSample1;Texture Sample 0;8;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;52;-1895.311,1015.868;Inherit;False;Property;_ND_Blend;ND_Blend;13;0;Create;True;0;0;0;False;0;False;-0.48;-0.48;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;64;-1588.552,1156.339;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;31;-2143.005,3.86908;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-1755.893,725.5176;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;22;-2194.21,-235.9343;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-1494.893,752.5176;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;78;-1147.334,-271.5955;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;73;-1379.772,-474.5183;Inherit;False;Property;_FlipbookTime;FlipbookTime;17;0;Create;True;0;0;0;False;0;False;0;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;24;-1690.573,-138.9678;Inherit;False;Property;_PannerSwitch;PannerSwitch;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1286.513,240.5157;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-1468.772,-710.5183;Inherit;False;Property;_FlipbookX;FlipbookX;16;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-992.634,-461.3955;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;69;-1455.772,-624.5183;Inherit;False;Property;_FlipbookY;FlipbookY;15;0;Create;True;0;0;0;False;0;False;0;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;67;-1103.238,-695.7587;Inherit;False;Flipbook;-1;;1;53c2488c220f6564ca6c90721ee16673;2,71,0,68,0;8;51;SAMPLER2D;0.0;False;13;FLOAT2;0,0;False;4;FLOAT;3;False;5;FLOAT;3;False;24;FLOAT;0;False;2;FLOAT;0;False;55;FLOAT;0;False;70;FLOAT;0;False;5;COLOR;53;FLOAT2;0;FLOAT;47;FLOAT;48;FLOAT;62
Node;AmplifyShaderEditor.StaticSwitch;55;-1105.526,-84.15598;Inherit;False;Property;_UV_Noise_Distortion_SW;UV_Noise_Distortion_SW;8;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;75;-804.1342,-348.2956;Inherit;False;Property;_FlipBookSW;FlipBookSW;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-692,-171.5;Inherit;True;Property;_BaseMap;BaseMap;1;0;Create;True;0;0;0;False;0;False;-1;None;9571cf5ddcd8b38459beeb2b7d75052f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;5;-543.3323,243.9783;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;19;-81.28949,5.605709;Inherit;False;Property;_AR;A><R;2;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-629,71.5;Inherit;False;Property;_BaseColor;BaseColor;0;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,0;1.059274,1.059274,1.059274,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-308,48.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;97.37103,84.28593;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-298.1797,476.2531;Inherit;True;Property;_Mask;Mask;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;263.176,236.0279;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-74,-162.5;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;485.176,204.0279;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;254.2586,-134.4451;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;35;586.176,67.02792;Inherit;False;Property;_MaskSW;MaskSW;3;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;90;1146.295,-189.5482;Float;False;True;-1;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;13;ShopHero/P_Add;cf964e524c8e69742b1d21fbe2ebcc4a;True;Unlit;0;0;Unlit;3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;0;True;True;4;1;False;-1;1;False;-1;3;1;False;-1;10;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;0;False;0;Hidden/InternalErrorShader;0;0;Standard;1;Vertex Position;1;0;1;True;False;;False;0
WireConnection;61;0;60;2
WireConnection;61;1;47;0
WireConnection;50;0;61;0
WireConnection;50;1;49;0
WireConnection;62;0;45;0
WireConnection;62;1;63;0
WireConnection;39;0;38;0
WireConnection;39;2;45;0
WireConnection;39;1;61;0
WireConnection;44;0;38;0
WireConnection;44;2;62;0
WireConnection;44;1;50;0
WireConnection;37;0;42;0
WireConnection;37;1;39;0
WireConnection;29;0;21;2
WireConnection;29;1;30;4
WireConnection;66;0;32;0
WireConnection;28;0;21;1
WireConnection;28;1;30;3
WireConnection;43;0;42;0
WireConnection;43;1;44;0
WireConnection;64;0;54;0
WireConnection;31;0;28;0
WireConnection;31;1;29;0
WireConnection;51;0;37;1
WireConnection;51;1;43;2
WireConnection;51;2;52;0
WireConnection;22;0;21;0
WireConnection;22;2;27;0
WireConnection;22;1;66;0
WireConnection;53;0;51;0
WireConnection;53;1;64;0
WireConnection;24;1;31;0
WireConnection;24;0;22;0
WireConnection;57;0;24;0
WireConnection;57;1;53;0
WireConnection;76;0;73;0
WireConnection;76;1;78;2
WireConnection;67;4;68;0
WireConnection;67;5;69;0
WireConnection;67;2;76;0
WireConnection;55;1;24;0
WireConnection;55;0;57;0
WireConnection;75;1;55;0
WireConnection;75;0;67;0
WireConnection;2;1;75;0
WireConnection;19;1;2;4
WireConnection;19;0;2;1
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;7;0;19;0
WireConnection;7;1;5;4
WireConnection;34;0;7;0
WireConnection;34;1;33;1
WireConnection;3;0;2;0
WireConnection;3;1;6;0
WireConnection;36;0;3;0
WireConnection;36;1;34;0
WireConnection;20;0;3;0
WireConnection;20;1;7;0
WireConnection;35;1;20;0
WireConnection;35;0;36;0
WireConnection;90;1;35;0
ASEEND*/
//CHKSM=BFE124ECE3CD230BC9E311EE46D7E81946DE9FFF