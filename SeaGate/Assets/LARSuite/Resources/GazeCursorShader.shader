// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Lotus/GazeCursor" {
  Properties {
    _color ("Color", Color) = ( 0.934, 0.934, 0.934, 1.0 )
	_explosion("Explosion", Range(0.0, 0.4)) = 0.0
    _scale ("Scale", Range(0.0, 1.0)) = 1.0
	_distance("Distance", Range(3.0, 10.0)) = 10.0
	_minRadius("MinRadius", Float) = 0.001
	_maxRadius("MaxRadius", Float) = 0.3
	_innerFadeout("InnerFadeout", Float) = 0.1
	_outerFadeout("OuterFadeout", Float) = 0.2
  }

  SubShader {
    Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
    Pass {
      Blend SrcAlpha OneMinusSrcAlpha
      AlphaTest Off
      Cull Back
      Lighting Off
      ZWrite Off
      ZTest Always

      Fog { Mode Off }
      CGPROGRAM

      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      uniform float4 _color;
      uniform float _explosion;
      uniform float _scale;
      uniform float _distance;
	  uniform float _minRadius;
	  uniform float _maxRadius;
	  uniform float _innerFadeout;
	  uniform float _outerFadeout;

      struct meshData {
        float4 vertex : POSITION;
      };

      struct v2f{
          float4 position : SV_POSITION;
		  float2 alpha : TEXCOORD0;
      };

	  v2f vert(meshData v) {
		
        float4 localPos = float4(v.vertex.x, v.vertex.y, _distance, 1.0);
		float radius = sqrt(localPos.x * localPos.x + localPos.y * localPos.y);
		float alpha = 1.0;
		if (radius >= _outerFadeout) {
			alpha = 1.0 - (radius - _outerFadeout) / (_maxRadius - _outerFadeout);
		}
		else if (radius <= _innerFadeout) {
			alpha = 1.0 - (_innerFadeout - radius) / (_innerFadeout - _minRadius);
		}
		
		if(radius > 0.0) {
			float explosionScale = (radius + _explosion) / radius;
			localPos.x = localPos.x * explosionScale;
			localPos.y = localPos.y * explosionScale;
        }
	    
		float4 vertOut = float4(localPos.x * _scale, localPos.y * _scale, _distance, 1.0);
		v2f fragIn;
		fragIn.position = UnityObjectToClipPos (vertOut);
		fragIn.alpha.x = alpha;
		fragIn.alpha.y = 0.0;
        return fragIn;
      }

      fixed4 frag(v2f i) : SV_Target {
		fixed4 color = _color;
	    color.a = i.alpha.x;
        return color;
      }

      ENDCG
    }
  }
}