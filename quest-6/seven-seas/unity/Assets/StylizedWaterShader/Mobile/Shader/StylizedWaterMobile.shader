// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:1,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:True,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|emission-5641-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3651,x:30626,y:33828,ptovrint:False,ptlb:Gradient2,ptin:_Gradient2,varname:_Gradient2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Color,id:7356,x:30684,y:33461,ptovrint:False,ptlb:Color1,ptin:_Color1,varname:_Color1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1877163,c2:0.6421192,c3:0.9117647,c4:1;n:type:ShaderForge.SFN_Color,id:711,x:31267,y:33651,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:_Color2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1712803,c2:0.2384142,c3:0.4852941,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:8230,x:31039,y:33854,varname:node_8230,prsc:2|DIST-3651-OUT;n:type:ShaderForge.SFN_Lerp,id:8952,x:31662,y:33675,varname:node_8952,prsc:2|A-7347-OUT,B-711-RGB,T-8230-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5420,x:30790,y:31656,ptovrint:False,ptlb:MainFoamIntensity,ptin:_MainFoamIntensity,varname:_MainFoamIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_DepthBlend,id:2468,x:31534,y:31788,varname:node_2468,prsc:2|DIST-4138-OUT;n:type:ShaderForge.SFN_Multiply,id:4240,x:31040,y:31742,varname:node_4240,prsc:2|A-5420-OUT,B-2913-R;n:type:ShaderForge.SFN_Tex2d,id:2913,x:30758,y:31762,varname:node_2913,prsc:2,tex:0af227fb6078e4f45a4ed1ff20c180c0,ntxv:0,isnm:False|UVIN-2122-UVOUT,TEX-7650-TEX;n:type:ShaderForge.SFN_Add,id:5641,x:32363,y:32117,varname:node_5641,prsc:2|A-8942-OUT,B-8648-OUT;n:type:ShaderForge.SFN_OneMinus,id:4950,x:31713,y:31788,varname:node_4950,prsc:2|IN-2468-OUT;n:type:ShaderForge.SFN_TexCoord,id:5781,x:30163,y:31749,varname:node_5781,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:8103,x:30163,y:31928,ptovrint:False,ptlb:MainFoamScale,ptin:_MainFoamScale,varname:_MainFoamScale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:753,x:30381,y:31776,varname:node_753,prsc:2|A-5781-UVOUT,B-8103-OUT;n:type:ShaderForge.SFN_Power,id:1393,x:31298,y:31788,varname:node_1393,prsc:2|VAL-4240-OUT,EXP-3721-OUT;n:type:ShaderForge.SFN_Vector1,id:3721,x:30924,y:31996,varname:node_3721,prsc:2,v1:2;n:type:ShaderForge.SFN_Panner,id:2122,x:30550,y:31762,varname:node_2122,prsc:2,spu:0.02,spv:0.02|UVIN-753-OUT;n:type:ShaderForge.SFN_Color,id:6687,x:30684,y:33251,ptovrint:False,ptlb:Color0,ptin:_Color0,varname:_Color0,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1877163,c2:0.9117647,c3:0.6421192,c4:1;n:type:ShaderForge.SFN_Lerp,id:7347,x:31257,y:33321,varname:node_7347,prsc:2|A-6687-RGB,B-7356-RGB,T-5941-OUT;n:type:ShaderForge.SFN_ValueProperty,id:6077,x:30684,y:33660,ptovrint:False,ptlb:Gradient1,ptin:_Gradient1,varname:_Gradient1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_DepthBlend,id:5941,x:31003,y:33578,varname:node_5941,prsc:2|DIST-6077-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7650,x:29682,y:32134,ptovrint:False,ptlb:WaterTexture,ptin:_WaterTexture,varname:_WaterTexture,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0af227fb6078e4f45a4ed1ff20c180c0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:8942,x:32159,y:31903,varname:node_8942,prsc:2|A-8292-OUT,B-6947-OUT;n:type:ShaderForge.SFN_Multiply,id:8292,x:32223,y:31504,varname:node_8292,prsc:2|A-9543-OUT,B-4950-OUT;n:type:ShaderForge.SFN_Slider,id:9543,x:31729,y:31411,ptovrint:False,ptlb:MainFoamOpacity,ptin:_MainFoamOpacity,varname:_MainFoamOpacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_ValueProperty,id:5527,x:30845,y:32371,ptovrint:False,ptlb:SecondaryFoamIntensity,ptin:_SecondaryFoamIntensity,varname:_SecondaryFoamIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_DepthBlend,id:647,x:31223,y:32324,varname:node_647,prsc:2|DIST-5527-OUT;n:type:ShaderForge.SFN_TexCoord,id:1426,x:31037,y:32506,varname:node_1426,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:9436,x:31037,y:32685,ptovrint:False,ptlb:SecondaryFoamScale,ptin:_SecondaryFoamScale,varname:_SecondaryFoamScale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:4427,x:31255,y:32533,varname:node_4427,prsc:2|A-1426-UVOUT,B-9436-OUT;n:type:ShaderForge.SFN_Panner,id:6407,x:31424,y:32519,varname:node_6407,prsc:2,spu:-0.01,spv:-0.01|UVIN-4427-OUT;n:type:ShaderForge.SFN_Multiply,id:4518,x:31798,y:32116,varname:node_4518,prsc:2|A-1873-OUT,B-6156-OUT;n:type:ShaderForge.SFN_Slider,id:1873,x:31375,y:32068,ptovrint:False,ptlb:SecondaryFoamOpacity,ptin:_SecondaryFoamOpacity,varname:_SecondaryFoamOpacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:6947,x:32010,y:32121,varname:node_6947,prsc:2|A-4518-OUT,B-3234-OUT,T-647-OUT;n:type:ShaderForge.SFN_Vector1,id:3234,x:31542,y:32194,varname:node_3234,prsc:2,v1:0;n:type:ShaderForge.SFN_Tex2d,id:5766,x:31643,y:32538,varname:node_5766,prsc:2,tex:0af227fb6078e4f45a4ed1ff20c180c0,ntxv:0,isnm:False|UVIN-6407-UVOUT,TEX-7650-TEX;n:type:ShaderForge.SFN_Add,id:4138,x:31344,y:31627,varname:node_4138,prsc:2|A-6140-OUT,B-1393-OUT;n:type:ShaderForge.SFN_Multiply,id:6140,x:31065,y:31573,varname:node_6140,prsc:2|A-2621-OUT,B-5420-OUT;n:type:ShaderForge.SFN_Vector1,id:2621,x:30821,y:31506,varname:node_2621,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Power,id:6156,x:31817,y:32409,varname:node_6156,prsc:2|VAL-5766-R,EXP-7973-OUT;n:type:ShaderForge.SFN_Vector1,id:7973,x:31456,y:32432,varname:node_7973,prsc:2,v1:2;n:type:ShaderForge.SFN_Lerp,id:8648,x:32104,y:33694,varname:node_8648,prsc:2|A-8952-OUT,B-1197-RGB,T-7017-OUT;n:type:ShaderForge.SFN_Color,id:1197,x:31678,y:33842,ptovrint:False,ptlb:FresnelColor,ptin:_FresnelColor,varname:_FresnelColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7205882,c2:0.907505,c3:1,c4:1;n:type:ShaderForge.SFN_Fresnel,id:7017,x:31475,y:34171,varname:node_7017,prsc:2|EXP-8254-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8254,x:31111,y:34191,ptovrint:False,ptlb:FresnelExp,ptin:_FresnelExp,varname:_FresnelExp,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;proporder:6687-6077-7356-3651-711-1197-8254-5420-8103-9543-5527-9436-1873-7650;pass:END;sub:END;*/

