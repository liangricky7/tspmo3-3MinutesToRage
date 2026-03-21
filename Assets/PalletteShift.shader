Shader "Custom/PaletteShift"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,1)) = 1.0
        _HueShift ("Hue Shift", Range(0,1)) = 0.0
        _Mode ("Mode", Int) = 0
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Intensity;
            float _HueShift;
            int _Mode;

            float3 RGBtoHSV(float3 c) {
                float4 K = float4(0,-1./3.,2./3.,-1.);
                float4 p = lerp(float4(c.bg,K.wz), float4(c.gb,K.xy), step(c.b,c.g));
                float4 q = lerp(float4(p.xyw,c.r), float4(c.r,p.yzx), step(p.x,c.r));
                float d = q.x - min(q.w,q.y);
                return float3(abs(q.z+(q.w-q.y)/(6.*d+1e-10)), d/(q.x+1e-10), q.x);
            }
            float3 HSVtoRGB(float3 c) {
                float4 K = float4(1,2./3.,1./3.,3.);
                float3 p = abs(frac(c.xxx+K.xyz)*6.-K.www);
                return c.z * lerp(K.xxx, clamp(p-K.xxx,0,1), c.y);
            }

            fixed4 frag(v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 orig = col.rgb;
                float3 result = orig;

                if (_Mode == 0)
                    result = 1.0 - orig;
                else if (_Mode == 1)
                    result = float3(orig.r*.6+orig.g*.2, orig.g*.1, orig.b*.1+orig.r*.3);
                else if (_Mode == 2) {
                    float v = dot(orig, float3(0.3,0.59,0.11));
                    result = saturate(float3(v*2, v<.5?v*2:(1-v)*2, (1-v)*2));
                }
                else if (_Mode == 3) {
                    float3 hsv = RGBtoHSV(orig);
                    hsv.x = frac(hsv.x + _HueShift);
                    result = HSVtoRGB(hsv);
                }
                else if (_Mode == 4)
                    result = float3(orig.r*.5+orig.b*.5, orig.g*.6+orig.r*.4, orig.b*.7+orig.g*.3);

                col.rgb = lerp(orig, result, _Intensity);
                return col;
            }
            ENDCG
        }
    }
}
