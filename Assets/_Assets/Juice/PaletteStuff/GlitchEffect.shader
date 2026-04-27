Shader "Custom/GlitchEffect"
{
    Properties
    {
        _MainTex ("Screen", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0
        _Speed ("Speed", Float) = 1.0
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            sampler2D _MainTex;
            float _Intensity;
            float _Speed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // XorDev hash noise
            float fsnoise(float2 p)
            {
                return fract(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            #define N(x) fsnoise(ceil(x))

            float2x2 rotate2D(float a)
            {
                float c = cos(a), s = sin(a);
                return float2x2(c, -s, s, c);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 scene = tex2D(_MainTex, i.uv);

                float2 r  = _ScreenParams.xy;
                float2 FC = i.uv * r;
                float  t  = _Time.y * _Speed;

                float4 o = 0;
                float2 p, c;

                [loop]
                for (float iter = -1.; iter < 1.; iter += .02)
                {
                    p = (FC * 2. - r) * .1 / (50. + iter);
                    c = p / (.1 + N(p));
                    float nc    = N(c);
                    float angle = ceil(nc * 8.) * 3.14159265 / 4.;
                    float denom = max(N(nc + ceil(c) + t), 0.001);
                    o += ceil(cos(mul(p, rotate2D(angle)).x / denom))
                       * float4(1. + iter, 2. - abs(iter + iter), 1. - iter, 1.) / 1e2;
                }

                return lerp(scene, saturate(o), _Intensity);
            }
            ENDCG
        }
    }
}
