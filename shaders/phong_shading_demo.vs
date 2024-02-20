#version 330 core

layout (location = 0) in vec3 Position;
layout (location = 1) in vec3 Normal;


uniform mat4 worldMatrix;
uniform mat4 normalMatrix;
uniform mat4 MVP;


out vec3 interpPos;
out vec3 interpNor;

void main()
{
    gl_Position = MVP * vec4(Position , 1.0)  ;  
    
    interpPos = vec3 (worldMatrix * vec4(Position , 1.0));    
    interpNor = vec3 (normalMatrix * vec4(Normal , 0.0) );
}