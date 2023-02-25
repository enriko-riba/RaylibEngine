
namespace Deeper.Entities;

using RaylibEngine.Components;
using System.Numerics;

internal class VehicleSpotMask : Sprite
{
    public const string NodeName = "SpotMask";

    private readonly Shader spotShader;
    private readonly int innerUniformLocation;
    private readonly int radiusUniformLocation;
    private readonly int screenHalfUniformLocation;

    public VehicleSpotMask(Texture texture, Rectangle srcFrame, float innerRadius, float outerRadius) : base(texture)
    {
        Name = NodeName;
        Frame = srcFrame;

        spotShader = LoadShader(null, "./Assets/Shaders/vehicle-spot.fs");
        innerUniformLocation = GetShaderLocation(spotShader, "inner");
        radiusUniformLocation = GetShaderLocation(spotShader, "radius");
        screenHalfUniformLocation = GetShaderLocation(spotShader, "screenHalfSize");

        SetShaderValue(spotShader, innerUniformLocation, innerRadius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(spotShader, radiusUniformLocation, outerRadius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
    }

    public override void Draw()
    {
        BeginShaderMode(spotShader);
        base.Draw();
        EndShaderMode();
    }

    public void UpdateViewport(Vector2 halfSize)
    {
        SetShaderValue(spotShader, screenHalfUniformLocation, halfSize, ShaderUniformDataType.SHADER_UNIFORM_VEC2);
    }
}
