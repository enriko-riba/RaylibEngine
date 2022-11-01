#version 330 core
precision mediump float;

// Output fragment color
out vec4 color;

uniform float inner;            // inner radius
uniform float radius;           // alpha fades out to this radius
uniform vec2 screenHalfSize;    // viewport dimensions

void main()
{
    float alpha = 0.0;    
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

    color = vec4(0, 0, 0, alpha);
}