Shader "Custom/VertexColorsDiffuse" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// BlinnPhong lighting model, and enable shadows on all light types
		#pragma surface surf BlinnPhong fullforwardshadows

		// Use shader model 3.0 target, for no particular reason
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float4 color : COLOR;
		};

		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = IN.color.rgb * _Color.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}