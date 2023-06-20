// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:1,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5078936,fgcg:0.7199216,fgcb:0.8970588,fgca:1,fgde:0.13,fgrn:36.7,fgrf:90.91,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32409,y:33055,varname:node_3138,prsc:2|emission-4449-OUT,alpha-7757-OUT,refract-5939-OUT,voffset-6456-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:28871,y:32811,ptovrint:False,ptlb:DepthGradient2,ptin:_DepthGradient2,varname:_DepthGradient2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.05882353,c2:0.1960784,c3:0.4627451,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:2772,x:25790,y:33962,varname:node_2772,prsc:2|DIST-4498-OUT;n:type:ShaderForge.SFN_Color,id:9477,x:25731,y:33645,ptovrint:False,ptlb:FoamColor,ptin:_FoamColor,varname:_FoamColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.854902,c2:0.9921569,c3:1,c4:1;n:type:ShaderForge.SFN_Power,id:9623,x:26162,y:33962,varname:node_9623,prsc:2|VAL-1428-OUT,EXP-6404-OUT;n:type:ShaderForge.SFN_Vector1,id:6404,x:25998,y:34089,varname:node_6404,prsc:2,v1:15;n:type:ShaderForge.SFN_Divide,id:2616,x:26335,y:33962,varname:node_2616,prsc:2|A-9623-OUT,B-213-OUT;n:type:ShaderForge.SFN_Vector1,id:213,x:26162,y:34089,varname:node_213,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Clamp01,id:12,x:26528,y:33963,varname:node_12,prsc:2|IN-2616-OUT;n:type:ShaderForge.SFN_RemapRange,id:4648,x:25342,y:33962,varname:node_4648,prsc:2,frmn:0,frmx:1,tomn:0.2,tomx:0.3|IN-5335-OUT;n:type:ShaderForge.SFN_Clamp01,id:1428,x:25998,y:33962,varname:node_1428,prsc:2|IN-2772-OUT;n:type:ShaderForge.SFN_Append,id:7355,x:22796,y:33450,varname:node_7355,prsc:2|A-6450-OUT,B-6450-OUT;n:type:ShaderForge.SFN_Time,id:4283,x:22175,y:33697,varname:node_4283,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9268,x:23088,y:33490,varname:node_9268,prsc:2|A-7355-OUT,B-4283-T;n:type:ShaderForge.SFN_OneMinus,id:9,x:26706,y:33963,varname:node_9,prsc:2|IN-12-OUT;n:type:ShaderForge.SFN_Multiply,id:5691,x:27643,y:33731,varname:node_5691,prsc:2|A-9477-RGB,B-9706-OUT;n:type:ShaderForge.SFN_Add,id:5972,x:31024,y:32635,varname:node_5972,prsc:2|A-3346-OUT,B-5691-OUT,C-1102-OUT;n:type:ShaderForge.SFN_Fresnel,id:7892,x:28937,y:31740,varname:node_7892,prsc:2|EXP-6132-OUT;n:type:ShaderForge.SFN_Lerp,id:7203,x:29624,y:32573,varname:node_7203,prsc:2|A-9062-RGB,B-7564-OUT,T-9446-OUT;n:type:ShaderForge.SFN_Color,id:492,x:28543,y:32257,ptovrint:False,ptlb:FresnelColor,ptin:_FresnelColor,varname:_FresnelColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5764706,c2:0.6980392,c3:0.8000001,c4:1;n:type:ShaderForge.SFN_Tex2d,id:601,x:23496,y:33607,varname:_FoamNoise,prsc:2,tex:0af227fb6078e4f45a4ed1ff20c180c0,ntxv:0,isnm:False|UVIN-6798-OUT,TEX-783-TEX;n:type:ShaderForge.SFN_TexCoord,id:1266,x:22330,y:33924,varname:node_1266,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1473,x:23899,y:33703,varname:node_1473,prsc:2|A-601-R,B-7041-OUT;n:type:ShaderForge.SFN_Slider,id:7041,x:23324,y:33821,ptovrint:False,ptlb:Main Foam Intensity,ptin:_MainFoamIntensity,varname:_MainFoamIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3.84466,max:10;n:type:ShaderForge.SFN_Add,id:6798,x:23273,y:33607,varname:node_6798,prsc:2|A-9268-OUT,B-3457-OUT;n:type:ShaderForge.SFN_Slider,id:6132,x:28190,y:31595,ptovrint:False,ptlb:FresnelExp,ptin:_FresnelExp,varname:_FresnelExp,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:10,max:10;n:type:ShaderForge.SFN_Tex2d,id:719,x:28918,y:30509,ptovrint:False,ptlb:ReflectionTex,ptin:_ReflectionTex,varname:_ReflectionTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-1893-OUT;n:type:ShaderForge.SFN_ScreenPos,id:3727,x:27760,y:30425,varname:node_3727,prsc:2,sctp:2;n:type:ShaderForge.SFN_Multiply,id:6985,x:29168,y:30611,varname:node_6985,prsc:2|A-719-RGB,B-749-OUT;n:type:ShaderForge.SFN_Vector1,id:749,x:28930,y:30746,varname:node_749,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:4498,x:25543,y:33962,varname:node_4498,prsc:2|A-4648-OUT,B-1473-OUT;n:type:ShaderForge.SFN_Multiply,id:8455,x:30917,y:33478,varname:node_8455,prsc:2|A-9429-OUT,B-9095-OUT;n:type:ShaderForge.SFN_Multiply,id:13,x:31205,y:33348,varname:node_13,prsc:2|A-5983-OUT,B-8455-OUT;n:type:ShaderForge.SFN_NormalVector,id:5983,x:30917,y:33293,prsc:2,pt:False;n:type:ShaderForge.SFN_TexCoord,id:6777,x:22701,y:31309,varname:node_6777,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:1343,x:24440,y:31374,varname:node_1343,prsc:2|A-9188-B,B-1683-OUT;n:type:ShaderForge.SFN_Sin,id:5335,x:24867,y:31374,varname:node_5335,prsc:2|IN-4998-OUT;n:type:ShaderForge.SFN_Time,id:8260,x:23474,y:31013,varname:node_8260,prsc:2;n:type:ShaderForge.SFN_Subtract,id:4998,x:24662,y:31374,varname:node_4998,prsc:2|A-186-OUT,B-1343-OUT;n:type:ShaderForge.SFN_Append,id:9867,x:28441,y:30402,varname:node_9867,prsc:2|A-3727-U,B-3727-V;n:type:ShaderForge.SFN_Multiply,id:1102,x:27346,y:33551,varname:node_1102,prsc:2|A-9477-RGB,B-7744-OUT;n:type:ShaderForge.SFN_Multiply,id:7563,x:24968,y:34513,varname:node_7563,prsc:2|A-7465-OUT,B-4707-OUT;n:type:ShaderForge.SFN_DepthBlend,id:2075,x:22577,y:34814,varname:node_2075,prsc:2|DIST-1928-OUT;n:type:ShaderForge.SFN_OneMinus,id:85,x:23625,y:34869,varname:node_85,prsc:2|IN-9796-OUT;n:type:ShaderForge.SFN_Divide,id:2330,x:23272,y:34869,varname:node_2330,prsc:2|A-1115-OUT,B-8618-OUT;n:type:ShaderForge.SFN_Vector1,id:8618,x:23097,y:35005,varname:node_8618,prsc:2,v1:0.8;n:type:ShaderForge.SFN_Clamp01,id:587,x:22812,y:34814,varname:node_587,prsc:2|IN-2075-OUT;n:type:ShaderForge.SFN_Clamp01,id:9796,x:23449,y:34869,varname:node_9796,prsc:2|IN-2330-OUT;n:type:ShaderForge.SFN_Power,id:1115,x:23013,y:34858,varname:node_1115,prsc:2|VAL-587-OUT,EXP-8937-OUT;n:type:ShaderForge.SFN_Vector1,id:8937,x:22787,y:34993,varname:node_8937,prsc:2,v1:1;n:type:ShaderForge.SFN_Power,id:253,x:24079,y:34512,varname:node_253,prsc:2|VAL-6615-OUT,EXP-5973-OUT;n:type:ShaderForge.SFN_Vector1,id:5973,x:23865,y:34647,varname:node_5973,prsc:2,v1:0.5;n:type:ShaderForge.SFN_RemapRange,id:1354,x:23680,y:34512,varname:node_1354,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:3|IN-4292-R;n:type:ShaderForge.SFN_Clamp01,id:6615,x:23877,y:34512,varname:node_6615,prsc:2|IN-1354-OUT;n:type:ShaderForge.SFN_Tex2d,id:4292,x:23412,y:34496,varname:node_4292,prsc:2,tex:0af227fb6078e4f45a4ed1ff20c180c0,ntxv:0,isnm:False|UVIN-5249-UVOUT,TEX-783-TEX;n:type:ShaderForge.SFN_TexCoord,id:4757,x:22458,y:34451,varname:node_4757,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5957,x:22839,y:34569,varname:node_5957,prsc:2|A-4757-UVOUT,B-3985-OUT;n:type:ShaderForge.SFN_Multiply,id:4707,x:24320,y:34512,varname:node_4707,prsc:2|A-253-OUT,B-4459-OUT;n:type:ShaderForge.SFN_Vector1,id:4459,x:24053,y:34647,varname:node_4459,prsc:2,v1:0.3;n:type:ShaderForge.SFN_LightColor,id:1469,x:30669,y:32877,varname:node_1469,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6425,x:31764,y:32848,varname:node_6425,prsc:2|A-5972-OUT,B-6366-OUT;n:type:ShaderForge.SFN_ConstantClamp,id:6299,x:30882,y:32877,varname:node_6299,prsc:2,min:0.3,max:1|IN-1469-RGB;n:type:ShaderForge.SFN_Add,id:9686,x:28196,y:30537,varname:node_9686,prsc:2|A-3727-U,B-721-OUT;n:type:ShaderForge.SFN_Vector1,id:721,x:27760,y:30591,varname:node_721,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Append,id:5320,x:28430,y:30604,varname:node_5320,prsc:2|A-9686-OUT,B-7376-OUT;n:type:ShaderForge.SFN_Lerp,id:1893,x:28586,y:30662,varname:node_1893,prsc:2|A-9867-OUT,B-5320-OUT,T-3182-OUT;n:type:ShaderForge.SFN_Add,id:7376,x:28196,y:30672,varname:node_7376,prsc:2|A-3727-V,B-721-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:8819,x:19460,y:34396,ptovrint:False,ptlb:DistortionTexture,ptin:_DistortionTexture,varname:_DistortionTexture,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:a67d2228caf588945b19a42fbd3c3967,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3457,x:22710,y:33947,varname:node_3457,prsc:2|A-1266-UVOUT,B-5889-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5889,x:22481,y:34081,ptovrint:False,ptlb:Main Foam Scale,ptin:_MainFoamScale,varname:_MainFoamScale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_ValueProperty,id:3985,x:22515,y:34650,ptovrint:False,ptlb:Secondary Foam Scale,ptin:_SecondaryFoamScale,varname:_SecondaryFoamScale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_Panner,id:5249,x:23160,y:34568,varname:node_5249,prsc:2,spu:0.01,spv:0.01|UVIN-5957-OUT;n:type:ShaderForge.SFN_Slider,id:1928,x:22102,y:34955,ptovrint:False,ptlb:Secondary Foam Intensity,ptin:_SecondaryFoamIntensity,varname:_SecondaryFoamIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.330097,max:10;n:type:ShaderForge.SFN_SwitchProperty,id:7465,x:24641,y:34360,ptovrint:False,ptlb:Secondary Foam Always Visible,ptin:_SecondaryFoamAlwaysVisible,varname:_SecondaryFoamAlwaysVisible,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-85-OUT,B-3899-OUT;n:type:ShaderForge.SFN_Vector1,id:3899,x:24348,y:34318,varname:node_3899,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:7744,x:25360,y:34542,varname:node_7744,prsc:2|A-7563-OUT,B-818-OUT;n:type:ShaderForge.SFN_Slider,id:818,x:25007,y:34693,ptovrint:False,ptlb:Secondary Foam Opacity,ptin:_SecondaryFoamOpacity,varname:_SecondaryFoamOpacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6310679,max:1;n:type:ShaderForge.SFN_Multiply,id:9706,x:27041,y:33992,varname:node_9706,prsc:2|A-9-OUT,B-4006-OUT;n:type:ShaderForge.SFN_Slider,id:4006,x:26637,y:34160,ptovrint:False,ptlb:Main Foam Opacity,ptin:_MainFoamOpacity,varname:_MainFoamOpacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8737864,max:1;n:type:ShaderForge.SFN_Slider,id:3867,x:22651,y:31602,ptovrint:False,ptlb:Waves Direction,ptin:_WavesDirection,varname:_WavesDirection,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:360;n:type:ShaderForge.SFN_Get,id:9566,x:28877,y:31606,varname:node_9566,prsc:2|IN-3158-OUT;n:type:ShaderForge.SFN_Multiply,id:186,x:23696,y:31100,varname:node_186,prsc:2|A-8260-T,B-4539-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4539,x:23512,y:31169,ptovrint:False,ptlb:Waves Speed,ptin:_WavesSpeed,varname:_WavesSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Set,id:3158,x:25711,y:31401,varname:Waves,prsc:2|IN-2677-OUT;n:type:ShaderForge.SFN_Get,id:9429,x:30540,y:33448,varname:node_9429,prsc:2|IN-3158-OUT;n:type:ShaderForge.SFN_Vector1,id:9095,x:30532,y:33575,varname:node_9095,prsc:2,v1:0.04;n:type:ShaderForge.SFN_Clamp01,id:9869,x:29151,y:31821,varname:node_9869,prsc:2|IN-7892-OUT;n:type:ShaderForge.SFN_Multiply,id:5502,x:29261,y:31632,varname:node_5502,prsc:2|A-8083-OUT,B-9566-OUT,C-2691-OUT;n:type:ShaderForge.SFN_Vector1,id:8083,x:28898,y:31703,varname:node_8083,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Vector1,id:3812,x:31285,y:33308,varname:node_3812,prsc:2,v1:0;n:type:ShaderForge.SFN_SwitchProperty,id:6456,x:31498,y:33308,ptovrint:False,ptlb:Vertex Offset,ptin:_VertexOffset,varname:_VertexOffset,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-3812-OUT,B-13-OUT;n:type:ShaderForge.SFN_Multiply,id:1683,x:24151,y:31548,varname:node_1683,prsc:2|A-8928-OUT,B-5083-OUT;n:type:ShaderForge.SFN_Vector1,id:5083,x:23859,y:31658,varname:node_5083,prsc:2,v1:30;n:type:ShaderForge.SFN_Slider,id:8928,x:23703,y:31537,ptovrint:False,ptlb:Waves Amplitude,ptin:_WavesAmplitude,varname:_WavesAmplitude,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.980582,max:10;n:type:ShaderForge.SFN_Multiply,id:2677,x:25542,y:31401,varname:node_2677,prsc:2|A-5335-OUT,B-7687-OUT,C-1145-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7687,x:25273,y:31531,ptovrint:False,ptlb:Waves Intensity,ptin:_WavesIntensity,varname:_WavesIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_SwitchProperty,id:7062,x:29720,y:30713,ptovrint:False,ptlb:Real Time Reflection,ptin:_RealTimeReflection,varname:_RealTimeReflection,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-9131-OUT,B-2131-OUT;n:type:ShaderForge.SFN_Vector1,id:9131,x:29471,y:30689,varname:node_9131,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:2691,x:28200,y:31746,ptovrint:False,ptlb:WaveDistortion Intensity,ptin:_WaveDistortionIntensity,varname:_WaveDistortionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.592233,max:4;n:type:ShaderForge.SFN_DepthBlend,id:9531,x:28637,y:32647,varname:node_9531,prsc:2|DIST-8803-OUT;n:type:ShaderForge.SFN_Color,id:9062,x:28871,y:32485,ptovrint:False,ptlb:DepthGradient1,ptin:_DepthGradient1,varname:_DepthGradient1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1098039,c2:0.5960785,c3:0.6196079,c4:1;n:type:ShaderForge.SFN_Clamp01,id:9446,x:28871,y:32647,varname:node_9446,prsc:2|IN-9531-OUT;n:type:ShaderForge.SFN_Lerp,id:8560,x:29770,y:32066,varname:node_8560,prsc:2|A-7203-OUT,B-492-RGB,T-9869-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1595,x:22149,y:33438,ptovrint:False,ptlb:Main Foam Speed,ptin:_MainFoamSpeed,varname:_MainFoamSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:6450,x:22439,y:33511,varname:node_6450,prsc:2|A-1595-OUT,B-7321-OUT;n:type:ShaderForge.SFN_Vector1,id:7321,x:22188,y:33567,varname:node_7321,prsc:2,v1:0.15;n:type:ShaderForge.SFN_ValueProperty,id:8803,x:28167,y:32653,ptovrint:False,ptlb:GradientPosition1,ptin:_GradientPosition1,varname:_GradientPosition1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1.6;n:type:ShaderForge.SFN_Tex2d,id:3842,x:28759,y:31872,varname:_DistortionExtra,prsc:2,tex:a67d2228caf588945b19a42fbd3c3967,ntxv:0,isnm:False|UVIN-5311-UVOUT,TEX-8819-TEX;n:type:ShaderForge.SFN_TexCoord,id:9161,x:28119,y:31893,varname:node_9161,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:5714,x:28336,y:31909,varname:node_5714,prsc:2|A-9161-UVOUT,B-6730-OUT;n:type:ShaderForge.SFN_Panner,id:5311,x:28515,y:31904,varname:node_5311,prsc:2,spu:0.01,spv:0.01|UVIN-5714-OUT;n:type:ShaderForge.SFN_Add,id:3182,x:29539,y:31661,varname:node_3182,prsc:2|A-5502-OUT,B-2414-OUT;n:type:ShaderForge.SFN_Multiply,id:5475,x:29105,y:31829,varname:node_5475,prsc:2|A-3842-G,B-5745-OUT;n:type:ShaderForge.SFN_RemapRange,id:2414,x:29353,y:31846,varname:node_2414,prsc:2,frmn:0,frmx:1,tomn:0,tomx:2|IN-5475-OUT;n:type:ShaderForge.SFN_Rotator,id:9794,x:23063,y:31325,varname:node_9794,prsc:2|UVIN-6777-UVOUT,ANG-3597-OUT;n:type:ShaderForge.SFN_Tex2d,id:9188,x:23423,y:31328,varname:_Gradient,prsc:2,tex:a67d2228caf588945b19a42fbd3c3967,ntxv:0,isnm:False|UVIN-9794-UVOUT,TEX-8819-TEX;n:type:ShaderForge.SFN_Divide,id:3597,x:23164,y:31601,varname:node_3597,prsc:2|A-3867-OUT,B-417-OUT;n:type:ShaderForge.SFN_Vector1,id:417,x:22939,y:31793,varname:node_417,prsc:2,v1:57;n:type:ShaderForge.SFN_Lerp,id:7564,x:29378,y:32740,varname:node_7564,prsc:2|A-7241-RGB,B-4514-RGB,T-5399-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5395,x:28223,y:32927,ptovrint:False,ptlb:GradientPosition2,ptin:_GradientPosition2,varname:_GradientPosition2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Color,id:4514,x:28891,y:33101,ptovrint:False,ptlb:DepthGradient3,ptin:_DepthGradient3,varname:_DepthGradient3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.0625,c3:0.25,c4:1;n:type:ShaderForge.SFN_DepthBlend,id:9631,x:28609,y:32899,varname:node_9631,prsc:2|DIST-4806-OUT;n:type:ShaderForge.SFN_Clamp01,id:5399,x:28995,y:32955,varname:node_5399,prsc:2|IN-1850-OUT;n:type:ShaderForge.SFN_Add,id:4806,x:28416,y:32899,varname:node_4806,prsc:2|A-8803-OUT,B-5395-OUT;n:type:ShaderForge.SFN_Vector1,id:2081,x:28560,y:33107,varname:node_2081,prsc:2,v1:3;n:type:ShaderForge.SFN_Power,id:1850,x:28751,y:33022,varname:node_1850,prsc:2|VAL-9631-OUT,EXP-2081-OUT;n:type:ShaderForge.SFN_Vector1,id:1145,x:25278,y:31602,varname:node_1145,prsc:2,v1:10;n:type:ShaderForge.SFN_Slider,id:5745,x:28420,y:32146,ptovrint:False,ptlb:Turbulence Distortion Intesity,ptin:_TurbulenceDistortionIntesity,varname:_TurbulenceDistortionIntesity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8155341,max:6;n:type:ShaderForge.SFN_ValueProperty,id:6730,x:28181,y:32106,ptovrint:False,ptlb:Turbulence Scale,ptin:_TurbulenceScale,varname:_TurbulenceScale,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Multiply,id:2131,x:29417,y:31015,varname:node_2131,prsc:2|A-6985-OUT,B-4869-OUT;n:type:ShaderForge.SFN_Slider,id:4869,x:29066,y:31066,ptovrint:False,ptlb:ReflectionsIntensity,ptin:_ReflectionsIntensity,varname:_ReflectionsIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Blend,id:3346,x:30820,y:31524,varname:node_3346,prsc:2,blmd:8,clmp:True|SRC-7062-OUT,DST-8560-OUT;n:type:ShaderForge.SFN_Slider,id:1089,x:31095,y:33035,ptovrint:False,ptlb:LightColorIntensity,ptin:_LightColorIntensity,varname:_LightColorIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7759457,max:1;n:type:ShaderForge.SFN_Lerp,id:6366,x:31585,y:32918,varname:node_6366,prsc:2|A-7513-OUT,B-6299-OUT,T-1089-OUT;n:type:ShaderForge.SFN_Vector1,id:7513,x:31262,y:32952,varname:node_7513,prsc:2,v1:1;n:type:ShaderForge.SFN_Set,id:3856,x:32850,y:32587,varname:CustomLight,prsc:2|IN-8337-OUT;n:type:ShaderForge.SFN_LightVector,id:7943,x:34627,y:32661,varname:node_7943,prsc:2;n:type:ShaderForge.SFN_ViewReflectionVector,id:6321,x:34627,y:32796,varname:node_6321,prsc:2;n:type:ShaderForge.SFN_Dot,id:2177,x:33606,y:32428,varname:node_2177,prsc:2,dt:0|A-7943-OUT,B-6321-OUT;n:type:ShaderForge.SFN_Slider,id:3444,x:34888,y:32932,ptovrint:False,ptlb:Roughness,ptin:_Roughness,varname:_Roughness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.01,cur:0.6357628,max:1;n:type:ShaderForge.SFN_Power,id:2377,x:33970,y:32786,varname:node_2377,prsc:2|VAL-2177-OUT,EXP-2717-OUT;n:type:ShaderForge.SFN_Exp,id:2717,x:34194,y:32991,varname:node_2717,prsc:2,et:1|IN-8981-OUT;n:type:ShaderForge.SFN_RemapRange,id:8981,x:34453,y:32928,varname:node_8981,prsc:2,frmn:0,frmx:1,tomn:5,tomx:10|IN-2711-OUT;n:type:ShaderForge.SFN_OneMinus,id:2711,x:34654,y:32928,varname:node_2711,prsc:2|IN-3444-OUT;n:type:ShaderForge.SFN_LightColor,id:9414,x:32871,y:32448,varname:node_9414,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5252,x:34318,y:32399,varname:node_5252,prsc:2|A-2006-OUT,B-9414-RGB;n:type:ShaderForge.SFN_Add,id:2354,x:31864,y:33044,varname:node_2354,prsc:2|A-6425-OUT,B-6874-OUT;n:type:ShaderForge.SFN_Get,id:6874,x:31003,y:32903,varname:node_6874,prsc:2|IN-3856-OUT;n:type:ShaderForge.SFN_Clamp01,id:8691,x:33115,y:32587,varname:node_8691,prsc:2|IN-5629-OUT;n:type:ShaderForge.SFN_Set,id:3395,x:29359,y:31591,varname:Turbulence,prsc:2|IN-3182-OUT;n:type:ShaderForge.SFN_Multiply,id:5629,x:34611,y:32472,varname:node_5629,prsc:2|A-4983-OUT,B-5252-OUT;n:type:ShaderForge.SFN_Get,id:9795,x:33030,y:32258,varname:node_9795,prsc:2|IN-3395-OUT;n:type:ShaderForge.SFN_Divide,id:3481,x:33461,y:32330,varname:node_3481,prsc:2|A-9795-OUT,B-3444-OUT;n:type:ShaderForge.SFN_RemapRange,id:4983,x:33296,y:32277,varname:node_4983,prsc:2,frmn:0,frmx:1,tomn:0.2,tomx:1|IN-3481-OUT;n:type:ShaderForge.SFN_Multiply,id:8337,x:32962,y:32643,varname:node_8337,prsc:2|A-8691-OUT,B-212-OUT;n:type:ShaderForge.SFN_Slider,id:212,x:32776,y:32885,ptovrint:False,ptlb:SpecularIntensity,ptin:_SpecularIntensity,varname:_SpecularIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Add,id:4068,x:26820,y:34450,varname:node_4068,prsc:2|A-9706-OUT,B-3041-OUT;n:type:ShaderForge.SFN_Set,id:6516,x:27084,y:34490,varname:FoamMask,prsc:2|IN-4068-OUT;n:type:ShaderForge.SFN_DepthBlend,id:4723,x:31129,y:33836,varname:node_4723,prsc:2|DIST-7824-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7824,x:30937,y:33847,ptovrint:False,ptlb:OpacityDepth,ptin:_OpacityDepth,varname:_OpacityDepth,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Slider,id:376,x:30972,y:33647,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:_Opacity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7378641,max:1;n:type:ShaderForge.SFN_Get,id:4922,x:31090,y:33558,varname:node_4922,prsc:2|IN-6516-OUT;n:type:ShaderForge.SFN_Clamp01,id:7757,x:31797,y:33612,varname:node_7757,prsc:2|IN-6312-OUT;n:type:ShaderForge.SFN_Add,id:6312,x:31471,y:33627,varname:node_6312,prsc:2|A-4922-OUT,B-376-OUT,C-8629-OUT,D-6874-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1497,x:31594,y:34089,ptovrint:False,ptlb:Refraction Intensity,ptin:_RefractionIntensity,varname:_RefractionIntensity,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Append,id:5939,x:32146,y:34213,varname:node_5939,prsc:2|A-7739-OUT,B-7739-OUT;n:type:ShaderForge.SFN_Get,id:4775,x:31535,y:34238,varname:node_4775,prsc:2|IN-3395-OUT;n:type:ShaderForge.SFN_Multiply,id:7739,x:31889,y:34198,varname:node_7739,prsc:2|A-8283-OUT,B-3872-OUT;n:type:ShaderForge.SFN_Multiply,id:8283,x:31760,y:34013,varname:node_8283,prsc:2|A-1659-OUT,B-1497-OUT;n:type:ShaderForge.SFN_Vector1,id:1659,x:31528,y:34007,varname:node_1659,prsc:2,v1:0.01;n:type:ShaderForge.SFN_Multiply,id:3872,x:31705,y:34329,varname:node_3872,prsc:2|A-4775-OUT,B-6600-OUT;n:type:ShaderForge.SFN_TexCoord,id:549,x:31377,y:34292,varname:node_549,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:6600,x:31477,y:34446,varname:node_6600,prsc:2|A-549-UVOUT,B-7778-OUT;n:type:ShaderForge.SFN_Vector1,id:7778,x:31303,y:34488,varname:node_7778,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:3041,x:26061,y:34619,varname:node_3041,prsc:2|A-7744-OUT,B-3362-OUT;n:type:ShaderForge.SFN_Vector1,id:3362,x:25673,y:34720,varname:node_3362,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Clamp01,id:8629,x:31350,y:33789,varname:node_8629,prsc:2|IN-4723-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:783,x:21229,y:34027,ptovrint:False,ptlb:FoamTexture,ptin:_FoamTexture,varname:_FoamTexture,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0af227fb6078e4f45a4ed1ff20c180c0,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2006,x:34130,y:32720,varname:node_2006,prsc:2|A-7898-OUT,B-2377-OUT;n:type:ShaderForge.SFN_Vector1,id:7898,x:33591,y:32758,varname:node_7898,prsc:2,v1:2;n:type:ShaderForge.SFN_Append,id:4449,x:32031,y:33100,varname:node_4449,prsc:2|A-2354-OUT,B-4333-OUT;n:type:ShaderForge.SFN_Get,id:4333,x:31723,y:33202,varname:node_4333,prsc:2|IN-3158-OUT;n:type:ShaderForge.SFN_Tex2d,id:5128,x:27338,y:31680,ptovrint:False,ptlb:node_5128,ptin:_node_5128,varname:_node_5128,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:9062-7241-4514-8803-5395-492-6132-3444-1089-212-9477-5889-7041-1595-4006-3985-1928-818-7465-5745-6730-2691-3867-8928-4539-7687-6456-7062-4869-7824-376-1497-8819-783-719;pass:END;sub:END;*/

