// Upgrade NOTE: upgraded instancing buffer 'ExoLaserShader_v2' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Exo/LaserShader_v2"
{
	Properties
	{
		_Activness("Activness", Range( 0 , 1)) = 0
		_OscilationTimeMultiplier("OscilationTimeMultiplier", Range( 0.5 , 50)) = 6.058798
		_NewOpacityLimits("NewOpacityLimits", Vector) = (0.3,0.8,0,0)
		[HDR]_ChargedColor("ChargedColor", Color) = (2.713726,2.996078,1.239216,0)
		[HDR]_UnhargedColor("UnhargedColor", Color) = (5.992157,1.192157,1.192157,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float4 _ChargedColor;
		uniform float4 _UnhargedColor;
		uniform float _OscilationTimeMultiplier;
		uniform float2 _NewOpacityLimits;

		UNITY_INSTANCING_BUFFER_START(ExoLaserShader_v2)
			UNITY_DEFINE_INSTANCED_PROP(float, _Activness)
#define _Activness_arr ExoLaserShader_v2
		UNITY_INSTANCING_BUFFER_END(ExoLaserShader_v2)


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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			Gradient gradient16 = NewGradient( 0, 3, 2, float4( 0, 0, 0, 0 ), float4( 1, 1, 1, 0.0749981 ), float4( 0, 0, 0, 0.1499962 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float _Activness_Instance = UNITY_ACCESS_INSTANCED_PROP(_Activness_arr, _Activness);
			float Activ33 = _Activness_Instance;
			float2 panner19 = ( -(-0.3 + (Activ33 - 0.0) * (1.0 - -0.3) / (1.0 - 0.0)) * float2( 1,0 ) + i.uv_texcoord);
			Gradient gradient2 = NewGradient( 0, 7, 2, float4( 0, 0, 0, 0 ), float4( 0.04117584, 0.04117584, 0.04117584, 0.1499962 ), float4( 0.7743808, 0.7743808, 0.7743808, 0.3000076 ), float4( 1, 1, 1, 0.5000076 ), float4( 0.772549, 0.772549, 0.772549, 0.7000076 ), float4( 0.03921569, 0.03921569, 0.03921569, 0.8500038 ), float4( 0, 0, 0, 1 ), 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 OpacityMask8 = SampleGradient( gradient2, i.uv_texcoord.y );
			float4 ActiveRay23 = ( SampleGradient( gradient16, panner19.x ) * OpacityMask8 );
			float4 lerpResult48 = lerp( _ChargedColor , _UnhargedColor , Activ33);
			o.Emission = ( ( ActiveRay23 + OpacityMask8 ) * lerpResult48 ).rgb;
			float mulTime40 = _Time.y * _OscilationTimeMultiplier;
			o.Alpha = ( i.vertexColor.a * OpacityMask8.r * saturate( (_NewOpacityLimits.x + (sin( mulTime40 ) - 0.0) * (_NewOpacityLimits.y - _NewOpacityLimits.x) / (1.0 - 0.0)) ) );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;73;1274;766;1351.277;587.8719;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;54;-3921.526,-955.1389;Inherit;False;2201.872;446.6686;Comment;12;45;33;1;21;18;19;16;20;26;17;27;23;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-3871.526,-725.8661;Inherit;False;InstancedProperty;_Activness;Activness;0;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;33;-3547.533,-723.2773;Inherit;False;Activ;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;53;-3210.614,-380.0651;Inherit;False;848.2369;318;Comment;4;4;2;3;8;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;45;-3346.753,-715.4703;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-0.3;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;2;-3139.614,-330.0651;Inherit;False;0;7;2;0,0,0,0;0.04117584,0.04117584,0.04117584,0.1499962;0.7743808,0.7743808,0.7743808,0.3000076;1,1,1,0.5000076;0.772549,0.772549,0.772549,0.7000076;0.03921569,0.03921569,0.03921569,0.8500038;0,0,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;18;-3216.515,-855.2148;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NegateNode;21;-3135.474,-715.7167;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-3160.614,-221.065;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;19;-2988.336,-800.7682;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GradientSampleNode;3;-2916.414,-288.6651;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;55;-1469.428,178.8803;Inherit;False;1322.522;753.9403;Comment;10;37;40;43;41;38;11;44;29;12;30;;1,1,1,1;0;0
Node;AmplifyShaderEditor.GradientNode;16;-2700.834,-905.1389;Inherit;False;0;3;2;0,0,0,0;1,1,1,0.0749981;0,0,0,0.1499962;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;20;-2706.336,-794.7682;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-2586.377,-294.167;Inherit;False;OpacityMask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;17;-2430.64,-902.3069;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;37;-1419.428,533.5989;Inherit;False;Property;_OscilationTimeMultiplier;OscilationTimeMultiplier;1;0;Create;True;0;0;False;0;False;6.058798;0;0.5;50;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;26;-2312.439,-696.0103;Inherit;False;8;OpacityMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;40;-1136.147,535.9498;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2105.149,-870.0991;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;43;-1064.308,768.8206;Inherit;False;Property;_NewOpacityLimits;NewOpacityLimits;2;0;Create;True;0;0;False;0;False;0.3,0.8;0.3,0.8;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SinOpNode;41;-945.4028,535.9491;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1345.858,-492.613;Inherit;False;1207.279;489.6869;Comment;8;47;48;46;49;14;24;15;25;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1943.653,-871.474;Inherit;False;ActiveRay;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-730.657,-442.613;Inherit;False;23;ActiveRay;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-1053.165,343.464;Inherit;True;8;OpacityMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;38;-718.5345,534.3498;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.39;False;4;FLOAT;0.71;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;47;-1313.974,-265.8757;Inherit;False;Property;_UnhargedColor;UnhargedColor;4;1;[HDR];Create;True;0;0;False;0;False;5.992157,1.192157,1.192157,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;46;-1312.137,-449.2437;Inherit;False;Property;_ChargedColor;ChargedColor;3;1;[HDR];Create;True;0;0;False;0;False;2.713726,2.996078,1.239216,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;49;-1301.403,-72.48653;Inherit;False;33;Activ;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;-734.6093,-342.6917;Inherit;False;8;OpacityMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;29;-546.5587,228.8803;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;12;-770.0024,388.3406;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;44;-445.6326,471.7987;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-479.2346,-359.0963;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;48;-1031.56,-237.0796;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-308.9056,324.3828;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;-359.3846,-239.0829;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Exo/LaserShader_v2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;33;0;1;0
WireConnection;45;0;33;0
WireConnection;21;0;45;0
WireConnection;19;0;18;0
WireConnection;19;1;21;0
WireConnection;3;0;2;0
WireConnection;3;1;4;2
WireConnection;20;0;19;0
WireConnection;8;0;3;0
WireConnection;17;0;16;0
WireConnection;17;1;20;0
WireConnection;40;0;37;0
WireConnection;27;0;17;0
WireConnection;27;1;26;0
WireConnection;41;0;40;0
WireConnection;23;0;27;0
WireConnection;38;0;41;0
WireConnection;38;3;43;1
WireConnection;38;4;43;2
WireConnection;12;0;11;0
WireConnection;44;0;38;0
WireConnection;24;0;25;0
WireConnection;24;1;15;0
WireConnection;48;0;46;0
WireConnection;48;1;47;0
WireConnection;48;2;49;0
WireConnection;30;0;29;4
WireConnection;30;1;12;0
WireConnection;30;2;44;0
WireConnection;14;0;24;0
WireConnection;14;1;48;0
WireConnection;0;2;14;0
WireConnection;0;9;30;0
ASEEND*/
//CHKSM=6AF72801B24CB51746F2C8F7D20BA7DAC8CD923B