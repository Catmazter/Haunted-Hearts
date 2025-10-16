Shader "Custom/InkOnParchment"
{
    Properties
    {
        _MapTex("Map Texture", 2D) = "white" {}
        _RevealTex("Reveal Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MapTex;
            sampler2D _RevealTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 radarCol = tex2D(_MapTex, i.uv);
                float revealMask = tex2D(_RevealTex, i.uv).r; // 0 = hidden, 1 = revealed

                // Smooth edges
                revealMask = saturate(pow(revealMask, 0.8));

                // Alpha is based on reveal amount (hidden = transparent)
                float alpha = revealMask;

                // Blend between radar color (revealed) and transparent
                fixed4 finalCol = lerp(fixed4(0,0,0,0), radarCol, revealMask);
                finalCol.a = alpha;

                return finalCol;
            }
            ENDCG
        }
    }
}
