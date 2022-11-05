#version 330 core
precision mediump float;

in vec2 fragTexCoord;           //  from Raylib vertex shader

out vec4 color;

uniform sampler2D texture0;     //  from Raylib vertex shader

uniform float inner;            // inner radius
uniform float radius;           // alpha fades out to this radius
uniform vec2 screenHalfSize;    // view-port dimensions

void main()
{
    vec4 texelColor = texture(texture0, fragTexCoord);
    float alpha = texelColor.z;    
    float d = distance(gl_FragCoord.xy, screenHalfSize) - radius;

    if (d > radius)
    {
        alpha = 1.0;
    }
    else
    {
        if (d < inner) alpha = 0.0;
        else alpha = ((d - inner) / (radius - inner));
    }
    
    alpha = min(texelColor.a, alpha);
    color = vec4(0, 0, 0, alpha);
}