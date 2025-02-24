Shader "Custom/OpacityLensShader"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _LensColor ("Lens Tint Color", Color) = (1,1,1,1)
        _opacityFactor ("Opacity Factor", Range(0,1)) = 1
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _LensColor;
            float _opacityFactor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 originalColor = tex2D(_MainTex, i.uv);

                float4 modifiedColor = originalColor * _LensColor;
                modifiedColor.a *= _opacityFactor;

                return modifiedColor;
            }
            ENDCG
        }
    }
}
