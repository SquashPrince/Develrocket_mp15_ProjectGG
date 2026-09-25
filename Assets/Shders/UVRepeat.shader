Shader "Custom/UVRepeat"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        // TGA에서 사용할 영역의 좌측 하단 UV
        _UVMin ("UV Min", Vector) = (0, 0, 0, 0)

        // TGA에서 사용할 영역의 우측 상단 UV
        _UVMax ("UV Max", Vector) = (1, 1, 0, 0)

        // 반복 횟수
        _Repeat ("Repeat", Vector) = (1, 1, 0, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            float4 _UVMin;
            float4 _UVMax;
            float4 _Repeat;

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

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);

                // 기존 UV를 현재 사용 영역 기준 0~1로 변환
                float2 uv = (v.uv - _UVMin.xy)
                          / (_UVMax.xy - _UVMin.xy);

                // 해당 영역만 반복
                uv *= _Repeat.xy;

                o.uv = uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }

            ENDCG
        }
    }
}
