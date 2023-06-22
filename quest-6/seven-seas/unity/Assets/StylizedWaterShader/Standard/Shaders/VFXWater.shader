// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:0,qpre:4,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33562,y:32816,varname:node_1873,prsc:2|emission-5376-RGB,alpha-9598-OUT,clip-3119-OUT;n:type:ShaderForge.SFN_VertexColor,id:5376,x:31665,y:33109,varname:node_5376,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:5171,x:31333,y:33311,ptovrint:False,ptlb:ParticleTextureA,ptin:_ParticleTextureA,varname:_ParticleTextureA,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Lerp,id:3119,x:32822,y:33168,varname:node_3119,prsc:2|A-8737-OUT,B-7923-OUT,T-6058-OUT;n:type:ShaderForge.SFN_Multiply,id:6058,x:32620,y:33248,varname:node_6058,prsc:2|A-5376-A,B-8343-OUT;n:type:ShaderForge.SFN_Vector1,id:8737,x:32591,y:33114,varname:node_8737,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:7923,x:32591,y:33168,varname:node_7923,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:7114,x:32760,y:32777,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:_Opacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.9,max:1;n:type:ShaderForge.SFN_Multiply,id:9598,x:33303,y:32990,varname:node_9598,prsc:2|A-7114-OUT,B-3119-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:5994,x:31764,y:33298,ptovrint:False,ptlb:R,ptin:_R,varname:_R,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-137-OUT,B-5171-R;n:type:ShaderForge.SFN_SwitchProperty,id:9034,x:31764,y:33440,ptovrint:False,ptlb:G,ptin:_G,varname:_G,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-137-OUT,B-5171-G;n:type:ShaderForge.SFN_SwitchProperty,id:4855,x:31764,y:33575,ptovrint:False,ptlb:B,ptin:_B,varname:_B,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-137-OUT,B-5171-B;n:type:ShaderForge.SFN_Vector1,id:137,x:31301,y:33491,varname:node_137,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:8343,x:32079,y:33419,varname:node_8343,prsc:2|A-5994-OUT,B-9034-OUT,C-4855-OUT,D-7817-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7817,x:31764,y:33720,ptovrint:False,ptlb:A,ptin:_A,varname:_A,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-137-OUT,B-5171-A;proporder:5171-7114-5994-9034-4855-7817;pass:END;sub:END;*/

Shader "Hidden/VFXWater" {
    Properties {
        _ParticleTextureA ("ParticleTextureA", 2D) = "white" {}
        _Opacity ("Opacity", Range(0, 1)) = 0.9
        [MaterialToggle] _R ("R", Float ) = 0
        [MaterialToggle] _G ("G", Float ) = 0
        [MaterialToggle] _B ("B", Float ) = 0
        [MaterialToggle] _A ("A", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _ParticleTextureA; uniform float4 _ParticleTextureA_ST;
            uniform float _Opacity;
            uniform fixed _R;
            uniform fixed _G;
            uniform fixed _B;
            uniform fixed _A;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float node_137 = 0.0;
                float4 _ParticleTextureA_var = tex2D(_ParticleTextureA,TRANSFORM_TEX(i.uv0, _ParticleTextureA));
                float node_3119 = lerp(0.0,1.0,(i.vertexColor.a*(lerp( node_137, _ParticleTextureA_var.r, _R )+lerp( node_137, _ParticleTextureA_var.g, _G )+lerp( node_137, _ParticleTextureA_var.b, _B )+lerp( node_137, _ParticleTextureA_var.a, _A ))));
                clip(node_3119 - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = i.vertexColor.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,(_Opacity*node_3119));
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
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _ParticleTextureA; uniform float4 _ParticleTextureA_ST;
            uniform fixed _R;
            uniform fixed _G;
            uniform fixed _B;
            uniform fixed _A;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float node_137 = 0.0;
                float4 _ParticleTextureA_var = tex2D(_ParticleTextureA,TRANSFORM_TEX(i.uv0, _ParticleTextureA));
                float node_3119 = lerp(0.0,1.0,(i.vertexColor.a*(lerp( node_137, _ParticleTextureA_var.r, _R )+lerp( node_137, _ParticleTextureA_var.g, _G )+lerp( node_137, _ParticleTextureA_var.b, _B )+lerp( node_137, _ParticleTextureA_var.a, _A ))));
                clip(node_3119 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
