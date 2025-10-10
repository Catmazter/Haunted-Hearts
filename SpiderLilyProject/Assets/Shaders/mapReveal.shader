
Shader "Custom/MapReveal"
{
    // These are shader inputs visible in the Material inspector.
    Properties
    {
        _MapTex ("Map Texture", 2D) = "white" {}        // RenderTexture from your MapCamera
        _RevealTex ("Reveal Mask", 2D) = "black" {}     // RenderTexture that stores explored areas
    }

    SubShader
    {
      
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

     
        LOD 100

        // Map will be transparent in unexplored areas
        ZWrite Off                      // Don't write to depth buffer (we're blending)
        Blend SrcAlpha OneMinusSrcAlpha // Standard transparency blending
        Cull Off                        // Don't cull any faces (full-screen pass)

        Pass
        {
        
            HLSLPROGRAM

          
            #pragma vertex vert
            #pragma fragment frag

  
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

       
            struct appdata
            {
                float4 vertex : POSITION;   // Vertex position in object space
                float2 uv : TEXCOORD0;      // Texture UV coordinates
            };

      
            struct v2f
            {
                float2 uv : TEXCOORD0;       
                float4 vertex : SV_POSITION; 
            };

           
            TEXTURE2D(_MapTex);            // Map texture (RenderTexture from MapCamera)
            SAMPLER(sampler_MapTex);

            TEXTURE2D(_RevealTex);         // Reveal mask texture (RenderTexture from RevealCamera)
            SAMPLER(sampler_RevealTex);

       
            // Runs once per vertex and transforms to clip space.
            v2f vert(appdata v)
            {
                v2f o;
             
             
                o.vertex = TransformObjectToHClip(v.vertex);

                // Pass along the same UVs to the fragment shader
                o.uv = v.uv;
                return o;
            }

   
            half4 frag(v2f i) : SV_Target
            {
                // Sample both textures using URP macros
                half4 mapCol = SAMPLE_TEXTURE2D(_MapTex, sampler_MapTex, i.uv);
                half4 revealMask = SAMPLE_TEXTURE2D(_RevealTex, sampler_RevealTex, i.uv);

                // Multiply the map color by the reveal mask’s red channel.
                // White = visible, Black = hidden.
                // The mask comes from your RevealRT texture drawn by the player’s movement.
                return mapCol * revealMask.r;
            }

            ENDHLSL
        }
    }
}
