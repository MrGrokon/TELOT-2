// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Exo/DistortionEffects"
{
	Properties
	{
		_Application_Mask("Application_Mask", 2D) = "white" {}
		_Activness("Activness", Range( 0 , 1)) = 0
		_Base_Color("Base_Color", Color) = (0.4481132,1,0.8859279,0.2196078)
		_DistortionForce("DistortionForce", Range( 0 , 5)) = 5
		_NoiseForceMultiplier("NoiseForceMultiplier", Range( 0 , 4)) = 1.312546
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 screenPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 uv_texcoord;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _DistortionForce;
		uniform sampler2D _Application_Mask;
		uniform float4 _Application_Mask_ST;
		uniform float _NoiseForceMultiplier;
		uniform float4 _Base_Color;
		uniform float _Activness;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float2 UnityGradientNoiseDir( float2 p )
		{
			p = fmod(p , 289);
			float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
			x = fmod( (34 * x + 1) * x , 289);
			x = frac( x / 41 ) * 2 - 1;
			return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
		}
		
		float UnityGradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 ip = floor( p );
			float2 fp = frac( p );
			float d00 = dot( UnityGradientNoiseDir( ip ), fp );
			float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
			float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
			float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
			fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
			return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_16_0_g7 = ( ase_worldPos * 100.0 );
			float3 crossY18_g7 = cross( ase_worldNormal , ddy( temp_output_16_0_g7 ) );
			float3 worldDerivativeX2_g7 = ddx( temp_output_16_0_g7 );
			float dotResult6_g7 = dot( crossY18_g7 , worldDerivativeX2_g7 );
			float crossYDotWorldDerivX34_g7 = abs( dotResult6_g7 );
			float2 uv_Application_Mask = i.uv_texcoord * _Application_Mask_ST.xy + _Application_Mask_ST.zw;
			float ApplicationMask48 = tex2D( _Application_Mask, uv_Application_Mask ).r;
			float2 center45_g9 = float2( 0.5,0.5 );
			float2 delta6_g9 = ( i.uv_texcoord - center45_g9 );
			float angle10_g9 = ( length( delta6_g9 ) * ( _SinTime.w * 10.0 ) );
			float x23_g9 = ( ( cos( angle10_g9 ) * delta6_g9.x ) - ( sin( angle10_g9 ) * delta6_g9.y ) );
			float2 break40_g9 = center45_g9;
			float mulTime64 = _Time.y * 0.2991063;
			float2 appendResult62 = (float2(mulTime64 , mulTime64));
			float2 break41_g9 = appendResult62;
			float y35_g9 = ( ( sin( angle10_g9 ) * delta6_g9.x ) + ( cos( angle10_g9 ) * delta6_g9.y ) );
			float2 appendResult44_g9 = (float2(( x23_g9 + break40_g9.x + break41_g9.x ) , ( break40_g9.y + break41_g9.y + y35_g9 )));
			float gradientNoise61 = UnityGradientNoise(appendResult44_g9,15.0);
			gradientNoise61 = gradientNoise61*0.5 + 0.5;
			float temp_output_20_0_g7 = ( ApplicationMask48 * gradientNoise61 * _NoiseForceMultiplier );
			float3 crossX19_g7 = cross( ase_worldNormal , worldDerivativeX2_g7 );
			float3 break29_g7 = ( sign( crossYDotWorldDerivX34_g7 ) * ( ( ddx( temp_output_20_0_g7 ) * crossY18_g7 ) + ( ddy( temp_output_20_0_g7 ) * crossX19_g7 ) ) );
			float3 appendResult30_g7 = (float3(break29_g7.x , -break29_g7.y , break29_g7.z));
			float3 normalizeResult39_g7 = normalize( ( ( crossYDotWorldDerivX34_g7 * ase_worldNormal ) - appendResult30_g7 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g7 = mul( ase_worldToTangent, normalizeResult39_g7);
			float4 screenColor8 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( _DistortionForce * worldToTangentDir42_g7 ) ).xy);
			float4 TwirledBackground9 = screenColor8;
			o.Emission = ( TwirledBackground9 * _Base_Color ).rgb;
			o.Alpha = ( _Base_Color.a * _Activness * ApplicationMask48 );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
8;81;1197;753;3153.098;479.669;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;54;-1407.925,545.7168;Inherit;False;981.7337;619.3657;Color Applocatop, & Masking;6;15;16;48;20;21;22;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-3807.652,-86.46423;Inherit;False;Constant;_TimeScale1;TimeScale;4;0;Create;True;0;0;False;0;False;0.2991063;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;15;-1357.925,935.0821;Inherit;True;Property;_Application_Mask;Application_Mask;0;0;Create;True;0;0;False;0;False;0000000000000000f000000000000000;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleTimeNode;64;-3533.767,-78.80294;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;57;-3577.977,-357.4952;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-3363.665,-277.4953;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;59;-3447.963,-402.0683;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-1121.924,932.1968;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;62;-3321.944,-101.2555;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-822.8206,958.6859;Inherit;False;ApplicationMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;60;-3101.379,-289.7612;Inherit;True;Twirl;-1;;9;90936742ac32db8449cd21ab6dd337c8;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;61;-2781.379,-289.7612;Inherit;True;Gradient;True;True;2;0;FLOAT2;0,0;False;1;FLOAT;15;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-2761.99,-35.56372;Inherit;False;Property;_NoiseForceMultiplier;NoiseForceMultiplier;4;0;Create;True;0;0;False;0;False;1.312546;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-2741.169,-404.7643;Inherit;False;48;ApplicationMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-2424.922,-335.271;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;1.66;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;55;-1877.959,-171.8274;Inherit;False;1515.564;630.2623;Convert Color map to difformation;10;2;5;3;4;6;7;8;10;9;23;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-1777.859,79.23497;Inherit;False;Property;_DistortionForce;DistortionForce;3;0;Create;True;0;0;False;0;False;5;5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;2;-1668.726,-121.8274;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;3;-1827.959,204.4349;Inherit;True;Normal From Height;-1;;7;1942fe2c5f1a1f94881a33d532e4afeb;0;1;20;FLOAT;0;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1457.859,111.235;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;5;-1444.727,-121.8274;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-1207.4,24.95557;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenColorNode;8;-1062.334,19.96681;Inherit;False;Global;_GrabScreen0;Grab Screen 0;8;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-864.1943,-65.14682;Inherit;False;TwirledBackground;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-938.0753,803.2017;Inherit;False;Property;_Activness;Activness;1;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;22;-879.0342,595.7167;Inherit;False;Property;_Base_Color;Base_Color;2;0;Create;True;0;0;False;0;False;0.4481132,1,0.8859279,0.2196078;0.4481132,1,0.8859279,0.2196078;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-588.1926,803.1363;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;30;-3248.487,225.5335;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-597.3947,12.64829;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-3195.56,455.3233;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;False;0.68;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;32;-3203.487,354.5334;Inherit;False;1;0;FLOAT;4.13;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-3468.487,348.5334;Inherit;False;Constant;_TimeScale;TimeScale;4;0;Create;True;0;0;False;0;False;4.588235;0;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;10;-837.9799,76.71968;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VoronoiNode;29;-2984.487,282.5334;Inherit;True;2;4;1;0;1;False;1;False;True;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;3.7;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-2699.479,282.9217;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-228.5488,-32.09874;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Exo/DistortionEffects;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;64;0;65;0
WireConnection;58;0;57;4
WireConnection;16;0;15;0
WireConnection;62;0;64;0
WireConnection;62;1;64;0
WireConnection;48;0;16;1
WireConnection;60;1;59;0
WireConnection;60;3;58;0
WireConnection;60;4;62;0
WireConnection;61;0;60;0
WireConnection;51;0;50;0
WireConnection;51;1;61;0
WireConnection;51;2;66;0
WireConnection;3;20;51;0
WireConnection;6;0;4;0
WireConnection;6;1;3;40
WireConnection;5;0;2;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;8;0;7;0
WireConnection;9;0;8;0
WireConnection;21;0;22;4
WireConnection;21;1;20;0
WireConnection;21;2;48;0
WireConnection;23;0;9;0
WireConnection;23;1;22;0
WireConnection;32;0;33;0
WireConnection;10;0;8;0
WireConnection;29;0;30;0
WireConnection;29;1;32;0
WireConnection;29;3;52;0
WireConnection;53;0;29;0
WireConnection;0;2;23;0
WireConnection;0;9;21;0
ASEEND*/
//CHKSM=7E7A4C7B3AB210D693E3BB493E7C4D3415E0E01E