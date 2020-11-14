Shader "Custom/ToonShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}

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
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Toon addshadow fullforwardshadows exclude_path:deferred exclude_path:prepass

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        fixed4 _Color;
        sampler2D _MainTex;
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


        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

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

            // specular
            half3 halfDir = normalize(lightDir + viewDir);
            float ndh = max(0, dot(normalDir, halfDir));

            float spec = pow(ndh, s.Specular * 128.0) * s.Gloss;
            spec *= atten;
            spec = smoothstep(0.5 - _SpecSmooth * 0.5, 0.5 + _SpecSmooth * 0.5, spec);

            fixed3 specular = _SpecColor.rgb * lightColor * spec;

            // rim
            float ndv = max(0, dot(normalDir, viewDir));

            float rim = (1.0 - ndv) * ndl;
            rim *= atten;
            rim = smoothstep(_RimThreshold - _RimSmooth * 0.5, _RimThreshold + _RimSmooth * 0.5, rim);

            fixed3 rimColor = _RimColor.rgb * lightColor * _RimColor.a * rim;

            color.rgb = diffuse + specular + rimColor;
            color.a = s.Alpha;
           
            return color;
        }
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = mainTex.rgb * _Color.rgb;            
            o.Alpha = mainTex.a * _Color.a;
            o.Specular = _Shininess;
            o.Gloss = mainTex.a;
        }
        ENDCG
    }
}
