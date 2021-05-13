// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Exo/Plasma_Sphere"
{
	Properties
	{
		_Lighting_Map("Lighting_Map", 2D) = "white" {}
		_Panner_TimeMult("Panner_TimeMult", Range( 0 , 5)) = 0.2677935
		[HDR]_Lighting_Color("Lighting_Color", Color) = (0.3137255,2.870588,2.996078,0)
		_VertexDisplacement_TimeScale("VertexDisplacement_TimeScale", Range( 0 , 3)) = 1
		_VertexDisplacement_NoiseScale("VertexDisplacement_NoiseScale", Float) = 5
		_IntersectionDistance("IntersectionDistance", Range( 0 , 1)) = 0.06
		_VertexDisplacement_NoiseForce("VertexDisplacement_NoiseForce", Float) = 5
		_BaseOpacity("BaseOpacity", Range( 0 , 1)) = 0
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
		#include "UnityCG.cginc"
		#pragma target 5.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
		};

		uniform float _VertexDisplacement_TimeScale;
		uniform float _VertexDisplacement_NoiseScale;
		uniform float _VertexDisplacement_NoiseForce;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _BaseOpacity;
		uniform sampler2D _Lighting_Map;
		uniform float _Panner_TimeMult;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _IntersectionDistance;
		uniform float4 _Lighting_Color;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


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


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime59 = _Time.y * _VertexDisplacement_TimeScale;
			float simplePerlin3D57 = snoise( ( ase_worldPos + mulTime59 )*_VertexDisplacement_NoiseScale );
			simplePerlin3D57 = simplePerlin3D57*0.5 + 0.5;
			float3 Dispacement49 = ( ase_vertexNormal * simplePerlin3D57 * _VertexDisplacement_NoiseForce );
			v.vertex.xyz += Dispacement49;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
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
			float2 center45_g1 = float2( 0.5,0.5 );
			float2 delta6_g1 = ( i.uv_texcoord - center45_g1 );
			float angle10_g1 = ( length( delta6_g1 ) * ( _SinTime.w * 10.0 ) );
			float x23_g1 = ( ( cos( angle10_g1 ) * delta6_g1.x ) - ( sin( angle10_g1 ) * delta6_g1.y ) );
			float2 break40_g1 = center45_g1;
			float2 break41_g1 = float2( 0,0 );
			float y35_g1 = ( ( sin( angle10_g1 ) * delta6_g1.x ) + ( cos( angle10_g1 ) * delta6_g1.y ) );
			float2 appendResult44_g1 = (float2(( x23_g1 + break40_g1.x + break41_g1.x ) , ( break40_g1.y + break41_g1.y + y35_g1 )));
			float gradientNoise91 = UnityGradientNoise(appendResult44_g1,11.5);
			gradientNoise91 = gradientNoise91*0.5 + 0.5;
			float temp_output_20_0_g7 = gradientNoise91;
			float3 crossX19_g7 = cross( ase_worldNormal , worldDerivativeX2_g7 );
			float3 break29_g7 = ( sign( crossYDotWorldDerivX34_g7 ) * ( ( ddx( temp_output_20_0_g7 ) * crossY18_g7 ) + ( ddy( temp_output_20_0_g7 ) * crossX19_g7 ) ) );
			float3 appendResult30_g7 = (float3(break29_g7.x , -break29_g7.y , break29_g7.z));
			float3 normalizeResult39_g7 = normalize( ( ( crossYDotWorldDerivX34_g7 * ase_worldNormal ) - appendResult30_g7 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g7 = mul( ase_worldToTangent, normalizeResult39_g7);
			float4 screenColor103 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( float3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( 5.0 * worldToTangentDir42_g7 ) ).xy);
			float4 TwirledBackground97 = screenColor103;
			o.Albedo = TwirledBackground97.rgb;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV29 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode29 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNdotV29, 5.0 ) );
			float mulTime11 = _Time.y * _Panner_TimeMult;
			float4 _Panner_DIrs = float4(0.8,0.8,-1,-1);
			float2 appendResult16 = (float2(_Panner_DIrs.x , _Panner_DIrs.y));
			float2 panner10 = ( mulTime11 * appendResult16 + i.uv_texcoord);
			float2 appendResult17 = (float2(_Panner_DIrs.z , _Panner_DIrs.w));
			float2 panner18 = ( mulTime11 * appendResult17 + i.uv_texcoord);
			float4 saferPower22 = max( ( fresnelNode29 + tex2D( _Lighting_Map, panner10 ) + tex2D( _Lighting_Map, panner18 ) ) , 0.0001 );
			float4 temp_cast_3 = (( _BaseOpacity + 1.0 )).xxxx;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth63 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth63 = abs( ( screenDepth63 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _IntersectionDistance ) );
			float4 Opacity_Mask28 = ( (float4( 0,0,0,0 ) + (( _BaseOpacity + saturate( pow( saferPower22 , 3.0 ) ) ) - float4( 0,0,0,0 )) * (float4( 1,1,1,0 ) - float4( 0,0,0,0 )) / (temp_cast_3 - float4( 0,0,0,0 ))) + saturate( ( 1.0 - distanceDepth63 ) ) );
			float4 Emisivness32 = ( Opacity_Mask28 * _Lighting_Color );
			o.Emission = Emisivness32.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