Shader "Marc Sureda/Mobile/StylizedWaterMobile" {
    Properties {
        _Color0 ("Color0", Color) = (0.1877163,0.9117647,0.6421192,1)
        _Gradient1 ("Gradient1", Float ) = 1
        _Color1 ("Color1", Color) = (0.1877163,0.6421192,0.9117647,1)
        _Gradient2 ("Gradient2", Float ) = 2
        _Color2 ("Color2", Color) = (0.1712803,0.2384142,0.4852941,1)
        _FresnelColor ("FresnelColor", Color) = (0.7205882,0.907505,1,1)
        _FresnelExp ("FresnelExp", Float ) = 5
        _MainFoamIntensity ("MainFoamIntensity", Float ) = 1
        _MainFoamScale ("MainFoamScale", Float ) = 1
        _MainFoamOpacity ("MainFoamOpacity", Range(0, 1)) = 1
        _SecondaryFoamIntensity ("SecondaryFoamIntensity", Float ) = 1
        _SecondaryFoamScale ("SecondaryFoamScale", Float ) = 1
        _SecondaryFoamOpacity ("SecondaryFoamOpacity", Range(0, 1)) = 1
        _WaterTexture ("WaterTexture", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "DisableBatching"="True"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 switch 
            #pragma target 2.0
            uniform sampler2D _CameraDepthTexture;
            uniform float _Gradient2;
            uniform float4 _Color1;
            uniform float4 _Color2;
            uniform float _MainFoamIntensity;
            uniform float _MainFoamScale;
            uniform float4 _Color0;
            uniform float _Gradient1;
            uniform sampler2D _WaterTexture; uniform float4 _WaterTexture_ST;
            uniform float _MainFoamOpacity;
            uniform float _SecondaryFoamIntensity;
            uniform float _SecondaryFoamScale;
            uniform float _SecondaryFoamOpacity;
            uniform float4 _FresnelColor;
            uniform float _FresnelExp;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float4 node_2892 = _Time;
                float2 node_2122 = ((i.uv0*_MainFoamScale)+node_2892.g*float2(0.02,0.02));
                float4 node_2913 = tex2D(_WaterTexture,TRANSFORM_TEX(node_2122, _WaterTexture));
                float2 node_6407 = ((i.uv0*_SecondaryFoamScale)+node_2892.g*float2(-0.01,-0.01));
                float4 node_5766 = tex2D(_WaterTexture,TRANSFORM_TEX(node_6407, _WaterTexture));
                float3 emissive = (((_MainFoamOpacity*(1.0 - saturate((sceneZ-partZ)/((0.2*_MainFoamIntensity)+pow((_MainFoamIntensity*node_2913.r),2.0)))))+lerp((_SecondaryFoamOpacity*pow(node_5766.r,2.0)),0.0,saturate((sceneZ-partZ)/_SecondaryFoamIntensity)))+lerp(lerp(lerp(_Color0.rgb,_Color1.rgb,saturate((sceneZ-partZ)/_Gradient1)),_Color2.rgb,saturate((sceneZ-partZ)/_Gradient2)),_FresnelColor.rgb,pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelExp)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
