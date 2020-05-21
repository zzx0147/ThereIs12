Shader "Custom/Toon"
{
    Properties
    {
		_Color ("Color", color) = (1,1,1,1)
		_Color2("Color2", color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_LitTex ("LightMap", 2D) = "white" {}
		_SpecVal ("SpecVal", int) = 0
		
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		cull back

		CGPROGRAM
		#pragma surface surf Toon fullforwardshadows
		#pragma target 3.0

		float4 _Color;
		float4 _Color2;
        sampler2D _MainTex;
		sampler2D _LitTex;
		float _SpecVal;
		

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_LitTex;
			float3 lightDir;
			float atten;
        };



        void surf (Input IN, inout SurfaceOutput o)
        {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 d = tex2D(_LitTex, IN.uv_LitTex);

			////ToonShader term
			//float diffColor;
			//float ndotl = dot (IN.lightDir , o.Normal);

			//float3 toon = step ( ndotl * (1 - _Color2), d.g )  * 0.5 + 0.5;

			////diffColor = c.rgb * ndotl * _LightColor0.rgb;// * atten


			o.Albedo = c.rgb * _Color;
			o.Alpha = c.a;
        }

		float4 LightingToon (SurfaceOutput s, float3 lightDir, float3 viewDir, float atten)
		{
			//ToonShader term
			float diffColor;
			float ndotl = dot(s.Normal, lightDir) * 0.5 + 0.5;

			float ceilNum = 3;
			diffColor = ceil(ndotl * ceilNum) / ceilNum;


			float3 H = normalize(lightDir + viewDir);
			float speclit = saturate(dot(s.Normal, H));
			float speclitt = pow(speclit, _SpecVal);

			float SpecSmooth = smoothstep(0.005, 0.01, speclitt);
			float SpecColor = SpecSmooth * 1;

			float4 final;
			final.rgb = (s.Albedo + SpecColor * _Color2) * diffColor * _LightColor0.rgb;
			final.a = s.Alpha;

			//float3 toon = step(ndotl * (1 - _Color2), d.g);

			//diffColor = c.rgb * ndotl * _LightColor0.rgb;// * atten
			return final; //* atten
		}

        ENDCG

		
	}

    FallBack "Diffuse"
}
