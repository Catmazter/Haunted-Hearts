Shader "Custom/InkOnParchment"
{
    Properties
    {
        _MapTex("Map Texture", 2D) = "white" {}
        _RevealTex("Reveal Texture", 2D) = "black" {}
        _ParchmentColor("Parchment Color", Color) = (1,0.95,0.85,1) // light beige
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
            float4 _ParchmentColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 mapCol = tex2D(_MapTex, i.uv);
                float revealMask = tex2D(_RevealTex, i.uv).r; // 0 = hidden, 1 = revealed

                // Smooth edges for a softer ink reveal
                revealMask = saturate(pow(revealMask, 0.8));

                // Inverted mask for black ink
                float ink = 1 - revealMask;

                // Mix: ink = black, hidden areas = parchment, revealed = map color
                fixed4 finalCol = lerp(_ParchmentColor, mapCol, revealMask); // revealed
                finalCol = lerp(finalCol, fixed4(0,0,0,1), ink);             // add ink to unrevealed

                return finalCol;
            }
            ENDCG
        }
    }
}
