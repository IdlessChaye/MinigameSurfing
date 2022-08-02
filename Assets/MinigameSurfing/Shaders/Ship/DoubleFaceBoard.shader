Shader "Surfing/DoubleFace" 
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess("Shininess", Range(0.03, 1)) = 0.078125
		_MainTex("MainTex", 2D) = "white" {}
		_NormalTex("NormalTex", 2D) = "bump" {}
		_BackColor("Back Main Color", Color) = (1,1,1,1)
	}
 
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 400
		//±³ÃæÌÞ³ý
		Cull back
		CGPROGRAM
			#pragma surface surf BlinnPhong
			sampler2D _MainTex;
			sampler2D _NormalTex;
			fixed4 _Color;
			half _Shininess;
 
			struct Input 
			{
				float2 uv_MainTex;
				float2 uv_NormalTex;
			};
 
			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = tex.rgb * _Color.rgb;
				o.Gloss = tex.a;
				o.Alpha = tex.a * _Color.a;
				o.Specular = _Shininess;
				o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));
			}
		ENDCG
 
		//ÕýÃæÌÞ³ý
		Cull front
		CGPROGRAM
			#pragma surface surf Lambert
			sampler2D _MainTex;
			fixed4 _BackColor;
			struct Input 
			{
				float2 uv_MainTex;
			};
 
			void surf(Input IN, inout SurfaceOutput o) 
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _BackColor;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
		ENDCG
	}
	FallBack "Specular"
}