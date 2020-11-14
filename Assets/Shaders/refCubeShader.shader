Shader "Custom/refCubeShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_CubeMap("CubeMap", CUBE) = "" {}
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 vertexLocal : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertexLocal = v.vertex;
				o.vertex = UnityObjectToClipPos(v.vertex);

				return o;
			}

			samplerCUBE _CubeMap;
			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 col = texCUBE(_CubeMap, normalize(i.vertexLocal.xyz));
				return fixed4(col, 1.0);
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}
