// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Exo/ControlableLaser"
{
	Properties
	{
		_MainLaserTexture("MainLaserTexture", 2D) = "white" {}
		_TimeScale("TimeScale", Range( 0 , 5)) = 0.4845214
		_TextureTargetedRatio("TextureTargetedRatio", Vector) = (0.165,1,0,0)
		[Toggle]_IsInverted("IsInverted", Float) = 0
		_EmisivnessValue("EmisivnessValue", Range( 1 , 5)) = 1
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
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
		};

		uniform float _EmisivnessValue;
		uniform sampler2D _MainLaserTexture;
		uniform float2 _TextureTargetedRatio;
		uniform float _IsInverted;
		uniform float _TimeScale;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float mulTime17 = _Time.y * _TimeScale;
			float2 appendResult13 = (float2((( _IsInverted )?( ( 1.0 - mulTime17 ) ):( mulTime17 )) , 0.0));
			float2 uv_TexCoord3 = i.uv_texcoord * _TextureTargetedRatio + appendResult13;
			float4 tex2DNode2 = tex2D( _MainLaserTexture, uv_TexCoord3 );
			o.Emission = ( _EmisivnessValue * i.vertexColor * tex2DNode2 ).rgb;
			o.Alpha = ( i.vertexColor.a * tex2DNode2.r );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;73;1252;766;2829.349;203.4232;2.088554;True;False
Node;AmplifyShaderEditor.CommentaryNode;14;-2111.723,551.8701;Inherit;False;939.2917;234.1095;Scrolling Parameters;6;5;17;18;13;10;12;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-2097.774,606.8616;Inherit;False;Property;_TimeScale;TimeScale;2;0;Create;True;0;0;False;0;False;0.4845214;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;17;-1828.375,612.0618;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;12;-1658.395,697.9796;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;10;-1501.45,612.1824;Inherit;False;Property;_IsInverted;IsInverted;4;0;Create;True;0;0;False;0;False;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;13;-1300.432,592.8702;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;4;-1373.42,398.4299;Inherit;False;Property;_TextureTargetedRatio;TextureTargetedRatio;3;0;Create;True;0;0;False;0;False;0.165,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-1104.241,379.3549;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1120.585,181.1451;Inherit;True;Property;_MainLaserTexture;MainLaserTexture;0;0;Create;True;0;0;False;0;False;bc81094525f6a7a4ba4b55d9fc795a8d;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.CommentaryNode;20;-583.5528,-234.3891;Inherit;False;550.2756;347.0204;Color Handling;3;16;19;21;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-533.5528,-184.3891;Inherit;False;Property;_EmisivnessValue;EmisivnessValue;5;0;Create;True;0;0;False;0;False;1;0;1;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-800.3636,246.8971;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;21;-484.4689,-77.89802;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-195.2772,-87.47459;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-258.2625,269.1825;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-2060.723,696.4319;Inherit;False;Property;_LoadingRatio;LoadingRatio;1;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Exo/ControlableLaser;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;18;0
WireConnection;12;0;17;0
WireConnection;10;0;17;0
WireConnection;10;1;12;0
WireConnection;13;0;10;0
WireConnection;3;0;4;0
WireConnection;3;1;13;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;16;0;19;0
WireConnection;16;1;21;0
WireConnection;16;2;2;0
WireConnection;22;0;21;4
WireConnection;22;1;2;1
WireConnection;0;2;16;0
WireConnection;0;9;22;0
ASEEND*/
//CHKSM=9D0C8B4891AF0D0F86F62354A7C652E557A32F96