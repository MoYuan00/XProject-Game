Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Gross ("_Gross", range(1, 100)) = 3
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Tags{"LightMode" = "UniversalForward"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 pos_WS : TEXCOORD1;
                float3 normal_WS : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Gross;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.pos_WS = mul(unity_ObjectToWorld, v.vertex);
                o.normal_WS = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.normal_WS = normalize(i.normal_WS);
                float3 view_WS = UnityWorldSpaceViewDir(i.pos_WS);
                float3 light_WS = UnityWorldSpaceLightDir(i.pos_WS);
                view_WS = normalize(view_WS);
                light_WS = normalize(light_WS);
                float3 color = clamp(0, 1, pow(max(0, dot(normalize(view_WS + light_WS), i.normal_WS)), _Gross));
                return float4(color, 1);
            }
            ENDCG
        }
    }
}
