Shader "Custom/InkBrush_Accumulate"
{
    Properties
    {
        _MainTex ("Base (Do not modify)", 2D) = "white" {}
        _BrushUV("Brush UV", Vector) = (0.5,0.5,0.08,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off
        // no global Blend here; we handle composition in the shader
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BrushUV; // x,y = UV center, z = radius

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                Debug.Log($"Painting at UV: {u:F2}, {v:F2}");

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // existing reveal value
                float prev = tex2D(_MainTex, i.uv).r;

                // brush mask
                float2 diff = i.uv - _BrushUV.xy;
                float dist = length(diff);
                float radius = _BrushUV.z;
                float alpha = smoothstep(radius, radius * 0.8, dist);
                float brush = 1.0 - alpha; // 1 = fully revealed at center, 0 outside

                // accumulate: new = max(old, brush)
                float result = max(prev, brush);
                return fixed4(result, result, result, 1.0);
            }
            ENDCG
        }
    }
}
