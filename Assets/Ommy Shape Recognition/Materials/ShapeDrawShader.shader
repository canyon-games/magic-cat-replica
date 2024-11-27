Shader "Hidden/ShapeDrawShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BrushTex ("Brush", 2D) = "black" {}
        _sPos ("Position", Vector) = (0,0,0)
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _BrushTex;
            float2 _sPos;
            float2 _size = float2(6,6);
            float4 _color = float4(0,0,0,1);

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 v : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.v = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Calculate integer positions relative to _sPos
                int x = floor(i.v.x - _sPos.x);
                int y = floor(i.v.y - _sPos.y);

                // Sample the base texture color
                float4 baseColor = tex2D(_MainTex, i.uv);

                // Check if the current fragment lies within the brush bounds
                if (x >= 0 && y >= 0 && x < _size.x && y < _size.y)
                {
                    // Calculate brush texture UV coordinates
                    float2 brushUV = float2(x / _size.x, y / _size.y);

                    // Get brush mask value and calculate blending factor
                    float brushMask = tex2D(_BrushTex, brushUV).w * _color.w;

                    // Blend the base color with the brush color
                    return lerp(baseColor, float4(_color.rgb, 1), brushMask);
                }

                return baseColor; // Return the original texture color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}