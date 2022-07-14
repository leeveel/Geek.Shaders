Shader "leeveel/CharacterCartoon"
{
    Properties
    {
        [Header(Texture)]
        _MainColor ("Base Color", color) = (1, 1, 1, 1)
        //[NoScaleOffset]_MainTex ("_MainTex(仅供低画质时使用)", 2D) = "white" {}
		[NoScaleOffset]_MixTex ("_MixTex", 2D) = "white" { }
		[NoScaleOffset]_RampTex ("_RampTex", 2D) = "white" { }

        [Header(Outline)]
        _InnerlineColor ("内勾线颜色", color) = (0, 0, 0, 0)
		_OutlineColor ("描边颜色", color) = (0, 0, 0, 0)
		_OutlineWidth ("描边宽度", range(0, 1)) = 0.1
		_UnifomWidth ("宽度统一", range(0, 1)) = 0.5

        [Header(RimLight)]
        [MaterialToggle(RIMLIGHTTOGGLE)]_RimLightToggle ("开关", float) = 0
        [NoScaleOffset]_RimTex ("_RimTex", 2D) = "white" { }

        [Header(Rim)]
        [Toggle]_RimToggle ("开关", float) = 0
        _RimColor ("外发光颜色", Color) = (0,0,0,1)
        _RimIntensity  ("外发光强度", Range(1, 8)) = 5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "TOON"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma shader_feature _ _RIMTOGGLE_ON
            #pragma shader_feature _ RIMLIGHTTOGGLE

            struct appdata
            {
                float4 vertex         : POSITION;
                float2 uv              : TEXCOORD0;
                float3 normalOS    : NORMAL0;
                half4 tangentOS    : TANGENT;   //切线，W代表UV方向
            };

            struct v2f
            {
                float4 vertex          : SV_POSITION;
                float2 uv               : TEXCOORD0;
                float2 texcoord1     : TEXCOORD1;   //x:dot(N,L)*1.5兰伯特光照    y:dot(n,v) 菲涅尔 
                float3 viewWS        : TEXCOORD2;   
                half3 normalWS      : TEXCOORD3;
                //half3 tangentWS     : TEXCOORD4;
                //half3 bitangentWS   : TEXCOORD5;
            };

            sampler2D _MainTex;
            sampler2D _MixTex;
			sampler2D _RampTex;float2 _RampMap_TexelSize;
            fixed3 _MainColor;
            fixed3 _InnerlineColor;

            sampler2D _RimTex;
            #if _RIMTOGGLE_ON
                fixed3 _RimColor;
                half _RimIntensity;
            #endif

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                float3 positionWS = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewWS = _WorldSpaceCameraPos - positionWS;

               //NTB
                o.normalWS = UnityObjectToWorldNormal(v.normalOS);
                //o.tangentWS.xyz = UnityObjectToWorldDir(v.tangentOS.xyz);
                //half sign = v.tangentOS.w * unity_WorldTransformParams.w;
                //o.bitangentWS.xyz = cross(o.normalWS, o.tangentWS) * sign;

                 //dot(N,L) 兰伯特光照 - 世界空间
				o.texcoord1.x = dot(o.normalWS, _WorldSpaceLightPos0) * 1.5;
				//限定在[0,1]范围内
                o.texcoord1.x = saturate(o.texcoord1.x);
                o.texcoord1.y = 1;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c;
                c.a = 1;
                fixed3 mixTex = tex2D(_MixTex, i.uv).rgb;
                //主纹理G通道 * 兰伯特
				float gColor = (mixTex.g * i.texcoord1.x); 
				//绿色通道的颜色不能小于0.196078435  高光
				gColor = max(gColor, 0.196078435);  //50/255

				//色阶图对应关系
				float2 rampUV;
				rampUV.x = gColor;
				rampUV.y = (mixTex.r * 32);     //一共分成32个色阶，不能超过32
				rampUV.y = floor(rampUV.y);  //向下取整
				//色阶图偏移  0.03125 = 1/32    _RampTex_TexelSize.y = 1/图片高度 = 1/256
				rampUV.y = _RampMap_TexelSize.y - rampUV.y * 0.03125;
				rampUV.y = (rampUV.y + 1);   //rampUV.y 这个是负值，+1代表从上往下取值
                fixed3 rampColor = tex2D(_RampTex, rampUV).rgb;
				//return fixed4(rampColor, 1);

                //内勾线
                c.rgb = _InnerlineColor * (1 - mixTex.b) + rampColor * mixTex.b;

                half3 N = normalize(i.normalWS);
                half3 V = normalize(i.viewWS);
                half NDotV = dot(N, V);
                
                #if RIMLIGHTTOGGLE
				half3 L = normalize(_WorldSpaceLightPos0.xyz);
                half NDotL = dot(N, L);
                half LightFallOff = 2 * max(0, NDotL);
                half2 RimUV = half2(
                saturate(0.5 * (1.0 + LightFallOff)) *
                clamp((1.0 - abs(NDotV)), 0.02, 0.98), 0.25);
                fixed rimR = tex2D(_RimTex, RimUV).r;
                c.rgb += c.rgb * 0.5 * rimR;
                #endif

                //外发光
                #if _RIMTOGGLE_ON
                    half NdotV = 1 - max(0, dot(N,V));
                    half3 fresnel = pow(NdotV, 8 - _RimIntensity) * _RimColor;
                    c.rgb += fresnel;
                #endif

                //主色混合
                c.rgb *= _MainColor.rgb;

                return c;
            }
            ENDCG
        }


        Pass
        {
            Name "Outline"
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 positionOS         : POSITION;
                float3 normalOS          : NORMAL0;
                float4 color                 : COLOR0;
            };

            struct v2f
            {
                float4 positionCS    : SV_POSITION;
                float4 color            : COLOR0;
            };

            fixed3 _OutlineColor;
            fixed _OutlineWidth;
            fixed _UnifomWidth;

            v2f vert (appdata v)
            {
                v2f o;

                float3 positionWS = mul(unity_ObjectToWorld, v.positionOS).xyz;
				float dis = length(_WorldSpaceCameraPos - positionWS);
				dis = lerp(1, dis, _UnifomWidth);
				float3 width = _OutlineWidth * normalize(v.normalOS) * 0.01;
				float3 positionOS = v.positionOS.xyz;
				positionOS += width * dis;
				o.positionCS = UnityObjectToClipPos(positionOS);

                o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return half4(_OutlineColor * i.color.xyz * i.color.a, 1);
            }
            ENDCG
        }


    }
}
