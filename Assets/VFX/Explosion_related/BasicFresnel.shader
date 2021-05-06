// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Exo/BasicFresnel"
{
	Properties
	{
		_FresnelBorderSize("FresnelBorderSize", Range( 0 , 10)) = 2.705882
		_Fresnel_Intensity("Fresnel_Intensity", Range( 0 , 10)) = 3.058824
		_TimeScale("TimeScale", Range( 0 , 10)) = 3.058824
		_VertexScale("VertexScale", Range( -1 , 1)) = 0.7472677
		_FresnelColor("FresnelColor", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 vertexColor : COLOR;
			float3 worldNormal;
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _VertexScale;
		uniform float _FresnelBorderSize;
		uniform float _Fresnel_Intensity;
		uniform float _TimeScale;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float4 _FresnelColor;


		float2 voronoihash8( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi8( float2 v, float time, inout float2 id, inout float2 mr, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -2; j <= 2; j++ )
			{
				for ( int i = -2; i <= 2; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash8( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.707 * sqrt(dot( r, r ));
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = UnityObjectToWorldNormal( v.normal );
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( 0.0 + _FresnelBorderSize * pow( 1.0 - fresnelNdotV1, _Fresnel_Intensity ) );
			float mulTime11 = _Time.y * _TimeScale;
			float time8 = mulTime11;
			float2 coords8 = v.texcoord.xy * 6.42;
			float2 id8 = 0;
			float2 uv8 = 0;
			float voroi8 = voronoi8( coords8, time8, id8, uv8, 0 );
			float simplePerlin2D12 = snoise( v.texcoord.xy*5.0 );
			simplePerlin2D12 = simplePerlin2D12*0.5 + 0.5;
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth23 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_LOD( _CameraDepthTexture, float4( ase_screenPosNorm.xy, 0, 0 ) ));
			float distanceDepth23 = abs( ( screenDepth23 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 18.49 ) );
			float OpacityValue16 = ( saturate( ( ( fresnelNode1 * voroi8 * simplePerlin2D12 ) + -distanceDepth23 ) ) * v.color.a );
			v.vertex.xyz += ( _VertexScale * ase_vertexNormal * OpacityValue16 );
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 appendResult26 = (float3(i.vertexColor.r , i.vertexColor.g , i.vertexColor.b));
			o.Emission = ( _FresnelColor * float4( appendResult26 , 0.0 ) ).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNdotV1 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode1 = ( 0.0 + _FresnelBorderSize * pow( 1.0 - fresnelNdotV1, _Fresnel_Intensity ) );
			float mulTime11 = _Time.y * _TimeScale;
			float time8 = mulTime11;
			float2 coords8 = i.uv_texcoord * 6.42;
			float2 id8 = 0;
			float2 uv8 = 0;
			float voroi8 = voronoi8( coords8, time8, id8, uv8, 0 );
			float simplePerlin2D12 = snoise( i.uv_texcoord*5.0 );
			simplePerlin2D12 = simplePerlin2D12*0.5 + 0.5;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth23 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth23 = abs( ( screenDepth23 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( 18.49 ) );
			float OpacityValue16 = ( saturate( ( ( fresnelNode1 * voroi8 * simplePerlin2D12 ) + -distanceDepth23 ) ) * i.vertexColor.a );
			o.Alpha = OpacityValue16;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;0;1920;1019;2860.415;703.3641;1.676696;True;False
Node;AmplifyShaderEditor.CommentaryNode;17;-2545.476,-402.6877;Inherit;False;1796.943;778.0718;Opacity registering;16;15;11;23;25;16;7;24;9;12;8;1;13;2;3;27;28;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-2513.985,-106.5627;Inherit;False;Property;_TimeScale;TimeScale;2;0;Create;True;0;0;False;0;False;3.058824;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;11;-2203.725,-105.3362;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2394.133,-325.6642;Inherit;False;Property;_FresnelBorderSize;FresnelBorderSize;0;0;Create;True;0;0;False;0;False;2.705882;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-2396.133,-230.6642;Inherit;False;Property;_Fresnel_Intensity;Fresnel_Intensity;1;0;Create;True;0;0;False;0;False;3.058824;0;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-2287.956,147.617;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;12;-2052.141,141.0576;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;23;-1786.188,103.5972;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;18.49;False;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;8;-2009.864,-127.4837;Inherit;True;1;1;1;0;1;False;1;False;False;4;0;FLOAT2;0,0;False;1;FLOAT;9.42;False;2;FLOAT;6.42;False;3;FLOAT;0;False;3;FLOAT;0;FLOAT2;1;FLOAT2;2
Node;AmplifyShaderEditor.FresnelNode;1;-2112.4,-352.6138;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1.27;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;25;-1674.595,-90.72904;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1766.308,-326.3832;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-1547.49,-323.7869;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;7;-1341.501,-337.6316;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;28;-1446.959,-64.54301;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1155.215,-304.3106;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;5;-670.0043,-78.04507;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-986.2886,-349.2881;Inherit;False;OpacityValue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-574.798,292.6536;Inherit;False;Property;_VertexScale;VertexScale;3;0;Create;True;0;0;False;0;False;0.7472677;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-542.1775,-246.3628;Inherit;False;Property;_FresnelColor;FresnelColor;4;0;Create;True;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;26;-467.2738,-54.04755;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-473.3672,538.242;Inherit;False;16;OpacityValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;19;-473.9431,376.942;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-221.0043,-19.04507;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-223.8195,186.3361;Inherit;False;16;OpacityValue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-230.5741,326.2724;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Exo/BasicFresnel;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;5;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;15;0
WireConnection;12;0;13;0
WireConnection;8;1;11;0
WireConnection;1;2;2;0
WireConnection;1;3;3;0
WireConnection;25;0;23;0
WireConnection;9;0;1;0
WireConnection;9;1;8;0
WireConnection;9;2;12;0
WireConnection;24;0;9;0
WireConnection;24;1;25;0
WireConnection;7;0;24;0
WireConnection;27;0;7;0
WireConnection;27;1;28;4
WireConnection;16;0;27;0
WireConnection;26;0;5;1
WireConnection;26;1;5;2
WireConnection;26;2;5;3
WireConnection;6;0;4;0
WireConnection;6;1;26;0
WireConnection;21;0;20;0
WireConnection;21;1;19;0
WireConnection;21;2;22;0
WireConnection;0;2;6;0
WireConnection;0;9;18;0
WireConnection;0;11;21;0
ASEEND*/
//CHKSM=99F3D76B5FCBBB67572DB14A8AE61A9D606090C1