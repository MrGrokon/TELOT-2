// Upgrade NOTE: upgraded instancing buffer 'ExoTrap_Warning' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Exo/Trap_Warning"
{
	Properties
	{
		_Base_Text("Base_Text", 2D) = "white" {}
		[HDR]_EmisionColor("EmisionColor", Color) = (1,0.4964635,0.3820755,0)
		_TimeMultiplier("TimeMultiplier", Range( 0.01 , 2)) = 0
		_TillingMultiplier("TillingMultiplier", Vector) = (1,1,0,0)
		_Activness("Activness", Range( 0 , 1)) = 0
		_OffsetValue("OffsetValue", Range( 0 , 0.0003)) = 0.00025
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _OffsetValue;
		uniform float4 _EmisionColor;
		uniform sampler2D _Base_Text;
		uniform float2 _TillingMultiplier;
		uniform float _TimeMultiplier;

		UNITY_INSTANCING_BUFFER_START(ExoTrap_Warning)
			UNITY_DEFINE_INSTANCED_PROP(float, _Activness)
#define _Activness_arr ExoTrap_Warning
		UNITY_INSTANCING_BUFFER_END(ExoTrap_Warning)


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1);
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1);
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 LocalOffset79 = ( ase_vertexNormal * _SinTime.w * _OffsetValue );
			v.vertex.xyz += LocalOffset79;
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _EmisionColor.rgb;
			float _Activness_Instance = UNITY_ACCESS_INSTANCED_PROP(_Activness_arr, _Activness);
			float mulTime4 = _Time.y * _TimeMultiplier;
			float2 uv_TexCoord3 = i.uv_texcoord * _TillingMultiplier + ( mulTime4 * float2( 1,0 ) );
			Gradient gradient58 = NewGradient( 0, 3, 2, float4( 0, 0, 0, 0 ), float4( 1, 1, 1, 0.5000076 ), float4( 0, 0, 0, 1 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			Gradient gradient60 = NewGradient( 0, 6, 2, float4( 0, 0, 0, 0 ), float4( 1, 1, 1, 0.05000382 ), float4( 0, 0, 0, 0.1000076 ), float4( 0, 0, 0, 0.9000076 ), float4( 1, 1, 1, 0.9499962 ), float4( 0, 0, 0, 1 ), 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			o.Alpha = ( _Activness_Instance * saturate( ( ( tex2D( _Base_Text, uv_TexCoord3 ) * SampleGradient( gradient58, i.uv_texcoord.y ) ) + SampleGradient( gradient60, i.uv_texcoord.y ) ) ) ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;6;1920;1013;6322.541;1776.205;5.27575;True;False
Node;AmplifyShaderEditor.CommentaryNode;51;-2057.916,96.81331;Inherit;False;925.0387;438.3748;Texture Tilling;6;5;14;13;4;6;3;;0.6223772,1,0.3726415,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2007.916,268.4712;Inherit;False;Property;_TimeMultiplier;TimeMultiplier;2;0;Create;True;0;0;False;0;False;0;0.146;0.01;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;4;-1705.701,278.188;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;5;-1808.701,371.188;Inherit;False;Constant;_OffsetMultiplier;OffsetMultiplier;1;0;Create;True;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-1527.701,282.188;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;14;-1581.411,146.8133;Inherit;False;Property;_TillingMultiplier;TillingMultiplier;3;0;Create;True;0;0;False;0;False;1,1;5,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;52;-1068.756,172.9631;Inherit;False;1039.117;664.2705;Texture Opacity Grading;9;17;16;48;56;24;54;58;60;61;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1340.088,-131.51;Inherit;True;Property;_Base_Text;Base_Text;0;0;Create;True;0;0;False;0;False;None;f71f2650b6853b84895b4665904c7591;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1374.877,190.6614;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;24;-1039.556,305.798;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;58;-1029.78,227.9768;Inherit;False;0;3;2;0,0,0,0;1,1,1,0.5000076;0,0,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1020.654,-57.61029;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;78;-1175.809,1169.56;Inherit;False;926.656;459.423;Comment;5;67;68;79;74;65;;0.9967629,0.4764151,1,1;0;0
Node;AmplifyShaderEditor.WireNode;53;-623.9008,28.61516;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientNode;60;-846.9163,492.1035;Inherit;False;0;6;2;0,0,0,0;1,1,1,0.05000382;0,0,0,0.1000076;0,0,0,0.9000076;1,1,1,0.9499962;0,0,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;54;-858.6258,576.1091;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientSampleNode;48;-810.7766,252.8766;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientSampleNode;56;-615.4393,502.0612;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-426.0985,214.7296;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1024.608,1514.259;Inherit;False;Property;_OffsetValue;OffsetValue;6;0;Create;True;0;0;False;0;False;0.00025;0.0003;0;0.0003;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;67;-1011.808,1369.16;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;74;-944.8539,1220.692;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-638.8083,1238.359;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;61;-273.8766,220.3659;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;49;-2657.817,1104.995;Inherit;False;1310.595;483.4767;Poubelle;8;41;34;43;40;15;18;22;50;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;79;-450.9119,1240.755;Inherit;False;LocalOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-301.8832,49.55466;Inherit;False;InstancedProperty;_Activness;Activness;5;0;Create;True;0;0;False;0;False;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;17;-153.8663,214.9314;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;15;-2323.267,1401.818;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;25;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-2415.805,1229.995;Inherit;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;False;0.08;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-2607.817,1472.472;Inherit;False;Property;_IntersectionDistance;IntersectionDistance;4;0;Create;True;0;0;False;0;False;0;23;0;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;18;-2075.424,1392.652;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;29.02103,51.82062;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;7;-319.9593,-219.5854;Inherit;False;Property;_EmisionColor;EmisionColor;1;1;[HDR];Create;True;0;0;False;0;False;1,0.4964635,0.3820755,0;2,0.9960784,0.7607843,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;80;190.9394,108.3872;Inherit;False;79;LocalOffset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;40;-2205.216,1296.902;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1582.223,1226.645;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;50;-2159.633,1152.749;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;41;-1904.807,1222.245;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;416.2956,-245.4798;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Exo/Trap_Warning;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;13;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;3;0;14;0
WireConnection;3;1;6;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;53;0;2;0
WireConnection;48;0;58;0
WireConnection;48;1;24;2
WireConnection;56;0;60;0
WireConnection;56;1;54;2
WireConnection;16;0;53;0
WireConnection;16;1;48;0
WireConnection;68;0;74;0
WireConnection;68;1;67;4
WireConnection;68;2;65;0
WireConnection;61;0;16;0
WireConnection;61;1;56;0
WireConnection;79;0;68;0
WireConnection;17;0;61;0
WireConnection;15;0;22;0
WireConnection;18;0;15;0
WireConnection;63;0;62;0
WireConnection;63;1;17;0
WireConnection;40;0;34;0
WireConnection;43;0;41;0
WireConnection;41;0;50;2
WireConnection;41;1;34;0
WireConnection;0;2;7;0
WireConnection;0;9;63;0
WireConnection;0;11;80;0
ASEEND*/
//CHKSM=FA46C546CFBCD192946B78858538B500CEC4602F