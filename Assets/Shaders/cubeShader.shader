Shader "Custom/cubeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _CubeMap("CubeMap", CUBE) = "" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _RampThreshold("RampThrehold", Range(0, 1)) = 0.6
        _RampSmooth("RampSmooth", Range(0, 1)) = 0.4

        _SColor("SColor", Color) = (1,1,1,1)
        _HColor("HColor", Color) = (1,1,1,1)
        _SpeColor("SpecColor", Color) = (1, 1, 1, 1)
        _SpecSmooth("SpecSmooth", Range(0, 1)) = 0.2
        _Shininess("Shininess", Range(0, 1)) = 0.5

        _RimColor("RimColor", Color) = (1, 1, 1, 1)
        _RimThreshold("RimThreshold", Float) = 0.5
        _RimSmooth("RimSmooth", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Toon addshadow fullforwardshadows vertex:vert exclude_path:deferred exclude_path:prepass

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float4 vertexLocalPos;
        };
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertexLocalPos = v.vertex;
        }

        samplerCUBE _CubeMap;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        half _RampThreshold;
        half _RampSmooth;
        fixed4 _SColor;
        fixed4 _HColor;

        fixed4 _SpeColor;
        half _SpecSmooth;
        half _Shininess;

        fixed4 _RimColor;
        half _RimThreshold;
        half _RimSmooth;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        inline fixed4 LightingToon(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
        {
            // diffuse
            half3 normalDir = normalize(s.Normal);
            float ndl = max(0, dot(normalDir, lightDir));

            fixed3 lightColor = _LightColor0.rgb;

            fixed4 color;

            fixed3 ramp = smoothstep(_RampThreshold - _RampSmooth * 0.5, _RampThreshold + _RampSmooth * 0.5, ndl);
            ramp *= atten;

            _SColor = lerp(_HColor, _SColor, _SColor.a);
            float3 rampColor = lerp(_SColor.rgb, _HColor.rgb, ramp);

            fixed3 diffuse = s.Albedo * lightColor * rampColor;

            
            color.rgb = diffuse;
            color.a = s.Alpha;

            return color;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 mainTex = texCUBE(_CubeMap, normalize(IN.vertexLocalPos.xyz));
            o.Albedo = mainTex.rgb * _Color.rgb;
            // Metallic and smoothness come from slider variables
            o.Alpha = mainTex.a * _Color.a;
            o.Specular = _Shininess;
            o.Gloss = mainTex.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