Shader "Marc Sureda/StylizedWater" {
    Properties {
        _DepthGradient1 ("DepthGradient1", Color) = (0.1098039,0.5960785,0.6196079,1)
        _DepthGradient2 ("DepthGradient2", Color) = (0.05882353,0.1960784,0.4627451,1)
        _DepthGradient3 ("DepthGradient3", Color) = (0,0.0625,0.25,1)
        _GradientPosition1 ("GradientPosition1", Float ) = 1.6
        _GradientPosition2 ("GradientPosition2", Float ) = 2
        _FresnelColor ("FresnelColor", Color) = (0.5764706,0.6980392,0.8000001,1)
        _FresnelExp ("FresnelExp", Range(0, 10)) = 10
        _Roughness ("Roughness", Range(0.01, 1)) = 0.6357628
        _LightColorIntensity ("LightColorIntensity", Range(0, 1)) = 0.7759457
        _SpecularIntensity ("SpecularIntensity", Range(0, 1)) = 1
        _FoamColor ("FoamColor", Color) = (0.854902,0.9921569,1,1)
        _MainFoamScale ("Main Foam Scale", Float ) = 40
        _MainFoamIntensity ("Main Foam Intensity", Range(0, 10)) = 3.84466
        _MainFoamSpeed ("Main Foam Speed", Float ) = 0.1
        _MainFoamOpacity ("Main Foam Opacity", Range(0, 1)) = 0.8737864
        _SecondaryFoamScale ("Secondary Foam Scale", Float ) = 40
        _SecondaryFoamIntensity ("Secondary Foam Intensity", Range(0, 10)) = 2.330097
        _SecondaryFoamOpacity ("Secondary Foam Opacity", Range(0, 1)) = 0.6310679
        [MaterialToggle] _SecondaryFoamAlwaysVisible ("Secondary Foam Always Visible", Float ) = 1
        _TurbulenceDistortionIntesity ("Turbulence Distortion Intesity", Range(0, 6)) = 0.8155341
        _TurbulenceScale ("Turbulence Scale", Float ) = 10
        _WaveDistortionIntensity ("WaveDistortion Intensity", Range(0, 4)) = 0.592233
        _WavesDirection ("Waves Direction", Range(0, 360)) = 0
        _WavesAmplitude ("Waves Amplitude", Range(0, 10)) = 4.980582
        _WavesSpeed ("Waves Speed", Float ) = 1
        _WavesIntensity ("Waves Intensity", Float ) = 2
        [MaterialToggle] _VertexOffset ("Vertex Offset", Float ) = 0
        [MaterialToggle] _RealTimeReflection ("Real Time Reflection", Float ) = 0
        _ReflectionsIntensity ("ReflectionsIntensity", Range(0, 3)) = 1
        _OpacityDepth ("OpacityDepth", Float ) = 5
        _Opacity ("Opacity", Range(0, 1)) = 0.7378641
        _RefractionIntensity ("Refraction Intensity", Float ) = 1
        _DistortionTexture ("DistortionTexture", 2D) = "white" {}
        _FoamTexture ("FoamTexture", 2D) = "white" {}
        _ReflectionTex ("ReflectionTex", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        GrabPass{ "Refraction" }
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
//            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x n3ds wiiu 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D Refraction;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform float4 _DepthGradient2;
            uniform float4 _FoamColor;
            uniform float4 _FresnelColor;
            uniform float _MainFoamIntensity;
            uniform float _FresnelExp;
            uniform sampler2D _ReflectionTex; uniform float4 _ReflectionTex_ST;
            uniform sampler2D _DistortionTexture; uniform float4 _DistortionTexture_ST;
            uniform float _MainFoamScale;
            uniform float _SecondaryFoamScale;
            uniform float _SecondaryFoamIntensity;
            uniform fixed _SecondaryFoamAlwaysVisible;
            uniform float _SecondaryFoamOpacity;
            uniform float _MainFoamOpacity;
            uniform float _WavesDirection;
            uniform float _WavesSpeed;
            uniform fixed _VertexOffset;
            uniform float _WavesAmplitude;
            uniform float _WavesIntensity;
            uniform fixed _RealTimeReflection;
            uniform float _WaveDistortionIntensity;
            uniform float4 _DepthGradient1;
            uniform float _MainFoamSpeed;
            uniform float _GradientPosition1;
            uniform float _GradientPosition2;
            uniform float4 _DepthGradient3;
            uniform float _TurbulenceDistortionIntesity;
            uniform float _TurbulenceScale;
            uniform float _ReflectionsIntensity;
            uniform float _LightColorIntensity;
            uniform float _Roughness;
            uniform float _SpecularIntensity;
            uniform float _OpacityDepth;
            uniform float _Opacity;
            uniform float _RefractionIntensity;
            uniform sampler2D _FoamTexture; uniform float4 _FoamTexture_ST;
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
                float4 screenPos : TEXCOORD3;
                float4 projPos : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            };

            float CorrectDepth(float rawDepth)
        {
            float persp = LinearEyeDepth(rawDepth);
            float ortho = (_ProjectionParams.z-_ProjectionParams.y)*(1-rawDepth)+_ProjectionParams.y;
            return lerp(persp,ortho,unity_OrthoParams.w);
        }
            
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_8260 = _Time + _TimeEditor;
                float node_9794_ang = (_WavesDirection/57.0);
                float node_9794_spd = 1.0;
                float node_9794_cos = cos(node_9794_spd*node_9794_ang);
                float node_9794_sin = sin(node_9794_spd*node_9794_ang);
                float2 node_9794_piv = float2(0.5,0.5);
                float2 node_9794 = (mul(o.uv0-node_9794_piv,float2x2( node_9794_cos, -node_9794_sin, node_9794_sin, node_9794_cos))+node_9794_piv);
                float4 _Gradient = tex2Dlod(_DistortionTexture,float4(TRANSFORM_TEX(node_9794, _DistortionTexture),0.0,0));
                float node_5335 = sin(((node_8260.g*_WavesSpeed)-(_Gradient.b*(_WavesAmplitude*30.0))));
                float Waves = (node_5335*_WavesIntensity*10.0);
                v.vertex.xyz += lerp( 0.0, (v.normal*(Waves*0.04)), _VertexOffset );
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,CorrectDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float4 node_8260 = _Time + _TimeEditor;
                float node_9794_ang = (_WavesDirection/57.0);
                float node_9794_spd = 1.0;
                float node_9794_cos = cos(node_9794_spd*node_9794_ang);
                float node_9794_sin = sin(node_9794_spd*node_9794_ang);
                float2 node_9794_piv = float2(0.5,0.5);
                float2 node_9794 = (mul(i.uv0-node_9794_piv,float2x2( node_9794_cos, -node_9794_sin, node_9794_sin, node_9794_cos))+node_9794_piv);
                float4 _Gradient = tex2D(_DistortionTexture,TRANSFORM_TEX(node_9794, _DistortionTexture));
                float node_5335 = sin(((node_8260.g*_WavesSpeed)-(_Gradient.b*(_WavesAmplitude*30.0))));
                float Waves = (node_5335*_WavesIntensity*10.0);
                float4 node_9238 = _Time + _TimeEditor;
                float2 node_5311 = ((i.uv0*_TurbulenceScale)+node_9238.g*float2(0.01,0.01));
                float4 _DistortionExtra = tex2D(_DistortionTexture,TRANSFORM_TEX(node_5311, _DistortionTexture));
                float node_3182 = ((0.05*Waves*_WaveDistortionIntensity)+((_DistortionExtra.g*_TurbulenceDistortionIntesity)*2.0+0.0));
                float Turbulence = node_3182;
                float2 node_7739 = ((0.01*_RefractionIntensity)*(Turbulence*(i.uv0*2.0)));
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + float4(node_7739,node_7739).rg;
                float4 sceneColor = tex2D(Refraction, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
////// Emissive:
                float node_721 = 0.01;
                float2 node_1893 = lerp(float2(sceneUVs.r,sceneUVs.g),float2((sceneUVs.r+node_721),(sceneUVs.g+node_721)),node_3182);
                float4 _ReflectionTex_var = tex2D(_ReflectionTex,TRANSFORM_TEX(node_1893, _ReflectionTex));
                float node_6450 = (_MainFoamSpeed*0.15);
                float4 node_4283 = _Time + _TimeEditor;
                float2 node_6798 = ((float2(node_6450,node_6450)*node_4283.g)+(i.uv0*_MainFoamScale));
                float4 _FoamNoise = tex2D(_FoamTexture,TRANSFORM_TEX(node_6798, _FoamTexture));
                float node_9706 = ((1.0 - saturate((pow(saturate(saturate((sceneZ-partZ)/((node_5335*0.1+0.2)*(_FoamNoise.r*_MainFoamIntensity)))),15.0)/0.1)))*_MainFoamOpacity);
                float2 node_5249 = ((i.uv0*_SecondaryFoamScale)+node_9238.g*float2(0.01,0.01));
                float4 node_4292 = tex2D(_FoamTexture,TRANSFORM_TEX(node_5249, _FoamTexture));
                float node_7744 = ((lerp( (1.0 - saturate((pow(saturate(saturate((sceneZ-partZ)/_SecondaryFoamIntensity)),1.0)/0.8))), 1.0, _SecondaryFoamAlwaysVisible )*(pow(saturate((node_4292.r*4.0+-1.0)),0.5)*0.3))*_SecondaryFoamOpacity);
                float node_7513 = 1.0;
                float3 CustomLight = (saturate((((Turbulence/_Roughness)*0.8+0.2)*((2.0*pow(dot(lightDirection,viewReflectDirection),exp2(((1.0 - _Roughness)*5.0+5.0))))*_LightColor0.rgb)))*_SpecularIntensity);
                float3 node_6874 = CustomLight;
                float3 emissive = float4((((saturate((lerp( 0.0, ((_ReflectionTex_var.rgb*0.3)*_ReflectionsIntensity), _RealTimeReflection )+lerp(lerp(_DepthGradient1.rgb,lerp(_DepthGradient2.rgb,_DepthGradient3.rgb,saturate(pow(saturate((sceneZ-partZ)/(_GradientPosition1+_GradientPosition2)),3.0))),saturate(saturate((sceneZ-partZ)/_GradientPosition1))),_FresnelColor.rgb,saturate(pow(1.0-max(0,dot(normalDirection, viewDirection)),_FresnelExp)))))+(_FoamColor.rgb*node_9706)+(_FoamColor.rgb*node_7744))*lerp(float3(node_7513,node_7513,node_7513),clamp(_LightColor0.rgb,0.3,1),_LightColorIntensity))+node_6874),Waves).rgb;
                float3 finalColor = emissive;
                float FoamMask = (node_9706+(node_7744*0.2));
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,saturate((FoamMask+_Opacity+saturate(saturate((sceneZ-partZ)/_OpacityDepth))+node_6874))),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "CustomMaterialInspector"
}
