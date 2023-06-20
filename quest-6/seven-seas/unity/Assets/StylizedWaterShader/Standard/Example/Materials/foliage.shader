// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:2,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:0,qpre:1,rntp:7,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:9361,x:33848,y:32586,varname:node_9361,prsc:2|emission-9717-OUT,clip-851-R,voffset-7044-OUT;n:type:ShaderForge.SFN_Tex2d,id:851,x:32843,y:32916,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_851,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:6ee42aaeeb2dccb4095663882f2605e7,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:5927,x:32360,y:32532,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_5927,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4294356,c2:0.6838235,c3:0.02514055,c4:1;n:type:ShaderForge.SFN_AmbientLight,id:7528,x:32734,y:32646,varname:node_7528,prsc:2;n:type:ShaderForge.SFN_Multiply,id:2460,x:32927,y:32598,cmnt:Ambient Light,varname:node_2460,prsc:2|A-5927-RGB,B-7528-RGB;n:type:ShaderForge.SFN_Lerp,id:3237,x:33383,y:32433,varname:node_3237,prsc:2|A-2460-OUT,B-4435-RGB,T-5129-OUT;n:type:ShaderForge.SFN_Color,id:4435,x:32601,y:32264,ptovrint:False,ptlb:Shadow,ptin:_Shadow,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2792029,c2:0.3823529,c3:0.1152682,c4:1;n:type:ShaderForge.SFN_Multiply,id:5463,x:33178,y:32106,varname:node_5463,prsc:2|A-580-OUT,B-8023-OUT;n:type:ShaderForge.SFN_Vector1,id:8023,x:32878,y:32257,varname:node_8023,prsc:2,v1:2;n:type:ShaderForge.SFN_TexCoord,id:4131,x:32858,y:32079,varname:node_4131,prsc:2,uv:0;n:type:ShaderForge.SFN_OneMinus,id:580,x:33021,y:32079,varname:node_580,prsc:2|IN-4131-V;n:type:ShaderForge.SFN_Clamp01,id:5129,x:33359,y:32258,varname:node_5129,prsc:2|IN-5463-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:8885,x:33272,y:32749,varname:node_8885,prsc:2;n:type:ShaderForge.SFN_Lerp,id:9717,x:33580,y:32616,varname:node_9717,prsc:2|A-732-RGB,B-3237-OUT,T-8885-OUT;n:type:ShaderForge.SFN_Color,id:732,x:33213,y:32610,ptovrint:False,ptlb:Unlit,ptin:_Unlit,varname:node_732,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1288418,c2:0.1764706,c3:0.05190311,c4:1;n:type:ShaderForge.SFN_FragmentPosition,id:6610,x:31944,y:33178,varname:node_6610,prsc:2;n:type:ShaderForge.SFN_Sin,id:1307,x:32214,y:33192,varname:node_1307,prsc:2|IN-6610-X;n:type:ShaderForge.SFN_Multiply,id:7120,x:32467,y:33225,varname:node_7120,prsc:2|A-1307-OUT,B-8919-OUT;n:type:ShaderForge.SFN_Multiply,id:8919,x:32234,y:33412,varname:node_8919,prsc:2|A-6613-OUT,B-7320-OUT;n:type:ShaderForge.SFN_TexCoord,id:1304,x:31158,y:33403,varname:node_1304,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:7320,x:32060,y:33556,varname:node_7320,prsc:2|A-5016-OUT,B-3717-OUT;n:type:ShaderForge.SFN_OneMinus,id:5016,x:31393,y:33441,varname:node_5016,prsc:2|IN-1304-V;n:type:ShaderForge.SFN_Time,id:8109,x:31305,y:33596,varname:node_8109,prsc:2;n:type:ShaderForge.SFN_Cos,id:3717,x:31906,y:33662,varname:node_3717,prsc:2|IN-1575-OUT;n:type:ShaderForge.SFN_Append,id:4298,x:32700,y:33214,varname:node_4298,prsc:2|A-3840-OUT,B-7120-OUT,C-7120-OUT;n:type:ShaderForge.SFN_Vector1,id:3840,x:32514,y:33096,varname:node_3840,prsc:2,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:6613,x:31908,y:33345,ptovrint:False,ptlb:intesity,ptin:_intesity,varname:node_7025,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:1575,x:31758,y:33662,varname:node_1575,prsc:2|A-8109-T,B-5116-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5116,x:31262,y:33761,ptovrint:False,ptlb:speed,ptin:_speed,varname:node_3348,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:3;n:type:ShaderForge.SFN_ComponentMask,id:285,x:32528,y:34212,varname:node_285,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3241-X;n:type:ShaderForge.SFN_Multiply,id:3415,x:33008,y:34212,varname:node_3415,prsc:2|A-1015-OUT,B-1189-OUT,C-8732-OUT;n:type:ShaderForge.SFN_Tau,id:8732,x:32955,y:34382,varname:node_8732,prsc:2;n:type:ShaderForge.SFN_Sin,id:5903,x:33214,y:34212,varname:node_5903,prsc:2|IN-3415-OUT;n:type:ShaderForge.SFN_RemapRange,id:4190,x:33416,y:34187,varname:node_4190,prsc:2,frmn:-1,frmx:1,tomn:-1,tomx:1|IN-5903-OUT;n:type:ShaderForge.SFN_Add,id:1189,x:32703,y:34360,varname:node_1189,prsc:2|A-285-OUT,B-5754-T;n:type:ShaderForge.SFN_Time,id:5754,x:32301,y:34410,varname:node_5754,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:3241,x:32279,y:34212,varname:node_3241,prsc:2;n:type:ShaderForge.SFN_ConstantClamp,id:4252,x:33767,y:34161,varname:node_4252,prsc:2,min:-1,max:1|IN-7829-OUT;n:type:ShaderForge.SFN_ComponentMask,id:4813,x:32537,y:33723,varname:node_4813,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-7067-Z;n:type:ShaderForge.SFN_Multiply,id:4334,x:33017,y:33723,varname:node_4334,prsc:2|A-7883-OUT,B-8604-OUT,C-4722-OUT;n:type:ShaderForge.SFN_Vector1,id:7883,x:32827,y:33610,varname:node_7883,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Tau,id:4722,x:32964,y:33893,varname:node_4722,prsc:2;n:type:ShaderForge.SFN_Sin,id:1758,x:33223,y:33723,varname:node_1758,prsc:2|IN-4334-OUT;n:type:ShaderForge.SFN_RemapRange,id:5912,x:33425,y:33698,varname:node_5912,prsc:2,frmn:-1,frmx:1,tomn:-1,tomx:1|IN-1758-OUT;n:type:ShaderForge.SFN_Add,id:8604,x:32712,y:33871,varname:node_8604,prsc:2|A-4813-OUT,B-5986-T;n:type:ShaderForge.SFN_Time,id:5986,x:32310,y:33921,varname:node_5986,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:7067,x:32288,y:33723,varname:node_7067,prsc:2;n:type:ShaderForge.SFN_Add,id:7829,x:33634,y:34009,varname:node_7829,prsc:2|A-5912-OUT,B-4190-OUT;n:type:ShaderForge.SFN_Multiply,id:7044,x:33424,y:33246,varname:node_7044,prsc:2|A-4298-OUT,B-4252-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1015,x:32709,y:34126,ptovrint:False,ptlb:Amplitude,ptin:_Amplitude,varname:node_1015,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:851-5927-4435-732-6613-5116-1015;pass:END;sub:END;*/

