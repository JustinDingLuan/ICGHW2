#version 330 core

in vec3 interpPos;
in vec3 interpNor;

uniform vec3 Ka;
uniform vec3 Kd;
uniform vec3 Ks;
uniform float Ns;

uniform vec3 ambientLight;

uniform vec3 cameraPos;

uniform vec3 dirLightDir;
uniform vec3 dirLightRadiance;

uniform vec3 pointLightPos;
uniform vec3 pointLightIntensity;

uniform vec3 spotLightPos;
uniform vec3 spotLightDir;
uniform vec3 spotLightIntensity;	
uniform float totalWidth;
uniform float falloffStart;
	 
out vec4 FragColor;

vec3 diffuse(vec3 kd , vec3 radiance , vec3 normal , vec3 lightdir){
    return kd * radiance * max(0.0f , dot(normal , lightdir));
}

vec3 specular(vec3 ks , vec3 intensity , vec3 cameradir , vec3 reflectdir , float ns){
    float value = dot(cameradir , reflectdir);
    float power = pow(max(value , 0.0) , ns);
    return ks * intensity * power;
}

vec3 PointLight(vec3 pos , vec3 nor , vec3 cameradir){
    vec3 PointLightDir = normalize(pointLightPos - pos);
    vec3 ReflectDir = reflect(-PointLightDir , nor);
    float Distance = length(pointLightPos - pos);
    float Attenuation = 1.0f / (Distance * Distance);
    vec3 Radiance = pointLightIntensity * Attenuation;

    vec3 D = diffuse(Kd , Radiance , nor , PointLightDir);
    vec3 S = specular(Ks , Radiance , cameradir , ReflectDir , Ns);
    
    return D + S;
}

vec3 DirLight(vec3 nor , vec3 cameradir){
    vec3 LightDir = normalize(-dirLightDir);
    vec3 ReflectDir = reflect(-LightDir , nor);

    vec3 D = diffuse(Kd , dirLightRadiance , nor , LightDir);
    vec3 S = specular(Ks , dirLightRadiance , cameradir , ReflectDir , Ns);
    return D + S;
}

vec3 SpotLight(vec3 pos , vec3 nor , vec3 cameradir){
    vec3 LightDir = normalize(spotLightPos - pos);
    vec3 ReflectDir = reflect(-LightDir , nor);

    float Cosinetheta = dot(LightDir , normalize(-spotLightDir));
    float Epsilon = cos(radians(falloffStart)) - cos(radians(totalWidth));
    float Distance = length(spotLightPos - pos);
    float Attenuation = 1.0 / (Distance * Distance);
    
    vec3 Intensity = spotLightIntensity * clamp((Cosinetheta - cos(radians(totalWidth))) / Epsilon , 0.0 , 1.0) * Attenuation;
    vec3 D = diffuse(Kd , Intensity , nor , LightDir);
    vec3 S = specular(Ks , Intensity , cameradir , ReflectDir , Ns);
    return D + S;
}

void main()
{
    vec3 interpNor = normalize(interpNor);
    vec3 CameraDir = normalize(cameraPos - interpPos);
    vec3 ambient = Ka * ambientLight;
    
    //pointlight + directionallight + spotlight   
    vec3 interpColor = ambient;
    interpColor += PointLight(interpPos , interpNor , CameraDir); 
    interpColor += DirLight(interpNor , CameraDir);
    interpColor += SpotLight(interpPos , interpNor , CameraDir);
    FragColor = vec4(interpColor , 1.0);
}