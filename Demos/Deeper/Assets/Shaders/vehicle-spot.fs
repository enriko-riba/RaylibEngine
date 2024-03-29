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
    float alpha = texelColor.a;    
    float d = distance(gl_FragCoord.xy, screenHalfSize);

    float r = 0, g = 0, b = 0;

    if (d > radius)
    {
        alpha = 1.0;        
    }
    else
    {
        //r = 0.1;
        g = 0.025; 
        //b = 0.1;
        if (d < inner) alpha = 0.0;
        else alpha = ((d - inner) / (radius - inner));
    }
    
    alpha = min(texelColor.a, alpha);
    color = vec4(r, g, b, alpha);
}