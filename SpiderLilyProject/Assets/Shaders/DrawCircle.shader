Shader "Custom/DrawCircle"
{
    Properties
    {
        _Center("Center", Vector) = (0.5, 0.5, 0, 0) // Center of circle in UV space
        _Radius("Radius", Float) = 0.1               // Circle radius in UV units
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Center;
            float _Radius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex); // URP-safe
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.uv, _Center.xy);
                return dist < _Radius ? half4(1,1,1,1) : half4(0,0,0,0);
            }

            ENDHLSL
        }
    }
}
