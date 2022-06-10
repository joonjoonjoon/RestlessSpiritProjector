

Shader "Hidden/BWDiffuse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_bwBlend ("Black & White blend", Range (0, 1)) = 0
		_hueVal("Hue val", Range(0, 360)) = 0
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "HSB.cginc"	

			uniform sampler2D _MainTex;
			uniform float _bwBlend;
			float _hueVal;

			float4 frag(v2f_img i) : COLOR {
				float4 c = tex2D(_MainTex, i.uv);
				
				float lum = c.r*.22 + c.g*.707+ c.b*.071;
				float3 bw = float3( lum, lum, lum ); 
				
				float4 result = c;
				result.rgb = lerp(c.rgb, bw, 1-_bwBlend);
				result.rgb = applyHue(result.rgb, _hueVal);
				return result;
			}
			ENDCG
		}
	}
}
