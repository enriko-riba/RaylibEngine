
using RaylibEngine.Components;
using System.Numerics;

namespace Deeper.Entities;
internal class VehicleSpotMask : Sprite
{
    public const string NodeName = "SpotMask";

    private readonly Shader shdrSpot;
    private readonly int innerUniformLocation;
    private readonly int radiusUniformLocation;
    private readonly int screenHalfUniformLocation;

    public VehicleSpotMask(Texture texture, Rectangle srcFrame, float innerRadius, float outerRadius) : base(texture)
    {
        Name = NodeName;
        Frame = srcFrame;

        shdrSpot = LoadShader(null, "./Assets/Shaders/vehicle-spot.fs");
        innerUniformLocation = GetShaderLocation(shdrSpot, "inner");
        radiusUniformLocation = GetShaderLocation(shdrSpot, "radius");
        screenHalfUniformLocation = GetShaderLocation(shdrSpot, "screenHalfSize");

        SetShaderValue(shdrSpot, innerUniformLocation, innerRadius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        SetShaderValue(shdrSpot, radiusUniformLocation, outerRadius, ShaderUniformDataType.SHADER_UNIFORM_FLOAT);
        //SetShaderValue(shdrSpot, screenHalfUniformLocation, new Vector2(GetScreenWidth() / 2, GetScreenHeight() / 2), ShaderUniformDataType.SHADER_UNIFORM_VEC2);
    }

    public override void Draw()
    {
        BeginShaderMode(shdrSpot);
        base.Draw();
        EndShaderMode();
    }

    public void UpdateViewport(Vector2 halfSize)
    {
        SetShaderValue(shdrSpot, screenHalfUniformLocation, halfSize, ShaderUniformDataType.SHADER_UNIFORM_VEC2);
    }
}