Shader "Shader Forge/foliage" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Color ("Color", Color) = (0.4294356,0.6838235,0.02514055,1)
        _Shadow ("Shadow", Color) = (0.2792029,0.3823529,0.1152682,1)
        _Unlit ("Unlit", Color) = (0.1288418,0.1764706,0.05190311,1)
        _intesity ("intesity", Float ) = 0
        _speed ("speed", Float ) = 3
        _Amplitude ("Amplitude", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "RenderType"="TreeTransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            // Dithering function, to use with scene UVs (screen pixel coords)
            // 3x3 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
            float BinaryDither3x3( float value, float2 sceneUVs ) {
                float3x3 mtx = float3x3(
                    float3( 3,  7,  4 )/10.0,
                    float3( 6,  1,  9 )/10.0,
                    float3( 2,  8,  5 )/10.0
                );
                float2 px = floor(_ScreenParams.xy * sceneUVs);
                int xSmp = fmod(px.x,3);
                int ySmp = fmod(px.y,3);
                float3 xVec = 1-saturate(abs(float3(0,1,2) - xSmp));
                float3 yVec = 1-saturate(abs(float3(0,1,2) - ySmp));
                float3 pxMult = float3( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec) );
                return round(value + dot(pxMult, xVec));
            }
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _Color;
            uniform float4 _Shadow;
            uniform float4 _Unlit;
            uniform float _intesity;
            uniform float _speed;
            uniform float _Amplitude;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float4 node_8109 = _Time + _TimeEditor;
                float node_7120 = (sin(mul(unity_ObjectToWorld, v.vertex).r)*(_intesity*((1.0 - o.uv0.g)*cos((node_8109.g*_speed)))));
                float4 node_5986 = _Time + _TimeEditor;
                float4 node_5754 = _Time + _TimeEditor;
                v.vertex.xyz += (float3(0.0,node_7120,node_7120)*clamp(((sin((0.3*(mul(unity_ObjectToWorld, v.vertex).b.r+node_5986.g)*6.28318530718))*1.0+0.0)+(sin((_Amplitude*(mul(unity_ObjectToWorld, v.vertex).r.r+node_5754.g)*6.28318530718))*1.0+0.0)),-1,1));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip( BinaryDither3x3(_Diffuse_var.r - 1.5, sceneUVs) );
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float3 emissive = lerp(_Unlit.rgb,lerp((_Color.rgb*UNITY_LIGHTMODEL_AMBIENT.rgb),_Shadow.rgb,saturate(((1.0 - i.uv0.g)*2.0))),attenuation);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
//            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            // Dithering function, to use with scene UVs (screen pixel coords)
            // 3x3 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
            float BinaryDither3x3( float value, float2 sceneUVs ) {
                float3x3 mtx = float3x3(
                    float3( 3,  7,  4 )/10.0,
                    float3( 6,  1,  9 )/10.0,
                    float3( 2,  8,  5 )/10.0
                );
                float2 px = floor(_ScreenParams.xy * sceneUVs);
                int xSmp = fmod(px.x,3);
                int ySmp = fmod(px.y,3);
                float3 xVec = 1-saturate(abs(float3(0,1,2) - xSmp));
                float3 yVec = 1-saturate(abs(float3(0,1,2) - ySmp));
                float3 pxMult = float3( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec) );
                return round(value + dot(pxMult, xVec));
            }
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float4 _Color;
            uniform float4 _Shadow;
            uniform float4 _Unlit;
            uniform float _intesity;
            uniform float _speed;
            uniform float _Amplitude;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float4 node_8109 = _Time + _TimeEditor;
                float node_7120 = (sin(mul(unity_ObjectToWorld, v.vertex).r)*(_intesity*((1.0 - o.uv0.g)*cos((node_8109.g*_speed)))));
                float4 node_5986 = _Time + _TimeEditor;
                float4 node_5754 = _Time + _TimeEditor;
                v.vertex.xyz += (float3(0.0,node_7120,node_7120)*clamp(((sin((0.3*(mul(unity_ObjectToWorld, v.vertex).b.r+node_5986.g)*6.28318530718))*1.0+0.0)+(sin((_Amplitude*(mul(unity_ObjectToWorld, v.vertex).r.r+node_5754.g)*6.28318530718))*1.0+0.0)),-1,1));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip( BinaryDither3x3(_Diffuse_var.r - 1.5, sceneUVs) );
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            // Dithering function, to use with scene UVs (screen pixel coords)
            // 3x3 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
            float BinaryDither3x3( float value, float2 sceneUVs ) {
                float3x3 mtx = float3x3(
                    float3( 3,  7,  4 )/10.0,
                    float3( 6,  1,  9 )/10.0,
                    float3( 2,  8,  5 )/10.0
                );
                float2 px = floor(_ScreenParams.xy * sceneUVs);
                int xSmp = fmod(px.x,3);
                int ySmp = fmod(px.y,3);
                float3 xVec = 1-saturate(abs(float3(0,1,2) - xSmp));
                float3 yVec = 1-saturate(abs(float3(0,1,2) - ySmp));
                float3 pxMult = float3( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec) );
                return round(value + dot(pxMult, xVec));
            }
            uniform float4 _TimeEditor;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _intesity;
            uniform float _speed;
            uniform float _Amplitude;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float4 node_8109 = _Time + _TimeEditor;
                float node_7120 = (sin(mul(unity_ObjectToWorld, v.vertex).r)*(_intesity*((1.0 - o.uv0.g)*cos((node_8109.g*_speed)))));
                float4 node_5986 = _Time + _TimeEditor;
                float4 node_5754 = _Time + _TimeEditor;
                v.vertex.xyz += (float3(0.0,node_7120,node_7120)*clamp(((sin((0.3*(mul(unity_ObjectToWorld, v.vertex).b.r+node_5986.g)*6.28318530718))*1.0+0.0)+(sin((_Amplitude*(mul(unity_ObjectToWorld, v.vertex).r.r+node_5754.g)*6.28318530718))*1.0+0.0)),-1,1));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                o.screenPos = o.pos;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                clip( BinaryDither3x3(_Diffuse_var.r - 1.5, sceneUVs) );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
