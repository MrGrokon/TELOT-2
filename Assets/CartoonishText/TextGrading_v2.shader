// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Exo/TextGrading_v2"
{
	Properties
	{
		_Text_Texture("Text_Texture", 2D) = "white" {}
		[HDR]_Top_Color("Top_Color", Color) = (1.796078,1.772549,0.5803922,0)
		[HDR]_Bottom_Color("Bottom_Color", Color) = (1.717647,0.7607843,0.07058824,0)
		_Emisive_Value("Emisive_Value", Range( 1 , 3)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _Emisive_Value;
		uniform float4 _Bottom_Color;
		uniform float4 _Top_Color;
		uniform sampler2D _Text_Texture;
		uniform float4 _Text_Texture_ST;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 lerpResult7 = lerp( _Bottom_Color , _Top_Color , i.uv_texcoord.y);
			float4 Color9 = ( _Emisive_Value * lerpResult7 );
			o.Emission = Color9.rgb;
			float2 uv_Text_Texture = i.uv_texcoord * _Text_Texture_ST.xy + _Text_Texture_ST.zw;
			float Opacity3 = tex2D( _Text_Texture, uv_Text_Texture ).r;
			float simplePerlin2D16 = snoise( i.uv_texcoord*6.2 );
			simplePerlin2D16 = simplePerlin2D16*0.5 + 0.5;
			float clampResult17 = clamp( i.vertexColor.r , 0.0 , 1.0 );
			float DisolveValue19 = step( simplePerlin2D16 , clampResult17 );
			o.Alpha = ( Opacity3 * DisolveValue19 * i.vertexColor.a );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;30;1920;989;970.3796;136.9316;1;True;False
Node;AmplifyShaderEditor.VertexColorNode;15;-1640.438,597.6833;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1613.361,370.116;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;16;-1397.334,367.2144;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;6.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-1404.812,-417.934;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-1377.778,-593.4829;Inherit;False;Property;_Bottom_Color;Bottom_Color;2;1;[HDR];Create;True;0;0;False;0;False;1.717647,0.7607843,0.07058824,0;1.059274,0.04436751,0.07545468,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;5;-1381.279,-767.8827;Inherit;False;Property;_Top_Color;Top_Color;1;1;[HDR];Create;True;0;0;False;0;False;1.796078,1.772549,0.5803922,0;1.254114,1.237697,0.1952159,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;17;-1386.282,625.4523;Inherit;False;3;0;FLOAT;1.05;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1962.811,-70.88605;Inherit;True;Property;_Text_Texture;Text_Texture;0;0;Create;True;0;0;False;0;False;a5da0d327446eec439e264ee08206a9a;a5da0d327446eec439e264ee08206a9a;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.LerpOp;7;-1103.578,-617.683;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-1730.811,-65.88603;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-1088.637,-728.5549;Inherit;False;Property;_Emisive_Value;Emisive_Value;3;0;Create;True;0;0;False;0;False;1;1;1;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;18;-1092.897,496.2584;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.99;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-781.838,-614.1551;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;3;-1431.411,-42.78604;Inherit;False;Opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-870.2931,491.1304;Inherit;False;DisolveValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;13;-389.2527,292.8231;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;4;-408.4781,113.2171;Inherit;False;3;Opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-403.6774,218.5225;Inherit;False;19;DisolveValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-611.8124,-617.434;Inherit;False;Color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;22;-1092.361,735.3106;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-1384.241,798.8102;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-182.4061,194.3525;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;10;-184.1118,44.56604;Inherit;False;9;Color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1706.069,817.3181;Inherit;False;Constant;_Float1;Float 0;5;0;Create;True;0;0;False;0;False;0.1;0;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-867.3384,732.2673;Inherit;False;ShityOutline;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-806.6741,-480.4765;Inherit;False;23;ShityOutline;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Exo/TextGrading_v2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;14;0
WireConnection;17;0;15;1
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;7;2;8;2
WireConnection;2;0;1;0
WireConnection;18;0;16;0
WireConnection;18;1;17;0
WireConnection;25;0;24;0
WireConnection;25;1;7;0
WireConnection;3;0;2;1
WireConnection;19;0;18;0
WireConnection;9;0;25;0
WireConnection;22;0;16;0
WireConnection;22;1;20;0
WireConnection;20;0;15;1
WireConnection;20;1;21;0
WireConnection;11;0;4;0
WireConnection;11;1;12;0
WireConnection;11;2;13;4
WireConnection;23;0;22;0
WireConnection;0;2;10;0
WireConnection;0;9;11;0
ASEEND*/
//CHKSM=917C73D4439C6B9CA5894C3E792CD9DF4AE285C2