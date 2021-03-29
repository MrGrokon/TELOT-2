// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/FireEffect_v1"
{
	Properties
	{
		_Fire_SpriteSheet("Fire_SpriteSheet", 2D) = "white" {}
		_Application_Gradient("Application_Gradient", 2D) = "white" {}
		_Display_Value("Display_Value", Float) = 0.3
		_BaseOpacity("BaseOpacity", Range( 0 , 1)) = 1
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
		};

		uniform sampler2D _Application_Gradient;
		uniform sampler2D _Fire_SpriteSheet;
		uniform float4 _Fire_SpriteSheet_ST;
		uniform float _Display_Value;
		uniform float _BaseOpacity;


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
			Gradient gradient10 = NewGradient( 0, 5, 3, float4( 0, 0, 0, 0.01176471 ), float4( 0.5763948, 0.03839182, 0.03118715, 0.1411765 ), float4( 0.8744174, 0.2043149, 0.1165647, 0.3911803 ), float4( 1, 0.9135779, 0.3915094, 0.8705882 ), float4( 1, 0.9752283, 0.8254717, 1 ), 0, 0, 0, float2( 0, 0 ), float2( 0.3441176, 0.5705959 ), float2( 1, 1 ), 0, 0, 0, 0, 0 );
			float2 uv_Fire_SpriteSheet = i.uv_texcoord * _Fire_SpriteSheet_ST.xy + _Fire_SpriteSheet_ST.zw;
			float2 temp_cast_0 = (( tex2D( _Fire_SpriteSheet, uv_Fire_SpriteSheet ).a * _Display_Value )).xx;
			float4 tex2DNode3 = tex2D( _Application_Gradient, temp_cast_0 );
			float3 appendResult14 = (float3(SampleGradient( gradient10, tex2DNode3.r ).r , SampleGradient( gradient10, tex2DNode3.r ).g , SampleGradient( gradient10, tex2DNode3.r ).a));
			o.Emission = appendResult14;
			o.Alpha = ( tex2DNode3.r * _BaseOpacity );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18500
315;177;1242;593;1457.589;270.2032;1.395024;True;False
Node;AmplifyShaderEditor.TexturePropertyNode;1;-1682.587,-83.52547;Inherit;True;Property;_Fire_SpriteSheet;Fire_SpriteSheet;0;0;Create;True;0;0;False;0;False;0ce1e781d5bdc5a4e86ebb444dc781a2;75fd85e83bb057542a590338d0b81267;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;2;-1428.589,-80.52547;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-1311.917,157.6909;Inherit;False;Property;_Display_Value;Display_Value;2;0;Create;True;0;0;False;0;False;0.3;0.39;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-1087.588,-154.9261;Inherit;True;Property;_Application_Gradient;Application_Gradient;1;0;Create;True;0;0;False;0;False;ecc8768f5a5ae0d4cbaef250abb2b9ee;ecc8768f5a5ae0d4cbaef250abb2b9ee;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1075.819,54.99039;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-820.0239,-27.88175;Inherit;True;Property;_GradientCenter02;GradientCenter02;1;0;Create;True;0;0;False;0;False;-1;ecc8768f5a5ae0d4cbaef250abb2b9ee;ecc8768f5a5ae0d4cbaef250abb2b9ee;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;10;-731.3027,-268.64;Inherit;False;0;5;3;0,0,0,0.01176471;0.5763948,0.03839182,0.03118715,0.1411765;0.8744174,0.2043149,0.1165647,0.3911803;1,0.9135779,0.3915094,0.8705882;1,0.9752283,0.8254717,1;0,0;0.3441176,0.5705959;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.GradientSampleNode;11;-493.3025,-268.64;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-489.4431,113.4285;Inherit;False;Property;_BaseOpacity;BaseOpacity;3;0;Create;True;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;14;-191.5797,-224.0561;Inherit;True;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-199.2778,18.56675;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;81.78962,-222.2177;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Custom/FireEffect_v1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.13;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;0
WireConnection;5;0;2;4
WireConnection;5;1;8;0
WireConnection;3;0;4;0
WireConnection;3;1;5;0
WireConnection;11;0;10;0
WireConnection;11;1;3;1
WireConnection;14;0;11;1
WireConnection;14;1;11;2
WireConnection;14;2;11;4
WireConnection;16;0;3;1
WireConnection;16;1;15;0
WireConnection;0;2;14;0
WireConnection;0;9;16;0
ASEEND*/
//CHKSM=50F40A07F61583D60013300C393A55C2C2F6685D