0;73;1274;766;1986.614;109.9995;2.468017;True;False
Node;AmplifyShaderEditor.CommentaryNode;83;-3229.585,-378.6377;Inherit;False;2940.89;852.4487;Color;29;13;15;16;9;11;17;5;10;18;19;6;29;20;22;65;63;70;26;71;69;73;72;75;74;24;28;23;32;68;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-3179.585,345.0373;Inherit;False;Property;_Panner_TimeMult;Panner_TimeMult;1;0;Create;True;0;0;False;0;False;0.2677935;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;15;-3176.909,87.54682;Inherit;False;Constant;_Panner_DIrs;Panner_DIrs;3;0;Create;True;0;0;False;0;False;0.8,0.8,-1,-1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;11;-2911.585,348.0373;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;-2889.74,58.59712;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-2978.559,-129.1766;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;17;-2841.766,224.811;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;18;-2686.765,219.811;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;10;-2683.558,-68.17645;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;5;-2675.401,-290.1133;Inherit;True;Property;_Lighting_Map;Lighting_Map;0;0;Create;True;0;0;False;0;False;d70a159171ab8684f98565d61a976d8e;d70a159171ab8684f98565d61a976d8e;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;19;-2334.253,83.90351;Inherit;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;29;-2293.947,-328.6377;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-2352.038,-127.5517;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinTimeNode;89;-1697.781,1469.745;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-1944.314,-37.5168;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-1892.049,228.658;Inherit;False;Property;_IntersectionDistance;IntersectionDistance;5;0;Create;True;0;0;False;0;False;0.06;23;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-1514.381,1555.594;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;22;-1720.868,-17.29509;Inherit;True;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;85;-1612.956,1331.395;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;82;-1288.107,1387.384;Inherit;True;Twirl;-1;;1;90936742ac32db8449cd21ab6dd337c8;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;26;-1480.776,-19.70196;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;84;-2600.185,580.244;Inherit;False;1671.303;612.799;Vertex Displacement;10;58;59;54;56;55;60;62;57;61;49;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1654.789,-136.2832;Inherit;False;Property;_BaseOpacity;BaseOpacity;7;0;Create;True;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;63;-1574.731,237.1813;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;25;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;91;-996.4119,1384.624;Inherit;True;Gradient;True;True;2;0;FLOAT2;0,0;False;1;FLOAT;11.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-1331.789,-86.28323;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-2550.185,818.013;Inherit;False;Property;_VertexDisplacement_TimeScale;VertexDisplacement_TimeScale;3;0;Create;True;0;0;False;0;False;1;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;69;-1314.174,200.6011;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1336.789,-205.2832;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;107;-559.028,943.2773;Inherit;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;75;-1122.559,203.67;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;72;-1181.789,-84.28323;Inherit;False;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;54;-2226.709,630.244;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;100;-746.0539,1387.533;Inherit;True;Normal From Height;-1;;7;1942fe2c5f1a1f94881a33d532e4afeb;0;1;20;FLOAT;0;False;2;FLOAT3;40;FLOAT3;0
Node;AmplifyShaderEditor.SimpleTimeNode;59;-2230.184,834.0129;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-665.4257,1161.602;Inherit;False;Constant;_DistortionForce;DistortionForce;8;0;Create;True;0;0;False;0;False;5;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;108;-332.028,943.2773;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;104;-350.4146,1200.768;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;55;-1985.112,724.2441;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-969.2805,-50.22623;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-2092.112,947.2449;Inherit;False;Property;_VertexDisplacement_NoiseScale;VertexDisplacement_NoiseScale;4;0;Create;True;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;109;-59.42717,1026.245;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;24;-893.4999,139.397;Inherit;False;Property;_Lighting_Color;Lighting_Color;2;1;[HDR];Create;True;0;0;False;0;False;0.3137255,2.870588,2.996078,0;1.631373,15.24706,16,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;57;-1687.674,826.3889;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-846.5509,-51.64072;Inherit;False;Opacity_Mask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalVertexDataNode;60;-1623.325,644.653;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;62;-1715.194,1077.043;Inherit;False;Property;_VertexDisplacement_NoiseForce;VertexDisplacement_NoiseForce;6;0;Create;True;0;0;False;0;False;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-1371.325,817.6529;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenColorNode;103;71.16662,1020.175;Inherit;False;Global;_GrabScreen0;Grab Screen 0;8;0;Create;True;0;0;False;0;False;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-661.2726,55.28121;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-512.6953,55.97388;Inherit;False;Emisivness;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;97;368.7373,1020.813;Inherit;False;TwirledBackground;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-1152.881,811.233;Inherit;False;Dispacement;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;111;243.7796,1113.442;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;265.3248,411.5193;Inherit;False;32;Emisivness;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;269.0176,750.6591;Inherit;False;49;Dispacement;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;68;-1305.893,266.5527;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;265.4073,317.5396;Inherit;False;97;TwirledBackground;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;253.0335,609.1806;Inherit;False;28;Opacity_Mask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;505.9867,394.9997;Float;False;True;-1;7;ASEMaterialInspector;0;0;Standard;Exo/Plasma_Sphere;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;11;0;13;0
WireConnection;16;0;15;1
WireConnection;16;1;15;2
WireConnection;17;0;15;3
WireConnection;17;1;15;4
WireConnection;18;0;9;0
WireConnection;18;2;17;0
WireConnection;18;1;11;0
WireConnection;10;0;9;0
WireConnection;10;2;16;0
WireConnection;10;1;11;0
WireConnection;19;0;5;0
WireConnection;19;1;18;0
WireConnection;6;0;5;0
WireConnection;6;1;10;0
WireConnection;20;0;29;0
WireConnection;20;1;6;0
WireConnection;20;2;19;0
WireConnection;90;0;89;4
WireConnection;22;0;20;0
WireConnection;82;1;85;0
WireConnection;82;3;90;0
WireConnection;26;0;22;0
WireConnection;63;0;65;0
WireConnection;91;0;82;0
WireConnection;71;0;70;0
WireConnection;71;1;26;0
WireConnection;69;0;63;0
WireConnection;73;0;70;0
WireConnection;75;0;69;0
WireConnection;72;0;71;0
WireConnection;72;2;73;0
WireConnection;100;20;91;0
WireConnection;59;0;58;0
WireConnection;108;0;107;0
WireConnection;104;0;110;0
WireConnection;104;1;100;40
WireConnection;55;0;54;0
WireConnection;55;1;59;0
WireConnection;74;0;72;0
WireConnection;74;1;75;0
WireConnection;109;0;108;0
WireConnection;109;1;104;0
WireConnection;57;0;55;0
WireConnection;57;1;56;0
WireConnection;28;0;74;0
WireConnection;61;0;60;0
WireConnection;61;1;57;0
WireConnection;61;2;62;0
WireConnection;103;0;109;0
WireConnection;23;0;28;0
WireConnection;23;1;24;0
WireConnection;32;0;23;0
WireConnection;97;0;103;0
WireConnection;49;0;61;0
WireConnection;111;0;103;0
WireConnection;68;0;63;0
WireConnection;0;0;98;0
WireConnection;0;2;33;0
WireConnection;0;11;50;0
ASEEND*/
//CHKSM=FDED85FB363DC52846F1869A22848C5D7D3AC9FD